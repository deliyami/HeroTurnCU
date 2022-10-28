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

public class TypeChart
{
    static float[][] chart =
    {
        //                    없음, 불꽃,    물,   풀, 번개, 얼음,  용기,   독,   흙, 하늘,  마법, 바람,  바위, 유령,   용, 악마, 강철, 이상함
        /*없음*/new float[] {   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   0f,   1f,   1f, 0.5f,   1f},
        /*불꽃*/new float[] {   1f, 0.5f, 0.5f,   2f,   1f,   2f,   1f,   1f,   1f,   1f,   1f,   2f, 0.5f,   1f, 0.5f,   1f,   2f,   1f},
        /*풀  */new float[] {   1f,   1f, 0.5f, 0.5f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f,   2f,   1f, 0.5f,   1f,   1f,   1f},
        /*물  */new float[] {   1f, 0.5f,   2f, 0.5f,   1f,   1f,   1f, 0.5f,   2f, 0.5f,   1f, 0.5f,   2f,   1f, 0.5f,   1f, 0.5f,   1f},
        /*번개*/new float[] {   1f,   1f,   2f, 0.5f,   1f,   1f,   1f,   1f,   0f,   2f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   1f},
        /*얼음*/new float[] {   1f, 0.5f, 0.5f,   2f,   1f, 0.5f,   1f,   1f,   2f,   2f,   1f,   1f,   1f,   1f,   2f,   1f, 0.5f,   1f},
        /*용기*/new float[] {   2f,   1f,   1f,   1f,   1f,   2f,   1f, 0.5f,   1f, 0.5f, 0.5f, 0.5f,   2f,   0f,   1f,   2f,   2f, 0.5f},
        /*독  */new float[] {   1f,   1f,   1f,   2f,   1f,   1f,   1f, 0.5f, 0.5f,   1f,   1f,   1f, 0.5f, 0.5f,   1f,   1f,   0f,   2f},
        /*흙  */new float[] {   1f,   2f,   1f,   1f,   2f,   1f,   1f,   2f,   1f,   0f,   1f, 0.5f,   2f,   1f,   1f,   1f,   2f,   1f},
        /*하늘*/new float[] {   1f,   1f,   1f,   2f, 0.5f,   1f,   2f,   1f,   1f,   1f,   1f,   2f, 0.5f,   1f,   1f,   1f, 0.5f,   1f},
        /*마법*/new float[] {   1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   1f,   1f, 0.5f,   1f,   1f,   1f,   1f,   0f, 0.5f,   1f},
        /*바람*/new float[] {   1f, 0.5f,   1f,   2f,   1f,   1f, 0.5f, 0.5f,   1f, 0.5f,   2f,   1f,   1f, 0.5f,   1f,   2f, 0.5f, 0.5f},
        /*바위*/new float[] {   1f,   2f,   1f,   1f,   1f,   2f, 0.5f,   1f, 0.5f,   2f,   1f,   2f,   1f,   1f,   1f,   1f, 0.5f,   1f},
        /*유령*/new float[] {   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f, 0.5f,   1f,   1f},
        /*용  */new float[] {   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f, 0.5f,   0f},
        /*악마*/new float[] {   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f, 0.5f,   1f, 0.5f},
        /*강철*/new float[] {   1f, 0.5f, 0.5f,   1f, 0.5f,   2f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f, 0.5f,   2f},
        /*이상*/new float[] {   1f, 0.5f,   1f,   1f,   1f,   1f,   2f, 0.5f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f, 0.5f,   1f},
    };

    public static float GetEffectiveness(UnitType attackType, UnitType defenseType)
    {
        if (attackType == UnitType.None || defenseType == UnitType.None)
            return 1;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}