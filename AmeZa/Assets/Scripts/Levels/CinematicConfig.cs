using SeganX;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Cinematic")]
public class CinematicConfig : ScriptableObject
{
    public enum Point : int { Start = 0, End = 1 }
    public enum Character : int { Empty = 0, Ame = 1, Ameza = 2, Joker = 3 }
    public enum Face : int { Happy = 0, Normal = 1, Sad = 2 }
    public enum Body : int { Normal = 0, Handup = 1 }
    public enum Item : int { Hide = 0, One = 1, Two = 2, Three = 3, Four = 4 }

    [System.Serializable]
    public class Sequence
    {
        public Character character = Character.Ameza;
        public Face face = Face.Normal;
        public Item item = Item.Hide;
        public Body body = Body.Normal;
        [PersianPreview(2, true)]
        public string text = string.Empty;
    }

    [SpritePreview(50)] public Sprite background = null;
    public List<Sequence> sequences = new List<Sequence>();

    private int levelIndex { set; get; }
    private Point storyPoint { set; get; }

    private void Awake()
    {
        var parts = name.Split('_');
        if (parts.Length < 2) return;
        levelIndex = parts[0].ToInt();
        storyPoint = (Point)parts[1].ToInt();
    }

    public bool Check(int levelIndex, Point point)
    {
        // verify once playing
        var prefName = "CinematicConfig." + name;
        if (PlayerPrefs.GetInt(prefName, 0) == 1)
            return false;

        if (this.levelIndex == levelIndex && storyPoint == point)
        {
            PlayerPrefs.SetInt(prefName, 1);
            return true;
        }

        return false;
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("SeganX/CreateStories")]
    public static void CreateStories()
    {
        var asset = UnityEditor.Selection.activeObject as TextAsset;
        if (asset == null) return;

        var list = new List<string>(asset.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries));
        list.RemoveAll(x => x.Length < 2);

        var currLevel = 1;
        var currPoint = Point.Start;
        var currChar = Character.Empty;
        var currItem = Item.Hide;
        var currBody = Body.Normal;
        var currFace = Face.Normal;
        var isLevelPoint = new string[] { "پارت", ":" };
        var isItem = new string[] { "سنگ", "دستکش" };
        var isAmeza = new string[] { "عمه", "زا", ":" };
        var isAme = new string[] { "عمه", ":" };
        var isJocker = new string[] { "جوکر", ":" };
        var isStart = new string[] { "شروع", "ابتدا" };
        var isEnd = new string[] { "پایان", "اتمام", "تمام", "انتها" };
        CinematicConfig currConfig = null;
        foreach (var line in list)
        {
            if (line.ContainsAll(isLevelPoint))
            {
                var number = ExtractNumber(line);
                if (number > -1)
                {
                    var hasStart = line.ContainsAny(isStart);
                    var hasEnd = line.ContainsAny(isEnd);
                    if (hasStart || hasEnd)
                    {
                        currLevel = number - 1;
                        currPoint = hasStart ? Point.Start : Point.End;
                        currConfig = CreateInstance<CinematicConfig>();
                        UnityEditor.AssetDatabase.CreateAsset(currConfig, "Assets/Resources/Game/Cinematics/" + currLevel + "_" + (int)currPoint + ".asset");
                        continue;
                    }
                }
            }

            if (line.ContainsAll(isItem))
            {
                var number = ExtractNumber(line);
                if (number > -1)
                {
                    currItem = (Item)number;
                    continue;
                }
            }

            if (line.ContainsAll(isAmeza))
            {
                currChar = Character.Ameza;
                currBody = ExtractBody(line);
                currFace = ExtractFace(line);
                continue;
            }

            if (line.ContainsAll(isAme))
            {
                currChar = Character.Ame;
                currBody = ExtractBody(line);
                currFace = ExtractFace(line);
                continue;
            }

            if (line.ContainsAll(isJocker))
            {
                currChar = Character.Joker;
                currBody = ExtractBody(line);
                currFace = ExtractFace(line);
                continue;
            }

            var sequence = new Sequence()
            {
                character = currChar,
                face = currFace,
                body = currBody,
                item = currItem,
                text = line.Trim(new char[] { ' ', '\n', '\r', '\t' }).CleanFromCode()
            };
            currConfig.sequences.Add(sequence);
            currBody = Body.Normal;
            currFace = Face.Normal;

            UnityEditor.EditorUtility.SetDirty(currConfig);
        }

        UnityEditor.AssetDatabase.SaveAssets();
    }

    private static int ExtractNumber(string line)
    {
        var numberlist = new List<char>(line);
        numberlist.RemoveAll(c => char.IsDigit(c) == false);
        return string.Join("", numberlist).ToInt(-1);
    }

    private static Body ExtractBody(string line)
    {
        var isHandsup = new string[] { "دست", "بالا" };
        return line.ContainsAny(isHandsup) ? Body.Handup : Body.Normal;

    }

    private static Face ExtractFace(string line)
    {
        var isHappy = new string[] { "خوش", "خوشحال", "شاد" };
        var isSad = new string[] { "ناراحت", "غم" };
        if (line.ContainsAny(isHappy))
            return Face.Happy;
        else if (line.ContainsAny(isSad))
            return Face.Sad;
        else return Face.Normal;
    }

#endif
}
