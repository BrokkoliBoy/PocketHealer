using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi
{
    public class EncounterPanelPauseMenu : MonoBehaviour
    {
        [SerializeField] private RectTransform _panel;
        [SerializeField] private RectTransform _parentEnemySkillInfoPanel;
        
        
        #region Life Cycle
        public void SetActive(bool isActive)
        {
            _panel.gameObject.SetActive(isActive);
            EnemySkillsInfoPanel.Instance.ShowPanel(_parentEnemySkillInfoPanel);
        }
        #endregion
    }
}
