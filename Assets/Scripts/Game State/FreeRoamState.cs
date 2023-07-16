using System.Collections;
using System.Collections.Generic;
using GDEUtils.StateMachine;
using UnityEngine;

public class FreeRoamState : State<GameController>
{
    public static FreeRoamState i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
    }
    public override void Execute()
    {
        PlayerController.i.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.C) && GameController.Instance.StateMachine.CurrentState != DexState.i && GameController.Instance.StateMachine.CurrentState != DexDescriptionState.i)
        {
            gc.StateMachine.Push(DexState.i);
        }

        if (Input.GetButtonDown("Cancel"))
        {
            gc.StateMachine.Push(GameMenuState.i);
        }
    }
}
