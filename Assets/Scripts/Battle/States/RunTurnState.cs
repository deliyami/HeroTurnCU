using GDEUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;


public class RunTurnState : State<BattleSystem>
{
    public static RunTurnState i { get; private set; }
    private void Awake()
    {
        i = this;
    }


    // insert value when enter function
    BattleSystem bs;
    List<BattleUnit> playerUnits;
    List<BattleUnit> enemyUnits;
    BattleDialogBox dialogBox;
    PartyScreen partyScreen;
    Field Field;
    bool isTrainerBattle;
    UnitParty playerParty;
    UnitParty trainerParty;
    int unitCount;
    int escapeAttempts = 0;

    public override void Enter(BattleSystem owner)
    {
        bs = owner;

        playerUnits = bs.PlayerUnits;
        enemyUnits = bs.EnemyUnits;
        dialogBox = bs.DialogBox;
        partyScreen = bs.PartyScreen;
        Field = bs.Field;
        isTrainerBattle = bs.IsTrainerBattle;
        playerParty = bs.PlayerParty;
        trainerParty = bs.TrainerParty;
        unitCount = bs.UnitCount;

        Debug.Log($"현재 액션 in runturnstate count:{bs.Actions.Count}");
        foreach (var arr in ActionSelectionState.i.Arrow) arr.gameObject.SetActive(false);
        if (bs.Actions.Count < bs.UnitCount && bs.PlayerParty.CheckHealthyUnits() >= bs.UnitCount)
        {
            bs.StateMachine.ChangeState(ActionSelectionState.i);
            return;
        }

        StartCoroutine(RunTurns());
    }

    IEnumerator RunTurns()
    {
        // var changedAction;
        // sort Actions
        foreach (var action in bs.Actions)
        {
            RunChangeTurn(action.User);
            yield return RunBeforeTurn(action.User);
        }
        IOrderedEnumerable<BattleAction> ac = bs.Actions.OrderByDescending(a => a.Priority);
        List<BattleAction> actions;


        if (bs?.Field?.Room?.condition?.ID == ConditionID.trickRoom)
            actions = ac.ThenBy(a =>
            {
                if (a.User.IsPlayerUnit)
                {
                    if (bs.Field.PlayerTailwind != null)
                        return a.User.Unit.Speed * 2;
                    return a.User.Unit.Speed;
                }
                else
                {
                    if (bs.Field.EnemyTailwind != null)
                        return a.User.Unit.Speed * 2;
                    return a.User.Unit.Speed;
                }
            }).ToList();
        else
            actions = ac.ThenByDescending(a =>
            {
                if (a.User.IsPlayerUnit)
                {
                    if (bs.Field.PlayerTailwind != null)
                        return a.User.Unit.Speed * 2;
                    return a.User.Unit.Speed;
                }
                else
                {
                    if (bs.Field.EnemyTailwind != null)
                        return a.User.Unit.Speed * 2;
                    return a.User.Unit.Speed;
                }
            }).ToList();


        foreach (var action in actions)
        {

            if (action.IsInvalid) continue;
            if (action.Type == ActionType.Move)
            {
                yield return RunMove(action.User, action.Target, action.Move);
                if (bs.IsbattleOver) yield break;
            }
            else if (action.Type == ActionType.SwitchUnit)
            {
                yield return bs.SwitchUnit(action.User, action.SelectedUnit);
            }
            else if (action.Type == ActionType.UseItem)
            {
                // 아이템 창 탈출하고 적 턴을 실행해야함
                // dialogBox.EnableActionSelector(false);
                if (action.SelectedItem is BallItem)
                {
                    yield return bs.ThrowBall(action.SelectedItem as BallItem);
                    if (bs.IsbattleOver) yield break;
                }
                else
                {
                    if (action.SelectedItem.isUsable(action.User.Unit))
                    {
                        bool itemUsed = action.SelectedItem.Use(action.User.Unit);
                        if (itemUsed)
                        {
                            // 유닛이 필드에 나와있는지 확인하고 이벤트 작성
                            yield return action.User.Hud.WaitForHPUpdate();
                            yield return dialogBox.TypeDialog($"{action.User.Unit.Base.Name}(은)는 아이템을 사용했다!");
                        }
                    }
                    else
                        yield return dialogBox.TypeDialog($"{action.User.Unit.Base.Name}(은)는 아이템 사용에 실패했다!");
                }
            }
            else if (action.Type == ActionType.Run)
            {
                yield return TryToEscape();
            }
            if (bs.IsbattleOver) break;
        }

        if (Field.Weather != null)
        {
            yield return dialogBox.TypeDialog(Field.Weather.condition.EffectMessage);
            for (int i = 0; i < playerUnits.Count; i++)
            {
                var pu = playerUnits[i];
                Field.Weather.condition.OnWeather?.Invoke(pu.Unit);
                yield return ShowStatusChanges(pu.Unit);
                if (pu.Unit.HPChanged) pu.PlayerDamagedtAnimation();
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
                Field.Weather.condition.OnWeather?.Invoke(eu.Unit);
                yield return ShowStatusChanges(eu.Unit);
                if (eu.Unit.HPChanged) eu.PlayerDamagedtAnimation();
                eu.Hud.UpdateHP();
                if (eu.Unit.HP <= 0)
                {
                    yield return dialogBox.TypeDialog($"{eu.Unit.Base.Name}은(는) 쓰러졌다!");
                    yield return HandleUnitFainted(eu);
                    yield break;
                }
            }
            if (Field.Weather != null)
            {
                Field.Weather.duration--;
                Debug.Log(Field.Weather);
                Debug.Log(Field.Weather.duration);
                if (Field.Weather.duration == 0)
                {
                    Field.Weather = null;
                    Field.Weather.duration = 0;
                    yield return dialogBox.TypeDialog("날씨가 원래대로 되돌아왔다!");
                }
            }
        }
        FinishTurnCheckField(Field.Room, "공간이 원래대로 되돌아왔다!");
        FinishTurnCheckField(Field.field, "필드가 원래대로 되돌아왔다!");
        FinishTurnCheckField(Field.PlayerReflect, "우리쪽의 분위기가 원래대로 되돌아왔다!");
        FinishTurnCheckField(Field.PlayerLightScreen, "우리쪽의 위화감이 원래대로 되돌아왔다!");
        FinishTurnCheckField(Field.EnemyReflect, "적의 분위기가 원래대로 되돌아왔다!");
        FinishTurnCheckField(Field.EnemyLightScreen, "적의 위화감이 원래대로 되돌아왔다!");
        FinishTurnCheckField(Field.PlayerTailwind, "상대측의 바람이 멎었다!");
        FinishTurnCheckField(Field.EnemyTailwind, "우리측의 바람이 멎었다!");

        foreach (var action in actions)
            yield return RunAfterTurn(action.User);

        if (!bs.IsbattleOver)
        {
            Debug.Log("reset battle action");
            bs.ResetActions();
            bs.StateMachine.ChangeState(ActionSelectionState.i);
        }
        GlobalSettings.i.TakedTurn++;
    }
    IEnumerator FinishTurnCheckField(FieldBase fieldBase, string returnMessage)
    {
        if (fieldBase != null)
        {
            fieldBase.duration--;
            if (fieldBase.duration == 0)
            {
                fieldBase.duration = 0;
                fieldBase = null;
                yield return dialogBox.TypeDialog(returnMessage);
            }
        }
    }
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Unit.OnBeforeMove();

        yield return ShowStatusChanges(sourceUnit.Unit);
        if (!canRunMove)
        {
            yield return sourceUnit.Hud.WaitForHPUpdate();
            yield break;
        }

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}(이)가 {move.Base.Name}을(를) 사용했다!");

        // 특성
        sourceUnit.Unit.Base.Ability?.BeforeAttack(sourceUnit, move);
        sourceUnit.Unit.Base.SecondAbility?.BeforeAttack(sourceUnit, move);

        // 여기서 맞을 친구들 정하기
        List<BattleUnit> targetedUnits = new List<BattleUnit>();

        List<BattleUnit> sourceUnits = sourceUnit.IsPlayerUnit ? playerUnits : enemyUnits;
        List<BattleUnit> targetUnits = targetUnit.IsPlayerUnit ? playerUnits : enemyUnits;

        bool isSingleTarget = move.Base.Target == MoveTarget.Foe || move.Base.Target == MoveTarget.Team || move.Base.Target == MoveTarget.TeamAnother || move.Base.Target == MoveTarget.Self || move.Base.Target == MoveTarget.Another;

        if (isSingleTarget)
        {
            if (targetUnit.HasUnit()) targetedUnits.Add(targetUnit);
            // 이하는 hasUnit == null => 남은 유닛이 하나라 나머지 자리에 null이 된 경우
            else if (move.Base.Target == MoveTarget.Foe) targetedUnits.Add(targetUnits.Where(t => t.HasUnit()).First());
            else if (move.Base.Target == MoveTarget.Team) targetedUnits.Add(sourceUnit);
            else if (move.Base.Target == MoveTarget.Another && move.Base.Category != MoveCategory.Status)
            {
                targetedUnits.Add(targetUnits.Where(t => t.HasUnit()).First());
            }
            else
            {
                yield return dialogBox.TypeDialog("기술을 사용 할 상대가 없다!");
                yield break;
            }
        }
        if (move.Base.Target == MoveTarget.FoeAll)
        {
            targetUnits.ForEach(t =>
            {
                Debug.Log($"여기는 run turn state{t.Unit.Base.Name}");
                targetedUnits.Add(t);
            });
        }
        if (move.Base.Target == MoveTarget.TeamAll)
        {
            sourceUnits.ForEach(s => targetedUnits.Add(s));
        }
        if (move.Base.Target == MoveTarget.AnotherAll)
        {
            sourceUnits.ForEach(s =>
            {
                if (s.Unit.Base.Name != sourceUnit.Unit.Base.Name) targetedUnits.Add(s);
            });
            targetUnits.ForEach(t => targetedUnits.Add(t));
        }
        if (move.Base.Target == MoveTarget.All)
        {
            sourceUnits.ForEach(s => targetedUnits.Add(s));
            targetUnits.ForEach(t => targetedUnits.Add(t));
        }
        // TODO : 여기서 공격하는 애니메이션을 수정해야함
        if (isSingleTarget)
            yield return PlayAttackAnimation(sourceUnit, targetedUnits, move);
        else
            yield return PlayBackgroundAnimation(sourceUnit, targetedUnits, move);
        // move.Base.GetHitTimes() == 1이면 단일공격, 2이상이면 멀티공격
        bool isSingleTimeAttack = move.Base.GetHitTimes() == 1;
        bool isAppliedableEffect = true;
        bool isAppliedableSecondaryEffect = true;
        foreach (var targeted in targetedUnits)
        {
            if (!isAppliedableEffect) break;
            if (!targeted.HasUnit()) continue;
            bool checkDefenseAbility = (targeted.Unit.Base.Ability?.BeforeDefense(targeted, move) ?? true) && (targeted.Unit.Base.SecondAbility?.BeforeDefense(targeted, move) ?? true);
            bool accuracyHit = CheckIfMoveHits(move, sourceUnit.Unit, targeted.Unit);
            if (accuracyHit && checkDefenseAbility && (!move.Base.FirstTurnChance || move.IsActivitable) && !targeted.Unit.IsProtectActivative)
            {
                int hitTimes = move.Base.GetHitTimes();
                float typeEffectiveness = 1f;
                int hit = 1;
                int damage = 0;
                for (int i = 1; i <= hitTimes; i++)
                {
                    if (i > 1)
                    {
                        yield return PlayAttackAnimation(sourceUnit, targetedUnits, move);
                        yield return dialogBox.TypeDialog($"{i}번째 공격!");
                    }
                    if (move.Base.Sound != null)
                        AudioManager.i.PlaySfx(move.Base.Sound);

                    yield return new WaitForSeconds(1f);
                    if (move.Base.Category != MoveCategory.Status)
                    {
                        // 공격 받고나서 깜빡이는 애니메이션인데... 추가적인 애니메이션이 필요한지 생각해야함
                        targeted.PlayerDamagedtAnimation();
                        AudioManager.i.PlaySfx(AudioId.Hit);
                    }

                    if (move.Base.Category == MoveCategory.Status && isAppliedableEffect)
                    {
                        // isAppliedableEffect = !(move.Base.Effects.Weather != ConditionID.none || move.Base.Effects.Room != ConditionID.none || move.Base.Effects.Field != ConditionID.none || move.Base.Effects.Reflect != ConditionID.none || move.Base.Effects.LightScreen != ConditionID.none || move.Base.Effects.Tailwind != ConditionID.none);
                        isAppliedableEffect = move.Base.Effects.Weather == ConditionID.none && move.Base.Effects.Room == ConditionID.none && move.Base.Effects.Field == ConditionID.none && move.Base.Effects.Reflect == ConditionID.none && move.Base.Effects.LightScreen == ConditionID.none && move.Base.Effects.Tailwind == ConditionID.none;

                        yield return RunMoveEffect(move.Base.Effects, sourceUnit, targeted.Unit, move.Base.Target);
                    }
                    else
                    {
                        var damageDetails = targeted.Unit.TakeDamage(move, sourceUnit.Unit, Field, sourceUnit.IsPlayerUnit);
                        damage = damageDetails.Damage;
                        yield return targeted.Hud.WaitForHPUpdate();
                        yield return ShowDamageDetails(damageDetails);
                        typeEffectiveness = damageDetails.TypeEffectiveness;
                    }

                    if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targeted.Unit.HP > 0 && isAppliedableSecondaryEffect)
                    {
                        foreach (var secondary in move.Base.Secondaries)
                        {
                            var rnd = UnityEngine.Random.Range(1, 101);
                            if (rnd <= secondary.Chance)
                            {
                                if (secondary.Target == MoveTarget.Self || !isSingleTimeAttack) isAppliedableSecondaryEffect = false;
                                yield return RunMoveEffect(secondary, sourceUnit, targeted.Unit, secondary.Target);
                            }
                        }
                    }
                    hit = i;
                    if (targeted.Unit.HP <= 0)
                    {
                        bool abilityFocusSash = targeted.Unit.Base.Ability?.isFocusSash() ?? false;
                        bool secondAbilityFocusSash = targeted.Unit.Base.Ability?.isFocusSash() ?? false;
                        if (abilityFocusSash || secondAbilityFocusSash) targeted.Unit.SetHP(1);
                        else break;
                    }
                }
                yield return ShowEffectiveness(typeEffectiveness);
                if (hitTimes > 1)
                    yield return dialogBox.TypeDialog($"{hit}번 공격했다!");

                if ((move.Base.Rebound.x != 0 && move.Base.Rebound.y != 0) || move.Base.Rebound.z != 0)
                {
                    Debug.Log($"{sourceUnit.Unit.Base.Name}반동피해 ${damage}");
                    sourceUnit.PlayerDamagedtAnimation();
                    AudioManager.i.PlaySfx(AudioId.Hit);
                    sourceUnit.Unit.ReboundTakeDamage(move.Base.Rebound, damage);
                    yield return sourceUnit.Hud.WaitForHPUpdate();
                    yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}은(는) 반동피해를 입었다!");
                }
                if (move.Base.IsStruggle)
                {
                    sourceUnit.PlayerDamagedtAnimation();
                    AudioManager.i.PlaySfx(AudioId.Hit);
                    sourceUnit.Unit.ReboundTakeDamage(move.Base.Rebound, sourceUnit.Unit.MaxHP / 4);
                    yield return sourceUnit.Hud.WaitForHPUpdate();
                    yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}은(는) 몸부림 쳤다!");
                }
                if (move.Base.BellyDrum)
                {
                    sourceUnit.PlayerDamagedtAnimation();
                    AudioManager.i.PlaySfx(AudioId.Hit);
                    sourceUnit.Unit.ReboundTakeDamage(move.Base.Rebound, sourceUnit.Unit.MaxHP / 2);
                    yield return sourceUnit.Hud.WaitForHPUpdate();
                    yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}의 분위기가 무지막지하게 변했다!");
                }

                if (targeted.Unit.HP <= 0)
                {
                    if (sourceUnit.Unit.HP > 0)
                    {
                        sourceUnit.Unit.ApplyBoosts(sourceUnit.Unit.Base.Ability?.OnFinish() ?? new List<StatBoost>());
                        sourceUnit.Unit.ApplyBoosts(sourceUnit.Unit.Base.SecondAbility?.OnFinish() ?? new List<StatBoost>());
                    }
                    // AudioManager.i.PlaySfx(AudioId.Faint);
                    yield return HandleUnitFainted(targeted);
                }
                else if (sourceUnit.Unit.HP <= 0)
                {
                    yield return HandleUnitFainted(sourceUnit);
                    break;
                }

                if (move.Base.Category != MoveCategory.Status)
                {
                    // 특성
                    var abilityConditionObject = sourceUnit.Unit.Base.Ability?.AfterAttack(sourceUnit, targeted, move);
                    var secondAbilityConditionObject = sourceUnit.Unit.Base.SecondAbility?.AfterAttack(sourceUnit, targeted, move);
                    // 맞는 녀석 상태이상 특성
                    var targetedAbilityConditionObject = targeted.Unit.Base.Ability?.AfterDefense(sourceUnit, targeted, move);
                    var targetedSecondAbilityConditionObject = targeted.Unit.Base.SecondAbility?.AfterDefense(sourceUnit, targeted, move);
                    yield return RunAbilityAfterAttack(abilityConditionObject, sourceUnit.Unit, targeted.Unit);
                    yield return RunAbilityAfterAttack(secondAbilityConditionObject, sourceUnit.Unit, targeted.Unit);
                    yield return RunAbilityAfterAttack(targetedAbilityConditionObject, sourceUnit.Unit, targeted.Unit);
                    yield return RunAbilityAfterAttack(targetedSecondAbilityConditionObject, sourceUnit.Unit, targeted.Unit);
                }
            }
            else
            {
                if (!checkDefenseAbility)
                {
                    yield return dialogBox.TypeDialog($"{targeted.Unit.Base.Name}에게는 효과가 듣지 않는 것 같다!");
                }
                else if (!accuracyHit)
                {
                    yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}의 공격이 빗나갔다!");
                }
                else if (!move.IsActivitable)
                {
                    yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}(은)는 공격을 실패했다!");
                }
                else if (targeted.Unit.IsProtectActivative)
                {
                    yield return dialogBox.TypeDialog($"{targeted.Unit.Base.Name}(은)는 몸을 지키고 있다!");
                }
            }
        }
        // 교체 기술 사용
        int sourceUnitCount = sourceUnit.IsPlayerUnit ? playerParty.GetHealthyUnitCount() : trainerParty.GetHealthyUnitCount();
        if (sourceUnit.Unit.HP > 0 && move.Base.IsChangeUnit && sourceUnitCount > 2)
        {
            yield return HandleChangeMove(sourceUnit);
        }
        move.IsActivitable = false;
    }


    IEnumerator ShowStatusChanges(Unit unit)
    {
        while (unit.StatusChanges.Count > 0)
        {
            var message = unit.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }
    void RunChangeTurn(BattleUnit sourceUnit)
    {
        int ability = sourceUnit.Unit.Base.Ability?.BeforeTurnChange(sourceUnit) ?? 0;
        int secondAbility = sourceUnit.Unit.Base.SecondAbility?.BeforeTurnChange(sourceUnit) ?? 0;
        if (ability + secondAbility != 0)
        {
            foreach (var a in bs.Actions)
                if (a.User.Unit.Base.Name == sourceUnit.Unit.Base.Name)
                    a.Move.Base.AddPriority(ability + secondAbility);
        }
    }
    IEnumerator RunBeforeTurn(BattleUnit sourceUnit)
    {
        // 특성
        yield return sourceUnit.Unit.Base.Ability?.BeforeRunTurn(Field, sourceUnit.Unit);
        yield return sourceUnit.Unit.Base.SecondAbility?.BeforeRunTurn(Field, sourceUnit.Unit);
    }
    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (bs.IsbattleOver) yield break;
        // 방어 제거
        sourceUnit.Unit.SetProtect(false);
        // 특성
        sourceUnit.Unit.Base.Ability?.AfterRunTurn(sourceUnit);
        sourceUnit.Unit.Base.SecondAbility?.AfterRunTurn(sourceUnit);
        yield return sourceUnit.Hud.WaitForHPUpdate();
        // 상태이상으로 쓰러지는가?
        sourceUnit.Unit.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Unit);
        yield return sourceUnit.Hud.WaitForHPUpdate();

        if (sourceUnit.Unit.HP <= 0)
        {
            // AudioManager.i.PlaySfx(AudioId.Faint);
            yield return HandleUnitFainted(sourceUnit);
        }
    }

    IEnumerator HandleUnitFainted(BattleUnit faintedUnit)
    {
        AudioManager.i.PlaySfx(AudioId.Faint);
        yield return dialogBox.TypeDialog($"{faintedUnit.Unit.Base.Name}(이)가 쓰러졌다!");
        yield return RemoveUnit(faintedUnit);
        // faintedUnit.PlayFaintAnimation();

        // yield return new WaitForSeconds(2f);

        // yield return NextStepsAfterFainting(faintedUnit);
    }
    IEnumerator HandleChangeMove(BattleUnit sourceUnit)
    {
        yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}(이)가 전투에서 빠진다!");
        yield return RemoveUnit(sourceUnit);
        // sourceUnit.PlayFaintAnimation();

        // yield return new WaitForSeconds(2f);

        // yield return NextStepsAfterFainting(sourceUnit);
    }
    IEnumerator RemoveUnit(BattleUnit unit)
    {
        unit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);
        yield return NextStepsAfterFainting(unit);
    }

    IEnumerator NextStepsAfterFainting(BattleUnit faintedUnit)
    {
        List<BattleAction> actions = bs.Actions;
        var actionToRemove = actions.FirstOrDefault(a => a.User == faintedUnit);
        if (actionToRemove != null)
            actionToRemove.IsInvalid = true;
        Debug.Log($"player unit?{faintedUnit.IsPlayerUnit}");
        if (faintedUnit.IsPlayerUnit)
        {
            var activeUnits = playerUnits.Select(unit => unit.Unit).Where(u => u.HP > 0).ToList();
            var nextUnit = playerParty.GetHealthyUnit(activeUnits);

            if (activeUnits.Count == 0 && nextUnit == null)
            {
                bs.BattleOver(false);
            }
            else if (nextUnit != null)
            {
                // TODO change unit
                // unitToSwitch = faintedUnit;
                // OpenPartyScreen();
                yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
                yield return bs.SwitchUnit(faintedUnit, PartyState.i.SelectedUnit);
                // return; // return remove
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
            if (!bs.IsTrainerBattle)
            {
                bs.BattleOver(true);
                yield break;
            }
            Debug.Log("트레이너 전투다!");
            var activeUnits = enemyUnits.Select(unit => unit.Unit).Where(u => u.HP > 0).ToList();
            var nextUnit = trainerParty.GetHealthyUnit(activeUnits);

            if (activeUnits.Count == 0 && nextUnit == null)
            {
                Debug.Log("트레이너 전투다!, 모든 유닛 패배!");
                bs.BattleOver(true);
            }
            else if (nextUnit != null)
            {
                Debug.Log("트레이너 전투다!, 교체 유닛이 있다!");
                if (unitCount == 1)
                {
                    // TODO change unit
                    // unitToSwitch = playerUnits[0];
                    // StartCoroutine(AboutToUse(nextUnit));
                }
                if (faintedUnit.Unit.HP > 0)
                {
                    yield return bs.ChangeNextTrainerUnitByUTurn(faintedUnit, nextUnit);
                }
                else
                {
                    AboutToUseState.i.NewUnit = nextUnit;
                    yield return bs.StateMachine.PushAndWait(AboutToUseState.i);
                }
                // else
                // TODO Change Unit
                // StartCoroutine(bs.ChangeNextTrainerUnit());
            }
            else if (nextUnit == null && activeUnits.Count > 0)
            {
                // 더이상의 적은 없지만 전투는 계속 됨
                Debug.Log("트레이너 전투다!, 교체유닛은 없지만 싸우고 있는 유닛은 있다!");
                enemyUnits.Remove(faintedUnit);
                faintedUnit.Hud.gameObject.SetActive(false);

                var actionsToChange = actions.Where(a => a.Target == faintedUnit).ToList();
                actionsToChange.ForEach(a => a.Target = enemyUnits.First());
            }
        }
    }

    IEnumerator PlayBackgroundAnimation(BattleUnit sourceUnit, List<BattleUnit> targetUnits, Move move)
    {
        // bs.BattleCanvas.GetComponent<Image>().DOColor(Color.white, 0.2f).SetLoops(2, LoopType.Yoyo);
        string moveName = move.Base.Name;
        switch (moveName)
        {
            case "적전체공격test":
                break;
            // tailWind
            case "업템포":
                break;
            // sparklingAria
            case "에코 슬라이드":
                for (int i = 0; i < playerUnits.Count; i++)
                {
                    yield return playerUnits[i].sparklingAriaHit();
                }
                break;
            // citronVeil
            case "R-77":
                float x = 0;
                playerUnits.ForEach(p =>
                {
                    x += p.transform.position.x;
                });
                yield return sourceUnit.citronVeilAttack(x);

                // effects[i].transform.position = this.transform.position + (isPlayerUnit ? Vector3.down * 0.55f : Vector3.zero) + new Vector3(x, y);
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator PlayAttackAnimation(BattleUnit sourceUnit, List<BattleUnit> targetUnits, Move move)
    {
        string moveName = move.Base.Name;
        switch (moveName)
        {
            // 테스트
            case "적전체공격test":
                sourceUnit.Test1Attack();
                targetUnits.ForEach(t => t.Test1Hit());
                break;
            // 아군
            // 용사
            // bodyPress
            case "가드 대쉬":
                sourceUnit.bodyPressAttack();
                // targetUnits.ForEach(t => t.bodyPressHit());
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].bodyPressHit();
                }
                break;
            // doubleIronBash
            case "검휘두르기":
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].doubleIronBashHit();
                }
                break;
            // headSmash
            case "필사의 돌진":
                yield return sourceUnit.headSmashAttack();
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    Debug.Log("필사의 돐진 피격 실행");
                    yield return targetUnits[i].headSmashHit();
                }
                break;
            // ironDefense
            case "방어자세":
                yield return sourceUnit.ironDefenseAttack();
                break;
            // 히나미
            // bulletPunch
            case "은탄":
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].bulletPunchHit();
                }
                break;
            // rockBlast
            case "속사":
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].rockBlastHit();
                }
                break;
            // snipeShot
            case "조준사격":
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].snipeShotHit();
                }
                break;
            // suckerPunch
            case "빈틈포착":
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].suckerPunchHit();
                }
                break;
            // 크라베
            // meteorBeam
            case "화살":
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].meteorBeamHit();
                }
                break;
            // meteorMash
            case "바늘":
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].meteorMashHit();
                }
                break;
            // moonBlast
            case "달":
                Image background = BattleSystem.i.BackgroundImage;
                Color backgroundOriginalColor = new Color(background.color.r, background.color.g, background.color.b, 1f);
                background.DOColor(new Color((backgroundOriginalColor.r * 255 - 50f) / 255, (backgroundOriginalColor.r * 255 - 50f) / 255, (backgroundOriginalColor.r * 255 - 50f) / 255, 1f), 0.5f);
                yield return sourceUnit.moonBlastAttack();
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    targetUnits[i].moonBlastHit();
                }
                background.DOColor(backgroundOriginalColor, 0.5f);
                break;
            // aircutter
            case "바람":
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].aircutterHit();
                }
                break;
            // 흑유령
            // shadowBall
            case "사령구체":
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].shadowBallHit();
                }
                break;
            // flameThrower
            case "화염방사":
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].flameThrowerHit();
                }
                break;
            // protect
            case "유체화":
                yield return sourceUnit.protectAttack();
                break;
            // calmMind
            case "집중":
                yield return sourceUnit.calmMindAttack();
                break;
            // 마시로
            // hyperVoice
            case "소리지르기":
                yield return sourceUnit.hyperVoiceAttack();
                // for (int i = 0; i < targetUnits.Count; i++)
                // {
                //     yield return targetUnits[i].hyperVoiceHit();
                // }
                BattleSystem.i.BackgroundImage.transform.DOShakePosition(1f, 0.3f, 10, 90, false, true);
                yield return sourceUnit.moonBlastAttack();
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    targetUnits[i].moonBlastHit();
                }
                break;
            // partingShot
            case "야유":
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].partingShotHit();
                }
                break;
            // 잇쨩
            // nuzzle
            case "CM-516":
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].nuzzleHit();
                }
                break;
            // voltSwitch
            case "PRPR-410":
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    yield return targetUnits[i].voltSwitchHit();
                }
                break;
            // skillSwap
            case "NK-714":
                break;

            // 적
            // 카를
            // phantomForce
            case "슬래시":
                break;
            // crunch
            case "가드크래시":
                break;
            // shadowClaw
            case "스크래치":
                break;
            // willOWisop
            case "헬파이어":
                break;
            // 로드
            // bellyDrum
            case "암흑의 정신":
                break;
            // icePunch
            case "냉동고드름":
                break;
            // ironHead
            case "철구투척":
                break;
            // explosion
            case "침묵의 길":
                break;
            // 살라만다
            // flameThrower
            // case "flameThrower":
            //     break;
            // dracoMeteor
            case "크게 화내기":
                break;
            // flareBlitz
            case "몸통박치기":
                break;
            // earthquake
            case "땅고르기":
                break;
            // 연화
            // overHeat
            case "화신섬광":
                break;
            // leafStorm
            case "녹각풍":
                break;
            // vacuumWave
            case "공기탄":
                break;
            // fakeOut
            case "엄습":
                break;
            // 컨트롤러
            // ufoBeam
            case "우상-창조-":
                break;
            // bulldoze
            case "둘 만의 결정":
                break;
            // stoneEdge
            case "소성단의 꿈":
                break;
            // airSlach
            case "업보":
                break;
            // 트레카
            // swordsDance
            case "순보":
                break;
            // uTurn
            case "영원한 토끼":
                break;
            // firstImpression
            case "허풍떨기":
                break;
            // spinOut
            case "발도:만월베기":
                break;
            // 무너
            // scald
            case "물주전자":
                break;
            // iceBeam
            case "빙하물주전자":
                break;
            // surf
            case "급격한 조수":
                break;
            // lightScreen
            case "lightScreen":
                break;
            default:
                sourceUnit.PlayDefaultAttackAnimation();
                break;
        }
        // yield return new WaitForSeconds(0.5f);
    }

    IEnumerator RunMoveEffect(MoveEffects effects, BattleUnit sourceUnit, Unit target, MoveTarget moveTarget)
    {
        Unit source = sourceUnit.Unit;
        // stat 증가 TODO 여러명 때리는 경우에 셀프 스텟 증가하면 2번 3번 중첩되는 문제가 생김
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        // status 이상
        if (effects.Status != ConditionID.none && target.Status == null)
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
            Field.Weather = new FieldBase();
            Field.Weather.SetCondition(effects.Weather);
            Field.Weather.duration = (int)(5 * (source.Base.Ability?.OnField() ?? 1) * (source.Base.SecondAbility?.OnField() ?? 1));
            yield return dialogBox.TypeDialog(Field.Weather.condition.StartMessage);
        }

        // 공간 변경
        if (effects.Room != ConditionID.none)
        {
            Field.Room = new FieldBase();
            Field.Room.SetCondition(effects.Room);
            Field.Room.duration = (int)(5 * (source.Base.Ability?.OnField() ?? 1) * (source.Base.SecondAbility?.OnField() ?? 1));
            yield return dialogBox.TypeDialog(Field.Room.condition.StartMessage);
        }

        // 필드 변경
        if (effects.Field != ConditionID.none)
        {
            Field.field = new FieldBase();
            Field.field.SetCondition(effects.Field);
            Field.field.duration = (int)(5 * (source.Base.Ability?.OnField() ?? 1) * (source.Base.SecondAbility?.OnField() ?? 1));
            yield return dialogBox.TypeDialog(Field.field.condition.StartMessage);
        }

        // 리플렉터, 빛의 장막
        if (effects.Reflect != ConditionID.none)
        {
            if (sourceUnit.IsPlayerUnit)
            {
                Field.PlayerReflect = new FieldBase();
                Field.PlayerReflect.SetCondition(effects.Tailwind);
                Field.PlayerReflect.duration = (int)(5 * (source.Base.Ability?.OnField() ?? 1) * (source.Base.SecondAbility?.OnField() ?? 1));
                yield return dialogBox.TypeDialog(Field.PlayerReflect.condition.StartMessage);
            }
            else
            {
                Field.EnemyReflect = new FieldBase();
                Field.EnemyReflect.SetCondition(effects.Tailwind);
                Field.EnemyReflect.duration = (int)(5 * (source.Base.Ability?.OnField() ?? 1) * (source.Base.SecondAbility?.OnField() ?? 1));
                yield return dialogBox.TypeDialog(Field.EnemyReflect.condition.StartMessage);
            }
        }
        if (effects.LightScreen != ConditionID.none)
        {
            if (sourceUnit.IsPlayerUnit)
            {
                Field.PlayerLightScreen = new FieldBase();
                Field.PlayerLightScreen.SetCondition(effects.Tailwind);
                Field.PlayerLightScreen.duration = (int)(5 * (source.Base.Ability?.OnField() ?? 1) * (source.Base.SecondAbility?.OnField() ?? 1));
                yield return dialogBox.TypeDialog(Field.PlayerLightScreen.condition.StartMessage);
            }
            else
            {
                Field.EnemyLightScreen = new FieldBase();
                Field.EnemyLightScreen.SetCondition(effects.Tailwind);
                Field.EnemyLightScreen.duration = (int)(5 * (source.Base.Ability?.OnField() ?? 1) * (source.Base.SecondAbility?.OnField() ?? 1));
                yield return dialogBox.TypeDialog(Field.EnemyLightScreen.condition.StartMessage);
            }
        }

        // 순풍
        if (effects.Tailwind != ConditionID.none)
        {
            if (sourceUnit.IsPlayerUnit)
            {
                Field.PlayerTailwind = new FieldBase();
                Field.PlayerTailwind.SetCondition(effects.Tailwind);
                Field.PlayerTailwind.duration = (int)(5 * (source.Base.Ability?.OnField() ?? 1) * (source.Base.SecondAbility?.OnField() ?? 1));
                yield return dialogBox.TypeDialog(Field.PlayerTailwind.condition.StartMessage);
            }
            else
            {
                Field.EnemyTailwind = new FieldBase();
                Field.EnemyTailwind.SetCondition(effects.Tailwind);
                Field.EnemyTailwind.duration = (int)(5 * (source.Base.Ability?.OnField() ?? 1) * (source.Base.SecondAbility?.OnField() ?? 1));
                yield return dialogBox.TypeDialog(Field.EnemyTailwind.condition.StartMessage);
            }
        }

        // 방어
        if (effects.Protect)
        {
            bool success = false;
            if (moveTarget == MoveTarget.Self)
                success = source.UseProtect();
            else
                success = target.UseProtect();
            yield return dialogBox.TypeDialog(success ? "몸을 지키기 시작했다!" : "몸을 지키는데 실패했다!");
        }

        // dialog 박스 변경
        // playerUnit.Hud.UpdateStatus();

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
    // 특성 보고 변경
    IEnumerator RunAbilityAfterAttack((ConditionID, ConditionID, Stat, int, MoveTarget)? ability, Unit sourceUnit, Unit targetUnit)
    {
        if (ability != null)
        {
            ConditionID abilityCondition = ability.Value.Item1;
            ConditionID abilityVolatileCondition = ability.Value.Item2;
            Stat stat = ability.Value.Item3;
            int statBoost = ability.Value.Item4;
            MoveTarget moveTarget = ability.Value.Item5;
            yield return RunAbilityStatBoost(stat, statBoost, sourceUnit, targetUnit, moveTarget);
            yield return RunAbilityEffect(abilityCondition, sourceUnit, targetUnit, moveTarget);
            yield return RunAbilityVolatileStatusEffect(abilityVolatileCondition, sourceUnit, targetUnit, moveTarget);
        }
    }
    // 살아있으면 대상에게 능력치 부스트
    IEnumerator RunAbilityStatBoost(Stat stat, int statBoost, Unit source, Unit target, MoveTarget moveTarget)
    {
        if (statBoost != 0)
        {
            List<StatBoost> newStat = new List<StatBoost>() { new StatBoost() { stat = stat, boost = statBoost } };
            if (moveTarget == MoveTarget.Self) source.ApplyBoosts(newStat);
            else target.ApplyBoosts(newStat);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
    // 살아있으면 대상에게 상태이성을 걸음
    IEnumerator RunAbilityEffect(ConditionID condition, Unit source, Unit target, MoveTarget moveTarget)
    {
        if (condition != ConditionID.none)
        {
            if (moveTarget == MoveTarget.Self) source.SetStatus(condition);
            else target.SetStatus(condition);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
    IEnumerator RunAbilityVolatileStatusEffect(ConditionID condition, Unit source, Unit target, MoveTarget moveTarget)
    {
        if (condition != ConditionID.none && target.Status == null)
        {
            if (moveTarget == MoveTarget.Self) source.SetVolatileStatus(condition);
            else target.SetVolatileStatus(condition);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
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
    IEnumerator TryToEscape()
    {
        if (bs.IsTrainerBattle)
        {
            yield return dialogBox.TypeDialog("도망 칠 순 없다!");
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
            bs.BattleOver(true);
            escapeAttempts = 0;
        }
        else
        {
            yield return dialogBox.TypeDialog("도망갈 수 없다!");
        }
    }
    IEnumerator RunTurnFail(BattleUnit battleUnit)
    {
        yield return dialogBox.TypeDialog($"{battleUnit.Unit.Base.Name}(은)는 행동하는 것을 실패했다!");
    }
}
