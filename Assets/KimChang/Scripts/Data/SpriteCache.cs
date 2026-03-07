using System.Collections.Generic;
using UnityEngine;

public static class SpriteCache
{
    private static readonly Dictionary<string, Sprite> spriteCache = new();

    static SpriteCache()
    {
        LoadSprites();
    }

    private static void LoadSprites()
    {
        spriteCache["Rock1"] = Resources.Load<Sprite>("Sprites/LeeBiri/Rock/1");
        spriteCache["Rock2"] = Resources.Load<Sprite>("Sprites/LeeBiri/Rock/2");
        spriteCache["Rock3"] = Resources.Load<Sprite>("Sprites/LeeBiri/Rock/3");

        spriteCache["SaveNone"] = Resources.Load<Sprite>("Image/Screen/Load/None");
        spriteCache["SaveStartRoom"] = Resources.Load<Sprite>("Image/Screen/Load/StartRoom");
        spriteCache["SavePalace"] = Resources.Load<Sprite>("Image/Screen/Load/Palace");
        spriteCache["SaveCorrider"] = Resources.Load<Sprite>("Image/Screen/Load/Corrider");
        spriteCache["SaveLibray"] = Resources.Load<Sprite>("Image/Screen/Load/Libray");
        spriteCache["SaveReadingRoom"] = Resources.Load<Sprite>("Image/Screen/Load/ReadingRoom");

        spriteCache["Item0"] = Resources.Load<Sprite>("UI/Item/Item0");
        spriteCache["Item1"] = Resources.Load<Sprite>("UI/Item/Item1");
        spriteCache["Item2"] = Resources.Load<Sprite>("UI/Item/Item2");
        spriteCache["Item3"] = Resources.Load<Sprite>("UI/Item/Item3");

        spriteCache["ItemText0"] = Resources.Load<Sprite>("UI/Item/TXTItem0");
        spriteCache["ItemText1"] = Resources.Load<Sprite>("UI/Item/TXTItem1");
        spriteCache["ItemText2"] = Resources.Load<Sprite>("UI/Item/TXTItem2");
        spriteCache["ItemText3"] = Resources.Load<Sprite>("UI/Item/TXTItem3");

        spriteCache["EmptyItem"] = Resources.Load<Sprite>("UI/Item/EmptyItem");
    }

    public static Sprite GetSprite(string key)
    {
        return spriteCache.TryGetValue(key, out var sprite) ? sprite : null;
    }
}
