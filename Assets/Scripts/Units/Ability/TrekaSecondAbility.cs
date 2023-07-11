using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrekaSecondAbility : AbilityBase
{
    public override float OnDefense(Unit defender, float type, MoveCategory moveCategory)
    {
        if (isActivatableAbiility && type > 1)
        {
            isActivatableAbiility = false;
            return 0.5f;
        }
        return 1;
    }
}
