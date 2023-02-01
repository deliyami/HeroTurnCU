using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EvolutionItem", menuName = "Items/Create new evolution items")]
public class EvolutionItem : ItemBase
{
    public override bool Use(Unit unit)
    {
        return base.Use(unit);
    }
}
