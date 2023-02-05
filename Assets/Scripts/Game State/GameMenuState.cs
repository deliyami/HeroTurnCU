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
        else if (selection == 1)
            gc.StateMachine.Push(InventoryState.i);
    }
    void OnBack()
    {
        gc.StateMachine.Pop();
    }
}
