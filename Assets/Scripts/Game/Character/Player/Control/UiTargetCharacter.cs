using System.Collections;
using System.Collections.Generic;
using Gavi.Characters;
using UnityEngine;

namespace Gavi
{
    public class UiTargetCharacter : UiTarget
    {
        [SerializeField] private Character _character;
        public Character Character => _character;
    }
}
