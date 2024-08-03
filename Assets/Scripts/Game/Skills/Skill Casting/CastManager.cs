using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Characters;
using Gavi.Encounter;
using Gavi.UI;
using Gavi.Utility;
using UnityEngine;

namespace Gavi.Skills
{
    public class CastManager : MonoBehaviour
    {
        [SerializeField] private Bar _castBar;
        
        private SkillManager _skillManager;

        private float _requiredCastTime;
        private float _currentCastTime;

        private float _requiredChannelTime;
        private float _currentChannelTime;

        private bool _isChanneling;
        public bool IsChanneling => _isChanneling;
        private bool _isCasting;
        public bool IsCasting => _isCasting;

        private SkillPerformance _performance;
        private float _channelPerformSequenceTime;
        private float _currentChannelPerformSequenceTime;
        private int _channelPerformancesToGo;

        public float CurrentChannelRate => _currentChannelTime / (_requiredChannelTime <= 0 ? 1 : _requiredChannelTime);
        public float CurrentCastRate => _currentCastTime / (_requiredCastTime <= 0 ? 1 : _requiredCastTime);

        private bool _wasUpdated;

        private List<Character> _highlightedCharacters = new List<Character>();
        private SkillUi _castHighlightSkillUi;
        
        
        #region Mono
        private void Awake()
        {
            _skillManager = GetComponent<SkillManager>();
        }

        private void OnEnable()
        {
            //if (_skillManager != null && _skillManager.Character != null)
            _skillManager.Character.OnDied.AddListener(AbortCasting);
            EncounterManager.Instance.OnEncounterStopped.AddListener(AbortCasting);
        }

        private void OnDisable()
        {
            //if (_skillManager != null && _skillManager.Character != null)
            _skillManager.Character.OnDied.RemoveListener(AbortCasting);
            EncounterManager.Instance.OnEncounterStopped.RemoveListener(AbortCasting);
        }

        protected virtual void Update()
        {
            UpdateCastSequence();
        }

        private void LateUpdate()
        {
            _wasUpdated = false;
        }
        #endregion


        #region Sequence
        private void UpdateCastSequence()
        {
            if (!EncounterManager.Instance.IsInEncounterAndUnpaused)
                return;
            
            // if (!forceUpdate && _wasUpdated)
            //     return;
            
            if (_isChanneling)
            {
                if(!_wasUpdated)
                {
                    _currentChannelTime += Simulation.DeltaTime;
                    UpdateChannelPerformance();
                }
                if (_currentChannelTime >= _requiredChannelTime)
                    FinishChanneling();
            }
            
            if (_isCasting)
            {
                if(!_wasUpdated)
                    UpdateCastPerformance();
                if (_currentCastTime >= _requiredCastTime)
                    FinishCasting();
            }
            _wasUpdated = true;
            UpdateUi();
        }

        private void UpdateChannelPerformance()
        {
            DisableTargetHighlight();
            EnableTargetHighlight(_performance.GetTargets());
            
            if (_channelPerformancesToGo <= 1) // we return while there is potentially one more performance to go because that last one is handled in channel finished
                return;
            _currentChannelPerformSequenceTime += Simulation.DeltaTime;
            if (_currentChannelPerformSequenceTime < _channelPerformSequenceTime)
                return;
            
            _currentChannelPerformSequenceTime = 0;
            _channelPerformancesToGo--;
            _performance.Perform(new List<SkillEffect>(_performance.Skill.Cast.EffectsChanneling));
        }

        private void UpdateCastPerformance()
        {
            DisableTargetHighlight();
            EnableTargetHighlight(_performance.GetTargets());
            _currentCastTime += Simulation.DeltaTime;
        }
        #endregion

        
        #region Cast Cycle
        public void InitiateCastSequence(SkillPerformance performance)
        {
            EnableTargetHighlight(performance.Targets);
            EnableCastingIndicator(performance);
            
            _performance = performance;
            _requiredChannelTime = performance.Skill.Cast.ChannelTime * performance.Skill.Cast.HasteRate(performance.Skill);
            _requiredCastTime = performance.Skill.Cast.CastTime * performance.Skill.Cast.HasteRate(performance.Skill);

            _currentCastTime = 0;
            _currentChannelTime = 0;

            _isChanneling = true;
            _isCasting = false;

            // prepare multiple channel performances
            if (_performance.Skill.Cast.ChannelPerformances > 0 && _performance.Skill.Cast.EffectsChanneling.Count > 0)
            {
                SkillCast cast = _performance.Skill.Cast;
                float channelTime = cast.ChannelTime;
                int performances = cast.ChannelPerformances;
                bool performAtStart = cast.ChannelPerformOneAtStart;
                _channelPerformSequenceTime = performances - (performAtStart ? 1 : 0) <= 0 ? 0 : (channelTime / (performances - (performAtStart ? 1 : 0)));
                _channelPerformancesToGo = performances;
                if (performAtStart)
                {
                    _channelPerformancesToGo--;
                    _performance.Perform(new List<SkillEffect>(_performance.Skill.Cast.EffectsChanneling)); // I guess it's important to deliver a copied list and not the original
                }
            }
            else
                _channelPerformancesToGo = 0;

            // _castBar.NameText.text = performance.Skill.name;
            UpdateCastSequence();
        }

        private void FinishChanneling()
        {
            _isChanneling = false;
            _isCasting = true;
            if (_channelPerformancesToGo >= 1)
                _performance.Perform(new List<SkillEffect>(_performance.Skill.Cast.EffectsChanneling));
        }

        private void FinishCasting()
        {
            _isCasting = false;
            if (_performance.Skill.Cast.EffectsCastFinish.Count > 0)
                _performance.Perform(new List<SkillEffect>(_performance.Skill.Cast.EffectsCastFinish));

            FinishSequence();
        }

        private void FinishSequence()
        {
            if (_performance == null) // is the case when a boss dies (no idea why)
            {
                DisableTargetHighlight();
                DisableCastingIndicator();
                return;
            }
            if (_performance.Skill.Cast.EffctsSequenceFinish.Count > 0)
                _performance.Perform(new List<SkillEffect>(_performance.Skill.Cast.EffctsSequenceFinish));
            
            DisableTargetHighlight();
            DisableCastingIndicator();
            _performance = null;
        }

        protected void AbortCasting()
        {
            if (_isCasting)
            {
                _skillManager.CooldownManager.AbortGlobalCooldown();
                _performance.Skill.Cooldown.SetLocalCooldown(0, true);
            }
            _isCasting = false;
            _isChanneling = false;

            DisableTargetHighlight();
            DisableCastingIndicator();
            _performance = null;
        }
        #endregion


        #region Misc
        public float HasteRate(Skill skill)
        {
            float rate = _skillManager.Character.Stats.HasteRateAdditive;
            if (rate <= 0)
                rate = 0.0001f;
            return 1 / rate;
        }
        #endregion


        #region Ui
        private void UpdateUi()
        {
            float castRate = -1;
            if (_isChanneling)
                castRate = 1 - CurrentChannelRate;
            else if (_isCasting)
                castRate = CurrentCastRate;
            _castBar.Value = castRate;
            float f = _isCasting ? (_requiredCastTime - _currentCastTime) : (_isCasting ? (_requiredCastTime - _currentCastTime): 0);
            _castBar.Text = string.Format("{0:0.0}", f);
        }

        private void EnableTargetHighlight(List<Character> targets)
        {
            foreach (Character target in targets)
            {
                if (target == null || target.IsDead)
                    continue;
                if (_skillManager.Character.IsPlayer && target.IsAlly)
                    target.Ui.HighlightTargetedAlly = true;
                else if (_skillManager.Character.IsEnemy && target.IsAlly)
                    target.Ui.HighlightTargetedEnemy = true;
                if (_highlightedCharacters.Contains(target))
                    Debugger.LogError("_highlightedCharacters.Contains(target) in CastManager.EnableTargetHighlight(" + target.Name + ") on " + _skillManager.Character.Name + ".");
                else
                    _highlightedCharacters.Add(target);
            }
        }

        private void DisableTargetHighlight()
        {
            foreach (Character target in _highlightedCharacters)
            {
                if (target == null || target.IsDead)
                    continue;
                
                if (_skillManager.Character.IsPlayer && target.IsAlly)
                    target.Ui.HighlightTargetedAlly = false;
                else if (_skillManager.Character.IsEnemy && target.IsAlly)
                    target.Ui.HighlightTargetedEnemy = false;
            }

            _highlightedCharacters.Clear();
        }

        private void EnableCastingIndicator(SkillPerformance performance)
        {
            if (performance.Skill.UI == null)
                return;
            _castHighlightSkillUi = performance.Skill.UI;
            _castHighlightSkillUi.EnableCastIndicator();
        }
        
        private void DisableCastingIndicator()
        {
            if (_castHighlightSkillUi == null)
                return;
            _castHighlightSkillUi.DisableCastIndicator();
            _castHighlightSkillUi = null;
        }
        #endregion
    }
}
