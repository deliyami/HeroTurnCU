using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletAbility : AbilityBase
{
    public override void BeforeAttack(BattleUnit attacker, Move move)
    {
        attacker.Unit.Base.SetType1(move.Base.Type);
    }
}
