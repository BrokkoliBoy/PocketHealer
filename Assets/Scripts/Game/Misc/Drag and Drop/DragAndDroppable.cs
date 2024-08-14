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
        #region Variables
        public RectTransform RootTransform => _rootTransform;
        [SerializeField] private RectTransform _rootTransform;

        private bool _isDragging;
        public DragAndDropZone HomeZone { get; set; }

        public static DragAndDroppable CurrentDragged;
        #endregion
        

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
            UpdateDragPosition();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (_isDragging && Input.GetKeyUp(KeyCode.Mouse0))
                Drop();
        }

        private void OnDestroy()
        {
            if (CurrentDragged == this)
                CurrentDragged = null;
        }
        #endregion

        
        #region Dragging & Dropping
        protected virtual void StartDrag()
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
                HomeZone.OnCurrentDroppableDragged();
        }

        protected virtual void UpdateDragPosition()
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
            HomeZone.DropManually(this);
        }

        public virtual void OnDroppedIntoZone(DragAndDropZone zone)
        {
            HomeZone = zone;
        }
        #endregion


        #region Mouse
        public override void OnMousePressedDown()
        {
            if (_isDragging)
                return;
            StartDrag();
        }
        #endregion
    }
}
