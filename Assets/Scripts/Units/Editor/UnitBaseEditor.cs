using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitBase))]
public class UnitBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var unitEffort = serializedObject.FindProperty("effort");
        int result = 0;
        for (int i = 0; i < unitEffort.arraySize; i++)
        {
            int unitEffortValue = unitEffort.GetArrayElementAtIndex(i).intValue;
            result += unitEffortValue;


            if (unitEffortValue > 252)
                EditorGUILayout.HelpBox("노력치의 스텟이 252를 넘겼습니다", MessageType.Error);
        }
        if (result <= 510)
            EditorGUILayout.HelpBox($"총합: {result}", MessageType.Info);
        else if (result > 510)
            EditorGUILayout.HelpBox("총합이 510을 넘으면 안됩니다.", MessageType.Error);
    }
}
