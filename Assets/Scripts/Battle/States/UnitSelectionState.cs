using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: 2vs2에서 자리 하나 비었을 때 확인해야 함
// 1. 상대자리 하나 비었을 때
// 1-1. UnitSelectionUI에서 방향키 이동 작동을 조작, ←↑는 selected 1, ↓→는 selected 3 고정 변환 및 살아있는거만 변환시키고... submit할 때는 살아있는거만 return하면 될 듯
// 2. 내자리 하나 비었을 때
// 2-1. UnitSelectionUI에서 selectedItem = Mathf.Clamp(selectedItem, 0, enemyUnits.Count + playerUnits.Count - 2); clamp최대치를 1 줄임
// 아니지... 죽은 녀석들 setActive를 false로 만들지 말고... isAlive라던가 추가해서, enemyUnits[i].SetSelected(i == selectedItem); 이런거 할 때 살아있는지 아닌지 확인하고 바꾸기
public class UnitSelectionState : State<BattleSystem>
{
    [SerializeField] UnitSelectionUI selectionUI;
    List<BattleUnit> playerUnits;
    List<BattleUnit> enemyUnits;
    public Move Move { get; set; }

    public static UnitSelectionState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    BattleSystem bs;

    public override void Enter(BattleSystem owner)
    {
        bs = owner;
        playerUnits = bs.PlayerUnits;
        enemyUnits = bs.EnemyUnits;

        if (bs.UnitCount == 1)
        {
            OnUnitSelected(Move.Base.Target == MoveTarget.Self || Move.Base.Target == MoveTarget.Team || Move.Base.Target == MoveTarget.TeamAll ? 1 : 0);
            return;
        }

        UnitSelectionUI.i.progressState();

        selectionUI.OnSelected += HandleSubmit;
        selectionUI.OnBack += OnBack;
    }
    public override void Execute()
    {
        selectionUI.HandleUpdate();
    }
    public override void Exit()
    {
        selectionUI.OnSelected -= HandleSubmit;
        selectionUI.OnBack -= OnBack;
    }
    void HandleSubmit(int selection)
    {
        selectionUI.ClearItems();
        OnUnitSelected(selection);
        Debug.Log("use submit");
    }
    void OnUnitSelected(int selection)
    {
        // 3. 결정키를 눌렀을 때.
        // 여기서 액션 추가하는게 더 나을까?
        // 일단은 여기서 계산 전부해서 bs.StateMachine.ChangeState(RunTurnState.i)넣어보고 RunTurnState에서 길이 부족한거 인식하고 ActionSelectionState로 돌아가면 개이득이고
        // 아무리봐도 emit같은것이 생각이 안나서 여기서 끝내는게 맞는거 같다
        // 제대로 돌아가면 그대로 쓰고
        // 만약 같은 state가 2~3개씩 겹치는 경우가 생기면, 이 State만 없애는 bs.StateMachine.ChangeState(ActionSelectionState.i)나 MoveSelectionState거쳐서 2단계를 올리거나 1단계식 올리거나 해야 할 듯


        // MoveSelectionState전부 옮겨서 if문으로 나눠서 스킬 타입별 선택 할 수 있는거 정해야 할 듯
        var currentUnit = bs.PlayerUnits[bs.ActionIndex];

        var checkPlayerUnit = playerUnits.FirstOrDefault(u => u.Unit.Base.Name != currentUnit.Unit.Base.Name);
        BattleUnit playerUnit = checkPlayerUnit is BattleUnit ? checkPlayerUnit : playerUnits.First();

        var enemyUnit = enemyUnits.Count < bs.UnitCount ? enemyUnits.First() : enemyUnits[selection];

        var targetedUnit = selection >= enemyUnits.Count ?
                playerUnit :
                enemyUnit;

        var action = new BattleAction()
        {
            Type = ActionType.Move,
            // 선택된 아군 유닛
            User = currentUnit,
            // 선택된 적 유닛
            Target = targetedUnit,
            Move = Move
        };

        bs.AddBattleAction(action);
        bs.StateMachine.ChangeState(RunTurnState.i);
    }
    void OnBack()
    {
        selectionUI.ClearItems();
        Debug.Log("use cancel");
        bs.StateMachine.ChangeState(MoveSelectionState.i);
    }
}
// 473