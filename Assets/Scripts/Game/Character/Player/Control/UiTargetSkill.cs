using System.Collections;
using System.Collections.Generic;
using Gavi.Skills;
using UnityEngine;

namespace Gavi
{
    public class UiTargetSkill : UiTarget
    {
        public Skill Skill => _skill;
        [SerializeField] private Skill _skill;
    }
}
