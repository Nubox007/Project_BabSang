using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveSystem 
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves";
    public const string FILE_NAME = "SaveFile";
    private const string FILE_EXTENTION = ".sav";
    public static string fileName { get; private set;}
    public static string filePath { get; private set;}


    public static void Initalize()
    {

        if(!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }

        fileName = FILE_NAME + FILE_EXTENTION;
        filePath = SAVE_FOLDER + FILE_NAME + FILE_EXTENTION;
    }

    public static void Save(SaveData _saveObject)
    {
        var settings = new JsonSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        string saveString = JsonConvert.SerializeObject(_saveObject, settings);
        Debug.Log("Saved string : " + saveString);
        File.WriteAllText(filePath, saveString);
    }

    public static SaveData Load()
    {
        if(File.Exists(filePath))
        {
            string saveString = File.ReadAllText(filePath);
            Debug.Log("Loaded string : "+ saveString);
            SaveData loaded = JsonConvert.DeserializeObject<SaveData>(saveString);
            
            if(loaded == null)
            {
                return new SaveData();
            } 
            return loaded;
        }else
        {
            return new SaveData();
        }
        
    }

}
