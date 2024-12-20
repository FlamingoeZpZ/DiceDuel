
using System;
using System.IO;
using System.Xml.Serialization;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class SaveManager
{
    private static int _currentSlot;
    public static SaveData SaveData { get; private set; }

    private static readonly string Path = Application.persistentDataPath +"/Saves";
    private static readonly string FileName = Application.persistentDataPath + "/Game";
    private static readonly string Extension = ".txt"; //Anything will work
    
    private static string GetPath(int saveSlot) => Path + FileName + saveSlot + Extension;
    

    //Static constructors are automatically called when the code loads.
    static SaveManager()
    {
        _ = LoadGame(0);
    }
    
    
    public static async UniTask<SaveData> LoadGame(int slot = 0)
    {
        string path = GetPath(slot);

        // Check if the file exists
        if (!File.Exists(path))
        {
            Debug.Log($"Save file not found at {path}. Initializing new save data.");
            SaveData = new SaveData(); // Initialize new save data
            return SaveData;
        }

        //Whenever working with files, always try catch
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            await using FileStream reader = File.OpenRead(path);
            SaveData = (SaveData)serializer.Deserialize(reader);
            Debug.Log($"Game loaded from slot {slot}.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load save file: {ex.Message}");
            SaveData = new SaveData(); // Fallback to new save data
        }
        
        _currentSlot = slot; 
        return SaveData;
    }

    public static async UniTask SaveGame(this SaveData data)
    {
       await data.SaveGame(_currentSlot);
    }
    
    public static async UniTask SaveGame(this SaveData data, int slot)
    {
        string path = GetPath(slot);

        // Ensure the directory exists
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
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

public struct SaveData
{
    private int _day;
    private int _gold;

    public readonly Color MinorArmorColor;
    public readonly Color ArmorColor;
    public readonly Color ClothColor;
    public readonly Color LinesColor;
    public readonly Color PantsColor;
    public readonly Color ShirtColor;
    public readonly Color TrailColor;
    public readonly Color DiceColor;

    public SaveData(Color minorArmorColor, Color armorColor, Color clothColor, Color linesColor, Color pantsColor, Color shirtColor, Color trailColor, Color diceColor)
    {
        MinorArmorColor = minorArmorColor;
        ArmorColor = armorColor;
        ClothColor = clothColor;
        LinesColor = linesColor;
        PantsColor = pantsColor;
        ShirtColor = shirtColor;
        TrailColor = trailColor;
        DiceColor = diceColor;

        _day = 1;
        _gold = 0;
    }


    //Getters;
    public int Day => _day;
    public int Gold => _gold;

    //Setters
    public void IncreaseDay()
    {
        _day += 1;
    }

    public void SetGold(int amount)
    {
        _gold = amount;
    }
}
