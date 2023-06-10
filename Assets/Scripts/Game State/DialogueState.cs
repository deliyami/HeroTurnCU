using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueState : State<GameController>
{
    public static DialogueState i { get; private set; }

    private void Awake()
    {
        i = this;
    }
    // GameController gc;
    // public override void Enter(GameController owner)
    // {
    //     gc = owner;
    // }
}
