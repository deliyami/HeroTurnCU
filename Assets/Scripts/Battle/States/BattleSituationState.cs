using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSituationState : State<BattleSystem>
{
    [SerializeField] BattleSituationUI selectionUI;
    // Start is called before the first frame update
    public static BattleSituationState i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    BattleSystem bs;

    public override void Enter(BattleSystem owner)
    {
        bs = owner;

        selectionUI.gameObject.SetActive(true);
        selectionUI.OnBack += OnBack;

        bs.DialogBox.EnableDialogText(false);
    }
    public override void Execute()
    {
        selectionUI.HandleUpdate();
    }
    public override void Exit()
    {
        selectionUI.gameObject.SetActive(false);
        selectionUI.OnBack -= OnBack;

        selectionUI.ClearItems();

        bs.DialogBox.EnableDialogText(true);
    }
    void OnBack()
    {
        bs.StateMachine.ChangeState(ActionSelectionState.i);
    }
}
