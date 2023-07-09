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

    public int UnitCount { get; private set; } = 1;
    public int ActionIndex { get; private set; } = 0;
    BattleUnit currentUnit;

    public event Action<bool> OnBattleOver;

    public bool IsbattleOver { get; private set; }

    public StateMachine<BattleSystem> StateMachine { get; private set; }

    BattleStates state;

    int currentAction;
    int currentMove;
    int currentTarget;
    bool aboutToUseChoice = true;
    public UnitParty PlayerParty { get; private set; }
    public UnitParty TrainerParty { get; private set; }
    public Unit WildUnit { get; private set; }
    public Field Field { get; private set; }

    public bool IsTrainerBattle { get; private set; } = false;
    public PlayerController Player { get; private set; }
    public TrainerController Trainer { get; private set; }

    int escapeAttempts;
    MoveBase moveToLearn;
    BattleUnit unitTryingToLearn;

    BattleUnit unitToSwitch;

    BattleTrigger battleTrigger;
    public void StartBattle(UnitParty PlayerParty, Unit WildUnit,
        BattleTrigger trigger = BattleTrigger.LongGrass)
    {
        battleTrigger = trigger;
        this.PlayerParty = PlayerParty;
        this.WildUnit = WildUnit;
        Player = PlayerParty.GetComponent<PlayerController>();
        IsTrainerBattle = false;

        UnitCount = 1;

        AudioManager.i.PlayMusic(wildBattleMusic);
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(UnitParty PlayerParty, UnitParty TrainerParty,
        int UnitCount = 1)
    {
        battleTrigger = BattleTrigger.Trainer;
        this.PlayerParty = PlayerParty;
        this.TrainerParty = TrainerParty;

        IsTrainerBattle = true;
        Player = PlayerParty.GetComponent<PlayerController>();
        Trainer = TrainerParty.GetComponent<TrainerController>();
        AudioManager.i.PlayMusic(trainerBattleMusic);
        this.UnitCount = UnitCount;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        Debug.Log("wake up in setupbattle in battle system");
        StateMachine = new StateMachine<BattleSystem>(this);

        singleBattleElements.SetActive(UnitCount == 1);
        multiBattleElements.SetActive(UnitCount > 1);

        if (UnitCount == 1)
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

        if (!IsTrainerBattle)
        {
            // 야생 전투
            playerUnits[0].Setup(PlayerParty.GetHealthyUnit());
            enemyUnits[0].Setup(WildUnit);

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
            playerImage.sprite = Player.Sprite;
            trainerImage.sprite = Trainer.Sprite;
            yield return dialogBox.TypeDialog($"{Trainer.Name}을 쓰러트리기 위한 전투가 시작된다!");

            // 상대 전투 유닛 출동
            trainerImage.gameObject.SetActive(false);
            var healthEnemyUnits = TrainerParty.GetHealthyUnits(UnitCount);
            for (int i = 0; i < UnitCount; i++)
            {
                enemyUnits[i].gameObject.SetActive(true);
                enemyUnits[i].Setup(healthEnemyUnits[i]);
            }
            string names = String.Join("와(과) ", healthEnemyUnits.Select(u => u.Base.Name));
            yield return dialogBox.TypeDialog($"상대 {names}(이)가 먼저 나온다!");

            // 팀 전투 유닛 출동
            playerImage.gameObject.SetActive(false);
            var healthPlayerUnits = PlayerParty.GetHealthyUnits(UnitCount);
            for (int i = 0; i < UnitCount; i++)
            {
                playerUnits[i].gameObject.SetActive(true);
                playerUnits[i].Setup(healthPlayerUnits[i]);
            }
            names = String.Join("와(과) ", healthPlayerUnits.Select(u => u.Base.Name));
            yield return dialogBox.TypeDialog($"힘내, {names}!");

            // 특성
            List<BattleUnit> allOfUnits = Enumerable.Concat(playerUnits, enemyUnits)
                                            .OrderByDescending(u => u.Unit.Speed)
                                            .ToList();

            allOfUnits.ForEach(u =>
            {
                u.Unit.Base.Ability.BeforeRunTurn();
                u.Unit.Base.SecondAbility.BeforeRunTurn();
            });
        }
        IsbattleOver = false;
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


    public void BattleOver(bool won)
    {
        IsbattleOver = true;
        PlayerParty.Units.ForEach(unit => unit.OnBattleOver());

        playerUnits.ForEach(pu => pu.Hud.ClearData());
        enemyUnits.ForEach(eu => eu.Hud.ClearData());

        OnBattleOver(won);
    }

    void ActionSelection(int actionIndex = 0)
    {
        state = BattleStates.ActionSelection;
        // StartCoroutine(dialogBox.SetDialog("행동을 선택하세요."));
        this.ActionIndex = actionIndex;
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
    public void AddBattleAction(BattleAction action)
    {
        actions.Add(action);
        Debug.Log($"here is battlesystem{actions.Count == UnitCount}, {actions.Count}, {UnitCount}");
        // 유저에게 선택된 것 확인
        if (actions.Count == UnitCount || PlayerParty.CheckHealthyUnits() == 1)
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

            // StartCoroutine(RunTurns());
        }
        else
        {
            ActionSelection(ActionIndex + 1);
        }
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
            ActionSelection(ActionIndex);
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
            for (int i = 0; i < UnitCount; i++)
            {
                playerUnits[i].SetSelected(false);
            }
        }
        if (Input.GetButtonDown("Submit"))
        {
            for (int i = 0; i < UnitCount; i++)
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
            for (int i = 0; i < UnitCount; i++)
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
            partyScreen.gameObject.SetActive(false);
        };
        Action onBack = () =>
        {
            if (playerUnits.Any(u => u.Unit.HP <= 0))
            {
                partyScreen.SetMessageText("전투를 계속하기 위해 팀원을 보내야합니다!");
                return;
            }
            partyScreen.gameObject.SetActive(false);
        };
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

    public IEnumerator SwitchUnit(BattleUnit unitToSwitch, Unit newUnit)
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

        // 특성
        newUnit.Base.Ability.BeforeRunTurn();
        newUnit.Base.SecondAbility.BeforeRunTurn();

        // if (isTrainerAboutToUse)
        //     StartCoroutine(SendNextTrainerUnit());
        // else
        //     state = BattleStates.RunningTurn;
    }

    public IEnumerator SendNextTrainerUnit()
    {
        state = BattleStates.Busy;

        var faintedUnit = enemyUnits.First(u => u.Unit.HP == 0);
        var activeUnits = enemyUnits.Select(unit => unit.Unit).Where(u => u.HP > 0).ToList();
        var nextUnit = TrainerParty.GetHealthyUnit(activeUnits);
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
    public IEnumerator ThrowBall(BallItem ballItem)
    {
        if (IsTrainerBattle)
        {
            bool hasKarlord = false;
            if (enemyUnits.Any(u => u.Unit.Base.Name == "로드"))
            {
                // TODO: 로드에게 던졌을 때, 카를 격노 이벤트?
                var karl = TrainerParty.Units.Any(u => u.Base.Name == "카를");
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

            PlayerParty.AddUnit(enemyUnit.Unit);
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

    public void ResetActions()
    {
        this.ActionIndex = 0;
        actions.Clear();
    }

    public BattleDialogBox DialogBox => dialogBox;
    public PartyScreen PartyScreen => partyScreen;

    // public List<BattleUnit> PlayerUnits()
    // {
    //     List<BattleUnit> returnUnits;
    //     if (UnitCount == 1)
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
    //     UnitCount == 1 ?
    //         new List<BattleUnit>() 

    // public List<BattleUnit> EnemyUnits =>
    //     UnitCount == 1 ?
    //         new List<BattleUnit>() { enemyUnitSingle } :
    //         enemyUnitsMulti.GetRange(0, enemyUnitsMulti.Count);

    public List<BattleUnit> PlayerUnits => playerUnits;
    public List<BattleUnit> EnemyUnits => enemyUnits;
    public BattleUnit CurrentUnit => currentUnit;

    public List<BattleAction> Actions => actions;

    public AudioClip WildBattleVictoryMusic => wildBattleVictoryMusic;
    public AudioClip TrainerBattleVictoryMusic => trainerBattleVictoryMusic;
}
