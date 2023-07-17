using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GDEUtils.StateMachine;
using UnityEngine;

public class PartyState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;

    public Unit SelectedUnit { get; private set; }
    public static PartyState i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;

        SelectedUnit = null;

        partyScreen.gameObject.SetActive(true);
        partyScreen.OnSelected += OnUnitSelected;
        partyScreen.OnBack += OnBack;
    }
    public override void Execute()
    {
        partyScreen.HandleUpdate();
    }
    public override void Exit()
    {
        partyScreen.gameObject.SetActive(false);
        partyScreen.OnSelected -= OnUnitSelected;
        partyScreen.OnBack -= OnBack;
    }

    void OnUnitSelected(int selection)
    {
        SelectedUnit = partyScreen.SelectedMember;

        var prevState = gc.StateMachine.GetPrevState();
        if (prevState == InventoryState.i)
        {
            // 아이템 사용
            StartCoroutine(GoToUseItemState());
        }
        else if (prevState == BattleState.i)
        {
            var battleState = prevState as BattleState;

            if (SelectedUnit.HP <= 0)
            {
                partyScreen.SetMessageText("그 동료는 지쳤다!");
                return;
            }
            if (battleState.BattleSystem.PlayerUnits.Any(u => u.Unit == SelectedUnit))
            {
                partyScreen.SetMessageText("그 동료는 싸우고 있다!");
                return;
            }

            gc.StateMachine.Pop();
        }
        else
        {
            // 뭐였지, 설명창인데
            Debug.Log($"{selection} 선택됨");
            if (partyScreen.changedItem == -1) partyScreen.changedItem = selection;
            else if (partyScreen.changedItem == selection)
            {
                partyScreen.changedItem = -1;
            }
            else
            {
                partyScreen.ResetUI();
                UnitParty party = UnitParty.GetPlayerParty();
                List<Unit> units = party.Units;
                Unit tmp = units[selection];
                units[selection] = units[partyScreen.changedItem];
                units[partyScreen.changedItem] = tmp;
                party.Units = units;
                // party.PartyUpdated();
                partyScreen.changedItem = -1;
            }
        }
    }
    IEnumerator GoToUseItemState()
    {
        yield return gc.StateMachine.PushAndWait(UseItemState.i);
        gc.StateMachine.Pop();
    }
    void OnBack()
    {
        SelectedUnit = null;
        var prevState = gc.StateMachine.GetPrevState();
        partyScreen.changedItem = -1;
        partyScreen.selectedItem = 0;
        if (prevState == BattleState.i)
        {
            var battleState = prevState as BattleState;

            if (battleState.BattleSystem.PlayerUnits.Any(u => u.Unit.HP <= 0))
            {
                partyScreen.SetMessageText("전투를 계속하기 위해 팀원을 보내야합니다!");
                return;
            }
            partyScreen.gameObject.SetActive(false);
        }
        gc.StateMachine.Pop();
    }
}
