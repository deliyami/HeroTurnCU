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
    [SerializeField] bool alwaysHits;
    [SerializeField] int pp;
    [SerializeField] int priority; // 선공기
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<Secondaries> secondaries;
    [SerializeField] MoveTarget target;
    [SerializeField] Vector2Int hitRange;

    [SerializeField] AudioClip sound;

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