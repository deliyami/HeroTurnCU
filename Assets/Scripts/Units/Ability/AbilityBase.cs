using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBase : MonoBehaviour
{
    [SerializeField] string name; // 특성
    [TextArea]
    [SerializeField] string description;

    public bool isActivatableAbiility = true;

    // 턴 시작 전, 교체 후에, 자신, 아군, 적, 적전체, 날씨, 필드, ex) 위협, 모래날림
    public virtual void BeforeRunTurn() { }

    // 공격 전에 스텟, 타입 변화, ex) 변환자재 등
    public virtual void BeforeAttack() { }

    // 공격시 데미지 증가, ex) 테크니컬
    public virtual int OnAttack() { return 1; }

    // 공격시, 피격시 상태이상 추가, ex) 불꽃몸, 왕의 징표석
    public virtual void AfterAttack() { }

    // 모든 턴이 끝나고 발동,
    public virtual void AfterRunTurn() { }

    public string Name => name;
    public string Description => description;
}
