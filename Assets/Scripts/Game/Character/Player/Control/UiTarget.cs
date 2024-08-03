using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi
{
    public class UiTarget : MonoBehaviour
    {
        public bool IsTargetable = true;

        protected bool _isHover;
        protected bool _wasHover;
        protected float _hoverTime;
        protected bool _isMouseHold;
        
        protected virtual void Update()
        {
            
        }

        protected virtual void LateUpdate()
        {
            if (_wasHover && !_isHover)
                OnMouseHoverStop();
            _wasHover = _isHover;
            _isHover = false;
        }

        public virtual void OnMousePressedDown()
        {
            _isMouseHold = true;
        }
        
        public virtual void OnMouseHold()
        {
            
        }
        
        public virtual void OnMousePressedUp()
        {
            _isMouseHold = false;
        }

        protected virtual void OnMouseHoverStart()
        {
            _hoverTime = 0;
        }
        
        public virtual void OnMouseHover()
        {
            if (!_wasHover)
                OnMouseHoverStart();
            _isHover = true;
            _hoverTime += Time.deltaTime;
        }

        protected virtual void OnMouseHoverStop()
        {
            _hoverTime = 0;
        }
    }
}
