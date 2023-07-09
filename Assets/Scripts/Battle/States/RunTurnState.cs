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
        // sort Actions
        IOrderedEnumerable<BattleAction> ac = bs.Actions.OrderByDescending(a => a.Priority);
        List<BattleAction> actions;

        if (bs?.Field?.Room?.ID == ConditionID.trickRoom)
            actions = ac.ThenBy(a => a.User.Unit.Speed).ToList();
        else
            actions = ac.ThenByDescending(a => a.User.Unit.Speed).ToList();

        foreach (var action in actions)
        {
            // 특성
            action.User.Unit.Base.Ability.BeforeRunTurn();
            action.User.Unit.Base.SecondAbility.BeforeRunTurn();
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

        foreach (var action in actions)
            yield return RunAfterTurn(action.User);

        if (!bs.IsbattleOver)
        {
            bs.ResetActions();
            bs.StateMachine.ChangeState(ActionSelectionState.i);
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
        sourceUnit.Unit.Base.Ability.BeforeAttack();
        sourceUnit.Unit.Base.SecondAbility.BeforeAttack();

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
            if (CheckIfMoveHits(move, sourceUnit.Unit, targeted.Unit))
            {
                int hitTimes = move.Base.GetHitTimes();
                float typeEffectiveness = 1f;
                int hit = 1;
                for (int i = 1; i <= hitTimes; i++)
                {
                    if (move.Base.Sound != null)
                        AudioManager.i.PlaySfx(move.Base.Sound);

                    yield return new WaitForSeconds(1f);
                    if (move.Base.Category != MoveCategory.Status)
                        targeted.PlayerHitAnimation();
                    AudioManager.i.PlaySfx(AudioId.Hit);

                    if (move.Base.Category == MoveCategory.Status)
                    {
                        yield return RunMoveEffect(move.Base.Effects, sourceUnit.Unit, targeted.Unit, move.Base.Target);
                    }
                    else
                    {
                        var damageDetails = targeted.Unit.TakeDamage(move, sourceUnit.Unit, Field.Weather);
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
                                yield return RunMoveEffect(secondary, sourceUnit.Unit, targeted.Unit, secondary.Target);
                        }
                    }
                    hit = i;
                    if (targeted.Unit.HP <= 0)
                        break;
                }
                yield return ShowEffectiveness(typeEffectiveness);
                if (hitTimes > 1)
                    yield return dialogBox.TypeDialog($"{hit}번 공격했다!");

                if (targeted.Unit.HP <= 0)
                {
                    // AudioManager.i.PlaySfx(AudioId.Faint);
                    yield return HandleUnitFainted(targeted);
                }
                else
                {
                    // 특성
                    sourceUnit.Unit.Base.Ability.AfterAttack();
                    sourceUnit.Unit.Base.SecondAbility.AfterAttack();
                    targeted.Unit.Base.Ability.AfterAttack();
                    targeted.Unit.Base.SecondAbility.AfterAttack();
                }
            }
            else
            {
                yield return dialogBox.TypeDialog($"{sourceUnit.Unit.Base.Name}의 공격이 빗나갔다!");
            }
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

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (bs.IsbattleOver) yield break;
        // 특성
        sourceUnit.Unit.Base.Ability.AfterRunTurn();
        sourceUnit.Unit.Base.SecondAbility.AfterRunTurn();
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

    IEnumerator RunMoveEffect(MoveEffects effects, Unit source, Unit target, MoveTarget moveTarget)
    {
        // stat 증가 TODO 여러명 때리는 경우에 셀프 스텟 증가하면 2번 3번 중첩되는 문제가 생김
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

        // 공간 변경
        if (effects.Room != ConditionID.none)
        {
            Field.SetRoom(effects.Room);
            Field.RoomDuration = 5;
            yield return dialogBox.TypeDialog(Field.Room.StartMessage);
        }

        // 필드 변경
        if (effects.Field != ConditionID.none)
        {
            Field.SetField(effects.Field);
            Field.FieldDuration = 5;
            yield return dialogBox.TypeDialog(Field.field.StartMessage);
        }

        // dialog 박스 변경
        // playerUnit.Hud.UpdateStatus();

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
