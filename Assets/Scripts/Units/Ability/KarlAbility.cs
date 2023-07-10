using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarlAbility : AbilityBase
{
    public override List<StatBoost> OnFinish()
    {
        return new List<StatBoost>(){
            new StatBoost(){
                stat = Stat.Attack,
                boost = 1
            }
        };
    }
}
