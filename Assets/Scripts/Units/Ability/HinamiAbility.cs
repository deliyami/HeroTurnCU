using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HinamiAbility : AbilityBase
{
    public override float OnAttack(Move move)
    {
        return move.Base.Power <= 60 ? 1.5f : 1.0f;
    }
}
