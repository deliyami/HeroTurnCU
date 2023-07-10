using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSecondAbility : AbilityBase
{
    public override void AfterRunTurn(BattleUnit sourceUnit)
    {
        sourceUnit.Unit.IncreaseHP(sourceUnit.Unit.MaxHP / 16);
    }
}
