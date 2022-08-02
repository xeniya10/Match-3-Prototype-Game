using UnityEngine;

public static class ImageLoader
{
    private static string spritePath = "Sprites/Crystals";
    public static Sprite[] CrystalSprites = new Sprite[9];

    public static void LoadAll()
    {
        CrystalSprites = Resources.LoadAll<Sprite>(spritePath);
    }
}
