using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Unit/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] UnitType type;
    [SerializeField] int statIndex; // 어느 스텟의 능력치를 쓰는가? 물리, 특공 이외에도 방어 체력 등...
    // 0 = 일반, 1 공, 2 방, 3 특공, 4 특방, 5 스피드
    [SerializeField] int power;
    [SerializeField] int accuracy;

    [SerializeField] int priority; // 선공기
    [SerializeField] int pp;
    [SerializeField] MoveCategory category;
    [Header("특수한 기술")]
    [SerializeField] bool alwaysHits;
    [SerializeField] int criticalRank;
    // todo 교체랑 첫턴 공격 구현해야함
    [SerializeField] bool isChangeUnit;
    [SerializeField] bool firstTurnChance = false;
    [Header("발버둥")]
    [SerializeField] bool isStruggle = false;

    [Header("확정 스텟/날씨")]
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;
    [SerializeField] Vector2Int hitRange;
    // n / 3
    [SerializeField] Vector3Int rebound;

    [SerializeField] AudioClip sound;
    [Header("확률 스텟/날씨")]
    // secondaries
    [SerializeField] List<Secondaries> secondaries;

    public int GetHitTimes()
    {
        if (hitRange == Vector2Int.zero)
            return 1;
        int hitCount = 1;
        if (hitRange.y == 0)
        {
            hitCount = hitRange.x;
        }
        else
        {
            hitCount = Random.Range(hitRange.x, hitRange.y + 1);
        }
        return hitCount;
    }

    public void AddPriority(int priority)
    {
        this.priority += priority;
    }
    public void SetMoveType(UnitType type)
    {
        this.type = type;
    }

    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public UnitType Type
    {
        get { return type; }
    }
    public int StatIndex
    {
        get { return statIndex; }
    }
    public int Power
    {
        get { return power; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }
    public bool AlwaysHits
    {
        get { return alwaysHits; }
    }
    public int CriticalRank
    {
        get { return criticalRank; }
    }
    public bool FirstTurnChance
    {
        get { return firstTurnChance; }
    }
    public bool IsStruggle
    {
        get { return isStruggle; }
    }
    public bool IsChangeUnit
    {
        get { return isChangeUnit; }
    }
    public int PP
    {
        get { return pp; }
    }
    public MoveCategory Category
    {
        get { return category; }
    }
    public MoveEffects Effects
    {
        get { return effects; }
    }
    public List<Secondaries> Secondaries
    {
        get { return secondaries; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }
    // 우선도
    public int Priority
    {
        get { return priority; }
    }

    public Vector3Int Rebound => rebound;
    public AudioClip Sound => sound;
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;
    [SerializeField] ConditionID weather;
    [SerializeField] ConditionID room;
    [SerializeField] ConditionID field;
    [SerializeField] ConditionID reflect;
    [SerializeField] ConditionID lightScreen;
    [SerializeField] ConditionID tailwind;
    [SerializeField] bool protect = false;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }

    public ConditionID Status
    {
        get { return status; }
    }
    public ConditionID VolatileStatus
    {
        get { return volatileStatus; }
    }
    public ConditionID Weather
    {
        get { return weather; }
    }
    public ConditionID Room
    {
        get { return room; }
    }
    public ConditionID Field
    {
        get { return field; }
    }
    public ConditionID Reflect
    {
        get { return reflect; }
    }
    public ConditionID LightScreen
    {
        get { return lightScreen; }
    }
    public ConditionID Tailwind
    {
        get { return tailwind; }
    }
    public bool Protect
    {
        get { return protect; }
    }
}

[System.Serializable]
public class Secondaries : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance
    {
        get { return chance; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Physical, Special, Status
}

public enum MoveTarget
{
    // Foe: 적 하나
    // FoeAll: 적 전부
    // Team: 팀하나
    // TeamAnother: 또 다른 팀
    // TeamAll: 팀 전부
    // Self: 자기자신
    // Another: 다른녀석
    // AnotherAll: 다른녀석 전부
    // All: 전부
    Foe, FoeAll, Team, TeamAnother, TeamAll, Self, Another, AnotherAll, All
}