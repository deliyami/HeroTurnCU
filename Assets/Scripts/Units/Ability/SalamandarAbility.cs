using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalamandarAbility : AbilityBase
{
    public override (ConditionID, ConditionID, Stat, int, MoveTarget) AfterDefense(BattleUnit attacker, BattleUnit defender, Move move)
    {
        if (UnityEngine.Random.Range(0, 10) < 3)
        {
            return (ConditionID.none, ConditionID.brn, Stat.Attack, 0, MoveTarget.Foe);
        }
        return (ConditionID.none, ConditionID.none, Stat.Attack, 0, MoveTarget.Foe);
    }
}
