using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarlSecondAbility : AbilityBase
{
    public override float OnAttack(Move move) { return 1.3f; }
    public override (ConditionID, ConditionID, Stat, int, MoveTarget) AfterAttack(BattleUnit attacker, BattleUnit defender, Move move)
    {
        attacker.Unit.DecreaseHP(attacker.Unit.MaxHP / 16);
        StartCoroutine(attacker.Hud.WaitForHPUpdate());
        StartCoroutine(BattleSystem.i.DialogBox.TypeDialog("몸을 무리하게 쓰고 있다!"));
        return base.AfterAttack(attacker, defender, move);
    }
}
