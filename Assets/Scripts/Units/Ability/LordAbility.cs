using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LordAbility : AbilityBase
{
    public override int BeforeTurnChange(BattleUnit sourceUnit)
    {
        if (sourceUnit.Unit.HP <= sourceUnit.Unit.MaxHP) return 1;
        return 0;
    }
}
