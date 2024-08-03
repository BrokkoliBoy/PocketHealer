using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Encounter;

namespace Gavi
{
    public class Simulation : MonoBehaviour
    {
        public static Simulation Instance;

        [SerializeField] private float _simulationSpeed = 1;
        public static float SimulationSpeed => Instance._simulationSpeed;

        private float _time;
        public static float Time => Instance._time;

        private float _deltaTime;
        public static float DeltaTime => Instance._deltaTime;

        private void Awake()
        {
            if (Instance != null)
                Utility.Debugger.LogError("Instance != null in Simulation");
            Instance = this;
        }

        private void Update()
        {
            _deltaTime = UnityEngine.Time.deltaTime * SimulationSpeed;
            _time += _deltaTime;
            
            if (!EncounterManager.Instance.IsInEncounterAndUnpaused)
                return;
        }
    }
}
