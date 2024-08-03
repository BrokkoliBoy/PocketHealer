using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Characters;
using Gavi.Skills;
using UnityEngine;

namespace Gavi
{
    public class EncounterPanelPreparation : MonoBehaviour
    {
        // public static EncounterPanelPreparation Instance;

        [SerializeField] private RectTransform _panel;
        [SerializeField] private RectTransform _parentEnemySkillInfoPanel;

        #region Mono
        private void Awake()
        {
            // Instance = this;
            Encounter.EncounterManager.Instance.OnEncounterInitialize.AddListener(OnEncounterInitialize);
        }
        #endregion

        #region Life Cycle
        public void SetActive(bool isActive)
        {
            _panel.gameObject.SetActive(isActive);
            EnemySkillsInfoPanel.Instance.ShowPanel(_parentEnemySkillInfoPanel);
        }
        #endregion
        
        private void OnEncounterInitialize()
        {
            // StartCoroutine(ShowEnemySkillsInfoPanelDelayed());
        }

        // private IEnumerator ShowEnemySkillsInfoPanelDelayed()
        // {
        //     yield return null;
        // }
    }
}
