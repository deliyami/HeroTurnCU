using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FadeInAction : CutsceneAction
{
    [SerializeField] float duration = 0.5f;
    public override IEnumerator Play()
    {
        yield return Fader.i.FadeIn(duration);
    }
}
