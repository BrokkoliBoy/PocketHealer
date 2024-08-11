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
        public DragAndDroppableSkill ContainingDroppable
        {
            get => _containingDroppable;
            set
            {
                // Debug.Log("Setting " + (value == null ? "null" : value.SkillUi.Skill.Name));
                _containingDroppable = value;
            }
        }
        private DragAndDroppableSkill _containingDroppable;

        public UnityEvent<DragAndDropZoneSkill, DragAndDroppableSkill> SkillDragged;
        public UnityEvent<DragAndDropZoneSkill, DragAndDroppableSkill> SkillDroppedPhysically;
        public UnityEvent<DragAndDropZoneSkill, DragAndDroppableSkill> SkillDroppedManually;

        public KeyBinding KeyBinding { get { Initialize(); return _keyBinding; } }

        private KeyBinding _keyBinding;

        private bool _isInitialized;
        
        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Initialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;
            _keyBinding = GetComponent<KeyBinding>();
        }

        public override bool IsValidDroppable(DragAndDroppable droppable)
        {
            return base.IsValidDroppable(droppable) && droppable is DragAndDroppableSkill;
        }

        public override void Drag()
        {
            SkillDragged.Invoke(this, _currentDroppable as DragAndDroppableSkill);
            ContainingDroppable = null;
            base.Drag();
        }

        public override void DropPhysically(DragAndDroppable droppable)
        {
            ContainingDroppable = droppable as DragAndDroppableSkill;
            base.DropPhysically(droppable);
            SkillDroppedPhysically.Invoke(this, ContainingDroppable);
        }

        public override void DropManually(DragAndDroppable droppable)
        {
            ContainingDroppable = droppable as DragAndDroppableSkill;
            SkillDroppedManually.Invoke(this, ContainingDroppable);
            base.DropManually(droppable);
        }

        public override void OnMouseHover()
        {
            base.OnMouseHover();
        }
    }
}
