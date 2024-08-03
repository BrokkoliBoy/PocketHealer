using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Encounter;
using Gavi.Skills;
using Gavi.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gavi
{
    [System.Serializable]
    public class SkillUnlockCondition
    {
        public Skill SkillPrefab;
        [Tooltip("CAUTION: This is not equal to the index of the encounter in any list but the ingame encounter number that " +
                 "the player will see.")]
        public int EncounterNumber = -1;
        public Encounter.Encounter.EncounterDifficulty EncounterDifficulty = Encounter.Encounter.EncounterDifficulty.Normal;
        // public Encounter.Encounter EncounterToBeDefeated;
    }
    
    public class SkillLearnSystem : MonoBehaviour
    {
        [Header("- Skills to Unlock -")]
        [SerializeField] private List<SkillUnlockCondition> _skillsToUnlock;
        
        [Header("- Initial Skills -")]
        [SerializeField] private List<Skill> _initialSkillsAvailablePrefabs;
        [SerializeField] private List<Skill> _initialSkillsChosenPrefabs;

        [Header("- UI -")]
        [SerializeField] private float _padding = 50f;
        [SerializeField] private Transform _parentInfoPanels;
        [SerializeField] private GameObject _panelNewSkillUnlocked;
        [SerializeField] private List<InfoPanelSkill> _infoPanelSkills;
        [SerializeField] private TextMeshProUGUI _textSkillUnlocked;

        [Header("- DEBUG -")] 
        [SerializeField] private List<Skill> _DEBUG_SKILLS_UNLOCK_AT_START;
        
        private void Awake()
        {
            EncounterManager.Instance.OnEncounterSuccess.AddListener(OnEncounterSuccess);
            DebugMode.Instance.OnDebugEnabled.AddListener(DebugLearnAllSkills);
        }

        private void Start()
        {
            PlayerSkillConfiguration.Instance.AssignInitialSetup(_initialSkillsAvailablePrefabs, _initialSkillsChosenPrefabs);
        }

        public void OnEncounterSuccess()
        {
            foreach (InfoPanelSkill infoPanel in _infoPanelSkills)
                infoPanel.HidePanel();
            _panelNewSkillUnlocked.SetActive(false);
            _textSkillUnlocked.text = "Congratulations! Sadly no skills have been unlocked.";
            
            int index = 0;
            foreach (SkillUnlockCondition condition in _skillsToUnlock)
            {
                if (condition.EncounterNumber != EncounterManager.Instance.CurrentEncounter.EncounterNumber)
                    continue;
                if (condition.EncounterDifficulty != EncounterManager.Instance.CurrentEncounter.Difficulty)
                    continue;

                Skill skillUnlocked = PlayerSkillConfiguration.Instance.UnlockSkill(condition.SkillPrefab);
                if (skillUnlocked != null)
                    ShowSkillUnlocked(skillUnlocked, index);
                index++;
            }
        }

        private void ShowSkillUnlocked(Skill skill, int index)
        {
            if (index >= _infoPanelSkills.Count)
            {
                Debugger.LogError("Unable to show unlocked skill because there are not enough info panels.");
                return;
            }
            _panelNewSkillUnlocked.SetActive(true);

            if (index == 1)
                _textSkillUnlocked.text = "Congratulations! You unlocked a new skill.";
            else
                _textSkillUnlocked.text = "Congratulations! You unlocked " + (index + 1) + " new skills.";
            _infoPanelSkills[index].ShowPanel(skill);

            for (int i = 0; i <= index; i++)
            {
                InfoPanelSkill infoPanel = _infoPanelSkills[i];
                float x = (infoPanel.Width + _padding) * (0.5f + i) - (infoPanel.Width + _padding) * (0.5f + index* 0.5f) ;
                infoPanel.Root.anchoredPosition = new Vector2(x, infoPanel.Root.anchoredPosition.y);
            }
        }

        
        #region Debug
        private void DebugLearnAllSkills()
        {
            foreach (Skill skill in _DEBUG_SKILLS_UNLOCK_AT_START)
            {
                Skill skillUnlocked = PlayerSkillConfiguration.Instance.UnlockSkill(skill);
            }
        }
        #endregion
    }
}
