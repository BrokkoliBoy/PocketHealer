using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Encounter;
using Gavi.Utility;
using UnityEngine;

namespace Gavi
{
    public class GameProgress : MonoBehaviour
    {
        public static GameProgress Instance;
        
        
        #region Mono
        private void Awake()
        {
            Instance = this;
            EncounterManager.Instance.OnEncounterSuccess.AddListener(OnEncounterSuccess);
        }

        private void Start()
        {
            if (DebugMode.Instance.IsDebug)
            {
                ApplyDebugEncounter();
            }
        }
        #endregion
        
        
        #region Encounter
        public List<int> EncountersSuccessNormal => _encountersSuccessNormal;
        private List<int> _encountersSuccessNormal = new ();
        public List<int> EncountersSuccessHeroic => _encountersSuccessHeroic;
        private List<int> _encountersSuccessHeroic = new ();
        public List<int> EncountersSuccessMythic => _encountersSuccessMythic;
        private List<int> _encountersSuccessMythic = new ();
        public KeyValueList<int, List<int>> EncountersSuccessMythicPlus => _encountersSuccessMythicPlus;
        private KeyValueList<int, List<int>> _encountersSuccessMythicPlus = new ();

        private void OnEncounterSuccess()
        {
            Encounter.Encounter encounter = EncounterManager.Instance.CurrentEncounter;
            AddEncounterToSuccess(encounter.EncounterNumber, encounter.Difficulty, 0); // TODO: Use correct mythic plus number
        }

        private void AddEncounterToSuccess(int encounterNumber, Encounter.Encounter.EncounterDifficulty difficulty, int mythicPlusNumber = 0)
        {
            // int encounterNumber = encounter.EncounterNumber;
            // Encounter.Encounter.EncounterDifficulty difficulty = encounter.Difficulty;
            if (difficulty == Encounter.Encounter.EncounterDifficulty.Normal && !_encountersSuccessNormal.Contains(encounterNumber))
                _encountersSuccessNormal.Add(encounterNumber);
            if (difficulty == Encounter.Encounter.EncounterDifficulty.Heroic && !_encountersSuccessHeroic.Contains(encounterNumber))
                _encountersSuccessHeroic.Add(encounterNumber);
            if (difficulty == Encounter.Encounter.EncounterDifficulty.Mythic && !_encountersSuccessMythic.Contains(encounterNumber))
                _encountersSuccessMythic.Add(encounterNumber);
            if (difficulty == Encounter.Encounter.EncounterDifficulty.MythicPlus)
            {
                if (!_encountersSuccessMythicPlus.ContainsKey(encounterNumber))
                    _encountersSuccessMythicPlus.Add(encounterNumber, new List<int>());
                // TODO: Use correct mythic plus number
                if (!_encountersSuccessMythicPlus.GetFirstValue(encounterNumber).Contains(mythicPlusNumber))
                    _encountersSuccessMythicPlus.GetFirstValue(encounterNumber).Add(mythicPlusNumber); 
            }
        }

        public void LoadEncounterProgress(int encounterNumber, Encounter.Encounter.EncounterDifficulty difficulty,
            int mythicPlusNumber = 0)
        {
            AddEncounterToSuccess(encounterNumber, difficulty, mythicPlusNumber);
        }

        private void ApplyDebugEncounter()
        {
            foreach (Encounter.Encounter encounter in EncounterManager.Instance.EncounterPrefabs)
            {
                if (encounter.Difficulty != Encounter.Encounter.EncounterDifficulty.MythicPlus)
                    AddEncounterToSuccess(encounter.EncounterNumber, encounter.Difficulty);
                else
                    AddEncounterToSuccess(encounter.EncounterNumber, encounter.Difficulty, -1); // TODO: Use correct mythic plus number
            }
        }

        public bool HasEncounterSuccess(int encounterNumber, Encounter.Encounter.EncounterDifficulty difficulty, int mythicPlusNumber = 0)
        {
            if (difficulty == Encounter.Encounter.EncounterDifficulty.Normal && _encountersSuccessNormal.Contains(encounterNumber))
                return true;
            if (difficulty == Encounter.Encounter.EncounterDifficulty.Heroic && _encountersSuccessHeroic.Contains(encounterNumber))
                return true;
            if (difficulty == Encounter.Encounter.EncounterDifficulty.Mythic && _encountersSuccessMythic.Contains(encounterNumber))
                return true;
            if (difficulty == Encounter.Encounter.EncounterDifficulty.MythicPlus)
            {
                if (_encountersSuccessMythicPlus.ContainsKey(encounterNumber) && _encountersSuccessMythicPlus.GetFirstValue(encounterNumber).Contains(mythicPlusNumber))
                    return true;
            }

            return false;
        }
        #endregion


    }
}
