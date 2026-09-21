using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Gavi.Skills;
using Gavi.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Gavi
{
    public class SkillBar : MonoBehaviour
    {
        [Header("- Settings -")]
        [SerializeField] private int _minDropZones = 0;
        [SerializeField] private int _maxDropZones = 6;

        [Space] 
        [SerializeField] private float _paddingLeft;
        [SerializeField] private float _paddingRight;
        [SerializeField] private bool _addHalfMargin = true;
        
        [Header("- References -")] 
        [SerializeField] private GameObject _prefabDropZone;
        [SerializeField] private RectTransform _parentSkillUis;
        [SerializeField] private RectTransform _parentSkillDropZones;
        
        
        public UnityEvent<DragAndDroppableSkill, SkillBar, int> SkillDragged; // int = zoneIndex
        public UnityEvent<DragAndDroppableSkill, SkillBar, int> SkillDroppedPhysically; // int = zoneIndex
        public UnityEvent<DragAndDroppableSkill, SkillBar, int> SkillDroppedManually; // int = zoneIndex
        public List<DragAndDropZoneSkill> Zones => _zones;
        private List<DragAndDropZoneSkill> _zones = new ();
        public int ZoneCount => _zones.Count;
        private bool _isInitialized;
        
        #region Mono
        private void Start()
        {
            Initialize();   
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            // add already existing zones
            for (int i = 0; i < _parentSkillDropZones.childCount; i++)
            {
                DragAndDropZoneSkill zone = _parentSkillDropZones.GetChild(i).GetChild(0).GetComponent<DragAndDropZoneSkill>();
                if (zone == null)
                    continue;
                if (!zone.enabled || !zone.RootTransform.gameObject.activeSelf)
                    continue;

                _zones.Add(zone);
                zone.PreSkillDragged.AddListener(OnSkillDragged);
                zone.PostSkillDroppedPhysically.AddListener(OnSkillDroppedPhysically);
                zone.PostSkillDroppedManually.AddListener(OnSkillDroppedManually);
            }

            while (_zones.Count < _minDropZones)
                AddZone();

            UpdateUi();
        }
        #endregion

        #region Control
        private bool AddZone()
        {
            Initialize();
            if (ZoneCount >= _maxDropZones)
                return false;
            
            DragAndDropZoneSkill zone = Instantiate(_prefabDropZone, _parentSkillDropZones).GetComponentInChildren<DragAndDropZoneSkill>();
            if (zone == null)
            {
                Debugger.LogError("zone == null in SkillBar.AddZone()!");
                return false;
            }
            
            _zones.Add(zone);
            zone.PreSkillDragged.AddListener(OnSkillDragged);
            zone.PostSkillDroppedPhysically.AddListener(OnSkillDroppedPhysically);
            zone.PostSkillDroppedManually.AddListener(OnSkillDroppedManually);
            UpdateUi();
            return true;
        }

        public void OnSkillDragged(DragAndDropZoneSkill zone, DragAndDroppableSkill droppableSkill)
        {
            if (!_zones.Contains(zone))
            {
                Debugger.LogError("!_zones.Contains(zone) in SkillBar.OnSkillDragged(...)!");
                return;
            }
            SkillDragged.Invoke(droppableSkill, this, _zones.IndexOf(zone));
        }
        
        public void OnSkillDroppedPhysically(DragAndDropZoneSkill zone, DragAndDroppableSkill droppableSkill)
        {
            if (!_zones.Contains(zone))
            {
                Debugger.LogError("!_zones.Contains(zone) in SkillBar.OnSkillDroppedPhysically(...)!");
                return;
            }
            
            SetUiParent(droppableSkill.SkillUi.Skill);
            SkillDroppedPhysically.Invoke(droppableSkill, this, _zones.IndexOf(zone));
        }
        
        public void OnSkillDroppedManually(DragAndDropZoneSkill zone, DragAndDroppableSkill droppableSkill)
        {
            if (!_zones.Contains(zone))
            {
                Debugger.LogError("!_zones.Contains(zone) in SkillBar.OnSkillDroppedManually(...)!");
                return;
            }
            
            SetUiParent(droppableSkill.SkillUi.Skill);
            SkillDroppedManually.Invoke(droppableSkill, this, _zones.IndexOf(zone));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="skillIndex">If equal -1, just put the skill into the first available free zone.</param>
        public bool AddSkillManually(Skill skill, int zoneIndex = -1)
        {
            Initialize();
            if (skill != null && skill.DontUseUi)
                return false;

            if (skill != null && zoneIndex < 0)
            {
                for (int i = 0; i < _maxDropZones; i++)
                {
                    if (i < ZoneCount)
                    {
                        if (_zones[i].ContainingDroppable == null)
                        {
                            SetUiParent(skill);
                            _zones[i].DropManually(skill.UI.DragAndDroppable);
                            return true;
                        }
                    }
                    else
                    {
                        bool success = AddZone();
                        if (!success)
                        {
                            Debugger.LogError("You tried to assign a Skill to a zone index of a SkillBar that is too high for its maximum capacity!");
                            return false;
                        }
                        SetUiParent(skill);
                        _zones[ZoneCount - 1].DropManually(skill.UI.DragAndDroppable);
                        return true;
                    }
                }
            }
            else
            {
                while (ZoneCount <= zoneIndex) // if zoneIndex == 4, then ZoneCount must be at least 5
                {
                    bool success = AddZone();
                    if (!success)
                    {
                        Debugger.LogError("You tried to assign a Skill to a zone index of a SkillBar that is too high for its maximum capacity!");
                        return false;
                    }
                }
            }
            
            // Debug.Log("Adding " + (skill == null ? "null" : skill.Name) + " // " + (_zones[zoneIndex].ContainingDroppable == null ? "null" : _zones[zoneIndex].ContainingDroppable.SkillUi.Skill.Name));
            if (_zones[zoneIndex].ContainingDroppable != null)
            {
                Debugger.LogError("You tried to assign a Skill to a zone of a SkillBar that already contains a Skill. This is currently not supported!");
                return false;
            }
            
            // skill can be null. That means that there is a gap in the SkillBar
            if (skill == null)
                return true;

            SetUiParent(skill);
            _zones[zoneIndex].DropManually(skill.UI.DragAndDroppable);
            
            return true;
        }

        public void EjectAllZones()
        {
            Initialize();
            foreach (DragAndDropZoneSkill zone in _zones)
            {
                zone.Eject();
            }

            if (_zones[0].ContainingDroppable != null)
                Debug.Log(_zones[0].ContainingDroppable.SkillUi.Skill.Name);
        }

        public void EjectZone(Skill skill)
        {
            if (skill == null)
                return;
            for (int i = 0; i < ZoneCount; i++)
            {
                if (_zones[i] != null && 
                    _zones[i].ContainingDroppable != null &&
                    _zones[i].ContainingDroppable.SkillUi!= null && 
                    _zones[i].ContainingDroppable.SkillUi.Skill == skill)
                {
                    _zones[i].Eject();
                    return;
                }
            }
            
            Debugger.LogError("Couldn't find Skill " + skill.Name + " in SkillBar " + name);
        }
        #endregion


        #region Getter
        public DragAndDropZoneSkill GetZoneOfSkill(Skill skill)
        {
            if (skill == null)
                return null;

            foreach (DragAndDropZoneSkill z in Zones)
            {
                if (z.ContainingDroppable == skill.UI.DragAndDroppable)
                    return z;
            }
            return null;
        }

        public int GetIndexOfZone(DragAndDropZoneSkill zone)
        {
            if (zone == null)
                return -1;

            for (int i = 0; i < Zones.Count; i++)
            {
                if (Zones[i] == zone)
                    return i;
            }

            return -1;
        }
        #endregion
        
        
        #region Ui
        private void SetUiParent(Skill droppableSkill)
        {
            if (droppableSkill.UI == null)
                droppableSkill.InstantiateSkillUi(_parentSkillUis);
            else
                droppableSkill.UI.SetParent(_parentSkillUis);
        }

        private void UpdateUi()
        {
            if (_zones.Count == 0)
                return;

            if (ZoneCount == 1)
            {
                _zones[0].RootTransform.anchoredPosition = new Vector2(_parentSkillDropZones.anchoredPosition.x, _parentSkillDropZones.anchoredPosition.y);
                return;
            }
            
            float widthTotal = _parentSkillDropZones.rect.width;
            float widthIncludingPadding = widthTotal - _paddingLeft - _paddingRight;
            float widthZone = _prefabDropZone.GetComponent<RectTransform>().rect.width;
            int count = _zones.Count;

            float step = widthZone * (widthIncludingPadding - widthZone) / (widthZone * Mathf.Max(1, count - 1));
            float startPos = _parentSkillDropZones.anchoredPosition.x - widthTotal * 0.5f + _paddingLeft + widthZone * 0.5f;
            for (int i = 0; i < count; i++)
            {
                DragAndDropZone zone = _zones[i];
                zone.RootTransform.anchoredPosition = new Vector2(startPos + i * step, _parentSkillDropZones.anchoredPosition.y);
                zone.UpdateDroppablePosition();
            }
        }
        #endregion
    }
}
