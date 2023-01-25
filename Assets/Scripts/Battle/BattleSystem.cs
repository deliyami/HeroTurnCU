using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState 
{ Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver,
}
public enum BattleAction { Move, SwitchUnit, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;

    public event Action<bool> OnBattleOver;

    BattleState state;
    BattleState? prevState;
    int currentAction;
    int currentMove;
    int currentMember;

    UnitParty playerParty;
    UnitParty trainerParty;
    Unit wildUnit;

    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;

    public void StartBattle(UnitParty playerParty, Unit wildUnit)
    {
        this.playerParty = playerParty;
        this.wildUnit = wildUnit;
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(UnitParty playerParty, UnitParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        if(!isTrainerBattle)
        {
            // 야생 전투
            playerUnit.Setup(playerParty.GetHealtyhUnit());
            enemyUnit.Setup(wildUnit);

            dialogBox.SetMoveNames(playerUnit.Unit.Moves);
            yield return dialogBox.TypeDialog($"야생의 {enemyUnit.Unit.Base.Name}(이)가 나타났다!");
        }
        else
        {
            // 네임드 전투

            // 스프라이트 먼저 출력
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;
            Debug.Log("Battlesystem: before trainer.name과의 전투");
            yield return dialogBox.TypeDialog($"{trainer.Name}와(과)의 전투가 시작된다!");
            Debug.Log("Battlesystem: after trainer.name과의 전투");

            // 상대 전투 유닛 출동
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var firstHealthEnemyUnit = trainerParty.GetHealtyhUnit();
            enemyUnit.Setup(firstHealthEnemyUnit);
            yield return dialogBox.TypeDialog($"상대 {firstHealthEnemyUnit.Base.Name}(이)가 먼저 나온다!");
            
            // 팀 전투 유닛 출동
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var firstHealthPlayerUnit = playerParty.GetHealtyhUnit();
            playerUnit.Setup(firstHealthPlayerUnit);
            yield return dialogBox.TypeDialog($"힘내, {firstHealthPlayerUnit.Base.Name}!");
            dialogBox.SetMoveNames(playerUnit.Unit.Moves);
        }

        partyScreen.Init();
        ActionSelection();
    }


    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Units.ForEach(unit => unit.OnBattleOver());
        OnBattleOver(won);
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        // StartCoroutine(dialogBox.SetDialog("행동을 선택하세요."));
        dialogBox.SetDialog("행동을 선택하세요.");
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Units);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }
    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Unit.CurrentMove = playerUnit.Unit.Moves[currentMove];
            enemyUnit.Unit.CurrentMove = enemyUnit.Unit.GetRandomMove();

            int playerMovePriority = playerUnit.Unit.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Unit.CurrentMove.Base.Priority;

            // 순서 정하기
            bool playerGoesFirst = true;

            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
            {
                playerGoesFirst = playerUnit.Unit.Speed >= enemyUnit.Unit.Speed;
            }

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var second = secondUnit.Unit;
            yield return RunMove(firstUnit, secondUnit, firstUnit.Unit.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;


            if (second.HP > 0){
                yield return RunMove(secondUnit, firstUnit, secondUnit.Unit.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchUnit)
            {
                var selectedUnit = playerParty.Units[currentMember];
                state = BattleState.Busy;
                yield return SwitchUnit(selectedUnit);
            }

            var enemyMove = enemyUnit.Unit.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Unit.OnBeforeMove();

        if(!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Unit);
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Unit);

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}(이)가 {move.Base.Name}을(를) 사용했다!");

        if (CheckIfMoveHits(move, sourceUnit.Unit, targetUnit.Unit))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.PlayerHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffect(move.Base.Effects, sourceUnit.Unit, targetUnit.Unit, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Unit.TakeDamage(move, sourceUnit.Unit);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Unit.HP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffect(secondary, sourceUnit.Unit, targetUnit.Unit, secondary.Target);
                }
            }

            if (targetUnit.Unit.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{targetUnit.Unit.Base.Name}(이)가 쓰러졌다!");
                targetUnit.PlayFaintAnimation();

                yield return new WaitForSeconds(2f);

                CheckForBattleOver(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}의 공격이 빗나갔다!");
        }
    }
    

    IEnumerator ShowStatusChanges(Unit unit)
    {
        while (unit.StatusChanges.Count > 0)
        {
            var message = unit.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextUnit = playerParty.GetHealtyhUnit();
            if (nextUnit != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextUnit = trainerParty.GetHealtyhUnit();
                if (nextUnit != null)
                    StartCoroutine(SendNextTrainerUnit(nextUnit));
                else
                    BattleOver(true);
            }
        }
    }

    IEnumerator RunMoveEffect(MoveEffects effects, Unit source, Unit target, MoveTarget moveTarget)
    {
        // stat 증가
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        // status 이상
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        // dialog 박스 변경
        // playerUnit.Hud.UpdateStatus();

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);
        // 상태이상으로 쓰러지는가?
        sourceUnit.Unit.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Unit);
        yield return sourceUnit.Hud.UpdateHP();

        if (sourceUnit.Unit.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}(이)가 쓰러졌다!");
            sourceUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
        }
    }

    bool CheckIfMoveHits(Move move, Unit source, Unit target)
    {
        if (move.Base.AlwaysHits) return true;
        
        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        // statVal = Mathf.FloorToInt(statVal * (2f + max(0, boost)) / (2f - min(0, boost)))
        moveAccuracy *= (3f + Math.Max(0, accuracy - evasion)) / (3f - Math.Min(0, accuracy - evasion));

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
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

    public void HandleUpdate() 
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            currentAction -= 2;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ++currentAction;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetButtonDown("Submit"))
        {
            if (currentAction == 0)
            {
                // fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Bag
            }
            else if (currentAction == 2)
            {
                // Unit
                prevState = state;
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // run
            }
        }
    }
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            currentMove -= 2;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ++currentMove;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Unit.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Unit.Moves[currentMove]);

        if (Input.GetButtonDown("Submit"))
        {
            var move = playerUnit.Unit.Moves[currentMove];
            if (move.PP == 0) return;

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }
    
    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            currentMember -= 2;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ++currentMember;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Units.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetButtonDown("Submit"))
        {
            var selectedMember = playerParty.Units[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("그 동료는 지쳤다!");
                return;
            }
            if (selectedMember == playerUnit.Unit)
            {
                partyScreen.SetMessageText("그 동료는 싸우고 있다!");
                return;
            }
            // StartCoroutine(PerformPlayerMove());
            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchUnit));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchUnit(selectedMember));
            }
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator SwitchUnit(Unit newUnit)
    {
        if (playerUnit.Unit.HP > 0)
        {
            yield return dialogBox.TypeDialog($"교체하자 {playerUnit.Unit.Base.Name}!");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newUnit);
        dialogBox.SetMoveNames(newUnit.Moves);
        yield return dialogBox.TypeDialog($"{newUnit.Base.Name}(이)가 나선다!");

        state = BattleState.RunningTurn;
    }

    IEnumerator SendNextTrainerUnit(Unit nextUnit)
    {
        state = BattleState.Busy;

        enemyUnit.Setup(nextUnit);
        yield return dialogBox.TypeDialog($"{nextUnit.Base.Name}(이)가 교대로 나온다!");

        state = BattleState.RunningTurn;
    }
}
