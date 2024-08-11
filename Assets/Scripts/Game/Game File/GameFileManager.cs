using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Gavi.Base;
using Gavi.Skills;
using Gavi.UI;
using Gavi.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace Gavi
{
    #region Game File
    public class GameFileManager : MonoBehaviour
    {
        public static GameFileManager Instance;
        
        /*
         * - Unlocked spells: PlayerSkillConfiguration.UnlockSkill(Skill skill)
         * - Spell bar positions: PlayerSkillConfiguration custom function
         * - Boss progress: GameProgress.AddEncounterToSuccess(int encounterNumber, EncounterDifficulty difficulty, int mythicPlusNumber)
         * - Settings: 
         */

        [SerializeField] private int _minGameFiles = 3;
        [SerializeField] private int _maxGameFiles = 3;
        
        private GameFile _currentGameFile;
        public static string SafeFileDirectoryPath => Application.dataPath + "/SaveFiles";


        #region DEBUG
        [SerializeField] private bool _GENERATE_FILES_FROM_DISK;

        private void OnDrawGizmosSelected()
        {
            if (_GENERATE_FILES_FROM_DISK)
            {
                _GENERATE_FILES_FROM_DISK = false;
                GenerateGameFilesFromDisk();
            }
        }
        #endregion


        #region Mono
        private void Awake()
        {
            if (Instance != null)
                Debugger.LogInstanceError(typeof(GameFileManager));
            Instance = this;
        }
        #endregion
        
        
        #region Life Cycle
        public List<GameFile> GenerateGameFilesFromDisk()
        {
            List<GameFile> gameFiles = new List<GameFile>();

            // read existing game files
            Directory.CreateDirectory(SafeFileDirectoryPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(SafeFileDirectoryPath);
            FileInfo[] files = directoryInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Extension != ".json")
                    continue;
                GameFile gameFile = GameFile.TryLoadGameFileFromDisk(file.Name);
                if (gameFile == null)
                    continue;
                gameFiles .Add(gameFile);
            }
            
            // fill up with new game files
            while (gameFiles.Count < _minGameFiles)
            {
                GameFile gameFile = GameFile.CreateNewGameFile();
                gameFiles.Add(gameFile);
            }

            return gameFiles;
        }

        // called from logout button
        public void Logout()
        {
            SaveCurrentGameFile();
            _currentGameFile = null;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        #endregion


        #region Load
        public void LoadGameFile(GameFile file)
        {
            _currentGameFile = file;
            GameFlowManager.Instance.StartGame(_currentGameFile);
        }
        #endregion

        
        #region Save
        public void SaveCurrentGameFile()
        {
            List<Skill> skillsChosenNormalHc = PlayerSkillConfiguration.Instance.SkillsChosenNormalHc;
            List<Skill> skillsChosenMythic = PlayerSkillConfiguration.Instance.SkillsChosenMythic;
            List<Skill> skillsAvailable = PlayerSkillConfiguration.Instance.SkillsAvailable;
            SaveSkills(skillsChosenNormalHc, skillsChosenMythic, skillsAvailable);

            _currentGameFile.WriteToDisk();
        }
        
        public void SaveSkills(List<Skill> chosenSkillsNormalHc, List<Skill> chosenSkillsMythic, List<Skill> availableSkills)
        {
            List<SkillEntry> skillEntries = new List<SkillEntry>();
            for (int i = 0; i < chosenSkillsNormalHc.Count; i++)
            {
                Skill skill = chosenSkillsNormalHc[i];
                if (skill == null)
                    continue;
                SkillEntry skillEntry = new SkillEntry();
                skillEntry.GUID = skill.SkillPrefab.GUID;
                skillEntry.BarType = PlayerSkillConfiguration.BarType.ActiveNormalHc;
                skillEntry.ZoneIndex = i;
                skillEntries.Add(skillEntry);
            }
            for (int i = 0; i < chosenSkillsMythic.Count; i++)
            {
                Skill skill = chosenSkillsMythic[i];
                if (skill == null)
                    continue;
                SkillEntry skillEntry = new SkillEntry();
                skillEntry.GUID = skill.SkillPrefab.GUID;
                skillEntry.BarType = PlayerSkillConfiguration.BarType.ActiveMythic;
                skillEntry.ZoneIndex = i;
                skillEntries.Add(skillEntry);
            }
            for (int i = 0; i < availableSkills.Count; i++)
            {
                Skill skill = availableSkills[i];
                if (skill == null)
                    continue;
                SkillEntry skillEntry = new SkillEntry();
                skillEntry.GUID = skill.SkillPrefab.GUID;
                skillEntry.BarType = PlayerSkillConfiguration.BarType.Available;
                skillEntry.ZoneIndex = i;
                skillEntries.Add(skillEntry);
            }
            
            SkillSafeFile skillSafeFile = new SkillSafeFile();
            skillSafeFile.SkillEntries = skillEntries;
            _currentGameFile.SaveSkills(skillSafeFile);
        }
        #endregion
    }

    [System.Serializable]
    public class GameFile
    {
        [SerializeField] private bool _isValidGameFile;
        
        private string _filePath => GameFileManager.SafeFileDirectoryPath + "/" + FileNamePlusExtension;
        public string FileNamePlusExtension => _fileName + ".json";
        public string FileName => _fileName;
        [SerializeField] private string _fileName;

        public SkillSafeFile SkillSafeFile => _skillSafeFile;
        [SerializeField] private SkillSafeFile _skillSafeFile;
        public BossProgressSafeFile BossProgressSafeFile => _bossProgressSafeFile;
        [SerializeField] private BossProgressSafeFile _bossProgressSafeFile;
        public SettingsSafeFile SettingsSafeFile => _settingsSafeFile;
        [SerializeField] private SettingsSafeFile _settingsSafeFile;


        #region Loading
        public static GameFile CreateNewGameFile()
        {
            GameFile gameFile = new GameFile();
            gameFile._isValidGameFile = true;
            gameFile._fileName = "New Game File " + UnityEngine.Random.Range(0, 10000);
            
            gameFile.WriteToDisk();
            return gameFile;
        }
        
        public static GameFile TryLoadGameFileFromDisk(string fileNamePlusExtension)
        {
            StreamReader reader = new StreamReader(GameFileManager.SafeFileDirectoryPath + "/" + fileNamePlusExtension);
            string fileContent = reader.ReadToEnd();
            reader.Close();

            GameFile gameFile = JsonUtility.FromJson<GameFile>(fileContent);
            if (gameFile == null)
            {
                Debugger.LogError("Something went wrong loading a game file. File name: " + fileNamePlusExtension);
                return null;
            }

            if (!gameFile._isValidGameFile)
                return null;

            gameFile._fileName = fileNamePlusExtension.Replace(".json", "");
            return gameFile;
        }
        #endregion

        
        #region Saving
        public void SaveSkills(SkillSafeFile skillSafeFile)
        {
            _skillSafeFile = skillSafeFile;
            // WriteToDisk();
        }
        
        public void WriteToDisk()
        {
            string json = JsonUtility.ToJson(this, true);
            
            StreamWriter writer = new StreamWriter(_filePath, false);
            writer.WriteLine(json);
            writer.Close();
        }
        #endregion
    }        
    #endregion


    [System.Serializable]
    public class SkillSafeFile
    {
        public List<SkillEntry> SkillEntries;
    }

    [System.Serializable]
    public class SkillEntry
    {
        public string GUID;
        public PlayerSkillConfiguration.BarType BarType;
        public int ZoneIndex;
    }

    [System.Serializable]
    public class BossProgressSafeFile
    {

    }

    [System.Serializable]
    public class SettingsSafeFile
    {

    }
}
