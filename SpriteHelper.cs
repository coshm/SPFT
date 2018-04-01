using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHelper
{

    private static Sprite[] circleSprites = Resources.LoadAll<Sprite>("Sprites/Circles");

    private static Dictionary<Color, String> circleSpriteNamesByColor = new Dictionary<Color, String>()
    {
        //[Color.WHITE] = "WhiteCircle",
        //[Color.BLUE] = "BlueCircle",
        //[Color.GREEN] = "GreenCircle",
        //[Color.PURPLE] = "PurpleCircle",
        //[Color.RED] = "RedCircle",
        //[Color.YELLOW] = "YellowCircle"
    };

    /*public static Sprite GetCircleForColor(Color color)
    {
        Sprite circle = circleSprites.Single(s => s.name == circleSpriteNamesByColor[color]);
        if (circle == null)
        {
            throw new InvalidOperationException($"Could not find a Sprite for color, {color.ToString()}");
        }
        return circle;
    }

    public static Sprite GetPlayerIndicatorSprite(PlayerController.Player player)
    {
        Sprite playerIndicator = circleSprites.Single(s => s.name == player.ToString());
        if (playerIndicator == null)
        {
            throw new InvalidOperationException($"Could not find a Sprite for color, {player.ToString()}");
        }
        return playerIndicator;
    }*/
}
