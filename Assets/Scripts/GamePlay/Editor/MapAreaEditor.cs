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
        
        int totalChanceInGrass = serializedObject.FindProperty("totalChance").intValue;
        var style = new GUIStyle();
        
        if (totalChanceInGrass < 0)
            EditorGUILayout.HelpBox("총합이 0을 넘겨야 합니다. 풀에서", MessageType.Error);

        int totalChanceInWater = serializedObject.FindProperty("totalChanceWater").intValue;
        if (totalChanceInWater < 0)
            EditorGUILayout.HelpBox("총합이 0을 넘겨야 합니다. 풀에서", MessageType.Error);
    }
}