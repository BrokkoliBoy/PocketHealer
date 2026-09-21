using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gavi
{
    public class GameFileUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TMP_InputField _characterName;
        [SerializeField] private UIButton _loginButton;
        [SerializeField] private UIButton _confirmNameButton;
        [SerializeField] private UIButton _deleteButton;

        private SafeFile _safeFile;


        #region Mono
        private void Awake()
        {
            _loginButton.Button.onClick.AddListener(OnLoginButtonPressed);
            _confirmNameButton.Button.onClick.AddListener(OnConfirmNameButtonPressed);
            _deleteButton.Button.onClick.AddListener(OnDeleteButtonPressed);
            HideNameUI();
        }
        #endregion


        #region Life Cycle
        public void ApplyGameFileUi(SafeFile safeFile)
        {
            if (safeFile == null)
            {
                _name.text = "ERROR";
                _safeFile = null;
                return;
            }
            _name.text = safeFile.CharacterName == "" ? SafeFile.DefaultCharacterName : safeFile.CharacterName;
            _safeFile = safeFile;

            bool hasExistingCharacter = _safeFile.CharacterName != "" && !_safeFile.CharacterName.Contains(SafeFile.DefaultCharacterName);
            if (hasExistingCharacter)
                HideNameUI();
            else
                ShowNameUI();
            SetDeleteButtonActive(hasExistingCharacter);
        }
        #endregion


        #region Button Clicks
        public void OnLoginButtonPressed()
        {
            if (_safeFile == null)
                return;

            Login();
        }

        public void OnConfirmNameButtonPressed()
        {
            _safeFile.SetCharacterName(_characterName.text);
            Login();
        }

        public void OnDeleteButtonPressed()
        {
            if (_safeFile == null)
                return;

            GameFileManager.Instance.DeleteSafeFile(_safeFile);
            GameFileUiManager.Instance.ShowUis();
        }
        #endregion


        #region Login
        private void Login()
        {
            GameFileManager.Instance.LoadGameFile(_safeFile);
        }
        #endregion

        
        #region Set Name
        private void ShowNameUI()
        {
            StartCoroutine(DeactivateLoginButtonNextFrame());
            _confirmNameButton.gameObject.SetActive(true);
            _characterName.gameObject.SetActive(true);
        }

        private IEnumerator DeactivateLoginButtonNextFrame()
        {
            yield return null;
            _loginButton.gameObject.SetActive(false);
        }

        private void HideNameUI()
        {
            _loginButton.gameObject.SetActive(true);
            _confirmNameButton.gameObject.SetActive(false);
            _characterName.gameObject.SetActive(false);
        }
        #endregion


        #region Delete Button
        private void SetDeleteButtonActive(bool active)
        {
            if (active)
                _deleteButton.gameObject.SetActive(true);
            else
                StartCoroutine(DeactivateDeleteButtonNextFrame());
        }

        private IEnumerator DeactivateDeleteButtonNextFrame()
        {
            yield return null;
            _deleteButton.gameObject.SetActive(false);
        }
        #endregion
    }
}