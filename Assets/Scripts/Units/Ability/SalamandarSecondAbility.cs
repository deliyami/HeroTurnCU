using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalamandarSecondAbility : AbilityBase
{
    public override float OnAttack(Move move)
    {
        return move.Base.Type == UnitType.Fire ? 1.2f : 1;
    }
}
