using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi
{
    public class HoverInfoMaster : MonoBehaviour
    {
        public static HoverInfoMaster Instance;
        
        public static RectTransform ParentHoverInfoPanel => Instance._parentHoverInfoPanel;
        [SerializeField] private RectTransform _parentHoverInfoPanel;

        public static InfoPanelSkill PrefabPanelSkill => Instance._prefabPanelSkill;
        [SerializeField] private InfoPanelSkill _prefabPanelSkill;
        
        private void Awake()
        {
            Instance = this;
        }
    }
}
