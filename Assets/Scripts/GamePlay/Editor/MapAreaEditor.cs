using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapArea))]
public class MapAreaEditor : Editor 
{
    public override void OnInspectorGUI() 
    {
        base.OnInspectorGUI();
        
        int totalChance = serializedObject.FindProperty("totalChance").intValue;
        var style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        GUILayout.Label($"확률 총합: {totalChance}", style);
        if (totalChance <= 0)
            EditorGUILayout.HelpBox("총합이 0을 넘겨야 합니다.", MessageType.Error);
    }
}