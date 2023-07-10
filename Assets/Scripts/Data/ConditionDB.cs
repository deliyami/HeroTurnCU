using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConditionDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionID = kvp.Key;
            var condition = kvp.Value;

            condition.ID = conditionID;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "중독",
                StartMessage = "은(는) 중독되었다!",
                OnAfterTurn = (Unit unit) =>
                {
                    unit.DecreaseHP(unit.MaxHP / 8);
                    unit.StatusChanges.Enqueue($"{unit.Base.Name}은(는) 중독 데미지를 입고있다!");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "화상",
                StartMessage = "은(는) 화상을 입었다!",
                OnAfterTurn = (Unit unit) =>
                {
                    unit.DecreaseHP(unit.MaxHP / 16);
                    unit.StatusChanges.Enqueue($"{unit.Base.Name}은(는) 화상 데미지를 입고있다!");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "마비",
                StartMessage = "은(는) 마비에 걸렸다!",
                OnBeforeMove = (Unit unit) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        unit.StatusChanges.Enqueue($"{unit.Base.Name}은(는) 몸이 움직이지 않는다!");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "냉동",
                StartMessage = "은(는) 얼어붙었다!",
                OnBeforeMove = (Unit unit) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        unit.StatusChanges.Enqueue($"{unit.Base.Name}은(는) 녹았다!");
                        unit.CureStatus();
                        return true;
                    }
                    unit.StatusChanges.Enqueue($"{unit.Base.Name}은(는) 얼어서 움직이지 못한다!");
                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "수면",
                StartMessage = "은(는) 골아떨어졌다!",
                OnStart = (Unit unit) =>
                {
                    unit.StatusTime = 0;
                },
                OnBeforeMove = (Unit unit) =>
                {
                    int awakeValue = Random.Range(-1 + Mathf.FloorToInt(unit.StatusTime / 2f),
                                                    1 + Mathf.RoundToInt(unit.StatusTime / 2f)); // 1개 늘리고 최대를 1개 뺴고 두개를 번갈아가면서 하네
                    // unit.StatusTime = 0, -1 이상 1 미만, -1, 0, 1이상 확률 0%
                    // unit.StatusTime = 1, -1 이상 2 미만, -1, 0, 1, 1이상 확률 33%
                    // unit.StatusTime = 2, 0 이상 2 미만, 0, 1, 1이상 확률 50%
                    // unit.StatusTime = 3, 0 이상 3 미만, 0, 1, 2, 1이상 확률 66%
                    // unit.StatusTime = 4, 1 이상 3 미만, 1, 2, 1이상 확률 100%
                    unit.StatusTime++;
                    if (awakeValue >= 1)
                    {
                        unit.StatusChanges.Enqueue($"{unit.Base.Name}은(는) 눈을 떴다!");
                        unit.CureStatus();
                        return true;
                    }
                    unit.StatusChanges.Enqueue($"{unit.Base.Name}은(는) 잠잔다!");
                    return false;
                }
            }
        },

        // volatile 특수한 상태이상
        {
            ConditionID.confusion,
            new Condition()
            {
                Name = "혼란",
                StartMessage = "은(는) 혼란스럽다!",
                OnStart = (Unit unit) =>
                {
                    unit.VolatileStatusTime = Random.Range(1, 5);
                },
                OnBeforeMove = (Unit unit) =>
                {
                    if (unit.VolatileStatusTime <= 0)
                    {
                        unit.StatusChanges.Enqueue($"{unit.Base.Name}은(는) 정신을 차렸다!");
                        unit.CureVolatileStatus();
                        return true;
                    }
                    unit.VolatileStatusTime--;
                    int value = Random.Range(1, 4);
                    if (value == 1)
                    {
                        unit.StatusChanges.Enqueue($"{unit.Base.Name}은(는) 정신을 못차린다!");
                        unit.DecreaseHP(unit.MaxHP / 8);
                        unit.StatusChanges.Enqueue($"{unit.Base.Name}은(는) 영문도 모른채 자신을 공격했다!");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.flinch,
            new Condition()
            {
                Name = "풀죽음",
                StartMessage = "은(는) 풀이죽었다!",
                OnStart = (Unit unit) =>
                {
                    unit.VolatileStatusTime = Random.Range(1, 1);
                },
                OnBeforeMove = (Unit unit) =>
                {
                    unit.StatusChanges.Enqueue($"{unit.Base.Name}은(는) 풀이죽어 움직이지 못한다!");
                    unit.CureVolatileStatus();
                    return false;
                }
            }
        },
        // 날씨
        {
            ConditionID.sunny,
            new Condition()
            {
                Name = "쾌청",
                StartMessage = "햇살이 강하다",
                EffectMessage = "햇살이 내리쬔다",
                OnDamageModify = (Unit source, Unit target, Move move) =>
                {
                    if (move.Base.Type == UnitType.Fire)
                        return 1.5f;
                    else if (move.Base.Type == UnitType.Water)
                        return 0.5f;
                    return 1f;
                }
            }
        },
        {
            ConditionID.rain,
            new Condition()
            {
                Name = "비",
                StartMessage = "비가 내리기 시작했다",
                EffectMessage = "비가 내린다",
                OnDamageModify = (Unit source, Unit target, Move move) =>
                {
                    if (move.Base.Type == UnitType.Water)
                        return 1.5f;
                    else if (move.Base.Type == UnitType.Fire)
                        return 0.5f;
                    return 1f;
                }
            }
        },
        {
            ConditionID.sandstorm,
            new Condition()
            {
                Name = "모래바람",
                StartMessage = "모래가 흩날리기 시작한다",
                EffectMessage = "세찬 모래바람이 휘날린다",
                OnWeather = (Unit unit) =>
                {
                    List<UnitType> checkType = new List<UnitType>(){UnitType.Soil, UnitType.Stone, UnitType.Steel};
                    if (!(checkType.Any(type => type == unit.Base.Type1) || checkType.Any(type => type == unit.Base.Type2)))
                    {
                        unit.DecreaseHP(Mathf.RoundToInt((float)unit.MaxHP / 16f));
                        unit.StatusChanges.Enqueue($"모래바람이 {unit.Base.Name}을(를) 덮친다!");
                    }
                }
            }
        },
        {
            ConditionID.trickRoom,
            new Condition()
            {
                Name = "트릭룸",
                StartMessage = "트릭룸 시작",
                EffectMessage = "트릭룸 발동중"
            }
        },
        {
            ConditionID.gravity,
            new Condition()
            {
                Name = "중력",
                StartMessage = "중력 시작",
                EffectMessage = "중력 발동중"
            }
        },
        {
            ConditionID.wonderRoom,
            new Condition()
            {
                Name = "원더룸",
                StartMessage = "원더룸 시작",
                EffectMessage = "원더룸 발동중"
            }
        },
        {
            ConditionID.magicRoom,
            new Condition()
            {
                Name = "매직룸",
                StartMessage = "매직룸 시작",
                EffectMessage = "매직룸 발동중"
            }
        },
        {
            ConditionID.grassyTerrain,
            new Condition()
            {
                Name = "그래스필드",
                StartMessage = "그래스필드 시작",
                EffectMessage = "그래스필드 발동중"
            }
        },
        {
            ConditionID.mistyTerrain,
            new Condition()
            {
                Name = "미스트필드",
                StartMessage = "미스트필드 시작",
                EffectMessage = "미스트필드 발동중"
            }
        },
        {
            ConditionID.psychicTerrain,
            new Condition()
            {
                Name = "사이코필드",
                StartMessage = "사이코필드 시작",
                EffectMessage = "사이코필드 발동중"
            }
        },
        {
            ConditionID.electricTerrain,
            new Condition()
            {
                Name = "일렉트릭필드",
                StartMessage = "일렉트릭필드 시작",
                EffectMessage = "일렉트릭필드 발동중"
            }
        },
        {
            ConditionID.reflect,
            new Condition()
            {
                Name = "리플렉터",
                StartMessage = "리플렉터 시작",
                EffectMessage = "리플렉터 발동중"
            }
        },
        {
            ConditionID.lightScreen,
            new Condition()
            {
                Name = "빛의 장막",
                StartMessage = "빛의 장막 시작",
                EffectMessage = "빛의 장막 발동중"
            }
        },
    };

    public static float GetStatusBonus(Condition condition)
    {
        // 독, 마비, 화상 상태에선 x1.5(3세대)
        // 수면 및 얼음 상태에선 ×2.5(5세대 이후)
        if (condition == null)
            return 1f;
        else if (condition.ID == ConditionID.slp || condition.ID == ConditionID.frz)
            return 2.5f;
        else if (condition.ID == ConditionID.psn || condition.ID == ConditionID.par || condition.ID == ConditionID.brn)
            return 1.5f;
        return 1f;
    }
}

public enum ConditionID
{
    /* 
        psn = 중독
        brn = 화상
        slp = 수면
        par = 마비
        frz = 냉동
        dpsn = 맹독 <= 구현 할 수 있을지 모르겠음
    */
    none, psn, brn, slp, par, frz, dpsn,
    confusion, flinch,
    sunny, rain, sandstorm, hail,
    trickRoom, gravity, wonderRoom, magicRoom,
    grassyTerrain, mistyTerrain, psychicTerrain, electricTerrain,
    reflect, lightScreen
}