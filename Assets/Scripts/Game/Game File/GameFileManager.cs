using System.Collections.Generic;
using System.IO;
using Gavi.Base;
using Gavi.Skills;
using Gavi.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gavi
{
    public class GameFileManager : MonoBehaviour
    {
        #region Variables

        public static GameFileManager Instance;

        [SerializeField] private int _fileAmount = 3;

        private SafeFile _currentSafeFile;
        public static string SafeFileDirectoryPath => Application.dataPath + "/SaveFiles";

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

        public List<SafeFile> GenerateGameFilesFromDisk()
        {
            // read existing game files
            List<SafeFile> existingFiles = new ();
            Directory.CreateDirectory(SafeFileDirectoryPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(SafeFileDirectoryPath);
            FileInfo[] files = directoryInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Extension != ".json")
                    continue;
                SafeFile safeFile = SafeFile.TryLoadGameFileFromDisk(file.Name);
                if (safeFile == null)
                    continue;

                // if we already have reached the max number of game files, just don't add any more and log an error
                if (existingFiles.Count >= _fileAmount)
                {
                    Debugger.LogError("Found too many game files on disk.");
                    break;
                }
                
                existingFiles.Add(safeFile);
            }

            // make indices work
            List<SafeFile> sortedFiles = new(_fileAmount); 
            while (sortedFiles.Count < _fileAmount) 
                sortedFiles.Add(null);
            for (int i = 0; i < existingFiles.Count; i++)
            {
                SafeFile file = existingFiles[i];
                // if the file index is too high, just set it to 0 and retry the iteration
                if (file.FileIndex >= _fileAmount)
                {
                    file.FileIndex = 0;
                    i--;
                    continue;
                }
                // if the slot is already occupied, just increase the index and retry the iteration. Even if the index
                // would overshoot the size of the list, due to the above statement it would go down to 0 eventually.
                if (sortedFiles[file.FileIndex] != null )
                {
                    file.FileIndex++;
                    i++;
                    continue;
                }

                sortedFiles[file.FileIndex] = file;
            }

            // fill up empty slots with new game files
            for (int i = 0; i < sortedFiles.Count; i++)
            {
                if (sortedFiles[i] != null)
                    continue;
                SafeFile newSafeFile = SafeFile.CreateNewGameFile(i);
                sortedFiles[i] = newSafeFile;
            }

            return sortedFiles;
        }

        // called from logout button
        public void Logout()
        {
            SaveCurrentGameFile();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        #endregion


        #region Load
        public void LoadGameFile(SafeFile file)
        {
            _currentSafeFile = file;
            GameFlowManager.Instance.StartGame(_currentSafeFile.GameFile);
        }

        #endregion


        #region Delete
        public void DeleteSafeFile(SafeFile file)
        {
            if (file == null)
                return;

            string path = SafeFileDirectoryPath + "/" + file.FileNamePlusExtension;
            if (File.Exists(path))
                File.Delete(path);
        }

        #endregion


        #region Save
        public void SaveCurrentGameFile()
        {
            SkillSafeFile skillSaveFile = GenerateSkillSaveFile();
            _currentSafeFile.GameFile.SaveSkills(skillSaveFile);

            EncounterProgressSafeFile encounterProgressSafeFile = GenerateEncounterProgressFile();
            _currentSafeFile.GameFile.SaveEncounterProgress(encounterProgressSafeFile);

            _currentSafeFile.WriteToDisk();
        }

        public SkillSafeFile GenerateSkillSaveFile()
        {
            List<Skill> skillsChosenNormalHc = PlayerSkillConfiguration.Instance.SkillsChosenNormalHc;
            List<Skill> skillsChosenMythic = PlayerSkillConfiguration.Instance.SkillsChosenMythic;
            List<Skill> skillsChosenAvailable = PlayerSkillConfiguration.Instance.SkillsAvailable;

            List<SkillEntry> skillEntries = new List<SkillEntry>();
            for (int i = 0; i < skillsChosenNormalHc.Count; i++)
            {
                Skill skill = skillsChosenNormalHc[i];
                if (skill == null)
                    continue;
                SkillEntry skillEntry = new SkillEntry();
                skillEntry.GUID = skill.SkillPrefab.GUID;
                skillEntry.BarType = PlayerSkillConfiguration.BarType.ActiveNormalHc;
                skillEntry.ZoneIndex = i;
                skillEntries.Add(skillEntry);
            }

            for (int i = 0; i < skillsChosenMythic.Count; i++)
            {
                Skill skill = skillsChosenMythic[i];
                if (skill == null)
                    continue;
                SkillEntry skillEntry = new SkillEntry();
                skillEntry.GUID = skill.SkillPrefab.GUID;
                skillEntry.BarType = PlayerSkillConfiguration.BarType.ActiveMythic;
                skillEntry.ZoneIndex = i;
                skillEntries.Add(skillEntry);
            }

            for (int i = 0; i < skillsChosenAvailable.Count; i++)
            {
                Skill skill = skillsChosenAvailable[i];
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
            return skillSafeFile;
        }

        private EncounterProgressSafeFile GenerateEncounterProgressFile()
        {
            List<int> encountersSuccessNormal = GameProgress.Instance.EncountersSuccessNormal;
            List<int> encountersSuccessHeroic = GameProgress.Instance.EncountersSuccessHeroic;
            List<int> encountersSuccessMythic = GameProgress.Instance.EncountersSuccessMythic;
            KeyValueList<int, List<int>> encountersSuccessMythicPlus =
                GameProgress.Instance.EncountersSuccessMythicPlus;

            EncounterProgressSafeFile encounterProgressSafeFile = new EncounterProgressSafeFile();
            encounterProgressSafeFile.EncountersSuccessNormal = encountersSuccessNormal;
            encounterProgressSafeFile.EncountersSuccessHeroic = encountersSuccessHeroic;
            encounterProgressSafeFile.EncountersSuccessMythic = encountersSuccessMythic;
            encounterProgressSafeFile.EncountersSuccessMythicPlus = encountersSuccessMythicPlus;

            return encounterProgressSafeFile;
        }

        #endregion
    }


    [System.Serializable]
    public class SafeFile
    {
        #region Variables
        public const string DefaultFileName = "New Safe File";
        public const string DefaultCharacterName = "New Game";

        [SerializeField] private bool _isValidGameFile;
        public string CharacterName => _characterName;
        [SerializeField] private string _characterName;
        public int FileIndex;

        // file name & path
        private string FilePath => GameFileManager.SafeFileDirectoryPath + "/" + FileNamePlusExtension;
        public string FileNamePlusExtension => _fileName + ".json";
        public string FileName => _fileName;
        [SerializeField] private string _fileName;

        public GameFile GameFile => _gameFile ??= new GameFile();
        [SerializeField] private GameFile _gameFile;

        #endregion


        #region Loading

        public static SafeFile CreateNewGameFile(int fileIndex)
        {
            SafeFile safeFile = new SafeFile();
            safeFile._isValidGameFile = true;
            safeFile._fileName = DefaultFileName + " " + fileIndex;
            safeFile._characterName = DefaultCharacterName;
            safeFile.FileIndex = fileIndex;

            safeFile.WriteToDisk();
            return safeFile;
        }

        public static SafeFile TryLoadGameFileFromDisk(string fileNamePlusExtension)
        {
            StreamReader reader = new StreamReader(GameFileManager.SafeFileDirectoryPath + "/" + fileNamePlusExtension);
            string fileContent = reader.ReadToEnd();
            reader.Close();

            SafeFile safeFile = JsonUtility.FromJson<SafeFile>(fileContent);
            if (safeFile == null)
            {
                Debugger.LogError("Something went wrong loading a game file. File name: " + fileNamePlusExtension);
                return null;
            }

            if (!safeFile._isValidGameFile)
                return null;

            safeFile._fileName = fileNamePlusExtension.Replace(".json", "");
            return safeFile;
        }

        public void SetCharacterName(string characterName)
        {
            _characterName = characterName;
            WriteToDisk();
        }
        #endregion


        #region Saving
        public void WriteToDisk()
        {
            string json = JsonUtility.ToJson(this, true);

            StreamWriter writer = new StreamWriter(FilePath, false);
            writer.WriteLine(json);
            writer.Close();
        }

        #endregion
    }


    #region Game File
    [System.Serializable]
    public class GameFile
    {
        public SkillSafeFile SkillSafeFile => _skillSafeFile;
        [SerializeField] private SkillSafeFile _skillSafeFile;
        public EncounterProgressSafeFile EncounterProgressSafeFile => _encounterProgressSafeFile;
        [SerializeField] private EncounterProgressSafeFile _encounterProgressSafeFile;

        public void SaveSkills(SkillSafeFile skillSafeFile)
        {
            _skillSafeFile = skillSafeFile;
        }

        public void SaveEncounterProgress(EncounterProgressSafeFile encounterProgressSafeFile)
        {
            _encounterProgressSafeFile = encounterProgressSafeFile;
        }
    }


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
    public class EncounterProgressSafeFile
    {
        public List<int> EncountersSuccessNormal = new();
        public List<int> EncountersSuccessHeroic = new();
        public List<int> EncountersSuccessMythic = new();
        public KeyValueList<int, List<int>> EncountersSuccessMythicPlus = new();
    }

    #endregion
}