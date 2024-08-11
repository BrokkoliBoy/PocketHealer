using System.Collections;
using System.Collections.Generic;
using Gavi.Utility;
using UnityEngine;

namespace Gavi.Base
{
    public class GameFlowManager : MonoBehaviour
    {
        public static GameFlowManager Instance;

        #region Mono
        private void Awake()
        {
            if (Instance != null)
                Debugger.LogInstanceError(GetType());
            Instance = this;
        }
        #endregion


        #region Life Cycle
        public void StartGame(GameFile gameFile)
        {
            // load empty file: initialize
            if (gameFile.SkillSafeFile == null || gameFile.SkillSafeFile.SkillEntries == null)
            {
                InitializeSkillsForNewSafeFile();
                GameFileManager.Instance.SaveCurrentGameFile();
                return;
            }
            
            // load existing file
            LoadSkillSafeFile(gameFile.SkillSafeFile);
            LoadBossProgressSafeFile(gameFile.BossProgressSafeFile);
            LoadSettingsSafeFile(gameFile.SettingsSafeFile);
        }
        #endregion


        #region Load Existing File
        private void LoadSkillSafeFile(SkillSafeFile skillSafeFile)
        {
            foreach (SkillEntry skillEntry in skillSafeFile.SkillEntries)
            {
                PlayerSkillPrefab prefab = PlayerSkillDatabase.Instance.GetPrefabByGUID(skillEntry.GUID);
                if (prefab == null)
                {
                    Debugger.LogError("Couldn't load skill because GUID could not be found: " + skillEntry.GUID);
                    continue;
                }

                PlayerSkillConfiguration.Instance.UnlockSkillFromSafeFile(prefab, skillEntry.BarType, skillEntry.ZoneIndex);
            }
        }
        private void LoadBossProgressSafeFile(BossProgressSafeFile bossProgressSafeFile)
        {
            
        }
        private void LoadSettingsSafeFile(SettingsSafeFile settingsSafeFile)
        {
            
        }
        #endregion

        
        #region New Game File
        private void InitializeSkillsForNewSafeFile()
        {
            SkillLearnSystem.Instance.LearnInitialSkills();
        }
        #endregion
    }
}
