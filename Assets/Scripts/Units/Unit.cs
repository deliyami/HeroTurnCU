using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Unit
{
    [SerializeField] UnitBase _base;
    [SerializeField] int level;

    public Unit(UnitBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;

        // Init();
    }

    // 개체치 31 {HP, attack, defense, spAttack, spDefense, speed}
    [SerializeField] int[] tribe;
    // 노력치 0~252 {HP, attack, defense, spAttack, spDefense, speed} /4해서 더해야 하는데... 적혀있네
    [SerializeField] int[] effort;
    // 성격 int[] = {상승 스텟 index, 하락 스텟 index}
    [SerializeField] int[] personality;
    public UnitBase Base { 
        get {
            return _base;
        }
    }
    public int Level { 
        get {
            return level;
        } 
    }

    public int[] Tribe{
        get {
            return tribe;
        }
    }
    public int[] Effort{
        get {
            return effort;
        }
    }
    public int[] Personality{
        get {
            return personality;
        }
    }
    public int Exp { get; set; }
    public int HP { get; set; }
    public List<Move> moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }

    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }


    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public bool HPChanged { get; set; }
    public event System.Action OnStatusChanged;
    public event System.Action OnHPChanged;

    public void Init()
    {
        // Base = uBase;
        // Level = uLevel;
        
        moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                moves.Add(new Move(move.Base));
            
            if (moves.Count >= UnitBase.MaxNumOfMoves)
                break;
        }

        Exp = Base.GetExpForLevel(level);

        // tribe = uTribe;
        // effort = uEffort;
        // personality = uPersonality;

        CalculateStats();
        HP = MaxHP;

        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    public Unit(UnitSaveData saveData)
    {
        _base = UnitDB.GetObjectByName(saveData.name);
        HP = saveData.hp;
        level = saveData.level;
        Exp = saveData.exp;
        tribe = saveData.tribe;
        effort = saveData.effort;
        personality = saveData.personality;

        if (saveData.statusId != null)
            Status = ConditionDB.Conditions[saveData.statusId.Value];
        else
            Status = null;

        moves = saveData.moves.Select(s => new Move(s)).ToList();

        CalculateStats();
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        VolatileStatus = null;
    }

    public UnitSaveData GetSaveData()
    {
        var saveData = new UnitSaveData()
        {
            name = Base.name,
            hp = HP,
            level = Level,
            exp = Exp,
            tribe = Tribe,
            effort = Effort,
            personality = Personality,
            statusId = Status?.ID,
            moves = Moves.Select(m => m.GetSaveData()).ToList(),
        };
        return saveData;
    }

    void CalculateStats()
    {
        // 여기 주의 할 것...
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, CalculateBaseStats(Base.Attack, 1));
        Stats.Add(Stat.Defense, CalculateBaseStats(Base.Defense, 2));
        Stats.Add(Stat.SpAttack, CalculateBaseStats(Base.SpAttack, 3));
        Stats.Add(Stat.SpDefense, CalculateBaseStats(Base.SpDefense, 4));
        Stats.Add(Stat.Speed, CalculateBaseStats(Base.Speed, 5));

        int oldMaxHP = MaxHP;
        MaxHP = CalculateBaseStats(Base.MaxHP, 0);

        if (oldMaxHP != 0)
        {
            HP = Mathf.Clamp(HP + MaxHP - oldMaxHP, 0, MaxHP);
        }
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0},
        };
    }

    private int CalculateBaseStats(int stat, int statIndex) {
        int staticValue = statIndex == 0 ? (10 + Level) : 5;
        float personalityData = (1.0f + (Personality[0] == statIndex ? 0.1f : 0) - (Personality[1] == statIndex ? 0.1f : 0));
        return (int)((((stat * 2) + Tribe[statIndex] + (Effort[statIndex] / 4)) * Level / 100 + staticValue) * personalityData);
    }

    int GetStat(Stat stat)
    {  
        int statVal = Stats[stat];

        // TODO: Apply stat boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };


        // statVal = Mathf.FloorToInt(statVal * (2f + max(0, boost)) / (2f - min(0, boost)))
        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            string statName = "";
            
            switch (stat)
            {
                case Stat.Attack:
                    statName = "공격";
                    break;
                case Stat.Defense:
                    statName = "방어";
                    break;
                case Stat.SpAttack:
                    statName = "마법력";
                    break;
                case Stat.SpDefense:
                    statName = "저항력";
                    break;
                case Stat.Speed:
                    statName = "스피드";
                    break;
                case Stat.Accuracy:
                    statName = "명중율";
                    break;
                case Stat.Evasion:
                    statName = "회피율";
                    break;
            }

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}의 {statName}은(는) 증가했다!");
            else
                StatusChanges.Enqueue($"{Base.Name}의 {statName}은(는) 감소했다!");

            Debug.Log($"{Base.Name}의 {stat}는 {StatBoosts[stat]}으로 됨...");
        }
    }
    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(level + 1))
        {
            level++;
            CalculateStats();
            return true;
        }
        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrLevel()
    {
        LearnableMove returnMove = null;
        foreach(LearnableMove lm in Base.LearnableMoves)
        {
            if (lm.Level == level) returnMove = lm;
        }
        return returnMove;
    }
    public void LearnMove(MoveBase moveToLearn)
    {
        if (Moves.Count > UnitBase.MaxNumOfMoves)
            return;
        Moves.Add(new Move(moveToLearn));
    }
    public bool HasMove(MoveBase moveToCheck)
    {
        return Moves.Count(m => m.Base == moveToCheck) > 0;
    }
    public Evolution CheckForEvolution()
    {
        return Base.Evolutions.FirstOrDefault(e => e.RequiredLevel <= level);
    }
    public Evolution CheckForEvolution(ItemBase item)
    {
        return Base.Evolutions.FirstOrDefault(e => e.RequireItem == item);
    }
    public void Evolve(Evolution evolution)
    {
        _base = evolution.EvolvesInto;
        CalculateStats();
    }
    public void Heal()
    {
        HP = MaxHP;
        foreach(var m in moves)
        {
            m.PP = m.Base.PP;
        }
        CureStatus();
        OnHPChanged?.Invoke();
    }
    public int MaxHP { get; private set; }
    public int Attack {
        get { return GetStat(Stat.Attack);}
    }
    public int Defense {
        get { return GetStat(Stat.Defense);}
    }
    public int SpAttack {
        get { return GetStat(Stat.SpAttack);}
    }
    public int SpDefense {
        get { return GetStat(Stat.SpDefense);}
    }
    public int Speed {
        get { return GetStat(Stat.Speed);}
    }
    public List<Move> Moves {
        get { return moves; }
    }

    // virtual public int SettingRealStat() {
    //     return (((STAT * 2) + tribe + (effort / 4)) * level / 100 + 5) * personality;
    // }
    public DamageDetails TakeDamage(Move move, Unit attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;
        float typeEffectiveness = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);
        critical = 2f;
        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = typeEffectiveness,
            Critical = critical,
            Fainted = false,
        };

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;

        // 랜덤수 × 타입상성1 × 타입상성2 × [[급소]] × Mod2) × 자속보정 × Mod3)
        float modifiers = Random.Range(85f, 100f) / 100f * typeEffectiveness * critical;
        // 데미지 = (레벨 × 2 + 10) ÷ 250
        float a = (2 * attacker.Level + 10) / 250f;
        // 위력 × (특수공격 ÷ 특수방어)) × Mod1) + 2)
        // mod1 mod2 mod3다 해둬야 됨...
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);
        Debug.Log($"Name: {Base.Name}, HP: {HP}");
        
        DecreaseHP(damage);

        return damageDetails;
    }
    public void IncreaseHP(int amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, MaxHP);
        OnHPChanged?.Invoke();
    }
    public void DecreaseHP(int damage)
    {
        Debug.Log($"Dmg: {damage}");
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        OnHPChanged?.Invoke();
    }

    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null) return;
        
        Status = ConditionDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name}은(는) {Status.StartMessage}!!");

        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;

        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null) return;
        
        VolatileStatus = ConditionDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name}은(는) {VolatileStatus.StartMessage}!!");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    {
        var movesWithPP = new List<Move>();
        foreach (Move m in Moves)
        {
            if(m.PP > 0) movesWithPP.Add(m);
        }
        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public bool OnBeforeMove()
    {
        bool cnaRunningTurn = true;
        if (Status?.OnBeforeMove != null)
        {
            if(!Status.OnBeforeMove(this))
                cnaRunningTurn = false;
        }
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if(!VolatileStatus.OnBeforeMove(this))
                cnaRunningTurn = false;
        }
        return cnaRunningTurn;
    }
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        CureVolatileStatus();
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}

[System.Serializable]
public class UnitSaveData
{
    public string name;
    public int hp;
    public int level;
    public int exp;
    public int[] tribe;
    public int[] effort;
    public int[] personality;
    public ConditionID? statusId;
    public List<MoveSaveData> moves;
}