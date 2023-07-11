using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSecondAbility : AbilityBase
{
    public override (ConditionID, ConditionID, Stat, int, MoveTarget) AfterDefense(BattleUnit attacker, BattleUnit defender, Move move)
    {
        if (isActivatableAbiility && (TypeChart.GetEffectiveness(move.Base.Type, defender.Unit.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, defender.Unit.Base.Type2) > 1.0f))
        {
            isActivatableAbiility = false;
            StatBoost boostAtk = new StatBoost()
            {
                stat = Stat.Attack,
                boost = 2
            };
            StatBoost boostSP = new StatBoost()
            {
                stat = Stat.SpAttack,
                boost = 2
            };
            defender.Unit.ApplyBoosts(new List<StatBoost>() { boostAtk, boostSP });
        }
        return (ConditionID.none, ConditionID.none, Stat.Attack, 0, MoveTarget.Foe);
    }
}
