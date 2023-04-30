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
        selectionUI.gameObject.SetActive(false);
        selectionUI.OnSelected -= OnActionSelected;
    }

    void OnActionSelected(int selection)
    {
        if (selection == 0)
        {

            // fight TODO 이거 2번 연속으로 싸우는 경우도 생각해야 됨
            // 이거 다른데서 하자... runturnstate에서 하는게 처리하는게 편하다\
            // 밑에 런이라던가 그런것들도 마찬가지. 여기서 처리하면 battle system에서 처리하는거랑 마찬가지이므로
            // 여기서는 다른 state로 이동하는 것만 처리
            // 다른 state에서 어떠한 액션이 들어가는지 처리
            // var action = new BattleAction()
            // {
            //     Type = ActionType.Move,
            //     User = currentUnit,
            //     Target = enemyUnits[0],
            //     Move = move
            // }
            // bs.AddBattleAction(action);
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
        else if (selection == 2)
        {
            // item
        }
        else if (selection == 3)
        {
            // unit
        }
        else
        {
            // run
            var action = new BattleAction()
            {
                Type = ActionType.Run
            };
            bs.AddBattleAction(action);
        }
    }
}
