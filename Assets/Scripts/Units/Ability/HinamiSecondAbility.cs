using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HinamiSecondAbility : AbilityBase
{
    public override List<ConditionID> AfterAttack(BattleUnit attacker, BattleUnit defender, Move move)
    {
        for (int i = 0; i < move.Base.GetHitTimes(); i++)
        {
            if (UnityEngine.Random.Range(0, 10) == 0)
            {
                return new List<ConditionID>() { ConditionID.none, ConditionID.flinch };
            }
        }

        return null;
    }
}
