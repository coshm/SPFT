using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;

public class DataLoader : SingletonBase<DataLoader> {
    
    private const string POWER_UP_DATA_FILE = "powerUpData.json";

    private string AssemblyDirectory {
        get {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            Debug.Log($"Assembly Name={codeBase}");
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }

    public IDictionary<Type, PowerUpData> LoadPowerUpData() {
        // Load the Json Data from the PowerUpData file into a local string
        string powerUpJsonData = LoadJsonAsString(POWER_UP_DATA_FILE);

        // Pass the json to JsonUtility, and tell it to create a GameData object from it
        PowerUpDataContainer powerUpDataContainer = JsonUtility.FromJson<PowerUpDataContainer>(powerUpJsonData);

        IDictionary<Type, PowerUpData> powerUpDataByType = new Dictionary<Type, PowerUpData>();
        foreach (PowerUpData powerUpData in powerUpDataContainer.powerUpDataList) {
            string fullyQualifiedTypeName = powerUpData.className + ", " + Assembly.GetExecutingAssembly().FullName;
            Debug.Log($"Loading Type for Class={fullyQualifiedTypeName}");

            Type powerUpType = Type.GetType(fullyQualifiedTypeName);
            if (powerUpType == null) {
                throw new InvalidOperationException($"No Type exists for {powerUpData.className}");
            }

            powerUpDataByType[powerUpType] = powerUpData;
        }
        return powerUpDataByType;
    }

    public string LoadJsonAsString(string fileName) {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath)) {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            Debug.Log($"Successfully read Json Data='{dataAsJson}'.");
            return dataAsJson;
        } else {
            throw new ArgumentException($"FileName={fileName} does not exist in under StreamingAssets.");
        }
    }
}

[System.Serializable]
public class PowerUpDataContainer {
    public PowerUpData[] powerUpDataList;
}

[System.Serializable]
public class PowerUpData {
    public string className;
    public PowerUpArg[] initArgs;
}

public enum PowerUpArgType {
    BOOL,
    CLASS,
    FLOAT,
    GUID,
    INT,
    STRING
}

[System.Serializable]
public class PowerUpArg {
    public string name;
    public string value;
    public string type;
    public PowerUpArgType Type {
        get {
            return (PowerUpArgType) Enum.Parse(typeof(PowerUpArgType), type);
        }
    } 
}
