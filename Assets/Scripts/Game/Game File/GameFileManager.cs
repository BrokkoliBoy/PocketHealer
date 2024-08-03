using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Gavi.Utility;
using UnityEngine;

namespace Gavi
{
    #region Game File
    public class GameFileManager : MonoBehaviour
    {
        /*
         * - Unlocked spells: PlayerSkillConfiguration.UnlockSkill(Skill skill)
         * - Spell bar positions: PlayerSkillConfiguration custom function
         * - Boss progress: EncounterManager.AddEncounterToSuccess(int encounterNumber, EncounterDifficulty difficulty, int mythicPlusNumber)
         * - Settings: 
         */

        [SerializeField] private int _minGameFiles = 3;
        [SerializeField] private int _maxGameFiles = 3;
        
        private GameFile _currentGameFile;
        private List<GameFile> _gameFiles = new List<GameFile>();
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
        private void Start()
        {
            GenerateGameFilesFromDisk();            
        }
        #endregion
        
        
        #region Life Cycle
        public void GenerateGameFilesFromDisk()
        {
            _gameFiles.Clear();

            Directory.CreateDirectory(SafeFileDirectoryPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(SafeFileDirectoryPath);
            FileInfo[] files = directoryInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Extension != ".json")
                    continue;
                GameFile gameFile = GameFile.LoadGameFileFromDisk(file.Name);
                if (gameFile == null)
                    continue;
                _gameFiles.Add(gameFile);
            }
            
            while (_gameFiles.Count < _minGameFiles)
            {
                _gameFiles.Add(new GameFile());
            }
        }

        public void Logout()
        {
            // TODO
            _currentGameFile = null;
        }
        #endregion


        #region Load
        // called from button Button - Login 0
        public void LoadGameFileViaButtonIndex(int buttonIndex)
        {
            if (buttonIndex < 0 || buttonIndex >= _gameFiles.Count)
            {
                Debugger.LogError("Button index was out of range. Index: " + buttonIndex + ", list size: " + _gameFiles.Count);
                buttonIndex = Mathf.Clamp(buttonIndex, 0, _gameFiles.Count);
            }
            LoadGameFile(_gameFiles[buttonIndex]);
        }
        
        private void LoadGameFile(GameFile file)
        {
            _currentGameFile = file;
            LoadSkillSafeFile(file.SkillSafeSafeFile);
            LoadBossProgressSafeFile(file.BossProgressSafeFile);
            LoadSettingsSafeFile(file.SettingsSafeFile);
        }

        private void LoadSkillSafeFile(SkillSafeFile skillSafeFile)
        {
            
        }
        private void LoadBossProgressSafeFile(BossProgressSafeFile bossProgressSafeFile)
        {
            
        }
        private void LoadSettingsSafeFile(SettingsSafeFile settingsSafeFile)
        {
            
        }
        #endregion

        
        #region Save
        public void SaveCurrentGameFile()
        {
            _currentGameFile.WriteToDisk();
        }
        #endregion
    }

    [System.Serializable]
    public class GameFile
    {
        private string _filePath => GameFileManager.SafeFileDirectoryPath + "/" + FileNamePlusExtension;
        public string FileNamePlusExtension;

        public SkillSafeFile SkillSafeSafeFile => _skillSafeFile;
        private SkillSafeFile _skillSafeFile;
        public BossProgressSafeFile BossProgressSafeFile => _bossProgressSafeFile;
        private BossProgressSafeFile _bossProgressSafeFile;
        public SettingsSafeFile SettingsSafeFile => _settingsSafeFile;
        private SettingsSafeFile _settingsSafeFile;

        public static GameFile LoadGameFileFromDisk(string fileNamePlusExtension)
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
            gameFile.FileNamePlusExtension = fileNamePlusExtension;
            return gameFile;
        }

        public void WriteToDisk()
        {
            string json = JsonUtility.ToJson(this);
            
            StreamWriter writer = new StreamWriter(_filePath, false);
            writer.WriteLine(json);
            writer.Close();
        }
    }
    #endregion


    [System.Serializable]
    public class SkillSafeFile
    {
        
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
