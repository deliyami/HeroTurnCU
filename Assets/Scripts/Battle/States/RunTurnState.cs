using GDEUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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
            actions = ac.ThenBy(a => a.User.Unit.Speed).ToList();
        else
            actions = ac.ThenByDescending(a => a.User.Unit.Speed).ToList();


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
                Field.Weather.condition.OnWeather?.Invoke(eu.Unit);
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
            if (Field.Weather.duration != null)
            {
                Field.Weather.duration--;
                if (Field.Weather.duration == 0)
                {
                    Field.Weather = null;
                    Field.Weather.duration = null;
                    yield return dialogBox.TypeDialog("날씨가 원래대로 되돌아왔다!");
                }
            }
        }
        FinishTurnCheckField(Field.Room, "공간이 원래대로 되돌아왔다!");
        FinishTurnCheckField(Field.field, "필드가 원래대로 되돌아왔다!");
        FinishTurnCheckField(Field.Reflect, "분위기가 원래대로 되돌아왔다!");
        FinishTurnCheckField(Field.LightScreen, "위화감이 원래대로 되돌아왔다!");
        // if (Field.Room != null)
        // {
        //     if (Field.Room.duration != null)
        //     {
        //         Field.Room.duration--;
        //         if (Field.Room.duration == 0)
        //         {
        //             Field.Room = null;
        //             Field.Room.duration = null;
        //             yield return dialogBox.TypeDialog("공간이 원래대로 되돌아왔다!");
        //         }
        //     }
        // }
        // if (Field.field != null)
        // {
        //     if (Field.field.duration != null)
        //     {
        //         Field.field.duration--;
        //         if (Field.field.duration == 0)
        //         {
        //             Field.field = null;
        //             Field.field.duration = null;
        //             yield return dialogBox.TypeDialog("필드가 원래대로 되돌아왔다!");
        //         }
        //     }
        // }
        // if (Field.Reflect != null)
        // {
        //     if (Field.Reflect.duration != null)
        //     {
        //         Field.Reflect.duration--;
        //         if (Field.Reflect.duration == 0)
        //         {
        //             Field.Reflect = null;
        //             Field.Reflect.duration = null;
        //             yield return dialogBox.TypeDialog("분위기가 원래대로 되돌아왔다!");
        //         }
        //     }
        // }
        // if (Field.LightScreen != null)
        // {
        //     if (Field.LightScreen.duration != null)
        //     {
        //         Field.LightScreen.duration--;
        //         if (Field.LightScreen.duration == 0)
        //         {
        //             Field.LightScreen = null;
        //             Field.LightScreen.duration = null;
        //             yield return dialogBox.TypeDialog("위화감이 원래대로 되돌아왔다!");
        //         }
        //     }
        // }

        foreach (var action in actions)
            yield return RunAfterTurn(action.User);

        if (!bs.IsbattleOver)
        {
            bs.ResetActions();
            bs.StateMachine.ChangeState(ActionSelectionState.i);
        }
    }
    IEnumerator FinishTurnCheckField(FieldBase fieldBase, string returnMessage)
    {
        if (fieldBase != null)
        {
            if (fieldBase.duration != null)
            {
                fieldBase.duration--;
                if (fieldBase.duration == 0)
                {
                    fieldBase = null;
                    fieldBase.duration = null;
                    yield return dialogBox.TypeDialog(returnMessage);
                }
            }
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

        // 특성
        sourceUnit.Unit.Base.Ability?.BeforeAttack(sourceUnit, move);
        sourceUnit.Unit.Base.SecondAbility?.BeforeAttack(sourceUnit, move);

        // 여기서 맞을 친구들 정하기
        // targetUnit.Unit
        List<BattleUnit> targetedUnits = new List<BattleUnit>();

        List<BattleUnit> sourceUnits = sourceUnit.IsPlayerUnit ? playerUnits : enemyUnits;
        List<BattleUnit> targetUnits = targetUnit.IsPlayerUnit ? playerUnits : enemyUnits;

        if (move.Base.Target == MoveTarget.Foe || move.Base.Target == MoveTarget.Team || move.Base.Target == MoveTarget.TeamAnother || move.Base.Target == MoveTarget.Self || move.Base.Target == MoveTarget.Another) targetedUnits.Add(targetUnit);
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
        if (move.Base.Category != MoveCategory.Status)
            sourceUnit.PlayAttackAnimation();
        foreach (var targeted in targetedUnits)
        {
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
                    if (move.Base.Sound != null)
                        AudioManager.i.PlaySfx(move.Base.Sound);

                    yield return new WaitForSeconds(1f);
                    if (move.Base.Category != MoveCategory.Status)
                    {
                        targeted.PlayerHitAnimation();
                        AudioManager.i.PlaySfx(AudioId.Hit);
                    }

                    if (move.Base.Category == MoveCategory.Status)
                    {
                        yield return RunMoveEffect(move.Base.Effects, sourceUnit, targeted.Unit, move.Base.Target);
                    }
                    else
                    {
                        var damageDetails = targeted.Unit.TakeDamage(move, sourceUnit.Unit, Field);
                        damage = damageDetails.Damage;
                        yield return targeted.Hud.WaitForHPUpdate();
                        yield return ShowDamageDetails(damageDetails);
                        typeEffectiveness = damageDetails.TypeEffectiveness;
                    }

                    if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targeted.Unit.HP > 0)
                    {
                        foreach (var secondary in move.Base.Secondaries)
                        {
                            var rnd = UnityEngine.Random.Range(1, 101);
                            if (rnd <= secondary.Chance)
                                yield return RunMoveEffect(secondary, sourceUnit, targeted.Unit, secondary.Target);
                        }
                    }
                    hit = i;
                    if (targeted.Unit.HP <= 0)
                    {
                        bool abilityFocusSash = targeted.Unit.Base.Ability.isFocusSash();
                        bool secondAbilityFocusSash = targeted.Unit.Base.Ability.isFocusSash();
                        if (abilityFocusSash || secondAbilityFocusSash) targeted.Unit.SetHP(1);
                        else break;
                    }
                }
                yield return ShowEffectiveness(typeEffectiveness);
                if (hitTimes > 1)
                    yield return dialogBox.TypeDialog($"{hit}번 공격했다!");

                if ((move.Base.Rebound.x != 0 && move.Base.Rebound.y != 0) || move.Base.Rebound.z != 0)
                {
                    sourceUnit.PlayerHitAnimation();
                    AudioManager.i.PlaySfx(AudioId.Hit);
                    sourceUnit.Unit.ReboundTakeDamage(move.Base.Rebound, damage);
                    yield return targeted.Hud.WaitForHPUpdate();
                    yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}은(는) 반동피해를 입었다!");
                }
                if (move.Base.IsStruggle)
                {
                    sourceUnit.PlayerHitAnimation();
                    AudioManager.i.PlaySfx(AudioId.Hit);
                    sourceUnit.Unit.ReboundTakeDamage(move.Base.Rebound, sourceUnit.Unit.MaxHP / 4);
                    yield return targeted.Hud.WaitForHPUpdate();
                    yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}은(는) 몸부림 쳤다!");
                }

                if (targeted.Unit.HP <= 0)
                {
                    if (sourceUnit.Unit.HP > 0)
                    {
                        sourceUnit.Unit.ApplyBoosts(sourceUnit.Unit.Base.Ability?.OnFinish());
                        sourceUnit.Unit.ApplyBoosts(sourceUnit.Unit.Base.SecondAbility?.OnFinish());
                    }
                    // AudioManager.i.PlaySfx(AudioId.Faint);
                    yield return HandleUnitFainted(targeted);
                }
                else if (sourceUnit.Unit.HP <= 0)
                {
                    yield return HandleUnitFainted(sourceUnit);
                    break;
                }
                else if (move.Base.Category != MoveCategory.Status)
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
        int sourceUnitCount = sourceUnit.IsPlayerUnit ? bs.PlayerUnitsMulti.Count : bs.EnemyUnitsMulti.Count;
        if (sourceUnit.Unit.HP > 0 && move.Base.IsChangeUnit && sourceUnitCount > 2)
        {
            HandleChangeMove(sourceUnit);
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
        faintedUnit.PlayFaintAnimation();

        yield return new WaitForSeconds(2f);
        yield return HandleExpGain(faintedUnit);

        yield return NextStepsAfterFainting(faintedUnit);
    }
    IEnumerator HandleChangeMove(BattleUnit sourceUnit)
    {
        yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}(이)가 전투에서 빠진다!");
        sourceUnit.PlayFaintAnimation();

        yield return new WaitForSeconds(2f);

        yield return NextStepsAfterFainting(sourceUnit);
    }

    IEnumerator HandleExpGain(BattleUnit faintedUnit)
    {
        if (!faintedUnit.IsPlayerUnit)
        {
            bool battleWon = true;
            if (isTrainerBattle)
                battleWon = trainerParty.GetHealthyUnit() == null;

            // TODO : 이거 노래 다 끝날 때 까지 움직이지 못하게 해야 함
            if (battleWon)
                AudioManager.i.PlayMusic(bs.IsTrainerBattle ? bs.TrainerBattleVictoryMusic : bs.WildBattleVictoryMusic, loop: false);
            // exp 획득
            int expYield = faintedUnit.Unit.Base.ExpYield;
            int enemyLevel = faintedUnit.Unit.Level;
            float trainerBonus = (bs.IsTrainerBattle) ? 1.5f : 1f;

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
                                // unitTryingToLearn = playerUnit;
                                yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name}은(는) 잊을 수 없는 스킬을 잊으려 한다!");
                                // yield return ChooseMoveToForget(playerUnit.Unit, newMove.Base);
                                yield return new WaitForSeconds(2f);
                            }
                        }
                        yield return playerUnit.Hud.SetExpSmooth(true);
                    }
                }
            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator NextStepsAfterFainting(BattleUnit faintedUnit)
    {
        List<BattleAction> actions = bs.Actions;
        var actionToRemove = actions.FirstOrDefault(a => a.User == faintedUnit);
        if (actionToRemove != null)
            actionToRemove.IsInvalid = true;
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
                yield return bs.SwitchUnit(bs.PlayerUnits[bs.ActionIndex], PartyState.i.SelectedUnit);
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
                AboutToUseState.i.NewUnit = nextUnit;
                yield return bs.StateMachine.PushAndWait(AboutToUseState.i);
                // else
                // TODO Change Unit
                // StartCoroutine(SendNextTrainerUnit());
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
            Field.Weather.SetCondition(effects.Weather);
            Field.Weather.duration = (int)(5 * source.Base.Ability?.OnField() * source.Base.SecondAbility?.OnField());
            yield return dialogBox.TypeDialog(Field.Weather.condition.StartMessage);
        }

        // 공간 변경
        if (effects.Room != ConditionID.none)
        {
            Field.Room.SetCondition(effects.Room);
            Field.Room.duration = (int)(5 * source.Base.Ability?.OnField() * source.Base.SecondAbility?.OnField());
            yield return dialogBox.TypeDialog(Field.Room.condition.StartMessage);
        }

        // 필드 변경
        if (effects.Field != ConditionID.none)
        {
            Field.field.SetCondition(effects.Field);
            Field.field.duration = (int)(5 * source.Base.Ability?.OnField() * source.Base.SecondAbility?.OnField());
            yield return dialogBox.TypeDialog(Field.field.condition.StartMessage);
        }

        // 리플렉터, 빛의 장막
        if (effects.Reflect != ConditionID.none)
        {
            Field.Reflect.SetCondition(effects.Reflect);
            Field.Reflect.duration = (int)(5 * source.Base.Ability?.OnField() * source.Base.SecondAbility?.OnField());
            yield return dialogBox.TypeDialog(Field.Reflect.condition.StartMessage);
        }
        if (effects.LightScreen != ConditionID.none)
        {
            Field.LightScreen.SetCondition(effects.LightScreen);
            Field.LightScreen.duration = (int)(5 * source.Base.Ability?.OnField() * source.Base.SecondAbility?.OnField());
            yield return dialogBox.TypeDialog(Field.LightScreen.condition.StartMessage);
        }

        // 순풍
        if (effects.Tailwind != ConditionID.none)
        {
            if (sourceUnit.IsPlayerUnit)
            {
                Field.PlayerTailwind.SetCondition(effects.Tailwind);
                Field.PlayerTailwind.duration = (int)(5 * source.Base.Ability?.OnField() * source.Base.SecondAbility?.OnField());
                yield return dialogBox.TypeDialog(Field.PlayerTailwind.condition.StartMessage);
            }
            else
            {
                Field.EnemyTailwind.SetCondition(effects.Tailwind);
                Field.EnemyTailwind.duration = (int)(5 * source.Base.Ability?.OnField() * source.Base.SecondAbility?.OnField());
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
