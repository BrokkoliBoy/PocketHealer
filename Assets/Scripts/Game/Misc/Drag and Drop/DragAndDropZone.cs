using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Control;
using Gavi.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Gavi
{
    public class DragAndDropZone : UiTarget
    {
        #region Variables
        public RectTransform RootTransform => _rootTransform;
        [SerializeField] private RectTransform _rootTransform;
        [SerializeField] private Image _imageBackground;
        [SerializeField] private Image _imageHighlightValid;
        [SerializeField] private Image _imageHighlightInvalid;

        public bool IsEmpty => _currentDroppable == null;
        protected DragAndDroppable _currentDroppable;
        #endregion
        
        
        #region Mono
        protected virtual void Awake()
        {
            // place holder for children
        }
        #endregion

        
        #region Zone
        public virtual void DropPhysically(DragAndDroppable droppable)
        {
            if (_currentDroppable != null)
            {
                DragAndDroppable oldDroppable = _currentDroppable;
                OnCurrentDroppableDragged();
                droppable.HomeZone.DropPhysically(oldDroppable);
            }

            _currentDroppable = droppable;
            _currentDroppable.OnDroppedIntoZone(this);
            UpdateDroppablePosition();
            _imageBackground.gameObject.SetActive(false);
            StopHighlight();
        }

        public virtual void DropManually(DragAndDroppable droppable)
        {
            if (_currentDroppable != null)
            {
                Debugger.LogError("_currentDroppable != null in DragAndDropZone.DropManually(...)! You currently can only manually drop a droppable into an empty DropZone.");
                return;
            }

            _currentDroppable = droppable;
            _currentDroppable.OnDroppedIntoZone(this);
            UpdateDroppablePosition();
            _imageBackground.gameObject.SetActive(false);
            StopHighlight();
        }
        
        public virtual void OnCurrentDroppableDragged()
        {
            if (_currentDroppable == null)
                return;
            _currentDroppable = null;
            _imageBackground.gameObject.SetActive(true);
        }

        public virtual void Eject()
        {
            if (_currentDroppable == null)
                return;
            _currentDroppable.HomeZone = null;
            OnCurrentDroppableDragged();
        }
        
        public void UpdateDroppablePosition()
        {
            if (_currentDroppable == null)
                return;
            
            _currentDroppable.RootTransform.anchorMin = _rootTransform.anchorMin;
            _currentDroppable.RootTransform.anchorMax = _rootTransform.anchorMax;
            _currentDroppable.RootTransform.pivot = _rootTransform.pivot;
            _currentDroppable.RootTransform.anchoredPosition = _rootTransform.anchoredPosition;
            
            // Debug.Log(_currentDroppable.RootTransform.anchorMin + " / " + _rootTransform.anchorMin + "\n" + 
            //           _currentDroppable.RootTransform.anchorMax + " / " + _rootTransform.anchorMax + "\n"+ 
            //           _currentDroppable.RootTransform.pivot + " / " + _rootTransform.pivot + "\n" +
            //           _currentDroppable.RootTransform.anchoredPosition + " / " + _rootTransform.anchoredPosition + "\n");
        }
        #endregion


        #region Droppable
        public virtual bool IsValidDroppable(DragAndDroppable droppable)
        {
            return droppable != null;
        }
        
        protected override void OnMouseHoverStart()
        {
            if (DragAndDroppable.CurrentDragged == null)
                return;
            if (IsValidDroppable(DragAndDroppable.CurrentDragged))
                HighlightValid();
            else
                HighlightInvalid();
            _imageBackground.gameObject.SetActive(false);
        }
        
        protected override void OnMouseHoverStop()
        {
            StopHighlight();
            if (_currentDroppable == null)
                _imageBackground.gameObject.SetActive(true);
        }
        #endregion

        
        #region Highligh
        private void HighlightValid()
        {
            _imageHighlightValid.gameObject.SetActive(true);
            _imageHighlightInvalid.gameObject.SetActive(false);
        }
        
        private void HighlightInvalid()
        {
            _imageHighlightValid.gameObject.SetActive(false);
            _imageHighlightInvalid.gameObject.SetActive(true);
        }

        private void StopHighlight()
        {
            _imageHighlightValid.gameObject.SetActive(false);
            _imageHighlightInvalid.gameObject.SetActive(false);
        }
        #endregion
    }
}
