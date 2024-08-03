using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gavi.Utility;

namespace Gavi.UI
{
    public class ScrollView : MonoBehaviour
    {
        [Header("--- Settings ---")]
        [SerializeField] private float _topMargin;

        [Header("--- References ---")]
        //[SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _scrollViewTransform;
        [SerializeField] private RectTransform _contentTransform;

        private List<ScrollViewElement> _elements = new List<ScrollViewElement>();
        private List<float> _elementPositions = new List<float>(); // top margin included, own height not included

        [SerializeField] private ScrollViewElement _dummyPrefab;
        [SerializeField] private int _ADD_ELEMENT_INDEX;
        [SerializeField] private bool _ADD_ELEMENT;
        [SerializeField] private int _REMOVE_ELEMENT_INDEX;
        [SerializeField] private bool _REMOVE_ELEMENT;

        public int ElementCount => _elements.Count;

        private void Update()
        {
            if (_ADD_ELEMENT)
                AddElement(Instantiate(_dummyPrefab, _contentTransform).GetComponent<ScrollViewElement>(), _ADD_ELEMENT_INDEX);
            _ADD_ELEMENT = false;

            if (_REMOVE_ELEMENT)
                RemoveElement(_REMOVE_ELEMENT_INDEX);
            _REMOVE_ELEMENT = false;
        }

        public void AddElement(ScrollViewElement element, int index = -1)
        {
            bool isValid = element.SetScrollView(this);
            if (!isValid)
                return;

            if (index < 0)
                index = _elements.Count;



            element.transform.SetParent(_contentTransform);
            Vector3 newLocalScale = element.transform.localScale;
            Vector3 lossyScale = _contentTransform.lossyScale;
            newLocalScale.x *= lossyScale.x;
            newLocalScale.z *= lossyScale.z;
            newLocalScale.y *= lossyScale.y;
            element.transform.localScale = newLocalScale;

            int elementCount = _elements.Count;
            float newElementHeight = element.Height;
            for (int i = index; i < elementCount + 1; i++)
            {
                // [0]
                // [20]

                // [0, 1]
                // [20, 100]

                // [0, 2, 1]
                // [20, 100, 150]

                if (i == index)
                {
                    if (_elementPositions.Count == 0) // if list is empty
                        _elementPositions.Insert(i, -_topMargin);
                    else // else list is not empty
                    {
                        if (i < _elements.Count) // if element is not inserted at the end
                            _elementPositions.Insert(i, _elementPositions[i] - _topMargin);
                        else // else element is inserted at the end
                            _elementPositions.Insert(i, _elementPositions[i - 1] - _topMargin - _elements[i - 1].Height);
                    }
                    _elements.Insert(i, element);
                }
                else if (i > index)
                {
                    _elementPositions[i] = _elementPositions[i] - _topMargin - newElementHeight;
                }
                _elements[i].transform.localPosition = new Vector3(0, _elementPositions[i] - element.Height * 0.5f, 0);
                _elements[i].transform.position = new Vector3(_scrollViewTransform.position.x/* + _scrollViewTransform.sizeDelta.x * 0.5f*/, _elements[i].transform.position.y, _elements[i].transform.position.z);
            }
        }

        public ScrollViewElement RemoveElement(ScrollViewElement element, bool destroyElement = true)
        {
            if (!_elements.Contains(element))
            {
                Debugger.LogError("element not in list!");
                return null;
            }
            int index = _elements.IndexOf(element);
            return RemoveElement(index, destroyElement);
        }
        public ScrollViewElement RemoveElement(int index, bool destroyElement = true)
        {
            if (index < 0 || index >= _elements.Count)
            {
                Debugger.LogError("Index in ScrollView.RemoveElement is corrupt: " + index + ", max " + _elements.Count);
                return null;
            }

            ScrollViewElement element = _elements[index];
            _elements.RemoveAt(index);
            _elementPositions.RemoveAt(index);
            float removedHeight = element.Height + _topMargin;
            for (int i = index; i < _elements.Count; i++)
            {
                // [0, 1, 2, 3]
                // [20, 50, 100, 170]

                // [0, 2, 3]
                // [20, 50, 120]
                _elementPositions[i] += removedHeight;
                _elements[i].transform.localPosition = new Vector3(0, _elementPositions[i] - element.Height * 0.5f, 0);
                _elements[i].transform.position = new Vector3(_scrollViewTransform.position.x/* + _scrollViewTransform.sizeDelta.x * 0.5f*/, _elements[i].transform.position.y, _elements[i].transform.position.z);
            }

            if (destroyElement)
            {
                Destroy(element.gameObject);
                element = null;
            }

            return element;
        }

        public void ClearElements(bool destroyElements = true)
        {
            for (int i = _elements.Count - 1; i >= 0; i--)
                RemoveElement(i, destroyElements);
        }
    }
}