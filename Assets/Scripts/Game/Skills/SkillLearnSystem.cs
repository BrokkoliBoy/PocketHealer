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
        public PlayerSkillPrefab SkillPrefab;
        [Tooltip("CAUTION: This is not equal to the index of the encounter in any list but the ingame encounter number that " +
                 "the player will see.")]
        public int EncounterNumber = -1;
        public Encounter.Encounter.EncounterDifficulty EncounterDifficulty = Encounter.Encounter.EncounterDifficulty.Normal;
        // public Encounter.Encounter EncounterToBeDefeated;
    }
    
    public class SkillLearnSystem : MonoBehaviour
    {
        public static SkillLearnSystem Instance;
        
        [Header("- Skills to Unlock -")]
        [SerializeField] private List<SkillUnlockCondition> _skillsToUnlock;
        
        [Header("- Initial Skills -")]
        [Tooltip("Skills that are available if a new game file is started.")]
        [SerializeField] private List<PlayerSkillPrefab> _initialSkillsAvailablePrefabs;
        [Tooltip("Skills that are chosen for normal and hc bar if a new game file is started.")]
        [SerializeField] private List<PlayerSkillPrefab> _initialSkillsChosenPrefabs;

        [Header("- UI -")]
        [SerializeField] private float _padding = 50f;
        [SerializeField] private Transform _parentInfoPanels;
        [SerializeField] private GameObject _panelNewSkillUnlocked;
        [SerializeField] private List<InfoPanelSkill> _infoPanelSkills;
        [SerializeField] private TextMeshProUGUI _textSkillUnlocked;

        [Header("- DEBUG -")] 
        [SerializeField] private List<PlayerSkillPrefab> _DEBUG_SKILLS_UNLOCK_AT_START;


        #region Mono

        private void Awake()
        {
            if (Instance != null)
                Debugger.LogInstanceError(GetType());
            Instance = this;
        }

        private void Start()
        {
            EncounterManager.Instance.OnEncounterSuccess.AddListener(OnEncounterSuccess);
            DebugMode.Instance.OnDebugEnabled.AddListener(DEBUG_LEARN_ALL_SKILLS);
            
        }
        #endregion


        #region Life Cycle
        public void LearnInitialSkills()
        {
            PlayerSkillConfiguration.Instance.AssignInitialSetup(_initialSkillsAvailablePrefabs, _initialSkillsChosenPrefabs);
        }
        #endregion
        
        
        
        #region Encoutner Success
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

                Skill skillUnlocked = PlayerSkillConfiguration.Instance.UnlockSkill(condition.SkillPrefab, PlayerSkillConfiguration.BarType.ActiveNormalHc);
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
        #endregion

        
        #region Debug
        private void DEBUG_LEARN_ALL_SKILLS()
        {
            foreach (PlayerSkillPrefab prefab in _DEBUG_SKILLS_UNLOCK_AT_START)
            {
                PlayerSkillConfiguration.Instance.UnlockSkill(prefab, PlayerSkillConfiguration.BarType.ActiveNormalHc);
            }
        }
        #endregion
    }
}
