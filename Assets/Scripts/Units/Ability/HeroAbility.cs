using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAbility : AbilityBase
{
    public override void AfterRunTurn(BattleUnit sourceUnit)
    {
        StatBoost boostAtk = new StatBoost()
        {
            stat = Stat.Defense,
            boost = 1
        };
        sourceUnit.Unit.ApplyBoosts(new List<StatBoost>() { boostAtk });
    }
}
