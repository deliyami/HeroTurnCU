using System.Collections;
using System.Collections.Generic;
using GDEUtils.StateMachine;
using UnityEngine;

public class GamePartyState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;
    public static GamePartyState i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;

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
        if (gc.StateMachine.GetPrevState() == InventoryState.i)
        {
            // 아이템 사용
            StartCoroutine(GoToUseItemState());
        }
        else
        {
            // 뭐였지, 설명창인데
            Debug.Log($"{selection} 선택됨");
        }
    }
    IEnumerator GoToUseItemState()
    {
        yield return gc.StateMachine.PushAndWait(UseItemState.i);
        gc.StateMachine.Pop();
    }
    void OnBack()
    {
        gc.StateMachine.Pop();
    }
}
