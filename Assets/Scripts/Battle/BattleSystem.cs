using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, AboutToUse, BattleOver }
public enum BattleAction { Move, SwitchUnit, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject ballSprite;

    public event Action<bool> OnBattleOver;

    BattleState state;
    BattleState? prevState;
    int currentAction;
    int currentMove;
    int currentMember;
    bool aboutToUseChoice = true;

    UnitParty playerParty;
    UnitParty trainerParty;
    Unit wildUnit;

    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;

    int escapeAttempts;

    public void StartBattle(UnitParty playerParty, Unit wildUnit)
    {
        this.playerParty = playerParty;
        this.wildUnit = wildUnit;
        player = playerParty.GetComponent<PlayerController>();
        isTrainerBattle = false;
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
        escapeAttempts = 0;
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
    IEnumerator AboutToUse(Unit newUnit)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{newUnit.Base.Name}(이)가 준비중이다! 팀원을 교체하겠습니까?");

        state = BattleState.AboutToUse;
        dialogBox.EnableChoiceBox(true);
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
            else if (playerAction == BattleAction.UseItem)
            {
                dialogBox.EnableActionSelector(false);
                yield return ThrowBall();
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
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
                yield return HandleUnitFainted(targetUnit);
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

    IEnumerator HandleUnitFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Unit.Base.Name}(이)가 쓰러졌다!");
        faintedUnit.PlayFaintAnimation();

        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit)
        {
            // exp 획득
            int expYield = faintedUnit.Unit.Base.ExpYield;
            int enemyLevel = faintedUnit.Unit.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;
            
            // int expGain = Mathf.FloorToInt(expYield * enemyLevel * trainerBonus / 7);
            int expGain = 0;
            playerUnit.Unit.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name}(은)는 자세를 다잡는다!");
            // yield return playerUnit.Hud.SetExpSmooth();
            // 레벨 업

            yield return new WaitForSeconds(1f);
            while (playerUnit.Unit.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name}(은)는 렙업했긴한데 사용하지 않는 코드다!!");
                // 스킬 배우기
                var newMove = playerUnit.Unit.GetLearnableMoveAtCurrLevel();
                if (newMove != null)
                {
                    if (playerUnit.Unit.Moves.Count < UnitBase.MaxNumOfMoves)
                    {
                        playerUnit.Unit.LearnMove(newMove);
                        yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name}(은)는 얻을 수 없는 스킬을 얻었다!");
                        dialogBox.SetMoveNames(playerUnit.Unit.Moves);
                    }
                    else
                    {
                        // 기술 잊기
                    }
                }
                yield return playerUnit.Hud.SetExpSmooth(true);
            }
        }

        CheckForBattleOver(faintedUnit);
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
                    StartCoroutine(AboutToUse(nextUnit));
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
            yield return HandleUnitFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
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
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
        // if (Input.GetKeyDown(KeyCode.T))
        //     StartCoroutine(ThrowBall());
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
                StartCoroutine(RunTurns(BattleAction.UseItem));
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
                StartCoroutine(RunTurns(BattleAction.Run));
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

    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            aboutToUseChoice = !aboutToUseChoice;

        dialogBox.UpdateChoiceBox(aboutToUseChoice);
        if (Input.GetButtonDown("Submit"))
        {
            dialogBox.EnableChoiceBox(false);
            if (aboutToUseChoice == true)
            {
                prevState = BattleState.AboutToUse;
                OpenPartyScreen();
            }
            else
            {
                StartCoroutine(SendNextTrainerUnit());
            }
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            if (playerUnit.Unit.HP <= 0)
            {
                partyScreen.SetMessageText("전투를 계속하기 위해 팀원을 보내야합니다!");
                return;
            }

            dialogBox.EnableChoiceBox(false);

            if (prevState == BattleState.AboutToUse)
            {
                prevState = null;
                StartCoroutine(SendNextTrainerUnit());
            }
            else
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

        if (prevState == null)
        {
            state = BattleState.RunningTurn;
        }
        else if (prevState == BattleState.AboutToUse)
        {
            prevState = null;
            StartCoroutine(SendNextTrainerUnit());
        }
    }

    IEnumerator SendNextTrainerUnit()
    {
        state = BattleState.Busy;

        var nextUnit = trainerParty.GetHealtyhUnit();
        enemyUnit.Setup(nextUnit);
        yield return dialogBox.TypeDialog($"{nextUnit.Base.Name}(이)가 교대로 나온다!");

        state = BattleState.RunningTurn;
    }
    IEnumerator ThrowBall()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            if (enemyUnit.name == "로드")
            {
                bool hasKarl = false;
                // TODO: 로드에게 던졌을 때, 카를 격노 이벤트?
                foreach(Unit u in trainerParty.Units)
                    if(u.Base.name == "카를") hasKarl = true;
                if (hasKarl)
                    yield return dialogBox.TypeDialog($"당신은 죄악이 등을 타고 오르는 것을 느꼈다.");
            }
            yield return dialogBox.TypeDialog($"이 녀석들에겐 통하지 않는다!");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog("밧줄을 던졌다!");

        var ballObj = Instantiate(ballSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var ball =  ballObj.GetComponent<SpriteRenderer>();

        // 애니메이션
        yield return ball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 1), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return ball.transform.DOMoveY(enemyUnit.transform.position.y - 2, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchUnit(enemyUnit.Unit);
        for (int i = 0; i < Mathf.Min(shakeCount, 3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return ball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            // unit 잡음
            yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name}(을)를 잡았다!");
            yield return ball.DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddUnit(enemyUnit.Unit);
            yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name}(을)를 겨우 잡았다.");

            Destroy(ball);
            BattleOver(true);
        }
        else
        {
            // unit 못잡음
            yield return new WaitForSeconds(1f);
            ball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            if (shakeCount < 2)
                yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name}(이)가 저항한다!");
            else
                yield return dialogBox.TypeDialog($"녀석이 저항한다!");

            Destroy(ball);
            state = BattleState.RunningTurn;
        }
    }

    int TryToCatchUnit(Unit unit)
    {
        // a = [{1 - (2/3 × 현재HP/최대HP)} × 포획률 × 몬스터볼 보정 × 상태이상 보정 × (잡기파워 보정)] 잡기파워는 없을 예정
        // 독, 마비, 화상 상태에선 x1.5(3세대)
        // 수면 및 얼음 상태에선 ×2.5(5세대 이후)
        // float a = ((1 - (2/3 * unit.HP / unit.MaxHP)) * unit.Base.CatchRate * ConditionDB.GetStatusBonus(unit.Status));
        float a = ((1 - (2/3 * unit.HP / unit.MaxHP)) * 255 * ConditionDB.GetStatusBonus(unit.Status));

        if (a >= 255) return 4;

        float b = 65536 / Mathf.Pow(255 / a, 0.1875f);

        // int shakeCount = -1;
        for (int i = 0; i < 4 ; i++){
            if (UnityEngine.Random.Range(0, 65536) > b)
            {
                // shakeCount = i;
                // break;
                return i;
            }
        }
        // return shakeCount == -1?4:shakeCount;
        return 4;
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog("도망 칠 순 없다!");
            state = BattleState.RunningTurn;
            yield break;
        }

        ++escapeAttempts;

        // 128×A÷B＋30×C
        // (A는 나와있는 포켓몬의 스피드, B는 상대 포켓몬의 스피드, C는 도망을 시도한 횟수.)
        // 를 256으로 나눈 나머지를 계산해서 0~255 사이의 난수를 생성해 계산값보다 작으면 도망갈 수 있다.
        int playerSpeed = playerUnit.Unit.Speed;
        int enemySpeed = enemyUnit.Unit.Speed;

        float f = (128 * playerSpeed / enemySpeed + 30 * escapeAttempts) % 256;
        if (f > UnityEngine.Random.Range(0, 255))
        {
            yield return dialogBox.TypeDialog("도망갔다!");
            BattleOver(true);
        }
        else
        {
            yield return dialogBox.TypeDialog("도망갈 수 없다!");
            state = BattleState.RunningTurn;
        }
    }
}
