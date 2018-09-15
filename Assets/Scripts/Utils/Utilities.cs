using System;
using System.Collections.Generic;
using UnityEngine;

public class Utilities {

    public const string POWER_UP_ICONS = "SPFT_powerUpIcons";

    private static Dictionary<string, Sprite[]> spriteSheetsByName;

    public static Sprite[] GetSpriteSheet(string sheetName)
    {
        if (!spriteSheetsByName.ContainsKey(sheetName))
        {
            spriteSheetsByName[sheetName] = Resources.LoadAll<Sprite>($"Sprites/{sheetName}");
        }
        return spriteSheetsByName[sheetName];
    }

    public static SpriteHelper GetSprite(string sheetName, int spriteIdx)
    {
        Sprite[] spriteSheet = GetSpriteSheet(sheetName);
        return spriteSheet[spriteIdx];
    }

    public static List<T> OrderObjsByName<T>(T[] unorderedObjs) where T : MonoBehaviour {
        try {
            List<T> list = new List<T>(unorderedObjs.Length);
            foreach (T obj in unorderedObjs) {
                string name = obj.name;
                string stringIdx = name.Substring(name.LastIndexOf("_") + 1);
                int idx = Int32.Parse(stringIdx);
                //int idx = Int32.Parse(name.Substring(name.LastIndexOf("_")));
                list[idx] = obj;
            }
            return list;
        } catch (Exception e) {
            return null;
        }
    }

    public static int ConvertStringOrDefault(string arg, int defaulVal) {
        try {
            return int.Parse(arg);
        } catch (Exception e) {
            return defaulVal;
        }
    }

    public static float ConvertStringOrDefault(string arg, float defaulVal) {
        try {
            return float.Parse(arg);
        } catch (Exception e) {
            return defaulVal;
        }
    }

    public static bool ConvertStringOrDefault(string arg, bool defaulVal) {
        try {
            return bool.Parse(arg);
        } catch (Exception e) {
            return defaulVal;
        }
    }

    // Vector2's will be a comma separated list of floats, e.g. "5,10"
    public static Vector2 ConvertStringOrDefault(string arg, Vector2 defaulVal) {
        try {
            string[] args = arg.Split(',');
            float x = float.Parse(args[0]);
            float y = float.Parse(args[1]);
            return new Vector2(x, y);
        } catch (Exception e) {
            return defaulVal;
        }
    }

}
