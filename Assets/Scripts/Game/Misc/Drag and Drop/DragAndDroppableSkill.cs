using System.Collections;
using System.Collections.Generic;
using Gavi.Player.Skills;
using Gavi.Skills;
using UnityEngine;

namespace Gavi
{
    public class DragAndDroppableSkill : DragAndDroppable
    {
        public SkillUi SkillUi => _skillUi;
        [SerializeField] private SkillUi _skillUi;
    }
}
