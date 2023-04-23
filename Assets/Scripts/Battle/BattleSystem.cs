using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GDEUtils.StateMachine;


public enum BattleStates { Start, ActionSelection, MoveSelection, TargetSelection, RunningTurn, Busy, Bag, PartyScreen, AboutToUse, MoveToForget, BattleOver }
public enum BattleTrigger { LongGrass, Water, Trainer }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnitSingle;
    [SerializeField] BattleUnit enemyUnitSingle;
    [SerializeField] List<BattleUnit> playerUnitsMulti;
    [SerializeField] List<BattleUnit> enemyUnitsMulti;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject ballSprite;
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] GameObject singleBattleElements;
    [SerializeField] GameObject multiBattleElements;

    [Header("소리")]
    [SerializeField] AudioClip wildBattleMusic;
    [SerializeField] AudioClip trainerBattleMusic;
    [SerializeField] AudioClip wildBattleVictoryMusic;
    [SerializeField] AudioClip trainerBattleVictoryMusic;

    [Header("배경")]
    [SerializeField] Image backgroundImage;
    [SerializeField] Sprite grassBackground;
    [SerializeField] Sprite waterBackground;
    [SerializeField] Sprite trainerBackground;

    // 2vs2

    List<BattleUnit> playerUnits;
    List<BattleUnit> enemyUnits;

    List<BattleAction> actions;

    int unitCount = 1;
    int actionIndex = 0;
    BattleUnit currentUnit;

    public event Action<bool> OnBattleOver;

    public StateMachine<BattleSystem> StateMachine { get; private set; }

    BattleStates state;

    int currentAction;
    int currentMove;
    int currentTarget;
    bool aboutToUseChoice = true;

    UnitParty playerParty;
    UnitParty trainerParty;
    Unit wildUnit;
    public Field Field { get; private set; }

    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;

    int escapeAttempts;
    MoveBase moveToLearn;
    BattleUnit unitTryingToLearn;

    BattleUnit unitToSwitch;

    BattleTrigger battleTrigger;
    public void StartBattle(UnitParty playerParty, Unit wildUnit,
        BattleTrigger trigger = BattleTrigger.LongGrass)
    {
        battleTrigger = trigger;
        this.playerParty = playerParty;
        this.wildUnit = wildUnit;
        player = playerParty.GetComponent<PlayerController>();
        isTrainerBattle = false;

        unitCount = 1;

        AudioManager.i.PlayMusic(wildBattleMusic);
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(UnitParty playerParty, UnitParty trainerParty,
        int unitCount = 1)
    {
        battleTrigger = BattleTrigger.Trainer;
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();
        AudioManager.i.PlayMusic(trainerBattleMusic);
        this.unitCount = unitCount;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        Debug.Log("wake up in setupbattle in battle system");
        StateMachine = new StateMachine<BattleSystem>(this);

        singleBattleElements.SetActive(unitCount == 1);
        multiBattleElements.SetActive(unitCount > 1);

        if (unitCount == 1)
        {
            playerUnits = new List<BattleUnit>() { playerUnitSingle };
            enemyUnits = new List<BattleUnit>() { enemyUnitSingle };
        }
        else
        {
            playerUnits = playerUnitsMulti.GetRange(0, playerUnitsMulti.Count);
            enemyUnits = enemyUnitsMulti.GetRange(0, enemyUnitsMulti.Count);
        }

        for (int i = 0; i < playerUnits.Count; i++)
        {
            playerUnits[i].Clear();
        }
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].Clear();
        }
        Sprite selectedBackground;
        if (battleTrigger == BattleTrigger.LongGrass)
            selectedBackground = grassBackground;
        else if (battleTrigger == BattleTrigger.Water)
            selectedBackground = waterBackground;
        else
            selectedBackground = trainerBackground;
        backgroundImage.sprite = selectedBackground;

        if (!isTrainerBattle)
        {
            // 야생 전투
            playerUnits[0].Setup(playerParty.GetHealtyhUnit());
            enemyUnits[0].Setup(wildUnit);

            dialogBox.SetMoveNames(playerUnits[0].Unit.Moves);
            yield return dialogBox.TypeDialog($"{enemyUnits[0].Unit.Base.Name}(이)가 나타났다!");
        }
        else
        {
            // 네임드 전투

            // 스프라이트 먼저 출력
            for (int i = 0; i < playerUnits.Count; i++)
            {
                playerUnits[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                enemyUnits[i].gameObject.SetActive(false);
            }

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;
            yield return dialogBox.TypeDialog($"{trainer.Name}을 쓰러트리기 위한 전투가 시작된다!");

            // 상대 전투 유닛 출동
            trainerImage.gameObject.SetActive(false);
            var healthEnemyUnits = trainerParty.GetHealtyhUnits(unitCount);
            for (int i = 0; i < unitCount; i++)
            {
                enemyUnits[i].gameObject.SetActive(true);
                enemyUnits[i].Setup(healthEnemyUnits[i]);
            }
            string names = String.Join("와(과) ", healthEnemyUnits.Select(u => u.Base.Name));
            yield return dialogBox.TypeDialog($"상대 {names}(이)가 먼저 나온다!");

            // 팀 전투 유닛 출동
            playerImage.gameObject.SetActive(false);
            var healthPlayerUnits = playerParty.GetHealtyhUnits(unitCount);
            for (int i = 0; i < unitCount; i++)
            {
                playerUnits[i].gameObject.SetActive(true);
                playerUnits[i].Setup(healthPlayerUnits[i]);
            }
            names = String.Join("와(과) ", healthPlayerUnits.Select(u => u.Base.Name));
            yield return dialogBox.TypeDialog($"힘내, {names}!");
        }
        escapeAttempts = 0;
        Field = new Field();

        // 시작하자마자 날씨적용
        // Field.SetWeather(ConditionID.sandstorm);
        // yield return dialogBox.TypeDialog(Field.Weather.StartMessage);
        partyScreen.Init();

        actions = new List<BattleAction>();
        Debug.Log("wake up in before actionselection");
        StateMachine.ChangeState(ActionSelectionState.i);
    }


    void BattleOver(bool won)
    {
        state = BattleStates.BattleOver;
        playerParty.Units.ForEach(unit => unit.OnBattleOver());

        playerUnits.ForEach(pu => pu.Hud.ClearData());
        enemyUnits.ForEach(eu => eu.Hud.ClearData());

        OnBattleOver(won);
    }

    void ActionSelection(int actionIndex)
    {
        state = BattleStates.ActionSelection;
        // StartCoroutine(dialogBox.SetDialog("행동을 선택하세요."));
        this.actionIndex = actionIndex;
        currentUnit = playerUnits[actionIndex];
        // 유저의 스킬을 넣어야 함
        dialogBox.SetMoveNames(currentUnit.Unit.Moves);
        dialogBox.SetDialog($"{currentUnit.Unit.Base.Name}의 행동을 선택하세요.");
        dialogBox.EnableActionSelector(true);
    }

    void OpenBag()
    {
        state = BattleStates.Bag;
        inventoryUI.gameObject.SetActive(true);
    }

    void OpenPartyScreen()
    {
        // partyScreen.CalledFrom = state;
        state = BattleStates.PartyScreen;
        partyScreen.gameObject.SetActive(true);
        partyScreen.UpdateStatusText();
    }

    void MoveSelection()
    {
        state = BattleStates.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }
    void TargetSelection()
    {
        state = BattleStates.TargetSelection;
        currentTarget = 0;
    }
    IEnumerator AboutToUse(Unit newUnit)
    {
        state = BattleStates.Busy;
        yield return dialogBox.TypeDialog($"{newUnit.Base.Name}(이)가 준비중이다! 팀원을 교체하겠습니까?");

        state = BattleStates.AboutToUse;
        dialogBox.EnableChoiceBox(true);
    }
    IEnumerator ChooseMoveToForget(Unit unit, MoveBase newMove)
    {
        state = BattleStates.Busy;
        yield return dialogBox.TypeDialog($"기술 배우는 창, 보인다면 버그입니다.");
        moveSelectionUI.gameObject.SetActive(true);
        // moveSelectionUI.SetMoveData(unit.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = BattleStates.MoveToForget;
    }
    void AddBattleAction(BattleAction action)
    {
        actions.Add(action);

        // 유저에게 설택된 것 확인
        if (actions.Count == unitCount)
        {
            // 적 유닛 확인
            foreach (var enemyUnit in enemyUnits)
            {
                var randAction = new BattleAction()
                {
                    Type = ActionType.Move,
                    User = enemyUnit,
                    Target = playerUnits[UnityEngine.Random.Range(0, playerUnits.Count)],
                    Move = enemyUnit.Unit.GetRandomMove()
                };
                actions.Add(randAction);
            }

            // sort Actions
            actions = actions.OrderByDescending(a => a.Priority)
                .ThenByDescending(a => a.User.Unit.Speed).ToList();

            StartCoroutine(RunTurns());
        }
        else
        {
            ActionSelection(actionIndex + 1);
        }
    }
    IEnumerator RunTurns()
    {
        state = BattleStates.RunningTurn;

        foreach (var action in actions)
        {
            if (action.IsInvalid) continue;
            if (action.Type == ActionType.Move)
            {
                yield return RunMove(action.User, action.Target, action.Move);
                yield return RunAfterTurn(action.User);
                if (state == BattleStates.BattleOver) yield break;
            }
            else if (action.Type == ActionType.SwitchUnit)
            {
                state = BattleStates.Busy;
                yield return SwitchUnit(action.User, action.SelectedUnit);
            }
            else if (action.Type == ActionType.UseItem)
            {
                // 아이템 창 탈출하고 적 턴을 실행해야함
                dialogBox.EnableActionSelector(false);
            }
            else if (action.Type == ActionType.Run)
            {
                yield return TryToEscape();
            }
            if (state == BattleStates.BattleOver) break;
        }

        if (Field.Weather != null)
        {
            yield return dialogBox.TypeDialog(Field.Weather.EffectMessage);
            for (int i = 0; i < playerUnits.Count; i++)
            {
                var pu = playerUnits[i];
                Field.Weather.OnWeather?.Invoke(pu.Unit);
                yield return ShowStatusChanges(pu.Unit);
                if (pu.Unit.HPChanged) pu.PlayerHitAnimation();
                pu.Hud.UpdateHP();
                if (pu.Unit.HP <= 0)
                {
                    yield return dialogBox.TypeDialog($"{pu.Unit.Base.Name}은(는) 쓰러졌다!");
                    yield return HandleUnitFainted(pu);
                    yield break;
                }
            }
            for (int i = 0; i < playerUnits.Count; i++)
            {
                var eu = enemyUnits[i];
                Field.Weather.OnWeather?.Invoke(eu.Unit);
                yield return ShowStatusChanges(eu.Unit);
                if (eu.Unit.HPChanged) eu.PlayerHitAnimation();
                eu.Hud.UpdateHP();
                if (eu.Unit.HP <= 0)
                {
                    yield return dialogBox.TypeDialog($"{eu.Unit.Base.Name}은(는) 쓰러졌다!");
                    yield return HandleUnitFainted(eu);
                    yield break;
                }
            }
            if (Field.WeatherDuration != null)
            {
                Field.WeatherDuration--;
                if (Field.WeatherDuration == 0)
                {
                    Field.Weather = null;
                    Field.WeatherDuration = null;
                    yield return dialogBox.TypeDialog("날씨가 원래대로 되돌아왔다!");
                }
            }
        }

        if (state != BattleStates.BattleOver)
        {
            actions.Clear();
            ActionSelection(0);
        }
    }
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Unit.OnBeforeMove();

        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Unit);
            yield return sourceUnit.Hud.WaitForHPUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Unit);

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}(이)가 {move.Base.Name}을(를) 사용했다!");

        if (CheckIfMoveHits(move, sourceUnit.Unit, targetUnit.Unit))
        {
            int hitTimes = move.Base.GetHitTimes();
            float typeEffectiveness = 1f;
            int hit = 1;
            for (int i = 1; i <= hitTimes; i++)
            {
                sourceUnit.PlayAttackAnimation();
                if (move.Base.Sound != null)
                    AudioManager.i.PlaySfx(move.Base.Sound);

                yield return new WaitForSeconds(1f);
                targetUnit.PlayerHitAnimation();
                AudioManager.i.PlaySfx(AudioId.Hit);

                if (move.Base.Category == MoveCategory.Status)
                {
                    yield return RunMoveEffect(move.Base.Effects, sourceUnit.Unit, targetUnit.Unit, move.Base.Target);
                }
                else
                {
                    var damageDetails = targetUnit.Unit.TakeDamage(move, sourceUnit.Unit, Field.Weather);
                    yield return targetUnit.Hud.WaitForHPUpdate();
                    yield return ShowDamageDetails(damageDetails);
                    typeEffectiveness = damageDetails.TypeEffectiveness;
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
                hit = i;
                if (targetUnit.Unit.HP <= 0)
                    break;
            }
            yield return ShowEffectiveness(typeEffectiveness);
            if (hitTimes > 1)
                yield return dialogBox.TypeDialog($"{hit}번 공격했다!");

            if (targetUnit.Unit.HP <= 0)
            {
                // AudioManager.i.PlaySfx(AudioId.Faint);
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
        AudioManager.i.PlaySfx(AudioId.Faint);
        yield return dialogBox.TypeDialog($"{faintedUnit.Unit.Base.Name}(이)가 쓰러졌다!");
        faintedUnit.PlayFaintAnimation();

        yield return new WaitForSeconds(2f);
        yield return HandleExpGain(faintedUnit);

        NextStepsAfterFainting(faintedUnit);
    }
    IEnumerator HandleExpGain(BattleUnit faintedUnit)
    {
        if (!faintedUnit.IsPlayerUnit)
        {
            bool battleWon = true;
            if (isTrainerBattle)
                battleWon = trainerParty.GetHealtyhUnit() == null;

            // TODO : 이거 노래 다 끝날 때 까지 움직이지 못하게 해야 함
            if (battleWon)
                AudioManager.i.PlayMusic(isTrainerBattle ? trainerBattleVictoryMusic : wildBattleVictoryMusic, loop: false);
            // exp 획득
            int expYield = faintedUnit.Unit.Base.ExpYield;
            int enemyLevel = faintedUnit.Unit.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            // int expGain = Mathf.FloorToInt(expYield * enemyLevel * trainerBonus / 7);
            int expGain = 0;
            if (expGain != 0)
                for (int i = 0; i < unitCount; i++)
                {
                    var playerUnit = playerUnits[i];
                    yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name}은(는) 자세를 다잡는다!");
                    playerUnit.Unit.Exp += expGain;
                    // yield return playerUnit.Hud.SetExpSmooth();
                    // 레벨 업

                    while (playerUnit.Unit.CheckForLevelUp())
                    {
                        playerUnit.Hud.SetLevel();
                        yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name}은(는) 렙업했긴한데 사용하지 않는 코드다!!");
                        // 스킬 배우기
                        var newMove = playerUnit.Unit.GetLearnableMoveAtCurrLevel();
                        if (newMove != null)
                        {
                            if (playerUnit.Unit.Moves.Count < UnitBase.MaxNumOfMoves)
                            {
                                playerUnit.Unit.LearnMove(newMove.Base);
                                yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name}은(는) 얻을 수 없는 스킬을 얻었다!");
                                dialogBox.SetMoveNames(playerUnit.Unit.Moves);
                            }
                            else
                            {
                                // 기술 잊기
                                unitTryingToLearn = playerUnit;
                                yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name}은(는) 잊을 수 없는 스킬을 잊으려 한다!");
                                yield return ChooseMoveToForget(playerUnit.Unit, newMove.Base);
                                yield return new WaitUntil(() => state != BattleStates.MoveToForget);
                                yield return new WaitForSeconds(2f);
                            }
                        }
                        yield return playerUnit.Hud.SetExpSmooth(true);
                    }
                }
            yield return new WaitForSeconds(1f);
        }
    }
    void NextStepsAfterFainting(BattleUnit faintedUnit)
    {
        var actionToRemove = actions.FirstOrDefault(a => a.User == faintedUnit);
        if (actionToRemove != null)
            actionToRemove.IsInvalid = true;
        if (faintedUnit.IsPlayerUnit)
        {
            var activeUnits = playerUnits.Select(unit => unit.Unit).Where(u => u.HP > 0).ToList();
            var nextUnit = playerParty.GetHealtyhUnit(activeUnits);

            if (activeUnits.Count == 0 && nextUnit == null)
            {
                BattleOver(false);
            }
            else if (nextUnit != null)
            {
                unitToSwitch = faintedUnit;
                OpenPartyScreen();
            }
            else if (nextUnit == null && activeUnits.Count > 0)
            {
                // 더이상의 적은 없지만 전투는 계속 됨
                playerUnits.Remove(faintedUnit);
                faintedUnit.Hud.gameObject.SetActive(false);

                var actionsToChange = actions.Where(a => a.Target == faintedUnit).ToList();
                actionsToChange.ForEach(a => a.Target = playerUnits.First());
            }
        }
        else
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);
                return;
            }
            // var nextUnit = trainerParty.GetHealtyhUnit();
            // if (nextUnit != null)
            //     StartCoroutine(AboutToUse(nextUnit));
            // else
            //     BattleOver(true);
            var activeUnits = enemyUnits.Select(unit => unit.Unit).Where(u => u.HP > 0).ToList();
            var nextUnit = trainerParty.GetHealtyhUnit(activeUnits);

            if (activeUnits.Count == 0 && nextUnit == null)
            {
                BattleOver(true);
            }
            else if (nextUnit != null)
            {
                if (unitCount == 1)
                {
                    unitToSwitch = playerUnits[0];
                    StartCoroutine(AboutToUse(nextUnit));
                }
                else
                    StartCoroutine(SendNextTrainerUnit());
            }
            else if (nextUnit == null && activeUnits.Count > 0)
            {
                // 더이상의 적은 없지만 전투는 계속 됨
                enemyUnits.Remove(faintedUnit);
                faintedUnit.Hud.gameObject.SetActive(false);

                var actionsToChange = actions.Where(a => a.Target == faintedUnit).ToList();
                actionsToChange.ForEach(a => a.Target = enemyUnits.First());
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

        // 날씨 변경
        if (effects.Weather != ConditionID.none)
        {
            Field.SetWeather(effects.Weather);
            Field.WeatherDuration = 5;
            yield return dialogBox.TypeDialog(Field.Weather.StartMessage);
        }

        // dialog 박스 변경
        // playerUnit.Hud.UpdateStatus();

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleStates.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleStates.RunningTurn);
        // 상태이상으로 쓰러지는가?
        sourceUnit.Unit.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Unit);
        yield return sourceUnit.Hud.WaitForHPUpdate();

        if (sourceUnit.Unit.HP <= 0)
        {
            // AudioManager.i.PlaySfx(AudioId.Faint);
            yield return HandleUnitFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleStates.RunningTurn);
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
    }
    IEnumerator ShowEffectiveness(float typeEffectiveness)
    {
        if (typeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("효과가 굉장한 듯 하다!");
        else if (typeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("효과가 별로인 듯 하다...");
    }

    public void HandleUpdate()
    {
        StateMachine.Execute();

        if (state == BattleStates.TargetSelection)
        {
            HandleTargetSelection();
        }
        else if (state == BattleStates.PartyScreen)
        {
            HandlePartySelection();
        }
        else if (state == BattleStates.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleStates.ActionSelection;
            };
            Action<ItemBase> onItemUsed = (ItemBase usedItem) =>
            {
                StartCoroutine(OnItemUsed(usedItem));
            };
        }
        else if (state == BattleStates.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if (state == BattleStates.MoveToForget)
        {
            Action<int> onMoveSelected = (int moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == UnitBase.MaxNumOfMoves)
                {
                    // 배우지 않음
                    StartCoroutine(dialogBox.TypeDialog($"배우지 않는다. 이것도 보이면 버그다..."));
                }
                else
                {
                    // 새로운 스킬 배움
                    // var selectedMove = playerUnit.Unit.Moves[moveIndex].Base;
                    unitTryingToLearn.Unit.Moves[moveIndex] = new Move(moveToLearn);
                    StartCoroutine(dialogBox.TypeDialog($"아님 핵을 썼던가..."));
                }
                moveToLearn = null;
                unitTryingToLearn = null;
                state = BattleStates.RunningTurn;
            };
            // TODO 배틀에서 선택하는거 처리해야함
            // moveSelectionUI.HandleMoveSelection(onMoveSelected);
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
                // StartCoroutine(RunTurns(ActionType.UseItem));
                OpenBag();

            }
            else if (currentAction == 2)
            {
                // Unit
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // run
                // StartCoroutine(RunTurns(ActionType.Run));
                var action = new BattleAction()
                {
                    Type = ActionType.Run,
                    User = currentUnit
                };
                AddBattleAction(action);
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

        currentMove = Mathf.Clamp(currentMove, 0, currentUnit.Unit.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, currentUnit.Unit.Moves[currentMove]);

        if (Input.GetButtonDown("Submit"))
        {
            var move = currentUnit.Unit.Moves[currentMove];
            if (move.PP == 0) return;

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);

            if (enemyUnits.Count > 1)
            {
                TargetSelection();
            }
            else
            {
                var action = new BattleAction()
                {
                    Type = ActionType.Move,
                    User = currentUnit,
                    Target = enemyUnits[0],
                    Move = move
                };
                AddBattleAction(action);
            }
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection(actionIndex);
        }
    }
    void HandleTargetSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            currentTarget += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            currentTarget -= 2;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            currentTarget--;
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            currentTarget++;
        currentTarget = Mathf.Clamp(currentTarget, 0, enemyUnits.Count + playerUnits.Count - 1);
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].SetSelected(i == currentTarget);
        }
        if (currentTarget >= enemyUnits.Count)
            foreach (var u in playerUnits)
            {
                u.SetSelected(u.Unit.Base.Name != currentUnit.Unit.Base.Name);
            }
        else
        {
            for (int i = 0; i < unitCount; i++)
            {
                playerUnits[i].SetSelected(false);
            }
        }
        if (Input.GetButtonDown("Submit"))
        {
            for (int i = 0; i < unitCount; i++)
            {
                enemyUnits[i].SetSelected(false);
                playerUnits[i].SetSelected(false);
            }
            var move = currentUnit.Unit.Moves[currentMove];
            if (move.PP == 0) return;
            var targetedUnit = currentTarget >= enemyUnits.Count ?
                playerUnits.First(u => u.Unit.Base.Name != currentUnit.Unit.Base.Name) :
                enemyUnits[currentTarget];
            var action = new BattleAction()
            {
                Type = ActionType.Move,
                User = currentUnit,
                Target = targetedUnit,
                Move = currentUnit.Unit.Moves[currentMove],
            };
            AddBattleAction(action);
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            for (int i = 0; i < unitCount; i++)
            {
                enemyUnits[i].SetSelected(false);
                playerUnits[i].SetSelected(false);
            }
            MoveSelection();
        }
    }
    void HandlePartySelection()
    {
        Action onSelected = () =>
        {
            var selectedMember = partyScreen.SelectedMember;
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("그 동료는 지쳤다!");
                return;
            }
            if (playerUnits.Any(u => u.Unit == selectedMember))
            {
                partyScreen.SetMessageText("그 동료는 싸우고 있다!");
                return;
            }
            // StartCoroutine(PerformPlayerMove());
            partyScreen.gameObject.SetActive(false);

            // if (partyScreen.CalledFrom == BattleStates.ActionSelection) {
            //     // StartCoroutine(RunTurns(ActionType.SwitchUnit));
            //     var action = new BattleAction()
            //     {
            //         Type = ActionType.SwitchUnit,
            //         User = currentUnit,
            //         SelectedUnit = selectedMember
            //     };
            //     AddBattleAction(action);
            // }
            // else
            // {
            //     state = BattleStates.Busy;
            //     bool isTrainerAboutToUse = partyScreen.CalledFrom == BattleStates.AboutToUse;
            //     StartCoroutine(SwitchUnit(unitToSwitch, selectedMember, isTrainerAboutToUse));
            //     unitToSwitch = null;
            // }
            // partyScreen.CalledFrom = null;
        };
        Action onBack = () =>
        {
            if (playerUnits.Any(u => u.Unit.HP <= 0))
            {
                partyScreen.SetMessageText("전투를 계속하기 위해 팀원을 보내야합니다!");
                return;
            }
            partyScreen.gameObject.SetActive(false);

            // if (partyScreen.CalledFrom == BattleStates.AboutToUse)
            //     StartCoroutine(SendNextTrainerUnit());
            // else
            //     ActionSelection(actionIndex);
            // partyScreen.CalledFrom = null;
        };
        // partyScreen.HandleUpdate(onSelected, onBack);
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
                OpenPartyScreen();
            }
            else
            {
                StartCoroutine(SendNextTrainerUnit());
            }
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerUnit());
        }
    }

    IEnumerator SwitchUnit(BattleUnit unitToSwitch, Unit newUnit, bool isTrainerAboutToUse = false)
    {
        if (unitToSwitch.Unit.HP > 0)
        {
            yield return dialogBox.TypeDialog($"교체하자 {unitToSwitch.Unit.Base.Name}!");
            unitToSwitch.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        unitToSwitch.Setup(newUnit);
        dialogBox.SetMoveNames(newUnit.Moves);
        yield return dialogBox.TypeDialog($"{newUnit.Base.Name}(이)가 나선다!");

        if (isTrainerAboutToUse)
            StartCoroutine(SendNextTrainerUnit());
        else
            state = BattleStates.RunningTurn;
    }

    IEnumerator SendNextTrainerUnit()
    {
        state = BattleStates.Busy;

        var faintedUnit = enemyUnits.First(u => u.Unit.HP == 0);
        var activeUnits = enemyUnits.Select(unit => unit.Unit).Where(u => u.HP > 0).ToList();
        var nextUnit = trainerParty.GetHealtyhUnit(activeUnits);
        faintedUnit.Setup(nextUnit);
        yield return dialogBox.TypeDialog($"{nextUnit.Base.Name}(이)가 교대로 나온다!");

        state = BattleStates.RunningTurn;
    }
    IEnumerator OnItemUsed(ItemBase usedItem)
    {
        state = BattleStates.Busy;
        inventoryUI.gameObject.SetActive(false);

        if (usedItem is BallItem)
        {
            yield return ThrowBall((BallItem)usedItem);
        }
        var action = new BattleAction()
        {
            Type = ActionType.UseItem,
            User = currentUnit,
        };
        AddBattleAction(action);
    }
    IEnumerator ThrowBall(BallItem ballItem)
    {
        state = BattleStates.Busy;

        if (isTrainerBattle)
        {
            bool hasKarlord = false;
            if (enemyUnits.Any(u => u.Unit.Base.Name == "로드"))
            {
                // TODO: 로드에게 던졌을 때, 카를 격노 이벤트?
                var karl = trainerParty.Units.Any(u => u.Base.Name == "카를");
                if (karl)
                    hasKarlord = true;
            }
            if (hasKarlord)
                yield return dialogBox.TypeDialog($"당신은 죄악이 등을 타고 오르는 것을 느꼈다.");
            else
                yield return dialogBox.TypeDialog($"이 녀석들에겐 통하지 않는다!");
            state = BattleStates.RunningTurn;
            yield break;
        }
        var playerUnit = playerUnits[0];
        var enemyUnit = enemyUnits[0];

        yield return dialogBox.TypeDialog($"{ballItem.Name}을(를) 사용했다!");
        yield return dialogBox.TypeDialog($"왕국으로 돌아가라!");

        var ballObj = Instantiate(ballSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var ball = ballObj.GetComponent<SpriteRenderer>();
        ball.sprite = ballItem.Icon;

        // 애니메이션
        yield return ball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 1), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return ball.transform.DOMoveY(enemyUnit.transform.position.y - 2, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchUnit(enemyUnit.Unit, ballItem);
        for (int i = 0; i < Mathf.Min(shakeCount, 3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return ball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            // unit 잡음
            yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name}을(를) 잡았다!");
            yield return ball.DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddUnit(enemyUnit.Unit);
            yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name}을(를) 겨우 잡았다.");


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
            state = BattleStates.RunningTurn;
        }
    }

    int TryToCatchUnit(Unit unit, BallItem ballitem)
    {
        // a = [{1 - (2/3 × 현재HP/최대HP)} × 포획률 × 몬스터볼 보정 × 상태이상 보정 × (잡기파워 보정)] 잡기파워는 없을 예정
        // 독, 마비, 화상 상태에선 x1.5(3세대)
        // 수면 및 얼음 상태에선 ×2.5(5세대 이후)
        // float a = ((1 - (2/3 * unit.HP / unit.MaxHP)) * unit.Base.CatchRate * ballitem.CatchRateModifier * ConditionDB.GetStatusBonus(unit.Status));
        float a = ((1 - (2 / 3 * unit.HP / unit.MaxHP)) * 255 * ballitem.CatchRateModifier * ConditionDB.GetStatusBonus(unit.Status));

        if (a >= 255) return 4;

        float b = 65536 / Mathf.Pow(255 / a, 0.1875f);

        // int shakeCount = -1;
        for (int i = 0; i < 4; i++)
        {
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
        state = BattleStates.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog("도망 칠 순 없다!");
            state = BattleStates.RunningTurn;
            yield break;
        }

        escapeAttempts++;

        // 128×A÷B＋30×C
        // (A는 나와있는 포켓몬의 스피드, B는 상대 포켓몬의 스피드, C는 도망을 시도한 횟수.)
        // 를 256으로 나눈 나머지를 계산해서 0~255 사이의 난수를 생성해 계산값보다 작으면 도망갈 수 있다.
        int playerSpeed = playerUnits[0].Unit.Speed;
        int enemySpeed = enemyUnits[0].Unit.Speed;

        float f = (128 * playerSpeed / enemySpeed + 30 * escapeAttempts) % 256;
        if (playerSpeed > enemySpeed || f > UnityEngine.Random.Range(0, 255))
        {
            yield return dialogBox.TypeDialog("도망갔다!");
            BattleOver(true);
        }
        else
        {
            yield return dialogBox.TypeDialog("도망갈 수 없다!");
            state = BattleStates.RunningTurn;
        }
    }

    public BattleDialogBox DialogBox => dialogBox;

    // public List<BattleUnit> PlayerUnits()
    // {
    //     List<BattleUnit> returnUnits;
    //     if (unitCount == 1)
    //     {
    //         playerUnits = new List<BattleUnit>() { playerUnitSingle };
    //         enemyUnits = new List<BattleUnit>() { enemyUnitSingle };
    //     }
    //     else
    //     {
    //         playerUnits = playerUnitsMulti.GetRange(0, playerUnitsMulti.Count);
    //         enemyUnits = enemyUnitsMulti.GetRange(0, enemyUnitsMulti.Count);
    //     }
    //     return playerUnits;
    //     // return returnUnits;
    // }

    // public List<BattleUnit> PlayerUnits =>
    //     unitCount == 1 ?
    //         new List<BattleUnit>() 

    // public List<BattleUnit> EnemyUnits =>
    //     unitCount == 1 ?
    //         new List<BattleUnit>() { enemyUnitSingle } :
    //         enemyUnitsMulti.GetRange(0, enemyUnitsMulti.Count);

    public List<BattleUnit> PlayerUnits => playerUnits;
    public List<BattleUnit> EnemyUnits => enemyUnits;

    public int ActionIndex => actionIndex;
}
