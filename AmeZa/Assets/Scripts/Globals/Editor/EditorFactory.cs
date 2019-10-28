using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorFactory : StaticConfig<EditorFactory>
{
    public Sprite boxKillSprite = null;
    public Sprite horizontalKillSprite = null;
    public Sprite verticalKillSprite = null;
    public Sprite crossKillSprite = null;
    public Sprite horizontalDamageSprite = null;
    public Sprite verticalDamageSprite = null;
    public Sprite crossDamageSprite = null;
    public Sprite ballSprite = null;
    public Sprite nullSprite = null;
    public Sprite randomValueSprite = null;
    public Sprite valueSprite = null;

    protected override void OnInitialize()
    {

    }

    public static Texture2D GetTexture(BlockType type)
    {
        switch (type)
        {
            case BlockType.BoxKill: return Instance.boxKillSprite.texture;
            case BlockType.HorizontalKill: return Instance.horizontalKillSprite.texture;
            case BlockType.VerticalKill: return Instance.verticalKillSprite.texture;
            case BlockType.CrossKill: return Instance.crossKillSprite.texture;
            case BlockType.HorizontalDamage: return Instance.horizontalDamageSprite.texture;
            case BlockType.VerticalDamage: return Instance.verticalDamageSprite.texture;
            case BlockType.CrossDamage: return Instance.crossDamageSprite.texture;
            case BlockType.Ball: return Instance.ballSprite.texture;
            case BlockType.Null: return Instance.nullSprite.texture;
            case BlockType.RandomValue: return Instance.randomValueSprite.texture;
            case BlockType.Value: return Instance.valueSprite.texture;
        }
        return null;
    }
}
