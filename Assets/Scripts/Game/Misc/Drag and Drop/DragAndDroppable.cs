using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Control;
using Gavi.Utility;
using UnityEngine;

namespace Gavi
{
    public class DragAndDroppable : UiTarget
    {
        public RectTransform RootTransform => _rootTransform;
        [SerializeField] private RectTransform _rootTransform;

        private bool _isDragging;
        public DragAndDropZone HomeZone { get; set; }


        public static DragAndDroppable CurrentDragged;


        #region Mono
        private void OnDisable()
        {
            if (_isDragging)
                DropBackToHomeZone();
        }

        protected override void Update()
        {
            if (!_isDragging)
                return;
            OnDrag();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (_isDragging && Input.GetKeyUp(KeyCode.Mouse0))
                Drop();
        }

        private void OnDestroy()
        {
            //Debugger.LogError("DragAndDroppable destroyed! This path is not yet implemented.");
        }

        #endregion

        
        #region Dragging & Dropping
        protected virtual void DragStart()
        {
            if (!PlayerSkillConfiguration.Instance.IsInConfigurationMenu)
                return;
            
            if (CurrentDragged != null)
            {
                Debugger.LogError("CurrentDragged != null in DragAndDroppable.OnDragStart()");
                return;
            }

            CurrentDragged = this;
            _isDragging = true;
            if (HomeZone != null)
                HomeZone.Drag();
        }

        protected virtual void OnDrag()
        {
            if (_rootTransform == null)
                return;

            Vector2 mousePosition = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UIMaster.Mastercanvas.GetComponent<RectTransform>(), (Vector2) Input.mousePosition, UIMaster.Camera, out mousePosition);
            _rootTransform.position = UIMaster.Mastercanvas.transform.TransformPoint(mousePosition);
        }

        protected virtual void Drop()
        {
            CurrentDragged = null;
            _isDragging = false;
            foreach (UiTarget uiTarget in PlayerInputManager.Instance.UiTargets)
            {
                if (uiTarget is DragAndDropZone zone && zone.IsValidDroppable(this))
                {
                    zone.DropPhysically(this);
                    return;
                }
            }

            DropBackToHomeZone();
            
        }
        #endregion


        #region Zone
        public virtual void DropBackToHomeZone()
        {
            if (HomeZone == null)
                return;
            // Debug.Log("Drop to home droppable: " + name + " // " + HomeZone.name);
            HomeZone.DropManually(this);
        }

        public virtual void OnDroppedIntoZone(DragAndDropZone zone)
        {
            // Debug.Log("OnDroppedIntoZone doppable " + name);
            HomeZone = zone;
        }
        #endregion


        #region Mouse
        public override void OnMousePressedDown()
        {
            if (_isDragging)
                return;
            DragStart();
        }

        // public override void OnMouseHold()
        // {
        //     if (!_isDragging)
        //         return;
        //     OnDrag();
        // }
        
        // public override void OnMousePressedUp()
        // {
        //     if (!_isDragging)
        //         return;
        //     Drop();
        // }
        #endregion
    }
}
