using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.States
{
    public class StateAura : MonoBehaviour
    {
        [SerializeField] private bool _removeStateOnRemoveAura = true;
        public State State => _state;
        protected State _state;
        
        public virtual void ApplyStateAura(StateAura origin, State state)
        {
            _state = state;
        }

        public virtual void RemoveAura()
        {
            if (_removeStateOnRemoveAura)
                _state.RemoveState();
            else
                _state.RemoveAura(this);
        }
    }
}
