using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Player.Skills;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Gavi
{
    public class DragAndDropZoneSkill : DragAndDropZone
    {
        #region Variables
        public DragAndDroppableSkill ContainingDroppable { get; set; }
        public UnityEvent<DragAndDropZoneSkill, DragAndDroppableSkill> PreSkillDragged;
        public UnityEvent<DragAndDropZoneSkill, DragAndDroppableSkill> PostSkillDroppedPhysically;
        public UnityEvent<DragAndDropZoneSkill, DragAndDroppableSkill> PostSkillDroppedManually;

        public KeyBinding KeyBinding { get { Initialize(); return _keyBinding; } }
        private KeyBinding _keyBinding;

        private bool _isInitialized;
        #endregion

        
        #region Mono
        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }
        #endregion

        
        #region Life Cycle
        private void Initialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;
            _keyBinding = GetComponent<KeyBinding>();
        }
        #endregion


        #region Overrides
        public override bool IsValidDroppable(DragAndDroppable droppable)
        {
            return base.IsValidDroppable(droppable) && droppable is DragAndDroppableSkill;
        }
        
        public override void OnCurrentDroppableDragged()
        {
            PreSkillDragged.Invoke(this, _currentDroppable as DragAndDroppableSkill);
            ContainingDroppable = null;
            base.OnCurrentDroppableDragged();
        }

        public override void DropPhysically(DragAndDroppable droppable)
        {
            DragAndDroppableSkill skillDragAndDroppable = droppable as DragAndDroppableSkill;
            if (skillDragAndDroppable == null)
            {
                base.DropPhysically(droppable);
                return;
            }
            
            // we have to make this check because when base.DropPhysically is called, before anything else happens,
            // the skill that is currently in this drop zone will be force dropped into the home zone of the incoming
            // skill. While doing so, the PlayerConfigurationManager will try to add the old skill to the list and index
            // of the incoming skill. Yet at that moment, the incoming skill has not yet been removed from that index
            // so the adding will fail. The incoming skill will only change it's index in the list of skills once
            // the below PostSkillDroppedPhysically is called (but as mentioned, at that point the old skill will 
            // already have tried to be added to the list).
            // To counter this, we remove the incoming skill from the list of skills first, and then call
            // base.DropPhysically, which will in turn do all it's stuff with the old skill.
            if (_currentDroppable != null && _currentDroppable != skillDragAndDroppable)
                PlayerSkillConfiguration.Instance.RemoveSkillFromLists(skillDragAndDroppable.SkillUi.Skill);
            
            base.DropPhysically(skillDragAndDroppable);
            ContainingDroppable = skillDragAndDroppable;
            PostSkillDroppedPhysically.Invoke(this, ContainingDroppable);
            // has to be called again because  invoking above event likely changes the droppables parent 
            UpdateDroppablePosition();
        }

        public override void DropManually(DragAndDroppable droppable)
        {
            base.DropManually(droppable);
            ContainingDroppable = droppable as DragAndDroppableSkill;
            PostSkillDroppedManually.Invoke(this, ContainingDroppable);
            // has to be called again because invoking above event likely changes the droppables parent
            UpdateDroppablePosition();
        }
        #endregion
    }        
}
