using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionState : State<BattleSystem>
{
    [SerializeField] MoveSelectionUI selectionUI;
    [SerializeField] GameObject moveDetailsUI;

    // input
    public List<Move> Moves { get; set; }
    public int currentMove { get; private set; }

    public static MoveSelectionState i { get; private set; }

    private void Awake()
    {
        Debug.Log("wake up in moveselection");
        i = this;
    }

    BattleSystem bs;

    public override void Enter(BattleSystem owner)
    {
        bs = owner;

        selectionUI.SetMoves(Moves);

        selectionUI.gameObject.SetActive(true);
        selectionUI.OnSelected += OnMoveSelected;
        selectionUI.OnBack += OnBack;

        moveDetailsUI.SetActive(true);
        bs.DialogBox.EnableDialogText(false);
    }

    public override void Execute()
    {
        selectionUI.HandleUpdate();
    }

    public override void Exit()
    {
        selectionUI.gameObject.SetActive(false);
        selectionUI.OnSelected -= OnMoveSelected;
        selectionUI.OnBack -= OnBack;

        selectionUI.ClearItems();

        moveDetailsUI.SetActive(false);
        bs.DialogBox.EnableDialogText(true);
    }

    // 2. 상하좌우이동을 moveselectionui -> selectionUI 의 handleupdate 이후에 여기로 옴.
    void OnMoveSelected(int selection)
    {
        // TODO 이거 수정 할 필요가 있음
        // state저장을 프리롬 -> 전투 -> 행동(전투, 아이템, 유닛) 선택 -> 스킬 선택 -> 적 유닛 선택(2번 반복) -> 전투 -> 반복...으로
        // 1대1은 그냥 밑처럼 쓰면 되는데
        // 2대2는 선택된 적 아군 유닛 구별 할 필요가 있음
        // 상대 자신도 구분해야함
        // BattleSystem에 void HandleTargetSelection()를 참조해서 작성 할 것

        // var action = new BattleAction()
        // {
        //     Type = ActionType.Move,
        //     // 선택된 아군 유닛
        //     User = bs.PlayerUnits[0],
        //     // 선택된 적 유닛
        //     Target = bs.EnemyUnits[0],
        //     Move = Moves[selection]
        // };
        // bs.AddBattleAction(action);
        // bs.StateMachine.ChangeState(RunTurnState.i);
        currentMove = selection;

        var testValue = bs.PlayerUnits[bs.ActionIndex].Unit.Moves;
        UnitSelectionState.i.Moves = testValue;
        bs.StateMachine.ChangeState(UnitSelectionState.i);
    }

    void OnBack()
    {
        bs.StateMachine.ChangeState(ActionSelectionState.i);
    }
}
