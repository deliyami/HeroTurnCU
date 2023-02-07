using System.Collections;
using System.Collections.Generic;
using GDEUtils.StateMachine;
using UnityEngine;

public class DexDescriptionState : State<GameController>
{
    [SerializeField] DexDescriptionUI dexDescriptionUI;
    public static DexDescriptionState i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        dexDescriptionUI.gameObject.SetActive(true);
        dexDescriptionUI.OnBack += OnBack;
    }
    public override void Execute()
    {
        dexDescriptionUI.HandleUpdate();
    }
    public override void Exit()
    {
        dexDescriptionUI.gameObject.SetActive(false);
        dexDescriptionUI.OnBack -= OnBack;
    }
    void OnBack()
    {
        gc.StateMachine.Pop();
    }
}
