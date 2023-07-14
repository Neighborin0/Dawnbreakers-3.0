using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System;

public class SaveTools
{
     public static bool Save(string saveName, object saveData)
    {
        var binaryFormatter = GetBinaryFormatter();
        if(Directory.Exists(Application.persistentDataPath + "/saves")) 
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }
        string path = Application.persistentDataPath + "/saves/" + saveName + ".save";
        FileStream fileStream = File.Create(path);
        binaryFormatter.Serialize(fileStream, saveData);
        fileStream.Close();

        return true;
    }

    public static object Load(string path)
    {
        if(File.Exists(path))
        {
            return null;
        }

        BinaryFormatter binaryFormatter = GetBinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            object save = binaryFormatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
            Debug.Log("File couldn't be found");
            file.Close();
            return null;
        }
    }
    public static BinaryFormatter GetBinaryFormatter()
    {
        var binaryFormatter = new BinaryFormatter();
        return binaryFormatter;
    }

}
