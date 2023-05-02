using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneState : State<GameController>
{
    [SerializeField] CutsceneController cutsceneController;
    public static CutsceneState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        cutsceneController.OnFinishCutscene += EndCutscene;
    }

    public override void Execute()
    {
    }

    public override void Exit()
    {
        cutsceneController.OnFinishCutscene -= EndCutscene;
    }

    void EndCutscene()
    {
        gc.StateMachine.Pop();
    }
}
