using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitInvisible : CutsceneAction
{
    public override IEnumerator Play()
    {
        yield return PlayerController.i.Invisible();
    }
}
