using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gavi.States
{
    public class StateUi : MonoBehaviour
    {
        public RectTransform Transform
        {
            get
            {
                if (_transform == null)
                    _transform = GetComponent<RectTransform>();
                return _transform;
            }
        }
        private RectTransform _transform;
        [SerializeField] private Image IconImage;
        [SerializeField] private UI.Bar DurationBar;
        [SerializeField] private UI.Bar PeriodBar;
        [SerializeField] private TMPro.TextMeshProUGUI _durationText;
        

        public Sprite Icon { set { if (IconImage != null) IconImage.sprite = value; } }
        public float DurationRate { set { if (DurationBar != null) DurationBar.Value = value; } }
        public float PeriodRate { set { if (PeriodBar != null) PeriodBar.Value = value; } }
        public string DurationText { set { if (_durationText != null) _durationText.text = value; } }
    }
}
