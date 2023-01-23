using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Unit/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] new string name;

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

    public string Name {
        get { return name; }
    }
    public string Description {
        get { return description; }
    }
    public UnitType Type {
        get { return type; }
    }
    public int StatIndex {
        get { return statIndex; }
    }
    public int Power {
        get { return power; }
    }
    public int Accuracy {
        get { return accuracy; }
    }
    public bool AlwaysHits {
        get { return alwaysHits; }
    }
    public int PP {
        get { return pp; }
    }
    public MoveCategory Category {
        get { return category; }
    }
    public MoveEffects Effects {
        get { return effects; }
    }
    public List<Secondaries> Secondaries {
        get { return secondaries; }
    }
    public MoveTarget Target {
        get { return target; }
    }
    // 우선도
    public int Priority {
        get { return priority; }
    }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;

    public List<StatBoost> Boosts {
        get { return boosts; }
    }

    public ConditionID Status {
        get { return status; }
    }
    public ConditionID VolatileStatus{
        get { return volatileStatus; }
    }
}

[System.Serializable]
public class Secondaries : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance {
        get {return chance; }
    }

    public MoveTarget Target {
        get {return target; }
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
    Foe, Self
}