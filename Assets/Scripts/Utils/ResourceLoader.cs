using UnityEngine;
using System.Collections;

public class ResourceLoader : MonoBehaviour
{
    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Sprite Helpers ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */

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

    /* ~~~~~~~~~~~~~~~~~~~~ Handlers for all other Resources ~~~~~~~~~~~~~~~~~~~~ */

    private static Dictionary<Type, String> resourcePathByType = new Dictionary<Type, String>() {
        [typeof(AnimationClip)] = "Animations/{0}"
    };

    public static T LoadResource<T>(Type<T> resourceType, string resourceName) {
        string resourcePath = resourcePathByType[resourceType];
        if (resourcePath == null) {
            throw new InvalidOperationException($"No ResourcePath found for type {resourceType}");
        }

        return Resources.Load<T>(string.Format(resourcePath, resourceName));
    }
}
