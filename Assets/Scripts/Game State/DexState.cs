using System.Collections;
using System.Collections.Generic;
using GDEUtils.StateMachine;
using UnityEngine;

public class DexState : State<GameController>
{
    [SerializeField] DexUI dexUI;
    public static DexState i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        dexUI.gameObject.SetActive(true);
        dexUI.OnSelected += OnDexSelected;
        dexUI.OnBack += OnBack;
    }
    public override void Execute()
    {
        dexUI.HandleUpdate();
    }
    public override void Exit()
    {
        dexUI.gameObject.SetActive(false);
        dexUI.OnSelected -= OnDexSelected;
        dexUI.OnBack -= OnBack;
    }
    void OnDexSelected(int selection)
    {
        // TODO: 유닛 상세보기 추가
        // gc.StateMachine.Push();
    }
    void OnBack()
    {
        gc.StateMachine.Pop();
    }
}
