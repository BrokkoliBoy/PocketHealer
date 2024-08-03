using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gavi.UI
{
    public class SliderBar : Bar
    {
        [SerializeField] private Slider _bar;

        protected override void Awake()
        {
            base.Awake();
            if (_bar == null)
                _bar = GetComponent<Slider>();
        }

        protected override void SetValue(float value)
        {
            if (_bar == null)
                return;
            _bar.value = value;
        }
        
        protected override void Enable()
        {
            if (_bar == null)
                return;
            
            _bar.gameObject.SetActive(true);
        }
    
        protected override void Disable()
        {
            if (_bar == null)
                return;
            
            _bar.gameObject.SetActive(false);
        }
    }
    
}
