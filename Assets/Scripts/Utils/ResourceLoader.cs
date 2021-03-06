﻿using UnityEngine;
using System;
using System.Collections.Generic;
using SPFT.PowerUpSystem.PowerUps;

public class ResourceLoader : SingletonBase<ResourceLoader> {

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Sprite Helpers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */

    public static readonly string POWER_UP_ICONS = "SPFT_powerUpIcons";
    public static readonly string POWER_UP_WIDGET_SPOTS = "SPFT_powerUpWidgetSpots";

    private static readonly IDictionary<Type, int> POWER_UP_TO_SPRITE_SHEET_IDX = new Dictionary<Type, int> {
        [typeof(PegExploder)] = 0,
        [typeof(PegSmasher)] = 1,
        [typeof(PuckBounceMod)] = 2,
        [typeof(RearrangeGoals)] = 3,
        [typeof(RearrangeStoredPowerUps)] = 4
    };

    private static Dictionary<string, Sprite[]> spriteSheetsByName;

    public static Sprite[] GetSpriteSheet(string sheetName) {
        if (!spriteSheetsByName.ContainsKey(sheetName)) {
            spriteSheetsByName[sheetName] = Resources.LoadAll<Sprite>($"Sprites/{sheetName}");
        }
        return spriteSheetsByName[sheetName];
    }

    public static Sprite GetSprite(string sheetName, int spriteIdx) {
        Sprite[] spriteSheet = GetSpriteSheet(sheetName);
        return spriteSheet[spriteIdx];
    }

    public static Sprite GetSpriteForPowerUp(Type powerUpType) {
        Sprite[] powerUpSprites = GetSpriteSheet(POWER_UP_ICONS);
        if (!POWER_UP_TO_SPRITE_SHEET_IDX.ContainsKey(powerUpType)) {
            throw new ArgumentException($"Could not find SpriteSheet index for type {powerUpType}.");
        }

        int powerUpIdx = POWER_UP_TO_SPRITE_SHEET_IDX[powerUpType];
        return powerUpSprites[powerUpIdx];
    }

    /* ~~~~~~~~~~~~~~~~~~~~ Handlers for all other Resources ~~~~~~~~~~~~~~~~~~~~ */

    private static Dictionary<Type, String> resourcePathByType = new Dictionary<Type, String>() {
        [typeof(AnimationClip)] = "Animations/{0}"
    };

    public static T LoadResource<T>(string resourceName) where T : UnityEngine.Object {
        string resourcePath = resourcePathByType[typeof(T)];
        if (resourcePath == null) {
            throw new InvalidOperationException($"No ResourcePath found for type {typeof(T)}");
        }
        return Resources.Load<T>(string.Format(resourcePath, resourceName));
    }
}
