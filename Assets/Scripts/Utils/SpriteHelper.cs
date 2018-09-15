using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHelper {

    public const string POWER_UP_ICONS = "SPFT_powerUpIcons";

    private static Dictionary<string, Sprite[]> spriteSheetsByName;

    public static Sprite[] GetSpriteSheet(string sheetName) {
        if (!spriteSheetsByName.ContainsKey(sheetName)) {
            spriteSheetsByName[sheetName] = Resources.LoadAll<Sprite>($"Sprites/{sheetName}");
        }
        return spriteSheetsByName[sheetName];
    }

    public static SpriteHelper GetSprite(string sheetName, int spriteIdx) {
        Sprite[] spriteSheet = GetSpriteSheet(sheetName);
        return spriteSheet[spriteIdx];
    }

}
