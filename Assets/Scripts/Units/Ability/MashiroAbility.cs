using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MashiroAbility : AbilityBase
{
    public override IEnumerator BeforeRunTurn(Field field, Unit sourceUnit)
    {
        foreach (BattleAction a in BattleSystem.i.Actions)
        {
            if (a.User.Unit.Base.Name == sourceUnit.Base.Name) a.Move.Base.SetMoveType(UnitType.Strange);
        }
        yield return null;
    }
}
