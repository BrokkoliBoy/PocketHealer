using System.Collections;
using System.Collections.Generic;
using Gavi.Utility;
using UnityEngine;

namespace Gavi.States
{
    public class StateAuraBarrier : StateAura
    {
        // public FloatRange Barrier => _barrier;
        [SerializeField] private FloatRange _barrier;
        // public int Duration => _duration;
        // [SerializeField] private int _duration;

        public float BarrierLeft => _barrierLeft;
        private float _barrierLeft = -1;

        public void RemoveBarrier(float barrier)
        {
            _barrierLeft = Mathf.Max(0, _barrierLeft - barrier);
            _state.Manager.Character.Ui.UpdateHealth();
            if (_barrierLeft <= 0)
                RemoveAura();
            
        }

        public override void ApplyStateAura(StateAura origin, State state)
        {
            base.ApplyStateAura(origin, state);
            StateAuraBarrier aura = (StateAuraBarrier)origin;
            _barrierLeft = aura._barrier.NewValue;
            _state.Manager.Character.Ui.UpdateHealth();
        }
    }
}
