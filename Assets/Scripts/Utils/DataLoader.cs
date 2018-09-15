using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using SPFT.PowerUpSystem.PowerUps;
using System.Reflection;

public class DataLoader : MonoBehaviour {

    private static DataLoader dataLoader;
    public static DataLoader Instance {
        get {
            if (!dataLoader) {
                dataLoader = FindObjectOfType(typeof(DataLoader)) as DataLoader;
                if (!dataLoader) {
                    Debug.LogError("There needs to be one active PowerUpManager script on a GameObject in your scene.");
                }
            }
            return dataLoader;
        }
    }

    public string AssemblyDirectory {
        get {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            Debug.Log($"Assembly Name={codeBase}");
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }

    private const string POWER_UP_DATA_FILE = "powerUpData.json";

    public IList<GameObject> LoadPowerUpData() {
        IList<GameObject> powerUps = new List<GameObject>();

        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, POWER_UP_DATA_FILE);

        if (File.Exists(filePath)) {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            Debug.Log($"Successfully read Json Data='{dataAsJson}'.");

            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            PowerUpDataContainer powerUpDataContainer = JsonUtility.FromJson<PowerUpDataContainer>(dataAsJson);

            foreach (PowerUpData powerUpData in powerUpDataContainer.powerUpDataList) {
                string fullyQualifiedTypeName = powerUpData.className + ", " + Assembly.GetExecutingAssembly().FullName;
                Debug.Log($"Loading Type for Class={fullyQualifiedTypeName}");

                Type powerUpType = Type.GetType(fullyQualifiedTypeName);
                if (powerUpType == null) {
                    throw new InvalidOperationException($"No Type exists for {powerUpData.className}");
                }

                // Create GameObject and add our powerUpType component.
                GameObject powerUp = new GameObject();
                (powerUp.AddComponent(powerUpType) as IPowerUp).Initialize(powerUpData.initArgs);
                powerUps.Add(powerUp);
            }
        } else {
            Debug.LogError($"Cannot load PowerUp data from {POWER_UP_DATA_FILE}! File does not exist.");
        }

        return powerUps;
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
