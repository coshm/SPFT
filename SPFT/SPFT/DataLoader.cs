using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class DataLoader : MonoBehaviour
{
    private const string POWER_UP_DATA_FILE = "powerUpData.json";

    private Transform storeManager;

    // Use this for initialization
    void Start()
    {
        storeManager = storeManager.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IList<IPowerUp> LoadPowerUpData()
    {
        IList<IPowerUp> powerUps = new List<IPowerUp>();

        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, POWER_UP_DATA_FILE);

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);

            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            PowerUpDataContainer powerUpDataContainer = JsonUtility.FromJson<PowerUpDataContainer>(dataAsJson);

            foreach (PowerUpData powerUpData : PowerUpDataContainer.powerUpDataList)
            {
                Type powerUpType = Type.GetType(powerUpData.className);
                if (powerUpType == null)
                {
                    throw new InvalidOperationException($"No Type exists for {powerUpData.className}");
                }

                // Create GameObject and add our powerUpType component.
                IPowerUp powerUp = new GameObject().AddComponent(powerUpType) as IPowerUp;
                powerUp.Initialize(powerUpData.initArgs);
                powerUps.Add(powerUp);
            }
        }
        else
        {
            Debug.LogError($"Cannot load PowerUp data from {POWER_UP_DATA_FILE}! File does not exist.");
        }

        return powerUps;
    }
