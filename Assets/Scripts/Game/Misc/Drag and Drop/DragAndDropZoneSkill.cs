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
            base.DropPhysically(droppable);
            ContainingDroppable = droppable as DragAndDroppableSkill;
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
