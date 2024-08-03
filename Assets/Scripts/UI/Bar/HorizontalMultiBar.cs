using System.Collections;
using System.Collections.Generic;
using Gavi.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Gavi
{
    public class HorizontalMultiBar : MultiBar
    {
        [SerializeField] private List<Image> _bars;
        [SerializeField] private GameObject _prefabBar;
        [SerializeField] private Transform _parentBars;
        
        public override void SetValue(int index, float value)
        {
            while (_bars.Count <= index)
            {
                if (_prefabBar == null)
                {
                    Debugger.LogError("_prefabBar == null in HorizontalMultiBar.SetValue(" + index + ", " + value + ")");
                    return;
                }
                if (_parentBars == null)
                {
                    Debugger.LogError("_parentBars == null in HorizontalMultiBar.SetValue(" + index + ", " + value + ")");
                    return;
                }
                if (_prefabBar.GetComponent<Image>() == null)
                {
                    Debugger.LogError("_prefabBar.GetComponent<Image>() == null in HorizontalMultiBar.SetValue(" + index + ", " + value + ")");
                    return;
                }

                GameObject barObject = Instantiate(_prefabBar, _parentBars);
                Image bar = barObject.GetComponent<Image>();
                _bars.Add(bar);
            }

            _bars[index].fillAmount = value;
        }
    }
}
