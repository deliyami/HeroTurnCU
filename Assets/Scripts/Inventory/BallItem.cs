using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BallItem", menuName = "Items/Create new ball items")]
public class BallItem : ItemBase
{
    [SerializeField] float catchRateModifier = 1;
    public override bool Use(Unit unit)
    {
        if (GameController.Instance.State == GameState.Battle) return true;
        return false;
    }

    public float CatchRateModifier => catchRateModifier;
}
