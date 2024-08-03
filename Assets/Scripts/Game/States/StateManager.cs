using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Characters;

namespace Gavi.States
{
    public class StateManager : MonoBehaviour
    {
        [SerializeField] private Transform _stateParentTransform;
        private StateUiManager _uiManager;

        public List<State> States => _states;
        private List<State> _states = new List<State>();

        public Character Character => _character;
        private Character _character;



        #region Mono
        private void Awake()
        {
            _character = GetComponent<Character>();
            _uiManager = GetComponent<StateUiManager>();
        }

        private void OnEnable()
        {
            Encounter.EncounterManager.Instance.OnEncounterStopped.AddListener(RemoveWithoutEffects);
        }
        private void OnDisable()
        {
            Encounter.EncounterManager.Instance.OnEncounterStopped.RemoveListener(RemoveWithoutEffects);
        }
        #endregion


        #region Life Cycle
        public void AddState(StateData stateData)
        {
            State state = GetStateByData(stateData);
            if (state == null || stateData.ReaplyType == StateData.ReaplyRule.AddNewState || stateData.ReaplyType == StateData.ReaplyRule.RemoveOldAndAddNewState)
            {
                if (state != null && stateData.ReaplyType == StateData.ReaplyRule.RemoveOldAndAddNewState) // Meh
                    state.RemoveState();
                GameObject gameObject = new GameObject();
                state = Instantiate(gameObject, _stateParentTransform).AddComponent<State>();
                state.gameObject.name = _stateParentTransform.name;
                States.Add(state);
                state.ApplyStateData(stateData, this);
                if (_uiManager != null)
                    _uiManager.AddStateUi(state);
                Character.Ui.UpdateHealth();
            }
            else
            {
                if (stateData.ReaplyType == StateData.ReaplyRule.Ignore)
                    ;
                else if (stateData.ReaplyType == StateData.ReaplyRule.AddDuration)
                    state.RemainingDuration += stateData.MaxDuration;
                else if (stateData.ReaplyType == StateData.ReaplyRule.RefreshDuration)
                    state.RemainingDuration = stateData.MaxDuration;
                    
            }
        }

        public void RemoveState(StateData stateData)
        {
            State state = GetStateByData(stateData);
            if (state == null)
                return;

            RemoveState(state);
        }

        /// <summary>
        /// Removes the state and triggers its OnStateRemove effects.
        /// </summary>
        /// <param name="state"></param>
        public void RemoveState(State state)
        {
            if (!States.Contains(state))
            {
                Utility.Debugger.LogInfo("Tried to remove state (" + state.name + ") but it wasn't in the list! (on " + gameObject.name);
                return;
            }
            if (_uiManager != null)
                _uiManager.RemoveStateUi(state);
            state.OnRemoveState();
            States.Remove(state);
        }

        /// <summary>
        /// Removes all states without proccing their OnStateRemove effects.
        /// </summary>
        public void RemoveWithoutEffects()
        {
            for (int i = States.Count - 1; i >= 0; i--)
            {
                if (_uiManager != null)
                    _uiManager.RemoveStateUi(States[i]);
                Destroy(States[i].gameObject);
                States.RemoveAt(i);
            }
        }
        #endregion


        #region Getter
        /// <summary>
        /// Returns whether or not the StateManager as a State with the parametered StateData applied to it.
        /// </summary>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool IsStateAffected(StateData stateData)
        {
            foreach (State state in States)
            {
                if (state.StateData == stateData)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Looks for a State applied to the StateManager by its StateData.
        /// </summary>
        /// <param name="stateData">StateData that the State in question should have.</param>
        /// <returns>Returns the first State found which StateData equals the parametered one. Returns null if none found.</returns>
        public State GetStateByData(StateData stateData)
        {
            for (int i = 0; i < States.Count; i++)
            {
                if (States[i].StateData == stateData)
                    return States[i];
            }
            return null;
        }
        #endregion
    }
}
