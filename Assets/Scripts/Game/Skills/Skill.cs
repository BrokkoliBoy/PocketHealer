using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gavi.Characters;
using Gavi.Encounter;
using Gavi.Player.Skills;
using Gavi.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gavi.Skills
{
    public class Skill : MonoBehaviour
    {
        #region Serialized Variables
        public string Name => _name;
        [Header("- General -")]
        [SerializeField] private string _name = "Skill name";

        public string Description => GenerateDescription(_description);
        [SerializeField][TextArea] private string _description;
        public int SkillPriority => _skillPriority;
        [Tooltip("Defines what skill to activate if there is more than one skill that can be activated at the same time. Higher values indicate a higher priority.")]
        [SerializeField] private int _skillPriority;


        public Sprite IconSkill => _iconSkill;
        [Header("- UI -")]
        [SerializeField] private Sprite _iconSkill;
        [SerializeField] private SkillUi _skillUiPrefab;

        public bool DontUseUi => _dontUseUi;
        [SerializeField] private bool _dontUseUi;
        #endregion


        #region Non-Serialized Variables
        private List<ActivationType> _activationTypes = new List<ActivationType>();
        public SkillCast Cast => _cast;
        private SkillCast _cast;
        public SkillCooldown Cooldown => _cooldown;
        private SkillCooldown _cooldown;
        public SkillRange Range => _skillRange;
        private SkillRange _skillRange;
        public ManaManager ManaManager => _manaManager;
        private ManaManager _manaManager;
        public SkillMana SkillMana => _skillMana;
        private SkillMana _skillMana;
        public SkillManager SkillManager => _skillManager;
        private SkillManager _skillManager;
        public KeyBinding KeyBinding
        {
            get
            {
                foreach (ActivationType activationType in _activationTypes)
                {
                    if (activationType is ActivationTypePlayerInput playerInput)
                    {
                        return playerInput.GetZoneKeyBinding();
                    }
                }

                return null;
            }
        }
        public SkillUi UI => _ui;
        private SkillUi _ui;

        public PlayerSkillPrefab SkillPrefab { get; set; }

        public string Identifier => _name;
        #endregion
        
        
        #region Mono
        private void Awake()
        {
            _cast = GetComponent<SkillCast>();
            _cooldown = GetComponent<SkillCooldown>();
            foreach (ActivationType activationType in GetComponents<ActivationType>())
                _activationTypes.Add(activationType);
            _skillRange = GetComponent<SkillRange>();
            _skillMana = GetComponent<SkillMana>();
        }
        #endregion

        
        #region Skill Performance
        public bool TryPerformSkill()
        {
            // NOTE: This isn't the final solution
            

            // check skill manager
            if (_skillManager == null)
            {
                Debugger.LogError("_skillManager == null in Skill.Update() (" + _name + ")");
                return false;
            }

            // check if character is alive
            if (_skillManager.Character != null && _skillManager.Character.IsDead)
                return false;

            // check activation type
            if (_activationTypes == null)
            {
                Debugger.LogInfo("_activationTypes == null in Skill.UpdateSkill() (" + _name + ")");
                return false;
            }
            if (_activationTypes.Count == 0)
            {
                Debugger.LogInfo("_activationTypes == 0 in Skill.UpdateSkill() (" + _name + ")");
                return false;
            }
            bool isActivated = false;
            for (int i = 0; i < _activationTypes.Count; i++)
            {
                if (_activationTypes[i].IsActivated())
                {
                    isActivated = true;
                    break;
                }
            }
            if (!isActivated)
                return false;
            
            // check cooldown
            if (_cooldown != null && !_cooldown.IsCastValid())
                return false;

            // check channeling & casting
            if (Cast != null && !Cast.IsCastValid())
                return false;
            
            // check range
            if (_skillRange == null)
            {
                Debugger.LogInfo("_skillRange == null in Skill.UpdateSkill() (" + _name + ")");
                return false;
            }
            if (!_skillRange.IsCastValid())
                return false;
                
            // check mana
            if (_manaManager != null && !_manaManager.IsCastValid(_skillMana))
                return false;
            
            PerformSkill();
            return true;
        }

        private void PerformSkill()
        {
            // create performance
            SkillPerformance performance = new SkillPerformance(); // TODO pooling
            performance.Skill = this;
            
            // targeting
            if (_skillRange.ActivationPoint == SkillRange.ActivisionPoint.OnActivision)
                performance.SetTargets(_skillRange.GetTargets());
            
            // mana
            if (performance.Skill.SkillMana != null && performance.Skill.SkillMana.Activation == SkillMana.ActivationType.OnActivision)
                _manaManager.CurrentMana -= performance.Skill._skillMana.ManaCost;
            
            // cooldown
            _cooldown.ApplyCooldown(performance);
            
            // initiate performing skill
            _skillManager.CastManager.InitiateCastSequence(performance);
            
            // call event
            foreach(ActivationType type in _activationTypes)
                type.OnSuccessfullyActivated();
        }
        #endregion


        #region Control
        public void AssignSkillManager(SkillManager skillManager)
        {
            if (_skillManager != null)
            {
                Debugger.LogAssertionFail("_skillManager != null in Skill.UnAssignSkillManager!");
                _skillManager.UnassignSkill(this);
            }
            _skillManager = skillManager;
            _manaManager = skillManager.ManaManager;
        }
        
        public void UnassignSkillManager(SkillManager skillManager)
        {
            if (_skillManager != skillManager)
                Debugger.LogAssertionFail("_skillManager != skillManager in Skill.UnAssignSkillManager!");
            _skillManager = null;
        }
        
        public virtual bool IsActivated()
        {
            for(int i = 0; i < _activationTypes.Count; i++)
            {
                if (_activationTypes[i].IsActivated())
                    return true;
            }
            return false;
        }
        #endregion


        #region Description
        // e.g. {{cast:1.3}} means it should get the description from the second effect that is applied after
        // finished casting with the secondary identifier 3 (which is context based).
        
        private string GenerateDescription(string input)
        {
            string description = input;
            Regex regex = new Regex(@"{{(cast|channel):.*\..*}}");
            MatchCollection matches = regex.Matches(input);
            foreach (Match match in matches)
            {
                string result = match.Value;
                result = result.Substring(2, result.Length - 4);
                
                // check if cast or channel
                int indexOfColon = result.IndexOf(':');
                string castOrChannel = result.Substring(0, indexOfColon);
                bool isCast = castOrChannel == "cast";
                bool isChannel = castOrChannel == "channel";
                result = result.Substring(indexOfColon + 1);
                
                int indexOfPeriod = result.IndexOf('.');
                string firstIdentifierString = result.Substring(0, indexOfPeriod);
                int firstIdentifier = int.Parse(firstIdentifierString);
                result = result.Substring(indexOfPeriod + 1);
                
                string secondIdentifierString = result;
                int secondIdentifier = int.Parse(secondIdentifierString);
                result = "";

                List<SkillEffect> pool = null;
                if (isCast)
                    pool = _cast.EffectsCastFinish;
                else if (isChannel)
                    pool = _cast.EffectsChanneling;
                else
                {
                    Debugger.LogError("!isCast and !isChannel: " + match.Value + " in " + _cast.name);
                    return "ERROR";
                }

                if (pool == null)
                {
                    Debugger.LogError("pool == null: " + match.Value + " in " + _cast.name);
                    return "ERROR";
                }
                
                if (firstIdentifier >= pool.Count)
                {
                    Debugger.LogError("firstIdentifier >= pool.Count (" + firstIdentifier + "/" + pool.Count + "): " + match.Value + " in " + _cast.name);
                    return "ERROR";
                }
                
                description = description.Replace(match.Value, pool[firstIdentifier].GetDescription(secondIdentifier));
            }
            return description;
        }
        #endregion
        

        #region Ui
        public SkillUi InstantiateSkillUi(RectTransform parent)
        {
            _ui = Instantiate(_skillUiPrefab, parent);
            _ui.ApplySkill(this);
            return _ui;
        }

        public void DestroySkillUi()
        {
            if (_ui == null)
                return;
            Destroy(_ui.gameObject);
            _ui = null;
        }
        #endregion
    }
}
