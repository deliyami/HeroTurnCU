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

        using(var scope = new GUILayout.HorizontalScope())
        {

            if (GUILayout.Button("대화 이벤트"))
                cutscene.AddAction(new DialogueAction());
            else if (GUILayout.Button("행동 이벤트"))
                cutscene.AddAction(new MoveActorAction());
            else if (GUILayout.Button("회전 이벤트"))
                cutscene.AddAction(new TurnActorAction());
        }
        using(var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("순간이동 이벤트"))
                cutscene.AddAction(new TeleportObjectAction());
            else if (GUILayout.Button("객체 enable 이벤트"))
                cutscene.AddAction(new EnableObjectAction());
            else if (GUILayout.Button("객체 disable 이벤트"))
                cutscene.AddAction(new DisableObjectAction());
        }
        using(var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("NPC interact 이벤트"))
                cutscene.AddAction(new NPCInteractAction());
            else if (GUILayout.Button("Fadein 이벤트"))
                cutscene.AddAction(new FadeInAction());
            else if (GUILayout.Button("Fadeout 이벤트"))
                cutscene.AddAction(new FadeOutAction());
        }
        base.OnInspectorGUI();
    }
}
