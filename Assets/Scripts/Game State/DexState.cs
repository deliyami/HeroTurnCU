using System;
using System.Collections;
using System.Collections.Generic;
using GDEUtils.StateMachine;
using UnityEngine;

public class DexState : State<GameController>
{
    [SerializeField] DexUI dexUI;
    public static DexState i { get; private set; }
    UnitBase currentUnit;

    public event Action DexDescriptionUIUpdate;
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
        // selection 
        // currentUnit = Dex.GetDex().allSlots[dexUI.SelectedCategory][selection]; 현재 유닛
        currentUnit = Dex.GetDex().GetItem(selection, dexUI.SelectedCategory);
        Debug.Log($"here is dexState {currentUnit.Name}");
        gc.StateMachine.Push(DexDescriptionState.i);
        DexDescriptionUIUpdate?.Invoke();
    }
    void OnBack()
    {
        gc.StateMachine.Pop();
    }
    public UnitBase CurrentUnit => currentUnit;
}
