using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType { Move, SwitchUnit, UseItem, Run }
public class BattleAction
{
    public ActionType Type { get; set; }
    public BattleUnit User { get; set; }
    public BattleUnit Target { get; set; }

    public Move Move { get; set; } // performing moves
    public Unit SelectedUnit { get; set; } // 스위칭
    public ItemBase SelectedItem { get; set; } // item

    public bool IsInvalid { get; set; }

    public int Priority => (Type == ActionType.Move) ? Move.Base.Priority : 99;
}
