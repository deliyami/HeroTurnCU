using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelectionState : State<BattleSystem>
{
    [SerializeField] ActionSelectionUI selectionUI;

    public static ActionSelectionState i { get; private set; }

    private void Awake()
    {
        Debug.Log("wake up in actionselection");
        i = this;
    }

    BattleSystem bs;

    public override void Enter(BattleSystem owner)
    {
        bs = owner;

        selectionUI.gameObject.SetActive(true);
        selectionUI.OnSelected += OnActionSelected;

        // TODO unit의 이름까지 넘겨야함
        bs.DialogBox.SetDialog($"행동을 선택하세요.");
        // bs.DialogBox.SetDialog($"{currentUnit.Unit.Base.Name}의 행동을 선택하세요.");
    }

    public override void Execute()
    {
        selectionUI.HandleUpdate();
    }

    public override void Exit()
    {
        selectionUI.gameObject.SetActive(true);
        selectionUI.OnSelected -= OnActionSelected;
    }

    void OnActionSelected(int selection)
    {
        if (selection == 0)
        {
            // fight TODO 이거 2번 연속으로 싸우는 경우도 생각해야 됨
            Debug.Log("here is ActionSelectionState");
            Debug.Log(bs.ActionIndex);
            Debug.Log(MoveSelectionState.i);
            Debug.Log(MoveSelectionState.i.Moves);
            var testValue = bs.PlayerUnits[bs.ActionIndex].Unit.Moves;
            MoveSelectionState.i.Moves = testValue;
            // MoveSelectionState.i.Moves = bs.PlayerUnits[bs.ActionIndex].Unit.Moves;
            // 순서 변경좀;
            bs.StateMachine.ChangeState(MoveSelectionState.i);
        }
    }
}
