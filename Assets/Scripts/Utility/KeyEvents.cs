using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gavi.Utility
{
    public class KeyEvents : MonoBehaviour
    {
        [System.Serializable]
        private class KeyEvent
        {
            [SerializeField] private KeyCode _keyCode;
            public KeyCode KeyCode => _keyCode;

            [SerializeField] private UnityEvent _event;
            public UnityEvent Event => _event;
        }

        [SerializeField] private List<KeyEvent> _keyEvents;

        private void Update()
        {
            if (_keyEvents.Count == 0)
                return;

            for(int i = 0; i < _keyEvents.Count; i++)
            {
                if (Input.GetKeyDown(_keyEvents[i].KeyCode))
                    _keyEvents[i].Event.Invoke();
            }
        }

    }

}
