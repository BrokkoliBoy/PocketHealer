using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Characters;
using Gavi.Control;
using Gavi.Encounter;
using Gavi.Player.Skills;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gavi.Skills
{
    public class ActivationTypePlayerInput : ActivationType
    {
        // private KeyBinding _binding; // optional, could also be done via the DragAndDropZoneSkill.KeyBinding
        private MouseClick _mouseClick;
        private Skill _skill;
       
        private void Awake()
        {
            // _binding = GetComponent<KeyBinding>();
            _mouseClick = GetComponent<MouseClick>();
            _skill = GetComponent<Skill>();
        }

        public override bool IsActivated()
        {
            if (!EncounterManager.Instance.IsInEncounterAndUnpaused)
                return false;
            // if (_binding != null && _binding.IsPressed())
            //     return true;
            KeyBinding zoneKeyBinding = GetZoneKeyBinding();
            if (zoneKeyBinding != null && zoneKeyBinding.IsPressed())
                return true;
            if (_mouseClick.IsClickDown)
                return true;
            
            return false;
        }
        private bool IsPressedViaMouseButton()
        {
            return false;
        }
        public override void OnSuccessfullyActivated()
        {
            // _binding.OnSucessfullyActivated();
            KeyBinding zoneKeyBinding = GetZoneKeyBinding();
            if (zoneKeyBinding != null)
                zoneKeyBinding.OnSucessfullyActivated();
        }

        public KeyBinding GetZoneKeyBinding()
        {
            // Debug.Log("1: " + (_skill != null));
            // Debug.Log("2: " + (_skill.UI != null));
            // Debug.Log("3: " + (_skill.UI.DragAndDroppable != null));
            // Debug.Log("4: " + (_skill.UI.DragAndDroppable.HomeZone != null));
            // Debug.Log("5: " + (_skill.UI.DragAndDroppable.HomeZone is DragAndDropZoneSkill));
            // Debug.Log("6: " + ((_skill.UI.DragAndDroppable.HomeZone as DragAndDropZoneSkill).KeyBinding != null));
            if (_skill != null &&
                _skill.UI != null &&
                _skill.UI.DragAndDroppable != null &&
                _skill.UI.DragAndDroppable.HomeZone != null &&
                _skill.UI.DragAndDroppable.HomeZone is DragAndDropZoneSkill zoneSkill &&
                zoneSkill.KeyBinding != null)
                return zoneSkill.KeyBinding;
            return null;
        }
    }
}