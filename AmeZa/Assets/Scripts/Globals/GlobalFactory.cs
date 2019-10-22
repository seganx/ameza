using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFactory : StaticConfig<GlobalFactory>
{
    protected override void OnInitialize()
    {
        //throw new System.NotImplementedException();
    }


    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static class Theme
    {
        public static Sprite GetSprite(BlockType type)
        {
            return Resources.Load<Sprite>("Game/Themes/Defaults/" + type);
        }

        public static Sprite GetSprite(int themeId, int index)
        {
            var sprites = Resources.LoadAll<Sprite>("Game/Themes/" + themeId + "/Items");
            return sprites[index % sprites.Length];
        }

        public static Sprite GetBackground(int themeId)
        {
            return Resources.Load<Sprite>("Game/Themes/" + themeId + "/Background");
        }
    }
}
