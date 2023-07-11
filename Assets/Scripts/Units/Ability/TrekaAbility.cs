using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrekaAbility : AbilityBase
{
    public override float OnAttack(Move move)
    {
        return move.Base.Type == UnitType.Steel || move.Base.Type == UnitType.Wind ? 4 / 3f : 1f;
    }
}
