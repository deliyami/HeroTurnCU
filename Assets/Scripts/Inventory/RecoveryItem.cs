using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecoveryItem", menuName = "Items/Create new recovery items")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;
    
    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;

    [Header("Status Conditions")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoveryAllStatus;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(Unit unit)
    {
        // 부활
        if (revive || maxRevive)
        {
            if (unit.HP <= 0)
            {
                unit.IncreaseHP(revive?unit.MaxHP / 2:unit.MaxHP);

                unit.CureStatus();
            }
        }
        // 부활 있는 것이 아닌 이상 쓸 필요가 없음, 근데 복합 회복 달아놓을거라서 상관없음
        if (unit.HP == 0)
            return false;
        // 체력 회복
        if (restoreMaxHP || hpAmount > 0)
        {
            if (unit.HP == unit.MaxHP || unit.HP <= 0)
                return false;

            if (restoreMaxHP)
                unit.IncreaseHP(unit.MaxHP);
            else
                unit.IncreaseHP(hpAmount);
        }
        // 상태이상 회복
        if (recoveryAllStatus || status != ConditionID.none)
        {
            if (unit.Status == null && unit.VolatileStatus == null)
            {

            }
            else
            {
                if (recoveryAllStatus)
                {
                    unit.CureStatus();
                    unit.CureVolatileStatus();
                }
                else
                {
                    if (unit.Status.ID == status)
                        unit.CureStatus();
                    else if (unit.VolatileStatus.ID == status)
                        unit.CureVolatileStatus();
                    else
                        return false;
                }
            }
        }
        // 기술 회복
        if (restoreMaxPP)
        {
            unit.Moves.ForEach(m => m.IncreasePP(m.Base.PP));
        }
        else if (ppAmount > 0)
        {
            unit.Moves.ForEach(m => m.IncreasePP(ppAmount));
        }
        return true;
    }
}
