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
        
        private SafeFile _safeFile;

        
        #region Mono
        private void Awake()
        {
            _loginButton.Button.onClick.AddListener(OnLoginButtonPressed);
            _confirmNameButton.Button.onClick.AddListener(OnConfirmNameButtonPressed);
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
        }
        #endregion


        #region Button Clicks
        public void OnLoginButtonPressed()
        {
            if (_safeFile == null)
                return;

            if (_safeFile.CharacterName == "" || _safeFile.CharacterName.Contains(SafeFile.DefaultCharacterName))
                ShowNameUI();
            else
                Login();
        }

        public void OnConfirmNameButtonPressed()
        {
            _safeFile.SetCharacterName(_characterName.text);
            Login();
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
            _loginButton.gameObject.SetActive(false);
            _confirmNameButton.gameObject.SetActive(true);
            _characterName.gameObject.SetActive(true);
        }

        private void HideNameUI()
        {
            _loginButton.gameObject.SetActive(true);
            _confirmNameButton.gameObject.SetActive(false);
            _characterName.gameObject.SetActive(false);
        }
        #endregion
    }
}