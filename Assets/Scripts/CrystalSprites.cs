using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrystalSprites", menuName = "ScriptableObject")]
public class CrystalSprites : ScriptableObject
{
    public List<Sprite> sprites = new List<Sprite>();
}
