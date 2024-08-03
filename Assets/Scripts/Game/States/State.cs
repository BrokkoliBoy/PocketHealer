using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Skills;
using Gavi.Characters;
using Gavi.Utility;

namespace Gavi.States
{
    public class State : MonoBehaviour
    {
        public StateData StateData => _stateData;
        private StateData _stateData;
        private StateManager _stateManager;
        public StateManager Manager => _stateManager;
        
        public float RemainingDuration
        {
            get => _remainingDuration;
            set => _remainingDuration = value;
        }
        private float _remainingDuration;

        public float CurrentPeriod => _currentPeriod;
        private float _currentPeriod;

        private Character _character => _stateManager != null ? _stateManager.Character : null;

        public List<StateAura> Auras => _auras;
        private List<StateAura> _auras = new List<StateAura>();
        
        
        #region Life Cycle
        public void ApplyStateData(StateData stateData, StateManager manager)
        {
            _stateManager = manager;
            _stateData = stateData;

            RemainingDuration = stateData.MaxDuration;

            foreach (StateAura aura in stateData.Auras)
            {
                StateAura newAura = Instantiate(aura.gameObject, transform).GetComponent<StateAura>();
                newAura.ApplyStateAura(aura, this);
                AddAura(newAura);
            }
            foreach (SkillEffect effect in _stateData.OnStateEnterEffects)
                effect.PerformEffect(_character);
        }

        /// <summary>
        /// Only redirects the actual call to remove the state to the manager. Do NOT call this function from the manager.
        /// </summary>
        public void RemoveState()
        {
            if (_stateManager == null)
            {
                Utility.Debugger.LogError("_stateManager == null in RemoveState() on " + gameObject.name);
                Destroy(gameObject);
                return;
            }
            _stateManager.RemoveState(this);
        }

        public void OnRemoveState()
        {
            foreach (SkillEffect effect in _stateData.OnStateExitEffects)
                effect.PerformEffect(_character);
            Destroy(gameObject);
        }
        #endregion


        #region Update
        private void Update()
        {
            UpdateDuration();
            UpdatePeriod();
        }

        private void UpdateDuration()
        {
            RemainingDuration -= Simulation.DeltaTime;
            if (RemainingDuration <= 0)
                RemoveState();
        }

        private void UpdatePeriod()
        {
            if (_stateData.PeriodicCooldown <= 0)
                return;

            _currentPeriod += Simulation.DeltaTime;
            if (_currentPeriod >= _stateData.PeriodicCooldown)
            {
                _currentPeriod -= _stateData.PeriodicCooldown;
                foreach (SkillEffect effect in _stateData.OnStatePeriodicEffects)
                    effect.PerformEffect(_character);
            }
        }
        #endregion
        
        
        #region Auras
        public void AddAura(StateAura aura)
        {
            if (_auras.Contains(aura))
            {
                Debugger.LogError("_auras.Contains(aura) in State.AddAura(" + aura + ") on " + aura.name);
                return;
            }
            _auras.Add(aura);
        }

        public void RemoveAura(StateAura aura)
        {
            if (!_auras.Contains(aura))
            {
                Debugger.LogError("!_auras.Contains(aura) in State.AddAura(" + aura + ") on " + aura.name);
                return;
            }

            _auras.Remove(aura);
            if (_stateData.RemoveStateOnRemoveLastAura && _auras.Count == 0)
                RemoveState();
        }
        #endregion
    }
}