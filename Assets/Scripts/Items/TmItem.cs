using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 기술머신
[CreateAssetMenu(fileName = "TmItem", menuName = "Items/Create new tm, hm items")]
public class TmItem : ItemBase
{
    [SerializeField] MoveBase move;
    [SerializeField] bool isHM;

    // public override string Name => base.Name + $": {move.Name}";

    public override bool Use(Unit unit)
    {
        // 못배우면 false 리턴해야 함
        // return unit.HasMove(move);
        return true;
    }
    public bool CanBeTaught(Unit unit)
    {
        return unit.Base.LearnableByItems.Contains(move);
    }
    public override bool IsReusable => isHM;
    public override bool CanBeUsedInBattle => false;

    public MoveBase Move => move;

    // 비전 머신
    public bool IsHM => isHM;
}
