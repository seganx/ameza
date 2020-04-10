using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Cinematic")]
public class CinematicConfig : ScriptableObject
{
    public enum Point : int { Start = 0, End = 1 }
    public enum Character : int { Empty = 0, Ameza = 1, Ame = 2 }
    public enum Face : int { Happy = 0, Normal = 1, Sad = 2 }
    public enum Body : int { Normal = 0, Handup = 1 }

    [System.Serializable]
    public class Sequence
    {
        [EnumToggle] public Character character = Character.Ameza;
        [EnumToggle] public Face face = Face.Normal;
        [EnumToggle] public Body body = Body.Normal;
        [PersianPreview(2, true)] public string text = string.Empty;
    }

    [SerializeField] private Point storyPoint = Point.Start;
    [SpritePreview(50)] public Sprite background = null;
    public List<Sequence> sequences = new List<Sequence>();

    private int seasonId { set; get; }
    private int levelIndex { set; get; }

    private void Awake()
    {
        var parts = name.Split('_');
        seasonId = parts[0].ToInt();
        levelIndex = parts[1].ToInt();
    }

    public bool Check(int season, int level, Point point)
    {
#if UNITY_EDITOR_OFF
        return (seasonId == season && levelIndex == level && storyPoint == point);
#else
        // verify once playing
        var prefName = "CinematicConfig." + name + "." + season + "." + level + "." + point;
        if (PlayerPrefs.GetInt(prefName, 0) == 1)
            return false;

        if (seasonId == season && levelIndex == level && storyPoint == point)
        {
            PlayerPrefs.SetInt(prefName, 1);
            return true;
        }

        return false;
#endif
    }
}
