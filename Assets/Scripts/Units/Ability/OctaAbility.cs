using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctaAbility : AbilityBase
{
    public override float OnDefense(Unit defender, float type, MoveCategory moveCategory)
    {

        return moveCategory == MoveCategory.Special || defender.Status.ID == ConditionID.none ? 1f : 0.5f;
    }
}
