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

    private static int gridOffset
    {
        get { return EditorPrefs.GetInt("PatternConfigEditor.gridOffset", 0); }
        set { EditorPrefs.SetInt("PatternConfigEditor.gridOffset", value); }
    }


    public override void OnInspectorGUI()
    {
        var obj = target as PatternConfig;

        obj.height = EditorGUILayout.DelayedIntField("Height", obj.height);
        obj.startLength = EditorGUILayout.IntField("Start Length", obj.startLength);
        obj.wrapMode = (PatternConfig.WrapMode)EditorGUILayout.EnumPopup("Wrap Mode", obj.wrapMode);

        obj.verticalRandom.activated = EditorGUILayout.Toggle("Vertical Random", obj.verticalRandom.activated);
        if (obj.verticalRandom.activated)
            obj.verticalRandom.startStep = EditorGUILayout.IntField(" Start Step", obj.verticalRandom.startStep);

        obj.horizontalRandom.activated = EditorGUILayout.Toggle("Horizontal Random", obj.horizontalRandom.activated);
        if (obj.horizontalRandom.activated)
            obj.horizontalRandom.startStep = EditorGUILayout.IntField(" Start Step", obj.horizontalRandom.startStep);

        SetHeight(obj, obj.height);
        brush = (BlockType)EditorGUILayout.EnumPopup("Brush", brush);
        if (brush == BlockType.Value)
            brushValue = EditorGUILayout.IntField("Value", brushValue);

        gridOffset = EditorGUILayout.IntSlider("Grid Offset", gridOffset, 0, obj.height);

        style.normal.textColor = Color.red;
        style.stretchHeight = style.stretchWidth = true;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 30;

        var rect = EditorGUILayout.GetControlRect();
        var w = rect.width / PatternConfig.width;
        var r = new Rect(0, 0, w, w);
        var offset = gridOffset * PatternConfig.width;
        for (int i = 0; i < obj.blocks.Count; i++)
        {
            if (i < offset) continue;
            var ioffset = i - offset;
            r.x = rect.x + (ioffset % PatternConfig.width) * w;
            r.y = rect.y + (ioffset / PatternConfig.width) * w;

            if (obj.blocks[i] > 0)
            {
                style.normal.background = EditorFactory.GetTexture(BlockType.Value);
                int value = (int)obj.blocks[i];
                if (GUI.Button(r, value.ToString().Persian(), style))
                {
                    obj.blocks[i] = brush == BlockType.Value ? (BlockType)brushValue : brush;
                }
            }
            else
            {
                style.normal.background = EditorFactory.GetTexture(obj.blocks[i]);
                if (GUI.Button(r, string.Empty, style))
                {
                    obj.blocks[i] = brush == BlockType.Value ? (BlockType)brushValue : brush;
                }
            }
        }
        EditorUtility.SetDirty(obj);
    }

    private static void SetHeight(PatternConfig obj, int height)
    {
        int length = PatternConfig.width * obj.height;
        while (obj.blocks.Count < length)
            obj.blocks.Add(BlockType.Null);
        while (obj.blocks.Count > length)
            obj.blocks.RemoveAt(obj.blocks.Count - 1);
    }
}
