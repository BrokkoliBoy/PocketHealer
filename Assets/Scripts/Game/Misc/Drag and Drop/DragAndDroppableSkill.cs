using System.Collections;
using System.Collections.Generic;
using Gavi.Player.Skills;
using Gavi.Skills;
using UnityEngine;

namespace Gavi
{
    public class DragAndDroppableSkill : DragAndDroppable
    {
        public SkillUi SkillUi => _skill;
        [SerializeField] private SkillUi _skill;
    }
}
