using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit/Create new unit")]
public class UnitBase : ScriptableObject
{
    [SerializeField] new string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] UnitType type1;
    [SerializeField] UnitType type2;

    // Base Stats
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;
    [SerializeField] string ability; // 특성
    
    [SerializeField] List<LearnableMove> learnableMoves;

    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }
    public Sprite FrontSprite {
        get { return frontSprite; }
    }
    public Sprite BackSprite {
        get { return backSprite; }
    }
    public UnitType Type1 {
        get { return type1; }
    }
    public UnitType Type2 {
        get { return type2; }
    }
    public int MaxHP {
        get { return maxHP; }
    }
    public int Attack {
        get { return attack; }
    }
    public int Defense {
        get { return defense; }
    }
    public int SpAttack {
        get { return spAttack; }
    }
    public int SpDefense {
        get { return spDefense; }
    }
    public int Speed {
        get { return speed; }
    }
    public string Ability { 
        get { return ability; }
    }
    public List<LearnableMove> LearnableMoves{
        get { return learnableMoves; }
    }
}

[System.Serializable]

public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base { get { return moveBase; }}
    public int Level { get { return level; }}
}

// public enum UnitType
// {
//     None,
//     Normal,
//     Fire,
//     Water,
//     Grass,
//     Electric,
//     Ice,
//     Courage,
//     Poison,
//     Soil,
//     Sky,
//     Psycho,
//     Wind,
//     Stone,
//     Ghost,
//     Dragon,
//     Devil,
//     Steel,
//     Strange
// }

public enum UnitType
{
    None,
    없음,
    불꽃,
    풀,
    물,
    번개,
    얼음,
    용기,
    독,
    흙,
    하늘,
    마법,
    바람,
    바위,
    유령,
    용,
    악마,
    강철,
    이상함
}