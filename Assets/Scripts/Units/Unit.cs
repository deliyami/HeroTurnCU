using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public UnitBase Base { get; set; }
    public int Level { get; set; }

    // 개체치 31 {HP, attack, defense, spAttack, spDefense, speed}
    public int[] tribe { get; set; }
    // 노력치 0~252 {HP, attack, defense, spAttack, spDefense, speed} /4해서 더해야 하는데... 적혀있네
    public int[] effort { get; set; } 
    // 성격 int[] = {상승 스텟 index, 하락 스텟 index}
    public int[] personality { get; set; }

    public int HP { get; set; }

    public List<Move> moves { get; set; }

    public Unit(UnitBase uBase, int uLevel, int[] uTribe, int[] uEffort, int[] uPersonality)
    {
        Base = uBase;
        Level = uLevel;
        tribe = uTribe;
        effort = uEffort;
        personality = uPersonality;

        HP = MaxHP;

        moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                moves.Add(new Move(move.Base));
            
            if (moves.Count >= 4)
                break;
        }
    }

    private int CalculateStat(int stat, int statIndex) {
        int HP = statIndex == 0 ? (10 + Level) : 5;
        float personalityData = (1.0f + (personality[0] == statIndex ? 0.1f : 0) - (personality[1] == statIndex ? 0.1f : 0));
        return (int)((((stat * 2) + tribe[statIndex] + (effort[statIndex] / 4)) * Level / 100 + HP) * personalityData);
    }

    public int MaxHP {
        get { return CalculateStat(Base.MaxHP, 0);}
    }
    public int Attack {
        get { return CalculateStat(Base.Attack, 0);}
    }
    public int Defense {
        get { return CalculateStat(Base.Defense, 0);}
    }
    public int SpAttack {
        get { return CalculateStat(Base.SpAttack, 0);}
    }
    public int SpDefense {
        get { return CalculateStat(Base.SpDefense, 0);}
    }
    public int Speed {
        get { return CalculateStat(Base.Speed, 0);}
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

        // 랜덤수 × 타입상성1 × 타입상성2 × [[급소]] × Mod2) × 자속보정 × Mod3)
        float modifiers = Random.Range(85f, 100f) / 100f * typeEffectiveness * critical;
        // 데미지 = (레벨 × 2 + 10) ÷ 250
        float a = (2 * attacker.Level + 10) / 250f;
        // 위력 × (특수공격 ÷ 특수방어)) × Mod1) + 2)
        // mod1 mod2 mod3다 해둬야 됨...
        float d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);
        Debug.Log($"Name: {Base.Name}");
        Debug.Log($"HP: {HP}");
        Debug.Log($"Dmg: {damage}");
        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }
    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}