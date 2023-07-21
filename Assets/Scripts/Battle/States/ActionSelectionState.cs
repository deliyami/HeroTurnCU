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
        selectionUI.OnBack += OnActionCancel;
        selectionUI.OnSituation += GoToBattleSituationState;

        // TODO unit의 이름까지 넘겨야함
        bs.DialogBox.SetDialog($"행동을 선택하세요.");

        if (bs.UnitCount != 1) arrow[bs.ActionIndex].gameObject.SetActive(true);
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
        selectionUI.OnBack -= OnActionCancel;
        selectionUI.OnSituation -= GoToBattleSituationState;
    }

    void OnActionSelected(int selection)
    {
        // 1. 버튼 누름 -> actionselectionUI->selectionUI의 public virtual void HandleUpdate()를 이용해 이 메소드에 들어옴, 각자의 state로 변경
        if (selection == 0)
        {
            // TODO 해당 유닛이 PP가 전부 0일 경우에 자폭공격
            // var action = new BattleAction()
            // {
            //     Type = ActionType.Move,
            //     User = currentUnit,
            //     Target = enemyUnits[0],
            //     Move = move
            // }
            // bs.AddBattleAction(action);
            bool isHavePP = true;
            foreach (Move m in bs.PlayerUnits[bs.ActionIndex].Unit.Moves)
            {
                if (m.PP > 0)
                {
                    isHavePP = true;
                    break;
                }
                isHavePP = false;
            }
            if (!isHavePP)
            {
                BattleUnit user = bs.PlayerUnits[bs.ActionIndex];
                Move move = (user.Unit.Attack >= user.Unit.SpAttack) ?
                                new Move(GlobalSettings.i.StrugglePhysical) :
                                new Move(GlobalSettings.i.StruggleSpecial);
                UnitSelectionState.i.Move = move;
                bs.StateMachine.ChangeState(UnitSelectionState.i);
                return;
            }

            MoveSelectionState.i.Moves = bs.PlayerUnits[bs.ActionIndex].Unit.Moves;
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
    void OnActionCancel()
    {
        bs.ResetActions();
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

    void GoToBattleSituationState()
    {
        bs.StateMachine.ChangeState(BattleSituationState.i);
        // bs.StateMachine.ChangeState(BattleSituationState.i);
    }

    public List<ActiveUnitArrow> Arrow => arrow;
}
