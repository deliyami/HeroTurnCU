using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItAbility : AbilityBase
{
    public override IEnumerator BeforeRunTurn(Field field, Unit sourceUnit)
    {
        if (isActivatableAbiility)
        {
            isActivatableAbiility = false;
            field.field.SetCondition(ConditionID.mistyTerrain);
            field.field.duration = 8;
            yield return BattleSystem.i.DialogBox.TypeDialog(field.field.condition.StartMessage);
        }
    }
}
