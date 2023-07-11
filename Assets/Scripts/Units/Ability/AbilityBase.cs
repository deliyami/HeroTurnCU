using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityBase : MonoBehaviour
{
    [SerializeField] string name; // 특성
    [TextArea]
    [SerializeField] string description;
    [SerializeField] string activiteDescription; // 발동시 대답

    public bool isActivatableAbiility = true;

    // 턴 시작 전, 교체 후에, 자신, 아군, 적, 적전체, 날씨, 필드, ex) 위협, 모래날림
    public virtual IEnumerator BeforeRunTurn(Field field, Unit sourceUnit) { yield return null; }
    public virtual int BeforeTurnChange(BattleUnit sourceUnit) { return 0; }

    // 공격 전에 스텟, 타입 변화, ex) 변환자재 등
    public virtual void BeforeAttack(BattleUnit attacker, Move move) { }
    // 피격 전에 스텟, 타입 변화, 아니면 회피 ex) 부유
    public virtual bool BeforeDefense(BattleUnit defender, Move move) { return true; }

    // 공격시 데미지 증가, ex) 테크니션
    public virtual float OnAttack(Move move) { return 1; }
    // 피격시 데미지 감소 ex) 멀티스케일, 옹골참
    public virtual float OnDefense(Unit defender, float type, MoveCategory moveCategory) { return 1; }
    public virtual bool isFocusSash() { return false; }
    // 리플렉터, 날씨, 필드 등의 실행시 유지되는 턴 증가, onattack하려고 했는데 중복이라서 이쪽으로 넘김
    public virtual float OnField() { return 1; }

    public virtual List<StatBoost> OnFinish() { return new List<StatBoost>(); }

    // 공격시, 피격시 상태이상 추가, ex) 불꽃몸, 왕의 징표석
    // return 0 = 상태이상, 1 = 불완전한 상태이상, 2 = Stat, 3 = boosts stat
    public virtual (ConditionID, ConditionID, Stat, int, MoveTarget) AfterAttack(BattleUnit attacker, BattleUnit defender, Move move) { return (ConditionID.none, ConditionID.none, Stat.Attack, 0, MoveTarget.Foe); }
    public virtual (ConditionID, ConditionID, Stat, int, MoveTarget) AfterDefense(BattleUnit attacker, BattleUnit defender, Move move) { return (ConditionID.none, ConditionID.none, Stat.Attack, 0, MoveTarget.Foe); }

    // 모든 턴이 끝나고 발동,
    public virtual void AfterRunTurn(BattleUnit sourceUnit) { }

    public string Name => name;
    public string Description => description;
    public string ActiviteDescription => activiteDescription;
}
