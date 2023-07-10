using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAbility : AbilityBase
{
    public override bool BeforeDefense(BattleUnit defender, Move move)
    {
        return move.Base.Type != UnitType.Soil ? true : false;
    }
}
