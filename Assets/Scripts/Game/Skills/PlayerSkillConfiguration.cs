using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Encounter;
using Gavi.Skills;
using Gavi.Utility;
using UnityEngine;

namespace Gavi
{
    public class PlayerSkillConfiguration : MonoBehaviour
    {
        public static PlayerSkillConfiguration Instance;

        public SkillBar SkillBarNormalHc => _skillBarNormalHc;
        [Header("- References -")] 
        [SerializeField] private SkillBar _skillBarNormalHc;
        public SkillBar SkillBarMythic => _skillBarMythic;
        [SerializeField] private SkillBar _skillBarMythic;
        [SerializeField] private List<SkillBar> _skillBarsAvailable;
        [SerializeField] private RectTransform _draggedParent;
        
        // This list filled with all spots at the initial setup. Empty slots are null.
        private List<Skill> _skillsAvailable = new List<Skill>();
        // This list filled with all spots at the initial setup. Empty slots are null.
        private List<Skill> _skillsChosenNormalHc = new List<Skill>();
        private List<Skill> _skillsChosenMythic = new List<Skill>();

        private bool _isInitialized;
        public bool IsInConfigurationMenu => _isInConfigurationMenu;
        private bool _isInConfigurationMenu;
        
        
        
        #region Mono
        private void Awake()
        {
            Instance = this;
            
            _skillBarNormalHc.Initialize();
            _skillBarNormalHc.SkillDragged.AddListener(OnSkillDraggedNormalHc);
            _skillBarNormalHc.SkillDroppedPhysically.AddListener(OnSkillDroppedPhysicallyNormalHc);
            // _skillBarNormalHc.SkillDroppedManually.AddListener(OnSkillDroppedManuallyNormalHc);
            
            _skillBarMythic.Initialize();
            _skillBarMythic.SkillDragged.AddListener(OnSkillDraggedMythic);
            _skillBarMythic.SkillDroppedPhysically.AddListener(OnSkillDroppedPhysicallyMythic);
            // _skillBarMythic.SkillDroppedManually.AddListener(OnSkillDroppedManuallyMythic);
            
            for (int barIndex = 0; barIndex < _skillBarsAvailable.Count; barIndex++) 
            {
                SkillBar bar = _skillBarsAvailable[barIndex];
                bar.Initialize();
                bar.SkillDragged.AddListener(OnSkillDraggedAvailable);
                bar.SkillDroppedPhysically.AddListener(OnSkillDroppedPhysicallyAvailable);
                // zone.SkillDroppedManually.AddListener(OnSkillDroppedManuallyAvailable);
            }
            EncounterManager.Instance.OnEncounterClear.AddListener(OnEncounterClear);
        }
        #endregion


        #region Life Cycle
        public void AssignInitialSetup(List<Skill> initialSkillsAvailablePrefabs, List<Skill> initialSkillsChosenPrefabs)
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            if (initialSkillsAvailablePrefabs == null || initialSkillsChosenPrefabs == null)
                return;

            // add initial available skills to list of available skills
            int skillIndex = 0;
            for (int barIndex = 0; barIndex < _skillBarsAvailable.Count; barIndex++)
            {
                SkillBar bar = _skillBarsAvailable[barIndex];
                bar.Initialize();
                for (int zoneIndex = 0; zoneIndex < bar.ZoneCount; zoneIndex++)
                {
                    // add skills from the initial skills available list if skillIndex is in range and != null
                    if (skillIndex < initialSkillsAvailablePrefabs.Count && initialSkillsAvailablePrefabs[skillIndex] != null)
                    {
                        Skill skill = Instantiate(initialSkillsAvailablePrefabs[skillIndex], SkillManagerPlayer.Instance.SkillsParent);
                        skill.SkillPrefab = initialSkillsAvailablePrefabs[skillIndex];
                        _skillsAvailable.Add(skill);
                    }
                    else // add null else to have the _skillsAvailable filled with skills and nulls on for each bar zone
                        _skillsAvailable.Add(null);
                    skillIndex++;
                }
            }

            
            // add initial active skills to list of active skills
            skillIndex = 0;
            _skillBarNormalHc.Initialize();
            AssignInitialKeyBindings(_skillBarNormalHc);
            for (int zoneIndex = 0; zoneIndex < _skillBarNormalHc.ZoneCount; zoneIndex++)
            {
                if (skillIndex < initialSkillsChosenPrefabs.Count && initialSkillsChosenPrefabs[skillIndex] != null)
                {
                    Skill skill = Instantiate(initialSkillsChosenPrefabs[skillIndex], SkillManagerPlayer.Instance.SkillsParent);
                    skill.SkillPrefab = initialSkillsChosenPrefabs[skillIndex];
                    _skillsChosenNormalHc.Add(skill);
                }
                else
                    _skillsChosenNormalHc.Add(null);
                skillIndex++;
            }
            
            _skillBarMythic.Initialize();
            AssignInitialKeyBindings(_skillBarMythic);
            
            UpdateSkillBars();
        }

        private void AssignInitialKeyBindings(SkillBar bar)
        {
            for (int index = 0; index < bar.ZoneCount; index++)
            {
                KeyCode keyCode = KeyCode.None;
                if (index == 0) keyCode = KeyCode.Alpha1;
                if (index == 1) keyCode = KeyCode.Alpha2;
                if (index == 2) keyCode = KeyCode.Alpha3;
                if (index == 3) keyCode = KeyCode.Alpha4;
                if (index == 4) keyCode = KeyCode.Alpha5;
                if (index == 5) keyCode = KeyCode.Alpha6;
                if (index == 6) keyCode = KeyCode.Alpha7;
                if (index == 7) keyCode = KeyCode.Alpha8;
                if (index == 8) keyCode = KeyCode.Alpha9;
                if (index == 9) keyCode = KeyCode.Alpha0;
                bar.Zones[index].KeyBinding.SetKeyCodePrimary(keyCode);
            }
        }

        /// <summary>
        /// Instantiates the skill and adds it to either the list of active skills (if there is space left)
        /// or to the list of available skills (else).
        /// </summary>
        /// <param name="skillPrefab">Skill to be learned. Must be a prefab.</param>
        public Skill UnlockSkill(Skill skillPrefab, bool addSkillToLists = true)
        {
            if (HasSkillUnlocked(skillPrefab, true))
                return null;
            
            Skill skill = Instantiate(skillPrefab, SkillManagerPlayer.Instance.SkillsParent);
            skill.SkillPrefab = skillPrefab;
            if (!addSkillToLists)
                return skill;
            
            bool wasAddedToActiveSkills = AddSkillToList(skill, _skillsChosenNormalHc);
            if (wasAddedToActiveSkills)
                return skill;
            bool wasAddedToAvailableSkills = AddSkillToList(skill, _skillsAvailable);
            if (!wasAddedToAvailableSkills)
                Debugger.LogError("Tried to learn skill " + skill.Name + " but there was no space available.");
            return skill;
        } 
        
        /// <summary>
        /// Adds the (already instantiated) skill to the first available space of the available skills.
        /// If there is no space on the skill bars, the skill will not be added and false will be returned, else true.
        /// </summary>
        /// <param name="skill">Skill to add.</param>
        /// <param name="listOfSkills">List of skills the skill should be added to.</param>
        /// <returns>Returns if the skill was successfully added to the skill bar.</returns>
        private bool AddSkillToList(Skill skill, List<Skill> listOfSkills)
        {
            for (int i = 0; i < listOfSkills.Count; i++)
            {
                if (listOfSkills[i] != null)
                    continue;
                
                listOfSkills[i] = skill;
                UpdateSkillBars();
                return true;
            }
            return false;
        }
        #endregion
        
        
        #region Ui
        private void UpdateSkillBars()
        {
            foreach (SkillBar bar in _skillBarsAvailable)
                bar.EjectAllZones();
            _skillBarNormalHc.EjectAllZones();
            _skillBarMythic.EjectAllZones();

            AddSkillsToAvailableBars();
            AddSkillsToNormalHcBar();
            AddSkillsToMythicBar();
        }

        private void AddSkillsToAvailableBars()
        {
            int barIndex = 0;
            int zoneIndex = 0;
            for (int skillIndex = 0; skillIndex < _skillsAvailable.Count; skillIndex++)
            {
                if (barIndex >= _skillBarsAvailable.Count)
                {
                    Debugger.LogError("Not enough bars available!");
                    break;
                }

                Skill skill = _skillsAvailable[skillIndex];
                SkillBar bar = _skillBarsAvailable[barIndex];
                bar.AddSkillManually(skill, zoneIndex);
                if (skill != null)
                    skill.UI.NeutralizeUi();
                zoneIndex++;
                if (zoneIndex >= bar.ZoneCount)
                {
                    barIndex++;
                    zoneIndex = 0;
                }
            }
        }

        private void AddSkillsToNormalHcBar()
        {
            for(int skillIndex = 0; skillIndex < _skillsChosenNormalHc.Count; skillIndex++)
            {
                Skill skill = _skillsChosenNormalHc[skillIndex];
                _skillBarNormalHc.AddSkillManually(skill, skillIndex); // skillIndex = zoneIndex
                if (skill != null)
                    skill.UI.NeutralizeUi();
            }
        }
        
        private void AddSkillsToMythicBar()
        {
            for(int skillIndex = 0; skillIndex < _skillsChosenMythic.Count; skillIndex++)
            {
                Skill skill = _skillsChosenMythic[skillIndex];
                _skillBarMythic.AddSkillManually(skill, skillIndex); // skillIndex = zoneIndex
                if (skill != null)
                    skill.UI.NeutralizeUi();
            }
        }

        private void RemoveSkillFromLists(Skill skill)
        {
            if (_skillsAvailable.Contains(skill))
                _skillsAvailable[_skillsAvailable.IndexOf(skill)] = null;
            else if (_skillsChosenNormalHc.Contains(skill))
                _skillsChosenNormalHc[_skillsChosenNormalHc.IndexOf(skill)] = null;
            else if (_skillsChosenMythic.Contains(skill))
                _skillsChosenMythic[_skillsChosenMythic.IndexOf(skill)] = null;
            else
                ;// Debugger.LogError("Skill not found in any list in PlayerSkillConfiguration.RemoveSkillFromList(" +
                //                    skill.Name + ")!");
        }
        #endregion
        

        #region Events
        public void OnSkillDraggedAvailable(DragAndDroppableSkill droppableSkill, SkillBar bar, int zoneIndex)
        {
            droppableSkill.RootTransform.SetParent(_draggedParent);
            // _skillsAvailable.Remove(skill.SkillUi.Skill);
        }

        public void OnSkillDroppedPhysicallyAvailable(DragAndDroppableSkill droppableSkill, SkillBar bar, int zoneIndex)
        {
            RemoveSkillFromLists(droppableSkill.SkillUi.Skill);
            int skillIndex = 0;
            int barZoneIndex = 0;
            for (int barIndex = 0; barIndex < _skillBarsAvailable.Count; barIndex++)
            {
                SkillBar tBar = _skillBarsAvailable[barIndex];
                if (tBar == bar)
                {
                    _skillsAvailable[barZoneIndex + zoneIndex] = droppableSkill.SkillUi.Skill;
                    break;
                }
                barZoneIndex += tBar.ZoneCount;
            }
        }
        
        public void OnSkillDraggedNormalHc(DragAndDroppableSkill droppableSkill, SkillBar bar, int zoneIndex)
        {
            droppableSkill.RootTransform.SetParent(_draggedParent);
            // _skillsChosenNormalHc[zoneIndex] = droppableSkill.SkillUi.Skill;
        }
        
        public void OnSkillDroppedPhysicallyNormalHc(DragAndDroppableSkill droppableSkill, SkillBar bar, int zoneIndex)
        {
            RemoveSkillFromLists(droppableSkill.SkillUi.Skill);
            _skillsChosenNormalHc[zoneIndex] = droppableSkill.SkillUi.Skill;
        }
        
        public void OnSkillDraggedMythic(DragAndDroppableSkill droppableSkill, SkillBar bar, int zoneIndex)
        {
            droppableSkill.RootTransform.SetParent(_draggedParent);
            // _skillsChosenMythic.Remove(skill.SkillUi.Skill);
        }
        
        public void OnSkillDroppedPhysicallyMythic(DragAndDroppableSkill droppableSkill, SkillBar bar, int zoneIndex)
        {
            RemoveSkillFromLists(droppableSkill.SkillUi.Skill);
            _skillsChosenMythic[zoneIndex] = droppableSkill.SkillUi.Skill;
        }

        
        public void OnOpenPanelConfiguration() // called from UI
        {
            UpdateSkillBars();
            _isInConfigurationMenu = true;
        }

        public void OnClosePanelConfiguration()// called from UI
        {
            _isInConfigurationMenu = false;
        }
        
        private void OnEncounterClear()
        {
            UpdateSkillBars();
        }
        #endregion


        #region Getter
        public List<Skill> GetSkills(Encounter.Encounter.EncounterDifficulty difficulty)
        {
            if (difficulty == Encounter.Encounter.EncounterDifficulty.Normal ||
                difficulty == Encounter.Encounter.EncounterDifficulty.Heroic)
                return _skillsChosenNormalHc;
            if (difficulty == Encounter.Encounter.EncounterDifficulty.Mythic)
                return _skillsChosenMythic;
            // fallback
            return new List<Skill>();
        }

        public bool HasSkillUnlocked(Skill skill, bool isPrefab = false)
        {
            foreach (Skill skillAvailable in _skillsAvailable)
            {
                if (skillAvailable == null)
                    continue;
                if ((!isPrefab && skillAvailable == skill) || (isPrefab && skillAvailable.SkillPrefab == skill))
                    return true;
            }
            foreach (Skill skillChosen in _skillsChosenNormalHc)
            {
                if (skillChosen == null)
                    continue;
                if ((!isPrefab && skillChosen == skill) || (isPrefab && skillChosen.SkillPrefab == skill))
                    return true;
            }
            
            return false;
        }
        #endregion
    }
}
