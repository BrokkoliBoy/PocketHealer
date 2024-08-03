using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi
{
    public class InfoPanel : MonoBehaviour
    {
        public RectTransform Root => _root;
        [SerializeField] private RectTransform _root;
    }
}
