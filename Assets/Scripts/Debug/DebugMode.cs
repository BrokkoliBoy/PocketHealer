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

        [Header("- Independent Debug Toggles -")]
        [Tooltip("If true, the DEBUG Kill Enemy skill is part of the starting loadout on a new save file.")]
        [SerializeField] private bool _startWithDebugKillSkill;
        public bool StartWithDebugKillSkill => _startWithDebugKillSkill;

        [Tooltip("If true, the debug test-boss encounter (\"Boss Debug\" button) is selectable in the Choose Encounter menu.")]
        [SerializeField] private bool _enableBossDebugEncounter;
        public bool EnableBossDebugEncounter => _enableBossDebugEncounter;

        [Tooltip("If true, the \"Debug Mode\" button itself is shown in the main menu.")]
        [SerializeField] private bool _enableDebugModeButton;
        public bool EnableDebugModeButton => _enableDebugModeButton;

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
