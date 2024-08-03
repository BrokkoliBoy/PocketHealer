using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Player.Skills;
using Ludiq.PeekCore.ReflectionMagic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Gavi
{
    public class DragAndDropZoneSkill : DragAndDropZone
    {
        [HideInInspector] public DragAndDroppableSkill ContainingDroppable;
        
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
            SkillDroppedPhysically.Invoke(this, ContainingDroppable);
            base.DropPhysically(droppable);
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
