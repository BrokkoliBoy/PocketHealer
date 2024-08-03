using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Gavi.Characters;
using Gavi.Utility;
using UnityEngine.Serialization;

namespace Gavi.Encounter
{
    public class EncounterManager : MonoBehaviour
    {
        public static EncounterManager Instance;

        
        #region Serialized Variables
        public List<Encounter> EncounterPrefabs => _encounterPrefabs;
        [SerializeField] private List<Encounter> _encounterPrefabs;

        [SerializeField] private Transform _encounterInstantiateRoot;
        [SerializeField] private RectTransform _bossUiParentTransform;
        public RectTransform BossUiParentTransform => _bossUiParentTransform;
        #endregion


        #region Non-Serialized Variables
        public Encounter CurrentEncounter => _currentEncounter;
        public UnityEvent OnEncounterSpawn;
        public UnityEvent OnEncounterInitialize;
        public UnityEvent OnEncounterStart;
        public UnityEvent OnEncounterResumed;
        public UnityEvent OnEncounterPaused;

        public UnityEvent OnEncounterSuccess;
        public UnityEvent OnEncounterFailure;
        public UnityEvent OnEncounterAborted;
        public UnityEvent OnEncounterStopped;
        public UnityEvent OnEncounterClear;

        private Encounter _currentEncounter;
        private bool _isInEncounter;
        public bool IsInEncounter => _isInEncounter;

        private bool _isInitialized;
        public bool IsInitialized => _isInitialized;

        private bool _isPaused;
        public bool IsPaused => _isPaused;

        [SerializeField] private float _timeSinceEncounterStart;
        public float TimeSinceEncounterStart => _timeSinceEncounterStart;

        public bool IsInEncounterAndUnpaused => !_isPaused && IsInEncounter && IsInitialized;
        private Encounter _encounterToSpawn;
        
        // encounter info
        public int HighestEncounterIndexNormal => _encounterCountNormal; // not ... - 1 or ... + 1
        private int _encounterCountNormal;
        public int HighestEncounterIndexHeroic => _encounterCountHeroic; // not ... - 1 or ... + 1
        private int _encounterCountHeroic;
        private int _encounterCountMythic;
        private int _encounterCountMythicPlus;
        #endregion
        
        
        #region Mono
        public void Awake()
        {
            if (Instance != null)
                Utility.Debugger.LogError("Instance != null on EncounterManager!");
            Instance = this;
            GatherEncounterInfo();
        }

        private void Update()
        {
            if (!IsInEncounterAndUnpaused)
                return;
            
            _timeSinceEncounterStart += Simulation.DeltaTime;
        }
        #endregion


        #region Life Cycle
        public void SpawnEncounter()
        {
            if (_encounterToSpawn == null)
            {
                Debugger.LogAssertionFail("_encounterToSpawn == null");
                return;
            }
            if (_currentEncounter != null)
            {
                Debugger.LogError("_currentEncounter != null in EncounterManager.SpawnEncounter");
                return;
            }
            Encounter encounter = Instantiate(_encounterToSpawn, _encounterInstantiateRoot);
            if (encounter == null)
            {
                Debugger.LogError("encounter == null in EncounterManager.SpawnEncounter");
                return;
            }
            
            _currentEncounter = encounter;
            encounter.EncounterPrefab = _encounterToSpawn;
            OnEncounterSpawn?.Invoke();
            StartCoroutine(InitializeEncounter());
        }

        private IEnumerator InitializeEncounter()
        {
            yield return null;
            _isInitialized = true;
            _isPaused = false;
            _timeSinceEncounterStart = 0;
            OnEncounterInitialize.Invoke();
        }

        public void StartEncounter()
        {
            _isInEncounter = true;
            _isPaused = false;
            OnEncounterStart.Invoke();
        }

        public void ResumeEncounter()
        {
            _isPaused = false;
            OnEncounterResumed.Invoke();
        }

        public void PauseEncounter()
        {
            _isPaused = true;
            OnEncounterPaused.Invoke();
        }
        
        // called from quit button
        public void AbortEncounter()
        {
            OnEncounterAborted.Invoke();
            StopEncounter();
        }

        public void FailEncounter()
        {
            OnEncounterFailure.Invoke();
            StopEncounter();
        }

        public void SuccessEncounter()
        {
            OnEncounterSuccess.Invoke();
            StopEncounter();
        }

        private void StopEncounter()
        {
            _isInEncounter = false;
            _isInitialized = false;
            _isPaused = false;
            OnEncounterStopped.Invoke();
            _currentEncounter = null;
        }

        public void LeaveEncounter() // called from event, when the encounter panel is completely closed
        {
            ClearEncounter();
        }

        private void ClearEncounter()
        {
            StopEncounter();
            OnEncounterClear.Invoke();
            _currentEncounter = null;
        }
        #endregion


        #region Control
        public void SetEncounterIndex(int encounterNumber, Encounter.EncounterDifficulty difficulty)
        {
            foreach (Encounter encounterPrefab in _encounterPrefabs)
            {
                if (encounterPrefab.EncounterNumber == encounterNumber && encounterPrefab.Difficulty == difficulty)
                {
                    _encounterToSpawn = encounterPrefab;
                    return;
                }
            }
            
            Debugger.LogError("Tried to start fight with number " + encounterNumber + " and difficulty " + difficulty + " but no match was found! " +
                              "Either you forgot to add the desired encounter to the list of encounters or you typo'ed the number or difficulty on the " +
                              "EncounterChooser script.");
        }
        #endregion


        #region Encounter Info
        private void GatherEncounterInfo()
        {
            foreach (Encounter encounter in _encounterPrefabs)
            {
                if (encounter.EncounterNumber == 0)
                    continue;
                if (encounter.Difficulty == Encounter.EncounterDifficulty.Normal)
                    _encounterCountNormal++;
                if (encounter.Difficulty == Encounter.EncounterDifficulty.Heroic)
                    _encounterCountHeroic++;
                if (encounter.Difficulty == Encounter.EncounterDifficulty.Mythic)
                    _encounterCountMythic++;
                if (encounter.Difficulty == Encounter.EncounterDifficulty.MythicPlus)
                    _encounterCountMythicPlus++;
            }
        }
        #endregion
    }        
}
