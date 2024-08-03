using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Encounter;
using UnityEngine;
using UnityEngine.AI;

namespace Gavi
{
    public class MenuPanelChooseEncounter : MonoBehaviour
    {
        [SerializeField] private List<ChooseEncounterButton> _buttons;

        private void OnEnable()
        {
            // ClearMenu();
            BuildMenu();
        }

        private void BuildMenu()
        {
            if (GameProgress.Instance == null) // TODO: That is just a hack to avoid getting null refs when the scene starts
                return;
            
            bool hasSuccessInAllNormal = GameProgress.Instance.HasEncounterSuccess(EncounterManager.Instance.HighestEncounterIndexNormal, Encounter.Encounter.EncounterDifficulty.Normal);
            bool hasSuccessInAllHeroic = GameProgress.Instance.HasEncounterSuccess(EncounterManager.Instance.HighestEncounterIndexHeroic, Encounter.Encounter.EncounterDifficulty.Heroic);
            foreach (ChooseEncounterButton button in _buttons)
            {
                int number = button.EncounterNumber;
                Encounter.Encounter.EncounterDifficulty difficulty = button.Difficulty;

                // always show the first encounter
                if (number == 1 && difficulty == Encounter.Encounter.EncounterDifficulty.Normal)
                {
                    button.Show(true);
                    continue;
                }

                if (difficulty == Encounter.Encounter.EncounterDifficulty.Normal)
                {
                    button.Show(GameProgress.Instance.HasEncounterSuccess(number - 1, difficulty));
                }
                else if (difficulty == Encounter.Encounter.EncounterDifficulty.Heroic)
                {
                    button.Show(GameProgress.Instance.HasEncounterSuccess(number, Encounter.Encounter.EncounterDifficulty.Normal));
                }
                else if (difficulty == Encounter.Encounter.EncounterDifficulty.Mythic)
                {
                    button.Show(hasSuccessInAllNormal && hasSuccessInAllHeroic);
                }
                else if (difficulty == Encounter.Encounter.EncounterDifficulty.MythicPlus)
                {
                    button.Show(GameProgress.Instance.HasEncounterSuccess(number, Encounter.Encounter.EncounterDifficulty.Mythic));
                }
            }
        }
        //
        // private void ClearMenu()
        // {
        //     
        // }
    }
}
