using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Encounter;
using Gavi.Skills;
using UnityEngine;

namespace Gavi.Player
{
    public class CastManagerPlayer : CastManager
    {
        [SerializeField] private List<KeyCode> _keyCodeAbortCast = new List<KeyCode> { KeyCode.Space };

        protected override void Update()
        {
            if (!EncounterManager.Instance.IsInEncounterAndUnpaused)
                return;
            
            foreach (KeyCode keyCode in _keyCodeAbortCast)
                if (Input.GetKeyDown(keyCode))
                    AbortCasting();
            
            base.Update();
        }
    }
}
