using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gavi.UI
{
    public class Bar : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private bool _disableWhenZero = true;
        [SerializeField] private bool _invert;
        public TextMeshProUGUI NameText => _nameText;

        public string Text
        {
            set
            {
                if (NameText == null)
                    return;
                NameText.text = value;
            }
        }
        
        public float Value
        {
            set
            {
                if ((value <= 0 && !_invert) || (value >= 1 && _invert))
                {
                    if (_disableWhenZero)
                        Disable();
                    return;
                }
                if (_disableWhenZero)
                    Enable();
                SetValue(_invert ? 1 - value : value);
            }
        }

        protected virtual void SetValue(float value)
        {
            
        }

        protected virtual void Awake()
        {
            
        }

        protected virtual void Enable()
        {
                
        }

        protected virtual void Disable()
        {
            
        }
    }
}
