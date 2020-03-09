using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Theme")]
public class ThemeConfig : ScriptableObject, IResource
{
    [SpritePreview(50)]
    public Sprite previewBackground = null;
    [SpritePreview(50)]
    public Sprite cinematicBackground = null;
    [SpritePreview(50)]
    public Sprite playingBackground = null;
    [SpritePreview(50)]
    public Sprite playingMissionBackground = null;
    [SpritePreview(20)]
    public Sprite[] items = new Sprite[0];
    public ThemeSounds sounds = null;

    public int Id { get; set; }

    public Sprite GetItemSprite(int index)
    {
        return items[index % items.Length];
    }
}
