using System.Collections;
using System.Collections.Generic;
using Gavi.Skills;
using Gavi.Utility;
using UnityEngine;

namespace Gavi
{
    public class SkillUiManagerPlayer : SkillUiManager
    {
        public override void AddUi(Skill skill, int skillIndex = -1)
        {
            base.AddUi(skill, skillIndex);
            if (skill == null || skill.DontUseUi)
                return;
            if (Encounter.EncounterManager.Instance.CurrentEncounter.Difficulty ==
                Encounter.Encounter.EncounterDifficulty.Normal ||
                Encounter.EncounterManager.Instance.CurrentEncounter.Difficulty ==
                Encounter.Encounter.EncounterDifficulty.Heroic)
            {
                DragAndDropZoneSkill thisZone = _skillBar.GetZoneOfSkill(skill);
                if (thisZone == null)
                {
                    Debugger.LogError("thisZone == null in SkillUiManagerPlayer.AddUi(...)!");
                    return;
                }
                DragAndDropZoneSkill configurationZone = PlayerSkillConfiguration.Instance.SkillBarNormalHc.Zones[_skillBar.GetIndexOfZone(thisZone)];
                if (configurationZone == null)
                {
                    Debugger.LogError("configurationZone == null in SkillUiManagerPlayer.AddUi(...)!");
                    return;
                }

                if (thisZone.KeyBinding != null && configurationZone.KeyBinding != null)
                    thisZone.KeyBinding.CopyKeyCodeFrom(configurationZone.KeyBinding);
                
            }                
        }
    }
}
