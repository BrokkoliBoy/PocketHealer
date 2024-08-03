using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Encounter;
using UnityEngine;
using UnityEngine.UI;

namespace Gavi
{
    public class ChooseEncounterButton : MonoBehaviour
    {
        public int EncounterNumber => _encounterNumber;
        [SerializeField] private int _encounterNumber;
        public Encounter.Encounter.EncounterDifficulty Difficulty => _difficulty;
        [SerializeField] private Encounter.Encounter.EncounterDifficulty _difficulty;

        [Header("- References -")] 
        [SerializeField] private GameObject _root;
        [SerializeField] private GameObject _imageSuccessIndicator;
        [SerializeField] private TMPro.TextMeshProUGUI _label;
        

        public void ChooseEncounter() // called from UI: Start fight buttons
        {
            EncounterManager.Instance.SetEncounterIndex(_encounterNumber, _difficulty);
        }

        public void Show(bool show)
        {
            _root.SetActive(show);
            if (!show)
                return;
            
            SetLabel();
            bool isSuccess = GameProgress.Instance.HasEncounterSuccess(_encounterNumber, _difficulty);
            _imageSuccessIndicator.SetActive(isSuccess);

            // TODO: indicator whether or not the encounter has been beaten before
        }
        
        private void SetLabel()
        {
            if (_encounterNumber == 0)
            {
                _label.text = "Debug";
                return;
            }
            
            string difficulty = "";
            if (_difficulty == Encounter.Encounter.EncounterDifficulty.Heroic)
                difficulty = " Heroic";
            if (_difficulty == Encounter.Encounter.EncounterDifficulty.Mythic)
                difficulty = " Mythic";
            if (_difficulty == Encounter.Encounter.EncounterDifficulty.MythicPlus)
                difficulty = " Mythic+";

            _label.text = "Boss " + _encounterNumber + difficulty;
        }
    }
}
