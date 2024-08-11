using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Characters;
using Gavi.Encounter;
using Gavi.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gavi.Skills
{
    public class SkillManager : MonoBehaviour
    {
        
        #region Serialized Variabled
        public Character Character => _character;
        [SerializeField] private Character _character;
        // [SerializeField] private bool _instantiateInitialSkills = true;

        [FormerlySerializedAs("_initialSkills")]
        [Header("- References -")]
        [SerializeField] private List<Skill> _initialSkillsPrefab;
        public Transform SkillsParent => _skillsParentTransform;
        [SerializeField] private Transform _skillsParentTransform;
        #endregion
        
        
        #region Non-Serialized Variables
        public CastManager CastManager => _castManager;
        private CastManager _castManager;
        public CooldownManager CooldownManager => _cooldownManager;
        private CooldownManager _cooldownManager;
        public ManaManager ManaManager => _manaManager;
        private ManaManager _manaManager;

        private SkillUiManager _uiManager;

        public List<Skill> ActiveSkillsPrioSorted => _activeSkillsPrioSorted;
         List<Skill> _activeSkillsPrioSorted = new List<Skill>();
        #endregion


        #region Mono
        protected virtual void Awake()
        {
            _castManager = GetComponent<CastManager>();
            _cooldownManager = GetComponent<CooldownManager>();
            _manaManager = GetComponent<ManaManager>();
            _uiManager = GetComponent<SkillUiManager>();
        }

        private void Start()
        {
            if (Character.IsPlayer && _initialSkillsPrefab.Count != 0)
            {
                Debugger.LogAssertionFail("This should not happen! Player skill should only be initialized when " +
                                          "a safe file is loaded and not in the Start function. Make sure that the " +
                                          "initial player skills are empty or rewrite some code.");
            }
                
            
            for (int i = 0; i < _initialSkillsPrefab.Count; i++)
            {
                Skill skill = Instantiate(_initialSkillsPrefab[i], _skillsParentTransform);
                AssignActiveSkill(skill, -1);
            }
        }

        protected virtual void OnEnable()
        {
            // place holder for children
        }

        protected virtual void OnDisable()
        {
            // place holder for children
        }

        protected virtual void Update()
        {
            if (!EncounterManager.Instance.IsInEncounterAndUnpaused)
                return;
            UpdateSkills();
        }
        #endregion

        
        #region Update Skils
        private void UpdateSkills()
        {
            if (!EncounterManager.Instance.IsInEncounterAndUnpaused)
                return;

            for (int i = 0; i < _activeSkillsPrioSorted.Count; i++)
            {
                Skill skill = _activeSkillsPrioSorted[i];
                if (!skill.IsActivated())
                    continue;
                skill.TryPerformSkill();
            }
        }
        #endregion


        #region Skill Control
        public void AssignActiveSkill(Skill skill, int skillIndex)
        {
            if (skill == null)
                return;
            
            if (_activeSkillsPrioSorted.Contains(skill))
            {
                Debugger.LogError("_activeSkills.Contains(skill) in SkillManager.AssignSkill()");
                return;
            }
            skill.AssignSkillManager(this);
            AddSkillToActiveList(skill);
            _uiManager.AddUi(skill, skillIndex);
        }

        private void AddSkillToActiveList(Skill skill)
        {
            if (_activeSkillsPrioSorted.Count == 0)
            {
                _activeSkillsPrioSorted.Add(skill);
                return;
            }

            for (int i = 0; i < _activeSkillsPrioSorted.Count; i++)
            {
                int prio = _activeSkillsPrioSorted[i].SkillPriority;
                if (skill.SkillPriority > prio)
                {
                    _activeSkillsPrioSorted.Insert(i, skill);
                    return;
                }
            }
            // in case the skill has the lowest prio or the same prio as the lowest
            _activeSkillsPrioSorted.Add(skill);
        }

        public void UnassignSkill(Skill skill)
        {
            if (!_activeSkillsPrioSorted.Contains(skill))
            {
                Debugger.LogError("!_activeSkills.Contains(skill) in SkillManager.UnassignSkill()");
                return;
            }
            skill.UnassignSkillManager(this);
            _activeSkillsPrioSorted.Remove(skill);
            _uiManager.RemoveUi(skill);
        }
        #endregion
    }
}
