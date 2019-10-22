using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PatternConfig))]
public class PatternConfigEditor : Editor
{
    private static GUIStyle style = new GUIStyle();

    private static BlockType brush
    {
        get { return (BlockType)EditorPrefs.GetInt("PatternConfigEditor.brush", -1); }
        set { EditorPrefs.SetInt("PatternConfigEditor.brush", (int)value); }
    }

    private static int brushValue
    {
        get { return EditorPrefs.GetInt("PatternConfigEditor.brushValue", 1); }
        set { EditorPrefs.SetInt("PatternConfigEditor.brushValue", value); }
    }

    public override void OnInspectorGUI()
    {
        var obj = target as PatternConfig;
        while (obj.blocks.Count < PatternConfig.Length)
            obj.blocks.Add(BlockType.Null);

        brush = (BlockType)EditorGUILayout.EnumPopup("Brush", brush);
        if (brush == BlockType.Value)
            brushValue = EditorGUILayout.IntField("Value", brushValue);

        style.normal.textColor = Color.red;
        style.stretchHeight = style.stretchWidth = true;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 30;

        var rect = EditorGUILayout.GetControlRect();
        var w = rect.width / PatternConfig.Width;
        var r = new Rect(0, 0, w, w);
        for (int i = 0; i < obj.blocks.Count; i++)
        {
            r.x = rect.x + (i % PatternConfig.Width) * w;
            r.y = rect.y + (i / PatternConfig.Width) * w;

            if (obj.blocks[i] > 0)
            {
                style.normal.background = GlobalFactory.Theme.GetSprite(BlockType.Value).texture;
                int value = (int)obj.blocks[i];
                if (GUI.Button(r, value.ToString().Persian(), style))
                {
                    obj.blocks[i] = brush == BlockType.Value ? (BlockType)brushValue : brush;
                    EditorUtility.SetDirty(obj);
                }
            }
            else
            {
                style.normal.background = GlobalFactory.Theme.GetSprite(obj.blocks[i]).texture;
                if (GUI.Button(r, string.Empty, style))
                {
                    obj.blocks[i] = brush == BlockType.Value ? (BlockType)brushValue : brush;
                    EditorUtility.SetDirty(obj);
                }
            }
        }
    }
}
