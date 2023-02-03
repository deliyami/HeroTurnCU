using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cutscene = target as Cutscene;
        if (GUILayout.Button("대화 액션 추가"))
            cutscene.AddAction(new DialogueAction());
        else if (GUILayout.Button("행동 액션 추가"))
            cutscene.AddAction(new MoveActorAction());
        else if (GUILayout.Button("회전 액션 추가"))
            cutscene.AddAction(new TurnActorAction());
        else if (GUILayout.Button("순간이동 액션 추가"))
            cutscene.AddAction(new TeleportObjectAction());
        base.OnInspectorGUI();
    }
}
