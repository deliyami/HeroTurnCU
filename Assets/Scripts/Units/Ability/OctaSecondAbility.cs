using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctaSecondAbility : AbilityBase
{

    // public override float OnDefense(Unit defender, float type, MoveCategory moveCategory)
    // {
    //     if (moveCategory == MoveCategory.Special)
    //     {
    //         foreach (var enemyUnit in BattleSystem.i.EnemyUnits)
    //         {
    //             if (enemyUnit.Unit.Base.Name == "카를" || enemyUnit.Unit.Base.Name == "로드") return 0.5f;
    //         }
    //     }
    //     return 1f;
    // }
    public override void AfterRunTurn(BattleUnit sourceUnit)
    {
        if (sourceUnit.Unit.Status.ID != ConditionID.none)
        {
            sourceUnit.Unit.SetStatus(ConditionID.brn);
        }
    }
}
