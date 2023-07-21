using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleSituationState : State<BattleSystem>
{
    [SerializeField] BattleSituationUI selectionUI;
    // Start is called before the first frame update
    public static BattleSituationState i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    BattleSystem bs;

    public override void Enter(BattleSystem owner)
    {
        bs = owner;
        selectionUI.gameObject.SetActive(true);
        Debug.Log(bs.PlayerUnits.Concat(bs.EnemyUnits).ToList());
        selectionUI.SetUnit(bs.PlayerUnits.Concat(bs.EnemyUnits).ToList());
        selectionUI.OnBack += OnBack;
    }
    public override void Execute()
    {
        selectionUI.HandleUpdate();
    }
    public override void Exit()
    {
        selectionUI.OnBack -= OnBack;
        selectionUI.selectedItem = 0;

        selectionUI.gameObject.SetActive(false);
    }
    void OnBack()
    {
        bs.StateMachine.ChangeState(ActionSelectionState.i);
    }
}
