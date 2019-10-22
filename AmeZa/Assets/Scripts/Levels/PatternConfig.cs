using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Pattern")]
public class PatternConfig : ScriptableObject
{
    public const int Width = 7;
    public const int Height = 6;
    public const int Length = Width * Height;
    public List<BlockType> blocks = new List<BlockType>(Length);
}
