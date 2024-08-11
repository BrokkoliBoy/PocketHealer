using System.Collections;
using System.Collections.Generic;
using Gavi.UI;
using UnityEngine;

namespace Gavi.States
{
    public class StateUiManager : MonoBehaviour
    {
        [SerializeField] private Transform _stateUiParentTransform;
        [SerializeField] private StateUi _stateUiPrefab;
        [SerializeField] private float _xOffset = 3.5f;

        private List<UI> _uis = new List<UI>();
        //private Dictionary<State, StateUi> _stateUis;

        #region Mono
        private void Update()
        {
            UpdateUi();
        }
        #endregion


        #region UI Control
        private void UpdateUi()
        {
            for (int i = 0; i < _uis.Count; i++)
            {
                UI ui = _uis[i];
                ui.StateUi.DurationRate = ui.State.StateData.MaxDuration <= 0 ? 0 : ui.State.RemainingDuration / ui.State.StateData.MaxDuration;
                ui.StateUi.PeriodRate = ui.State.StateData.PeriodicCooldown <= 0 ? 0 : ui.State.CurrentPeriod / ui.State.StateData.PeriodicCooldown;
                ui.StateUi.DurationText = "" + ((int)ui.State.RemainingDuration);
                if (_stateUiParentTransform != null)
                    ui.StateUi.Transform.position = _stateUiParentTransform.position + new Vector3(_xOffset * i, 0, 0);
            }
        }


        public void AddStateUi(State state)
        {
            if (_stateUiPrefab == null || _stateUiParentTransform == null)
                return;

            StateUi stateUi = Instantiate(_stateUiPrefab, _stateUiParentTransform).GetComponent<StateUi>();
            stateUi.Icon = state.StateData.Icon;
            _uis.Add(new UI(state, stateUi));
        }


        public void RemoveStateUi(State state)
        {
            for (int i = _uis.Count - 1; i >= 0; i--)
            {
                if (_uis[i].State == state)
                {
                    Destroy(_uis[i].StateUi.gameObject);
                    _uis.RemoveAt(i);
                }
            }
        }
        #endregion



        private class UI
        {
            public State State;
            public StateUi StateUi;

            public UI(State state, StateUi stateUi)
            {
                State = state;
                StateUi = stateUi;
            }
        }
    }
}
