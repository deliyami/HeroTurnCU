using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BallItem", menuName = "Items/Create new ball items")]
public class BallItem : ItemBase
{
    public override bool Use(Unit unit)
    {
        return base.Use(unit);
    }
}
