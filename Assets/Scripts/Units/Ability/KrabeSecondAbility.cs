using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrabeSecondAbility : AbilityBase
{
    public override float OnDefense(Unit defender, float type)
    {
        if (isActivatableAbiility && type > 1.0f)
        {
            isActivatableAbiility = false;
            return 0.5f;
        }
        return 1.0f;
    }
}
