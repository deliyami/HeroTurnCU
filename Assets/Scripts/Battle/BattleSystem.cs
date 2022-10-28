using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState 
{
    Start,
    PlayerAction,
    PlayerMove,
    EnemyMove,
    Busy
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    BattleState state;
    int currentAction;
    int currentMove;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        playerHud.SetData(playerUnit.Unit);
        enemyUnit.Setup();
        enemyHud.SetData(enemyUnit.Unit);

        dialogBox.SetMoveNames(playerUnit.Unit.Moves);

        yield return dialogBox.TypeDialog($"야생의 {enemyUnit.Unit.Base.Name}(이)가 나타났다!");

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("행동을 선택하세요."));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.Unit.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name}(이)가 {move.Base.Name}을(를) 사용했다!");

        var damageDetails = enemyUnit.Unit.TakeDamage(move, playerUnit.Unit);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);


        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name}(이)가 쓰러졌다!");
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }
    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;
        
        var move = enemyUnit.Unit.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name}(이)가 {move.Base.Name}을(를) 사용했다!");

        var damageDetails = playerUnit.Unit.TakeDamage(move, enemyUnit.Unit);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name}(이)가 쓰러졌다!");
        }
        else
        {
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("치명타가 적중했다!");
        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("효과가 굉장한 듯 하다!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("효과가 별로인 듯 하다...");
    }

    private void Update() 
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetButtonDown("Submit"))
        {
            if (currentAction == 0)
            {
                // fight
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                // run
            }
        }
    }
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentMove < playerUnit.Unit.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentMove < playerUnit.Unit.Moves.Count - 1)
                ++currentMove;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Unit.Moves[currentMove]);

        if (Input.GetButtonDown("Submit"))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);

            StartCoroutine(PerformPlayerMove());
        }
    }
}
