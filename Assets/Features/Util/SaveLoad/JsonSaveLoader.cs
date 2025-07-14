using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;

public static class JsonSaveLoader
{
    private const string metaPostFix = ".meta";
    private const string jsonPostFix = ".json";

    public static List<T> LoadJsonFolder<T>(string folderPath)
    {
        List<T> myList = new List<T>();
        var info = new DirectoryInfo(folderPath);
        var fileInfo = info.GetFiles();
        foreach(FileInfo file in fileInfo)
        {
            //assuming files have at least 1 dot.
            var split = file.Name.Split('.');
            var filePostFix = "." + split[split.Length - 1]; //last postfix. In case of Unity meta files these have .meta at the end.
            if(filePostFix != jsonPostFix) continue; //not correct filetype, skip!

            var path = file.DirectoryName + "/" + file.Name;
            using StreamReader streamReader = new StreamReader(path);
            string json = streamReader.ReadToEnd();
            var item = JsonConvert.DeserializeObject<T>(json);
            myList.Add(item);
        }
        return myList;
    }

    public static T LoadJson<T>(string folderPath, string fileName)
    {
        var path = folderPath + "/" + fileName + jsonPostFix;
        using StreamReader streamReader = new StreamReader(path);
        var json = streamReader.ReadToEnd();
        var item = JsonConvert.DeserializeObject<T>(json);
        return item;
    }

    public static void SaveAsJson(object objectToSave, string folderPath, string fileName)
    {
        var jsonString = JsonConvert.SerializeObject(objectToSave, Formatting.Indented);
        var path = folderPath + "/" + fileName + jsonPostFix;
        //CLogger.Log($"Saving as Json as {path}");
        using StreamWriter streamWriter = File.CreateText(path);
        streamWriter.Write(jsonString);
    }

    public static bool JsonExists(string folderPath, string fileName)
    {
        var path = folderPath + "/" + fileName + jsonPostFix;
        return File.Exists(path);
    }

    public static bool RemoveJson(string folderPath, string fileName)
    {
        var path = folderPath + "/" + fileName + jsonPostFix;
        CLogger.Log($"Removing json: {fileName}");
        try
        {
            if (JsonExists(folderPath, fileName))
            {
                File.Delete(path);
            }
            return true;
        }
        catch (Exception e)
        {
            CLogger.LogError(e);
            return false;
        }
    }
}