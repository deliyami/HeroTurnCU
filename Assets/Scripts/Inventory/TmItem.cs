using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 기술머신
[CreateAssetMenu(fileName = "TmItem", menuName = "Items/Create new tm, hm items")]
public class TmItem : ItemBase
{
    [SerializeField] MoveBase move;


    public override bool Use(Unit unit)
    {
        // 못배우면 false 리턴해야 함
        // return unit.HasMove(move);
        return true;
    }

    public MoveBase Move => move;
}
