using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Encounter;
using UnityEngine;
using Gavi.UI;
using UnityEngine.Serialization;


namespace Gavi.Skills
{
    public class SkillCooldown : MonoBehaviour
    {
        #region Serialized Variables
        [Header("- Warmup -")]
        [SerializeField] private Utility.FloatRange _warmupCooldown;

        public Utility.FloatRange LocalCooldown => _localCooldownApply;
        [Header("- Cooldown -")] 
        [SerializeField] private Utility.FloatRange _localCooldownApply;
        //[SerializeField] private float _localCooldownApplyVariance;
        [SerializeField] private float _globalCooldownApply = 1.5f;
        
        [Space]
        [SerializeField] private bool _doNotApplyGlobalCooldown;
        
        //[Header("- References -")]
        //[SerializeField] private Bar _cooldownBar;
        #endregion


        #region Non-Serialized Variables
        // cooldown
        private float _localCooldownApplied;
        private float _localCooldownCurrent;
        private float _localCooldownRate => _localCooldownApplied > 0 ? (_localCooldownCurrent / _localCooldownApplied) : 0;
        
        private float _globalCooldownApplied;
        private float _globalCooldownCurrent;
        private float _globalCooldownRate => _globalCooldownApplied > 0 ? (_globalCooldownCurrent / _globalCooldownApplied) : 0;

        private bool _isOnLocalCooldown => _localCooldownCurrent > 0;
        private bool _isOnGlobalCooldown => _globalCooldownCurrent > 0;
        private bool _isOnCooldown => _isOnLocalCooldown || _isOnGlobalCooldown;
        
        public float LongestCooldownRemaining => Mathf.Max(0, _localCooldownCurrent, _globalCooldownCurrent);
        private float _longestCooldownApplied => Mathf.Max(_isOnLocalCooldown ? _localCooldownApplied : 0, _isOnGlobalCooldown ? _globalCooldownApplied : 0);
        public float LongestCooldownRate => _longestCooldownApplied > 0 ? LongestCooldownRemaining / _longestCooldownApplied : 0;

        // references
        private CooldownManager _manager
        {
            get
            {
                if (_skill == null) return null;
                if (_skill.SkillManager == null) return null;
                return _skill.SkillManager.CooldownManager;
            }
        }

        private Skill _skill;
        #endregion


        #region Mono
        private void Awake()
        {
            _skill = GetComponent<Skill>();
        }

        private void OnEnable()
        {
            EncounterManager.Instance.OnEncounterInitialize.AddListener(OnEncounterInitialize);   
        }

        private void OnDisable()
        {
            EncounterManager.Instance.OnEncounterInitialize.RemoveListener(OnEncounterInitialize);   
        }

        private void Start()
        {
            // if (_skill.SkillManager != null)
            //     _manager = _skill.SkillManager.CooldownManager;
            //UpdateUi();
        }

        private void Update()
        {
            UpdateCooldown();
        }
        #endregion
        
        
        #region Update
        private void UpdateCooldown()
        {
            if (!EncounterManager.Instance.IsInEncounterAndUnpaused)
                return;

            if (_isOnLocalCooldown)
                _localCooldownCurrent -= Simulation.DeltaTime;
            if (_isOnGlobalCooldown)
                _globalCooldownCurrent -= Simulation.DeltaTime;
        }
        #endregion

        
        #region Set Cooldown
        public void SetLocalCooldown(float localCooldown, bool force = false)
        {
            _localCooldownApplied = localCooldown;
            _localCooldownCurrent = localCooldown;
        }

        public void SetGlobalCooldown(float globalCooldown, bool force = false)
        {
            if (!force)
                globalCooldown = Mathf.Max(globalCooldown, _globalCooldownApplied);
            _globalCooldownApplied = globalCooldown;
            _globalCooldownCurrent = globalCooldown;
        }

        public void SetCooldown(float localCooldown, float globalCooldown, bool force = false)
        {
            SetLocalCooldown(localCooldown, force);
            SetGlobalCooldown(globalCooldown, force);
        }
        #endregion


        #region Apply Cooldown
        public void ApplyCooldown(SkillPerformance performance)
        {
            if (!_doNotApplyGlobalCooldown)
            {
                if(_manager != null)
                    _manager.ApplyGlobalCooldown(_globalCooldownApply);
                else
                    SetGlobalCooldown(_globalCooldownApply);  
            }

            SetLocalCooldown(_localCooldownApply.NewValue * _skill.SkillManager.CooldownManager.CooldownRate(_skill));
        }
        #endregion


        #region Events

        private void OnEncounterInitialize()
        {
            SetLocalCooldown(_warmupCooldown.Value);
        }
        #endregion

        
        #region Validity
        public bool IsCastValid()
        {
            return !_isOnCooldown;
        }
        #endregion
    }
}