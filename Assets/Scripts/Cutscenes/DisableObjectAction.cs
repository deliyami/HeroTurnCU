using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DisableObjectAction : CutsceneAction
{
    [SerializeField] GameObject go;
    public override IEnumerator Play()
    {
        go.SetActive(false);
        yield break;
    }
}
