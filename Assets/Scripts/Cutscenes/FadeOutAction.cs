using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FadeOutAction : CutsceneAction
{
    [SerializeField] float duration = 0.5f;
    public override IEnumerator Play()
    {
        yield return Fader.i.FadeOut(duration);
    }
}
