using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gavi.UI
{
    public class RadialImageBar : Bar
    {
        [SerializeField] private Image _cooldownImage;


        protected override void Awake()
        {
            base.Awake();
            if (_cooldownImage == null)
                _cooldownImage = GetComponent<Image>();
        }
        
        protected override void SetValue(float value)
        {
            if (_cooldownImage == null)
                return;
            _cooldownImage.fillAmount = value;
        }
    
        protected override void Enable()
        {
            if (_cooldownImage == null)
                return;
            
            _cooldownImage.gameObject.SetActive(true);
        }
    
        protected override void Disable()
        {
            if (_cooldownImage == null)
                return;
            
            _cooldownImage.gameObject.SetActive(false);
        }
    }
}
