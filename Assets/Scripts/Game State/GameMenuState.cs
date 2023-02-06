using System.Collections;
using System.Collections.Generic;
using GDEUtils.StateMachine;
using UnityEngine;

public class GameMenuState : State<GameController>
{
    [SerializeField] MenuController menuController;
    public static GameMenuState i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        menuController.gameObject.SetActive(true);
        menuController.OnSelected += OnMenuItemSelected;
        menuController.OnBack += OnBack;
    }
    public override void Execute()
    {
        menuController.HandleUpdate();

    }
    public override void Exit()
    {
        menuController.gameObject.SetActive(false);
        menuController.OnSelected -= OnMenuItemSelected;
        menuController.OnBack -= OnBack;
    }
    void OnMenuItemSelected(int selection)
    {
        Debug.Log($"메뉴 선택{selection}");
        if (selection == 0) // 유닛 선택
            gc.StateMachine.Push(GamePartyState.i);
        else if (selection == 1) // 아이템 선택
            gc.StateMachine.Push(InventoryState.i);
        else if (selection == 4) // 원래는 도감, 이제는 인물? 확인하는거
            gc.StateMachine.Push(DexState.i);
    }
    void OnBack()
    {
        gc.StateMachine.Pop();
    }
}
