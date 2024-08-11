using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Characters;
using UnityEngine.Serialization;

namespace Gavi.Encounter
{
    public class Encounter : MonoBehaviour
    {
        #region Serialized Variables
        [FormerlySerializedAs("_enemies")] [SerializeField] private List<Enemy> _initialenemies;
        [SerializeField] private Transform _bossInstantiateTransform;
        public int EncounterNumber => _encounterNumber;
        [Tooltip("0 is Debug, normal encounters start with 1.")]
        [SerializeField] private int _encounterNumber;
        public EncounterDifficulty Difficulty => _difficulty;
        [SerializeField] private EncounterDifficulty _difficulty;
        public enum EncounterDifficulty { Normal, Heroic, Mythic, MythicPlus }
        #endregion
        
        
        #region Non-Serialized Variables
        public List<Enemy> Enemies => _enemies;
        private List<Enemy> _enemies = new List<Enemy>();
        public List<Enemy> AliveEnemies => _aliveEnemies;
        private List<Enemy> _aliveEnemies = new List<Enemy>();
        private int _currentBossIndex;

        public EncounterPartySettings PartySettings => _partySettings;
        private EncounterPartySettings _partySettings;

        [HideInInspector] public Encounter EncounterPrefab;
        #endregion
        

        #region Mono
        private void Awake()
        {
            _partySettings = GetComponent<EncounterPartySettings>();
        }

        private void OnEnable()
        {
            EncounterManager.Instance.OnEncounterSpawn.AddListener(OnEncounterSpawn);
            EncounterManager.Instance.OnEncounterClear.AddListener(OnEncounterClear);
        }

        private void OnDisable()
        {
            EncounterManager.Instance.OnEncounterSpawn.RemoveListener(OnEncounterSpawn);
            EncounterManager.Instance.OnEncounterClear.RemoveListener(OnEncounterClear);
        }

        private void OnEncounterSpawn()
        {
            _currentBossIndex = 0;
            _aliveEnemies.Clear();
            _enemies.Clear();
            foreach (Enemy prefab in _initialenemies)
            {
                Enemy enemy = Instantiate(prefab, _bossInstantiateTransform).GetComponent<Enemy>();
                enemy.InitializeForBattle();
                _aliveEnemies.Add(enemy);
                _enemies.Add(enemy);
            }
        }

        private void OnEncounterClear()
        {
            ClearEncounter();
        }
        #endregion


        public void OnEnemyDied(Enemy enemy)
        {
            if (!_aliveEnemies.Contains(enemy))
                Utility.Debugger.LogAssertionFail("_aliveEnemies.Contains(enemy) in Encounter.OnEnemyDied(...)");
            else
            {
                _aliveEnemies.Remove(enemy);
                // Destroy(enemy.gameObject);
            }
            if (_aliveEnemies.Count <= 0)
                EncounterManager.Instance.SuccessEncounter();
        }

        private void ClearEncounter()
        {
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                _enemies[i].DestroySelf();
                _enemies.RemoveAt(i);
            }
            
            Destroy(gameObject);
        }
    }
}