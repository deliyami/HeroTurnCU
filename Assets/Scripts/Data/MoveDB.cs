using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveDB : ScriptableObjectDB<MoveBase>
{
    public static Dictionary<MoveTarget, string> MoveTargets { get; set; } = new Dictionary<MoveTarget, string>()
    {
        {
            MoveTarget.Foe,
            "적 하나를"
        },
        {
            MoveTarget.FoeAll,
            "적 전부를"
        },
        {
            MoveTarget.Team,
            "팀 하나를"
        },
        {
            MoveTarget.TeamAnother,
            "자신을 제외한 팀 하나를"
        },
        {
            MoveTarget.TeamAll,
            "팀 전부를"
        },
        {
            MoveTarget.Self,
            "자신을"
        },
        {
            MoveTarget.Another,
            "자신을 제외한 하나를"
        },
        {
            MoveTarget.AnotherAll,
            "자신을 제외한 전부를"
        },
        {
            MoveTarget.All,
            "전부를"
        }
    };
}
// Foe, FoeAll, Team, TeamAnother, TeamAll, Self, Another, AnotherAll, All