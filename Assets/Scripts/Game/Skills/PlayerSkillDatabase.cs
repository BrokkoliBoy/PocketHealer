using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Utility;
using UnityEngine;

namespace Gavi
{
    
    public class PlayerSkillDatabase : MonoBehaviour
    {
        public static PlayerSkillDatabase Instance;
        
        [SerializeField] private List<PlayerSkillPrefab> _skillPrefabs;

        private void Awake()
        {
            if (Instance != null)
                Debugger.LogInstanceError(typeof(PlayerSkillDatabase));
            Instance = this;
        }


        public PlayerSkillPrefab GetPrefabByGUID(string guid)
        {
            foreach (PlayerSkillPrefab prefab in _skillPrefabs)
            {
                if (prefab.GUID == guid)
                    return prefab;
            }

            Debugger.LogError("Couldn't find skill prefab for GUID " + guid);
            return null;
        }
    }
}
