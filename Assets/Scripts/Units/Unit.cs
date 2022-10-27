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
}
