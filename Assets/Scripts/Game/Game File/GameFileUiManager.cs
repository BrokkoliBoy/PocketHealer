using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Utility;
using UnityEngine;

namespace Gavi
{
    public class GameFileUiManager : MonoBehaviour
    {
        public static GameFileUiManager Instance;

        [SerializeField] private List<GameFileUI> _uis;

        private void Awake()
        {
            if (Instance != null)
                Debugger.LogInstanceError(typeof(GameFileUiManager));
            Instance = this;
        }

        // called from event in login panel
        public void ShowUis()
        {
            List<GameFile> files = GameFileManager.Instance.GenerateGameFilesFromDisk();

            for (int i = 0; i < _uis.Count; i++)
            {
                if (i >= files.Count)
                    _uis[i].SetGameFileUi(null);
                else
                    _uis[i].SetGameFileUi(files[i]);
            }
        }
    }
}