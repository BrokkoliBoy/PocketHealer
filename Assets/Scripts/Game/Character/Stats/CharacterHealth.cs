using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Encounter;
using Gavi.States;
using Gavi.Utility;
using UnityEngine;

namespace Gavi.Characters
{
    public class CharacterHealth : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField][Min(1)] private float _maxHealth = 100;
        public float MaxHealth => _maxHealth;
        
        [Header("--- DEBUG ---")]
        [SerializeField] private float _DMG_PER_SECOND;
        #endregion
        
        
        #region Non-Serialized Variables
        public bool IsDead => CurrentHealth <= 0;
        
        /// returns the percent of health compared to max health
        public float CurrentHealthRate => Mathf.Max(0, CurrentHealth / _maxHealth);
        /// returns the percent of health compared to max health + barrier
        public float CurrentHealthWithoutBarrierRate => Mathf.Max(0, CurrentHealth / (_maxHealth + CurrentBarrierPoints));
        /// returns the percent of health + barrier compared to max health + barrier
        public float CurrentHealthWithBarrierRate => Mathf.Max(0, (CurrentHealth + CurrentBarrierPoints) / (_maxHealth + CurrentBarrierPoints));
        /// returns the percent of barrier compared to max health
        public float CurrentBarrierRate => Mathf.Max(0, CurrentBarrierPoints / (_maxHealth + CurrentBarrierPoints));
        
        public float CurrentHealth => _currentHealth;
        private float _currentHealth;

        public float CurrentBarrierPoints => GetTotalBarrier();
        // private float _currentBarrierPoints;

        private Character _character;
        private FloatingTextManager _textManager;
        #endregion
        
        
        #region Mono
        private void Awake()
        {
            _character = GetComponent<Character>();
            _textManager = GetComponent<FloatingTextManager>();
        }

        private void OnEnable()
        {
            _character.OnInitialize.AddListener(OnInitialize);
            EncounterManager.Instance.OnEncounterInitialize.AddListener(OnInitialize);
        }
        
        private void OnDisable()
        {
            _character.OnInitialize.RemoveListener(OnInitialize);
            EncounterManager.Instance.OnEncounterInitialize.RemoveListener(OnInitialize);
        }

        private void OnInitialize()
        {
            _currentHealth = MaxHealth;
            _character.Ui.UpdateHealth();
        }

        private void Update()
        {
            if (!EncounterManager.Instance.IsInEncounterAndUnpaused)
                return;
            if (_DMG_PER_SECOND == 0)
                return;
            TakeDamage(_DMG_PER_SECOND * Time.deltaTime);
        }
        #endregion


        #region HP Control
        public void TakeDamage(float dmg)
        {
            if (dmg <= 0)
            {
                Debugger.LogError("dmg <= 0 in CharacterHealth.TakeDamage on " + _character.Name + " (" + dmg + ")");
                return;
            }
            
            float oldHealth = _currentHealth;
            float barrierDmg = Mathf.Min(dmg, CurrentBarrierPoints);
            float healthDmg = Mathf.Min(dmg - barrierDmg, _currentHealth);
            
            LoseBarrier(barrierDmg);
            _currentHealth -= healthDmg;
            if (_textManager != null)
                _textManager.EmitHpChange(-dmg);
            if (oldHealth > 0 && _currentHealth <= 0)
                _character.Die();
            _character.Ui.UpdateHealth();
        }

        public void GainHeal(float heal)
        {
            if (heal <= 0)
            {
                Debugger.LogError("heal <= 0 in CharacterHealth.GainHeal on " + _character.Name + " (" + heal + ")");
                return;
            }
            
            float effectiveHeal = Mathf.Min(heal, MaxHealth - _currentHealth);
            _currentHealth += effectiveHeal;
            if (_textManager != null)
                _textManager.EmitHpChange(heal);
            _character.Ui.UpdateHealth();
            _character.Ui.OnHealed(effectiveHeal / MaxHealth);
        }
        #endregion


        #region Barrier
        private float GetTotalBarrier()
        {
            List<StateAuraBarrier> barriers = GetBarriers();
            float barrier = 0;
            foreach (StateAuraBarrier aura in barriers)
                barrier += aura.BarrierLeft;
            return barrier;
        }

        private List<StateAuraBarrier> GetBarriers()
        {
            List<StateAuraBarrier> barriers = new List<StateAuraBarrier>();
            foreach (State state in _character.StateManager.States)
                foreach (StateAura aura in state.Auras)
                    if (aura is StateAuraBarrier)
                        barriers.Add((StateAuraBarrier)aura);
            return barriers;
        }

        private void LoseBarrier(float barrier)
        {
            List<StateAuraBarrier> barriers = GetBarriers();
            SortBarriersByTurns(barriers);
            for (int i = barriers.Count - 1; i >= 0; i--)
            {
                float barrierLoss = Mathf.Min(barriers[i].BarrierLeft, barrier);
                barriers[i].RemoveBarrier(barrierLoss);
                barrier -= barrierLoss;
            }
        }

        /// <summary>
        /// Sorts the list of barriers by their remaining duration. Highest duration is at position 0, lowest duration is at the last position.
        /// </summary>
        /// <param name="barriers"></param>
        private void SortBarriersByTurns(List<StateAuraBarrier> barriers)
        {
            if (barriers.Count == 0)
                return;
            
            List<StateAuraBarrier> newBarriers = new List<StateAuraBarrier>();
            newBarriers.Add(barriers[0]);
            for (int oldIndex = 1; oldIndex < barriers.Count; oldIndex++)
            {
                for (int newIndex = 0; newIndex < newBarriers.Count; newIndex++)
                {
                    if (barriers[oldIndex].State.RemainingDuration > newBarriers[newIndex].State.RemainingDuration)
                    {
                        newBarriers.Insert(newIndex, barriers[oldIndex]);
                        break;
                    }
                }
            }

            for (int i = 0; i < barriers.Count; i++)
            {
                barriers[i] = newBarriers[i];
            }
        }
        #endregion
    }
}
