using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi
{
    public class HoverInfoTarget : UiTarget
    {
        [SerializeField] private float _showDelay = 1.5f;

        private float _newHoverTime;
        
        #region Mouse Events

        protected override void OnMouseHoverStart()
        {
            _newHoverTime = 0;
        }

        public override void OnMouseHover()
        {
            base.OnMouseHover();
            _newHoverTime += Time.deltaTime;
            if (!_isMouseHold && _newHoverTime >= _showDelay)
                ShowInfoPanel();
            if (_isMouseHold)
            {
                HideInfoPanel();
                _newHoverTime = 0;
            }
        }

        protected override void OnMouseHoverStop()
        {
            base.OnMouseHoverStop();
            HideInfoPanel();
            _newHoverTime = 0;
        }
        #endregion


        #region Panel Control
        protected virtual void ShowInfoPanel()
        {
            
        }

        protected virtual void HideInfoPanel()
        {
            
        }
        #endregion
    }
}
