using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIDataHelper: MonoBehaviour
{
    // Create a field for the save file
    string saveFile;

    // Create a PlaceData array
    public PlaceData[] places;
    public int Total;
    public int Destroyed;

    public static POIDataHelper instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        saveFile = Application.persistentDataPath + "/PlaceData.json";
    }

    // Check whether local JSON file exists. If so, read the local one. If not, create a new one.
    public PlaceData[] readFile()
    {
        if (File.Exists(saveFile))
        {
            string fileContents = File.ReadAllText(saveFile);

            places = JsonHelper.FromJson<PlaceData>(fileContents);

            return places;
        }
        else
        {
            LoadFromResources();
            return readFile();
        }
    }

    // Write to local JSON file
    private void writeFile()
    {
        string jsonString = JsonHelper.ToJson(places);

        File.WriteAllText(saveFile, jsonString);
    }

    // Copy the pre-defined JSON file from apk resources to local storage
    public void LoadFromResources()
    {
        TextAsset ta = Resources.Load<TextAsset>("PlaceData");
        string jsonText = ta.text;
        System.IO.File.WriteAllText(Application.persistentDataPath + "/PlaceData.json", jsonText);
    }

    // Update POI data ("HasDefeated")
    public void SetHasDefeated(int index, bool newHasDefeated)
    {
        Debug.Log(index);
        places[index].hasDefeated = newHasDefeated;
        writeFile();
    }

    // Update POI data ("HasClaimedReward")
    public void SetHasClaimedReward(int index, bool newHasClaimedReward)
    {
        places[index].hasClaimedReward = newHasClaimedReward;
        writeFile();
    }
}

// A JSON Helper class to handle reading/updating JSON file
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}