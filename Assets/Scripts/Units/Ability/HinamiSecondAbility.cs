using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HinamiSecondAbility : AbilityBase
{
    public override (ConditionID, ConditionID, Stat, int, MoveTarget) AfterAttack(BattleUnit attacker, BattleUnit defender, Move move)
    {
        for (int i = 0; i < move.Base.GetHitTimes(); i++)
        {
            if (UnityEngine.Random.Range(0, 10) == 0)
            {
                return (ConditionID.none, ConditionID.flinch, Stat.Attack, 0, MoveTarget.Foe);
            }
        }

        return base.AfterAttack(attacker, defender, move);
    }
}
