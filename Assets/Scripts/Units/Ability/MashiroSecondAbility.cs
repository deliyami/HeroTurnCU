using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MashiroSecondAbility : AbilityBase
{
    public override (ConditionID, ConditionID, Stat, int, MoveTarget) AfterAttack(BattleUnit attacker, BattleUnit defender, Move move)
    {
        if (isActivatableAbiility) return base.AfterAttack(attacker, defender, move);
        isActivatableAbiility = false;
        return (ConditionID.none, ConditionID.none, Stat.SpAttack, 1, MoveTarget.Self);
    }
}
