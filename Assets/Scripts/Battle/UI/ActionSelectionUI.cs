using GDE.GenericSelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionSelectionUI : SelectionUI<TextSlot>
{
    public event Action OnSituation;
    public override void HandleUpdate()
    {

        base.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.C))
        {
            // BattleSituationState 에 추가 할 것
            HandleSituation();
        }
    }
    private void Start()
    {
        SetSelectionSettings(SelectionType.Grid, 2);
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }
    protected void HandleSituation()
    {
        OnSituation?.Invoke();
    }
}
