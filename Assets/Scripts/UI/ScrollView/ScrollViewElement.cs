using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Utility;

namespace Gavi.UI
{
    public class ScrollViewElement : MonoBehaviour
    {
        [SerializeField] private float _height = -1;
        [SerializeField] private float _width = -1;

        public float Height => _height <= 0 ? RectTransform.sizeDelta.y : _height;
        public float Width => _width <= 0 ? RectTransform.sizeDelta.x : _width;

        public RectTransform RectTransform => GetRectTransform();
        private RectTransform GetRectTransform()
        {
            if (_rectTransform != null)
                return _rectTransform;
            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform == null)
                Debug.LogError("Error: ScrollViewElement does not have a RectTransform attached to it!");
            return _rectTransform;
        }
        private RectTransform _rectTransform;
        private ScrollView _scrollView;

        public void DestroySelf()
        {
            if (_scrollView == null)
            {
                Debugger.LogAssertionFail("In ScrollViewElement.KillSelf(): ScrolllViewElement " + name + " exected to be an element of a scroll view, but it isn't!");
                return;
            }

            _scrollView.RemoveElement(this);
        }

        public bool SetScrollView(ScrollView scrollView)
        {
            if (_scrollView != null)
            {
                Debugger.LogAssertionFail("In SetScrollView.KillSelf(): ScrolllViewElement " + name + " exected not to be an element of a scroll view");
                return false;
            }
            _scrollView = scrollView;
            return true;
        }
    }
}