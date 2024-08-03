using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Bosses;

namespace Gavi.Encounter
{
    public class EncounterUi : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField] private KeyCode _keyCodePauseMenu = KeyCode.Escape;
        [SerializeField] private EncounterPanelPreparation PanelPanelPreparation;
        [SerializeField] private EncounterPanelPauseMenu _panelPauseMenu;
        [SerializeField] private GameObject _panelSuccessMenu;
        [SerializeField] private GameObject _panelFailureMenu;
        
        [SerializeField] private RectTransform _encounterAvatarTransform;
        #endregion


        #region Mono
        private void Update()
        {
            if (!EncounterManager.Instance.IsInEncounter)
                return;
            if (Input.GetKeyDown(_keyCodePauseMenu))
            {
                if (EncounterManager.Instance.IsPaused)
                    EncounterManager.Instance.ResumeEncounter();
                if (!EncounterManager.Instance.IsPaused)
                    EncounterManager.Instance.PauseEncounter();
            }
        }

        private void OnEnable()
        {
            EncounterManager.Instance.OnEncounterInitialize.AddListener(OnEncounterInitialize);
            EncounterManager.Instance.OnEncounterResumed.AddListener(OnEncounterResumed);
            EncounterManager.Instance.OnEncounterPaused.AddListener(OnEncounterPaused);
            EncounterManager.Instance.OnEncounterSuccess.AddListener(OnEncounterSuccess);
            EncounterManager.Instance.OnEncounterFailure.AddListener(OnEncounterFailure);
            EncounterManager.Instance.OnEncounterStart.AddListener(OnEncounterStart);
        }
        private void OnDisable()
        {
            EncounterManager.Instance.OnEncounterInitialize.RemoveListener(OnEncounterInitialize);
            EncounterManager.Instance.OnEncounterResumed.RemoveListener(OnEncounterResumed);
            EncounterManager.Instance.OnEncounterPaused.RemoveListener(OnEncounterPaused);
            EncounterManager.Instance.OnEncounterSuccess.RemoveListener(OnEncounterSuccess);
            EncounterManager.Instance.OnEncounterFailure.RemoveListener(OnEncounterFailure);
            EncounterManager.Instance.OnEncounterStart.RemoveListener(OnEncounterStart);
        }
        #endregion


        #region Life Cycle
        private void OnEncounterInitialize()
        {
            DeactivateAllPanels();
            PanelPanelPreparation.SetActive(true);
            EncounterManager.Instance.CurrentEncounter.AliveEnemies[0].EnemyUi.RootTransform.position = _encounterAvatarTransform.position;
        }
        
        private void OnEncounterStart()
        {
            DeactivateAllPanels();
            PanelPanelPreparation.SetActive(false);
        }
        
        private void OnEncounterResumed()
        {
            DeactivateAllPanels();
            // _panelPauseMenu.SetActive(false);
        }

        private void OnEncounterPaused()
        {
            DeactivateAllPanels();
            _panelPauseMenu.SetActive(true);
        }
        
        private void OnEncounterSuccess()
        {
            DeactivateAllPanels();
            _panelSuccessMenu.SetActive(true);
        }

        private void OnEncounterFailure()
        {
            DeactivateAllPanels();
            _panelFailureMenu.SetActive(true);
        }

        private void DeactivateAllPanels()
        {
            PanelPanelPreparation.SetActive(false);
            _panelPauseMenu.SetActive(false);
            _panelSuccessMenu.SetActive(false);
            _panelFailureMenu.SetActive(false);
        }
        #endregion
    }
}
