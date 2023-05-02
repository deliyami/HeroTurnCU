using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitVisible : CutsceneAction
{
    public override IEnumerator Play()
    {
        yield return PlayerController.i.Visible();
    }
}
