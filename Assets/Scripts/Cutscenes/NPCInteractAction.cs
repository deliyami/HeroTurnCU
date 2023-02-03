using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCInteractAction : CutsceneAction
{
    [SerializeField] NPCController npc;
    public override IEnumerator Play()
    {
        yield return npc.Interact(PlayerController.i.transform);
    }
}
