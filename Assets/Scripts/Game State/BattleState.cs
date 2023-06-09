using System.Collections;
using System.Collections.Generic;
using GDEUtils.StateMachine;
using UnityEngine;

public class BattleState : State<GameController>
{
    [SerializeField] BattleSystem battleSystem;
    public static BattleState i { get; private set; }
    public TrainerController trainer { get; set; }
    public BattleTrigger trigger { get; set; }
    public int unitCount { get; set; }
    private void Awake()
    {
        Debug.Log("wake up in battlestate");
        i = this;
    }
    GameController gc;
    public override void Enter(GameController owner)
    {
        Debug.Log("wake up in battlestate enter");
        gc = owner;

        battleSystem.gameObject.SetActive(true);
        gc.WorldCamera.gameObject.SetActive(false);

        var playerParty = gc.PlayerController.GetComponent<UnitParty>();
        if (trainer == null)
        {
            // var wildUnit = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildUnit();
            var wildUnit = gc.CurrentScene.GetComponent<MapArea>().GetRandomWildUnit(trigger);
            var wildUnitCopy = new Unit(wildUnit.Base, wildUnit.Level);
            battleSystem.StartBattle(playerParty, wildUnit, trigger);
        }
        else
        {
            var trainerParty = trainer.GetComponent<UnitParty>();
            battleSystem.StartTrainerBattle(playerParty, trainerParty, unitCount);
        }

        battleSystem.OnBattleOver += EndBattle;
    }
    public override void Execute()
    {
        battleSystem.HandleUpdate();
    }
    public override void Exit()
    {
        battleSystem.gameObject.SetActive(false);
        gc.WorldCamera.gameObject.SetActive(true);
        battleSystem.OnBattleOver -= EndBattle;
    }
    void EndBattle(bool won)
    {
        if (trainer != null && won == true)
        {
            trainer.BattleLost();
            trainer = null;
        }
        gc.StateMachine.Pop();
    }
    public BattleSystem BattleSystem => battleSystem;
}
