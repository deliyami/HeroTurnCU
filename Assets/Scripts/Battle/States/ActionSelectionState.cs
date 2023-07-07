using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelectionState : State<BattleSystem>
{
    [SerializeField] ActionSelectionUI selectionUI;
    [SerializeField] List<ActiveUnitArrow> arrow;

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

        arrow[bs.ActionIndex].gameObject.SetActive(true);
        // bs.PlayerUnits[bs.ActionIndex].SetSelected(true);
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
        // 1. 버튼 누름 -> actionselectionUI->selectionUI의 public virtual void HandleUpdate()를 이용해 이 메소드에 들어옴, 각자의 state로 변경
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
            Debug.Log(bs.ActionIndex);
            Debug.Log(MoveSelectionState.i);
            Debug.Log(MoveSelectionState.i.Moves);
            var testValue = bs.PlayerUnits[bs.ActionIndex].Unit.Moves;
            MoveSelectionState.i.Moves = testValue;
            // MoveSelectionState.i.Moves = bs.PlayerUnits[bs.ActionIndex].Unit.Moves;
            // 순서 변경좀;
            bs.StateMachine.ChangeState(MoveSelectionState.i);
        }
        else if (selection == 1)
        {
            // item
            StartCoroutine(GoToInventoryState());
        }
        else if (selection == 2)
        {
            // unit
            StartCoroutine(GoToPartyState());
        }
        else
        {
            // run
            var action = new BattleAction()
            {
                Type = ActionType.Run,
                User = bs.PlayerUnits[bs.ActionIndex]
            };
            bs.AddBattleAction(action);
            bs.StateMachine.ChangeState(RunTurnState.i);
        }
    }

    IEnumerator GoToPartyState()
    {
        yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
        var selectedUnit = PartyState.i.SelectedUnit;
        if (selectedUnit != null)
        {
            var action = new BattleAction()
            {
                Type = ActionType.SwitchUnit,
                User = bs.PlayerUnits[bs.ActionIndex],
                SelectedUnit = selectedUnit
            };
            bs.AddBattleAction(action);
            bs.StateMachine.ChangeState(RunTurnState.i);
        }
    }

    IEnumerator GoToInventoryState()
    {
        yield return GameController.Instance.StateMachine.PushAndWait(InventoryState.i);

        var selectedItem = InventoryState.i.SelectedItem;
        if (selectedItem != null)
        {
            // TODO: need to change action turn new BATTLEACTION
            var action = new BattleAction()
            {
                Type = ActionType.UseItem,
                User = bs.PlayerUnits[bs.ActionIndex],
                SelectedUnit = PartyState.i.SelectedUnit,
                SelectedItem = selectedItem
            };
            bs.AddBattleAction(action);
            bs.StateMachine.ChangeState(RunTurnState.i);
        }
    }

    public List<ActiveUnitArrow> Arrow => arrow;
}
