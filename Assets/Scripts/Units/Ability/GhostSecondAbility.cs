using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSecondAbility : AbilityBase
{
    public override float OnAttack(Move move) { return 1.3f; }
    public override List<ConditionID> AfterAttack(BattleUnit attacker, BattleUnit defender, Move move)
    {
        attacker.Unit.DecreaseHP(attacker.Unit.MaxHP / 10);
        StartCoroutine(attacker.Hud.WaitForHPUpdate());
        StartCoroutine(BattleSystem.i.DialogBox.TypeDialog("기묘한 힘 때문에 체력이 깎인다!"));
        return null;
    }
}
