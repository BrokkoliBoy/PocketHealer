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
        public List<Skill> SkillsAvailable => _skillsAvailable;
        private List<Skill> _skillsAvailable = new ();
        // This list filled with all spots at the initial setup. Empty slots are null.
        public List<Skill> SkillsChosenNormalHc => _skillsChosenNormalHc;
        private List<Skill> _skillsChosenNormalHc = new ();
        public List<Skill> SkillsChosenMythic => _skillsChosenMythic;
        private List<Skill> _skillsChosenMythic = new ();

        private bool _isInitialized;
        public bool IsInConfigurationMenu => _isInConfigurationMenu;
        private bool _isInConfigurationMenu;
        
        public enum BarType { ActiveNormalHc, ActiveMythic, Available }
        
        
        #region Mono
        private void Awake()
        {
            Instance = this;
            
            _skillBarNormalHc.Initialize();
            _skillBarNormalHc.SkillDragged.AddListener(OnSkillDraggedNormalHc);
            _skillBarNormalHc.SkillDroppedPhysically.AddListener(OnSkillDroppedPhysicallyNormalHc);
            AssignInitialKeyBindings(_skillBarNormalHc);
            
            _skillBarMythic.Initialize();
            _skillBarMythic.SkillDragged.AddListener(OnSkillDraggedMythic);
            _skillBarMythic.SkillDroppedPhysically.AddListener(OnSkillDroppedPhysicallyMythic);
            AssignInitialKeyBindings(_skillBarMythic);
            
            for (int barIndex = 0; barIndex < _skillBarsAvailable.Count; barIndex++) 
            {
                SkillBar bar = _skillBarsAvailable[barIndex];
                bar.Initialize();
                bar.SkillDragged.AddListener(OnSkillDraggedAvailable);
                bar.SkillDroppedPhysically.AddListener(OnSkillDroppedPhysicallyAvailable);
            }
            EncounterManager.Instance.OnEncounterClear.AddListener(OnEncounterClear);
        }
        #endregion


        #region Life Cycle
        public void AssignInitialSetup(List<PlayerSkillPrefab> initialSkillsAvailablePrefabs, List<PlayerSkillPrefab> initialSkillsChosenPrefabs)
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            foreach (PlayerSkillPrefab prefab in initialSkillsChosenPrefabs)
            {
                if (prefab == null)
                    continue;
                UnlockSkill(prefab, BarType.ActiveNormalHc);
            }

            foreach (PlayerSkillPrefab prefab in initialSkillsAvailablePrefabs)
            {
                if (prefab == null)
                    continue;
                UnlockSkill(prefab, BarType.Available);
            }
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
        /// Unlocks and thus instantiates the skill and adds it to the specified list of skills. If the bar type is
        /// active but active is full, it will automatically add it to the list of available ones.
        /// </summary>
        /// <param name="skillPrefab">Skill to be learned. Must be a prefab.</param>
        /// <param name="addToBarType">Bar to add the skill to. Either the bar of active skills or the bar of available
        /// skills.</param>
        public Skill UnlockSkill(PlayerSkillPrefab skillPrefab, BarType addToBarType)
        {
            if (HasSkillUnlocked(skillPrefab))
                return null;
            
            Skill skill = Instantiate(skillPrefab, SkillManagerPlayer.Instance.SkillsParent).Skill;
            skill.SkillPrefab = skillPrefab;

            bool wasAddedToActiveSkills = false;
            if (addToBarType == BarType.ActiveNormalHc)
            {
                wasAddedToActiveSkills = AddSkillToList(skill, _skillsChosenNormalHc, _skillBarNormalHc.ZoneCount);
                if (wasAddedToActiveSkills)
                {
                    GameFileManager.Instance.SaveCurrentGameFile();
                    return skill;
                }
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (addToBarType == BarType.Available || (addToBarType == BarType.ActiveNormalHc && !wasAddedToActiveSkills))
            {
                bool wasAddedToAvailableSkills = AddSkillToList(skill, _skillsAvailable, -1);
                if (wasAddedToAvailableSkills)
                {
                    GameFileManager.Instance.SaveCurrentGameFile();
                    return skill;
                }
            }
            
            Debugger.LogError("Tried to learn skill " + skill.Name + " but there was no space available.");
            return null;
        }

        /// <summary>
        /// Unlocks and thus instantiates the skill and adds it to the specified list of skills. Given the index it will
        /// attempt to put the skill to the specified index position of said bar(s). 
        /// </summary>
        /// <param name="skillPrefab">Skill to be learned. Must be a prefab.</param>
        /// <param name="addToBarType">Bar to add the skill to. Either the bar of active skills or the bar of available
        /// skills.</param>
        /// <param name="skillIndex">Index of the bar(s) to put the skill to. If multiple bars are available, it
        /// will go from top to bottom bar and increase the index with every bar entry.</param>
        public Skill UnlockSkillFromSafeFile(PlayerSkillPrefab skillPrefab, BarType addToBarType, int skillIndex)
        {
            if (HasSkillUnlocked(skillPrefab))
                return null;

            Skill skill = Instantiate(skillPrefab, SkillManagerPlayer.Instance.SkillsParent).Skill;
            skill.SkillPrefab = skillPrefab;

            int maxSize = -1;
            List<Skill> listOfSkills = null;
            if (addToBarType == BarType.ActiveNormalHc)
            {
                maxSize = _skillBarNormalHc.ZoneCount;
                listOfSkills = _skillsChosenNormalHc;
            }
            else if (addToBarType == BarType.ActiveMythic)
            {
                maxSize = _skillBarMythic.ZoneCount;
                listOfSkills = _skillsChosenMythic;
            }
            else if (addToBarType == BarType.Available)
            {
                maxSize = -1;
                listOfSkills = _skillsAvailable;
            }
            bool success = AddSkillToList(skill, listOfSkills, maxSize, skillIndex);
            if (!success)
                return null;
            return skill;
        }


        /// <summary>
        /// Adds the (already instantiated) skill to the first available space of the available skills.
        /// If there is no space on the skill bars, the skill will not be added and false will be returned, else true.
        /// </summary>
        /// <param name="skill">Skill to add.</param>
        /// <param name="listOfSkills">List of skills the skill should be added to.</param>
        /// <param name="maxSize">Maximum size of the list. If smaller 0, no limit is applied.</param>
        /// <param name="index">Specifies the index of the skill to be added at. If smaller 0, the first available
        /// index will be chosen.</param>
        /// <returns>Returns if the skill was successfully added to the skill bar.</returns>
        private bool AddSkillToList(Skill skill, List<Skill> listOfSkills, int maxSize, int index = -1)
        {
            if (index >= 0 && maxSize >= 0 && index >= maxSize) // >= because e.g. with size = 5, index = 5 would be bad
            {
                Debugger.LogError("Tried to add skill to list, but index was higher than max size.");
                return false;
            }
            
            // if an index was specified, add spaces until index is reached. If it was reached before, make sure the
            // specified index is not occupied.
            if (index >= 0)
            {
                // add new entries until we have enough for index. No need to check max size since this was checked
                // at the start of the function.
                while (listOfSkills.Count <= index)
                    listOfSkills.Add(null);
                if (listOfSkills[index] != null)
                    return false;
                listOfSkills[index] = skill;

                // UpdateSkillBars();
                return true;
            }
            
            // if no index was specified, just append to first empty slot. 
            for (int i = 0; i < listOfSkills.Count; i++)
            {
                if (listOfSkills[i] != null)
                    continue;
                listOfSkills[i] = skill;
                // UpdateSkillBars();
                GameFileManager.Instance.SaveCurrentGameFile();
                return true;
            }

            // if all slots are occupied, add new entry to the list, but only if it doesn't exceed maxSize.
            if (maxSize < 0 || listOfSkills.Count < maxSize)
                listOfSkills.Add(skill);
            else
                return false;
            
            // UpdateSkillBars();
            GameFileManager.Instance.SaveCurrentGameFile();
            return true;
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
        }
        #endregion
        

        #region Events
        public void OnSkillDraggedAvailable(DragAndDroppableSkill droppableSkill, SkillBar bar, int zoneIndex)
        {
            droppableSkill.RootTransform.SetParent(_draggedParent);
        }

        public void OnSkillDroppedPhysicallyAvailable(DragAndDroppableSkill droppableSkill, SkillBar bar, int zoneIndex)
        {
            RemoveSkillFromLists(droppableSkill.SkillUi.Skill);
            int barZoneIndex = 0;
            for (int barIndex = 0; barIndex < _skillBarsAvailable.Count; barIndex++)
            {
                SkillBar tBar = _skillBarsAvailable[barIndex];
                if (tBar == bar)
                {
                    AddSkillToList(droppableSkill.SkillUi.Skill, _skillsAvailable, -1, barZoneIndex + zoneIndex);
                    GameFileManager.Instance.SaveCurrentGameFile();
                    break;
                }
                barZoneIndex += tBar.ZoneCount;
            }
        }
        
        public void OnSkillDraggedNormalHc(DragAndDroppableSkill droppableSkill, SkillBar bar, int zoneIndex)
        {
            droppableSkill.RootTransform.SetParent(_draggedParent);
        }
        
        public void OnSkillDroppedPhysicallyNormalHc(DragAndDroppableSkill droppableSkill, SkillBar bar, int zoneIndex)
        {
            RemoveSkillFromLists(droppableSkill.SkillUi.Skill);
            AddSkillToList(droppableSkill.SkillUi.Skill, _skillsChosenNormalHc, _skillBarNormalHc.ZoneCount, zoneIndex);
            GameFileManager.Instance.SaveCurrentGameFile();
        }
        
        public void OnSkillDraggedMythic(DragAndDroppableSkill droppableSkill, SkillBar bar, int zoneIndex)
        {
            droppableSkill.RootTransform.SetParent(_draggedParent);
        }
        
        public void OnSkillDroppedPhysicallyMythic(DragAndDroppableSkill droppableSkill, SkillBar bar, int zoneIndex)
        {
            RemoveSkillFromLists(droppableSkill.SkillUi.Skill);
            AddSkillToList(droppableSkill.SkillUi.Skill, _skillsChosenMythic, _skillBarMythic.ZoneCount, zoneIndex);
            GameFileManager.Instance.SaveCurrentGameFile();
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

        public bool HasSkillUnlocked(PlayerSkillPrefab skill)
        {
            foreach (Skill skillAvailable in _skillsAvailable)
            {
                if (skillAvailable == null)
                    continue;
                if (skillAvailable.SkillPrefab == skill)
                    return true;
            }
            foreach (Skill skillChosen in _skillsChosenNormalHc)
            {
                if (skillChosen == null)
                    continue;
                if (skillChosen.SkillPrefab == skill)
                    return true;
            }
            
            return false;
        }
        #endregion
    }
}
