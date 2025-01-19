using System;
using System.IO;
using System.Xml.Serialization;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Managers.Core
{
    public static class SaveManager
    {
        private static int _currentSlot;
        public static SaveData CurrentSave { get; private set; }

        private static readonly string Path = Application.persistentDataPath + "/Saves";
        private static readonly string FileName = "/Game";
        private static readonly string Extension = ".txt"; //Anything will work

        private static string GetPath(int saveSlot) => Path + FileName + saveSlot + Extension;

        static SaveManager()
        {
            _ = LoadGame();
        }
        
    
        public static async UniTask<bool> LoadGame(int slot = 0)
        {
            string path = GetPath(slot);

            // Check if the file exists
            if (!File.Exists(path))
            {
                Debug.Log($"Save file not found at {path}. Initializing new save data.");
                await CreateNewGame("Player");
                return false;
            }

            //Whenever working with files, always try catch
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                await using FileStream reader = File.OpenRead(path);
                CurrentSave = (SaveData)serializer.Deserialize(reader);
                Debug.Log($"Game loaded from slot {slot} at path {path}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load save file: {ex.Message}");
                return false;
            }
        
            _currentSlot = slot; 
            return true;
        }

        public static async UniTask CreateNewGame(string name)
        {
            CurrentSave = new SaveData(name);
            await CurrentSave.SaveGame();
        }

        public static void DeleteSaveCurrent()
        {
            string path = GetPath(_currentSlot);
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                Debug.LogError("Tried to delete a file that didn't exist: " + path + ", " + ex);
            }
        }
        

        public static async UniTask SaveGame(this SaveData data)
        {
            await data.SaveGame(_currentSlot);
        }
    
        public static async UniTask SaveGame(this SaveData data, int slot)
        {
            string path = GetPath(slot);

            // Ensure the directory exists
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }

            // Serialize the data
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                await using (FileStream writer = File.OpenWrite(path)) //Await when opening a new file
                {
                    serializer.Serialize(writer, data);
                    Debug.Log($"Game saved to slot {slot}.");
                }
                _currentSlot = slot; 

            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save game: {ex.Message}");
            }
        }
    }
}