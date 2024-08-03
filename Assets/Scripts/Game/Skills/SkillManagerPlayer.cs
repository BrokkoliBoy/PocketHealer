using System.Collections;
using System.Collections.Generic;
using Gavi.Encounter;
using Gavi.Skills;
using UnityEngine;

namespace Gavi
{
    public class SkillManagerPlayer : SkillManager
    {
        public static SkillManager Instance;
        
        
        #region Mono
        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EncounterManager.Instance.OnEncounterInitialize.AddListener(OnEncounterInitialize);
            EncounterManager.Instance.OnEncounterClear.AddListener(OnEncounterClear);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EncounterManager.Instance.OnEncounterInitialize.RemoveListener(OnEncounterInitialize);
            EncounterManager.Instance.OnEncounterClear.RemoveListener(OnEncounterClear);
        }
        #endregion


        #region Events
        private void OnEncounterInitialize()
        {
            AssignSkillsChosen();
        }

        private void OnEncounterClear()
        {
            for (int i = ActiveSkillsPrioSorted.Count - 1; i >= 0; i--)
                UnassignSkill(ActiveSkillsPrioSorted[i]);
        }
        #endregion


        #region Logic
        private void AssignSkillsChosen()
        {
            List<Skill> skills = PlayerSkillConfiguration.Instance.GetSkills(EncounterManager.Instance.CurrentEncounter.Difficulty);
            for (int i = 0; i < skills.Count; i++)
                AssignActiveSkill(skills[i], i);
        }
        #endregion
    }
}
