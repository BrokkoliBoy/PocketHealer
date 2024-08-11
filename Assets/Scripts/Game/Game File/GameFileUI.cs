using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gavi
{
    public class GameFileUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;

        private GameFile _gameFile;
        
        public void SetGameFileUi(GameFile gameFile)
        {
            if (gameFile == null)
            {
                _name.text = "ERROR";
                _gameFile = null;
                return;
            }
            
            _name.text = gameFile.FileName;
            _gameFile = gameFile;
        }

        // called from event of login buttons
        public void OnButtonPressed()
        {
            if (_gameFile == null)
                return;
            
            GameFileManager.Instance.LoadGameFile(_gameFile);
        }
    }
}