using UnityEngine;
using System.Collections;

[System.Serializable]
public class PowerUpDataContainer
{
    public IList<PowerUpData> powerUpDataList;
}

[System.Serializable]
public class PowerUpData
{
    public string className;
    public string[] initArgs;
}
