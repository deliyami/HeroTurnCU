using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrabeAbility : AbilityBase
{
    public override float OnDefense(Unit defender, float type, MoveCategory moveCategory)
    {
        return defender.HP != defender.MaxHP ? 1.0f : 0.5f;
    }
}
