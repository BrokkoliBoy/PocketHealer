using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi
{
    public class DynamicPanels : MonoBehaviour
    {
        [SerializeField] private RectTransform _parent;
        public static RectTransform Parent;

        private void Awake()
        {
            Parent = _parent;
        }
    }
}
