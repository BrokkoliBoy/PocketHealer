using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gavi
{
    public class DebugMode : MonoBehaviour
    {
        public static DebugMode Instance;

        public bool IsDebug { get; private set; }

        [SerializeField] private bool _setDebugAtStart;

        public UnityEvent OnDebugEnabled;
        
        // [SerializeField] private GameO
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (_setDebugAtStart)
                EnableDebugMode();
        }

        public void EnableDebugMode()
        {
            IsDebug = true;
            OnDebugEnabled.Invoke();
        }
    }    
}
