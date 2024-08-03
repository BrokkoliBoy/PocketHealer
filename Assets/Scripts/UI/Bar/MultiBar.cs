using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Gavi
{
    public class MultiBar : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        // public TextMeshProUGUI NameText => _nameText;

        public string Text
        {
            set
            {
                if (_text == null)
                    return;
                _text.text = value;
            }
        }
        
        public virtual void SetValue(int index, float value)
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
