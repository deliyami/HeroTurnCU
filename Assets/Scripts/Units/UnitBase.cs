using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit/Create new unit")]
public class UnitBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite smallSprite;
    [SerializeField] Sprite portraitSprite;

    [SerializeField] UnitType type1;
    [SerializeField] UnitType type2;

    // Base Stats
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;
    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;

    [SerializeField] int catchRate = 255;

    [SerializeField] string ability; // 특성
    // 개체치 31 {HP, attack, defense, spAttack, spDefense, speed}
    [SerializeField] int[] individual = new int[6];
    // // 노력치 0~252 {HP, attack, defense, spAttack, spDefense, speed} /4해서 더해야 하는데... 적혀있네
    [SerializeField] int[] effort = new int[6];
    // // 성격 int[] = {상승 스텟 index, 하락 스텟 index}
    [SerializeField] int[] personality = new int[2];

    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] List<MoveBase> learnableByItems;
    [SerializeField] List<Evolution> evolutions;
    public static int MaxNumOfMoves { get; set; } = 4;

    public int GetExpForLevel(int level)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * (Mathf.FloorToInt(Mathf.Pow(level, 3))) / 5;
        }
        else if (growthRate == GrowthRate.MediumFast)
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3));
        }
        return 100;
    }

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }
    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }
    public Sprite BackSprite
    {
        get { return backSprite; }
    }
    public Sprite SmallSprite
    {
        get { return smallSprite; }
    }
    public Sprite PortraitSprite => portraitSprite;
    public UnitType Type1
    {
        get { return type1; }
    }
    public UnitType Type2
    {
        get { return type2; }
    }
    public int MaxHP
    {
        get { return maxHP; }
    }
    public int Attack
    {
        get { return attack; }
    }
    public int Defense
    {
        get { return defense; }
    }
    public int SpAttack
    {
        get { return spAttack; }
    }
    public int SpDefense
    {
        get { return spDefense; }
    }
    public int Speed
    {
        get { return speed; }
    }
    // public int CatchRate {
    //     get { return catchRate; }
    // }
    public int CatchRate => catchRate;
    public int ExpYield => expYield;
    public GrowthRate GrowthRate => growthRate;
    public string Ability
    {
        get { return ability; }
    }
    public int[] Individual => individual;
    public int[] Effort => effort;
    public int[] Personality => personality;
    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }
    public List<MoveBase> LearnableByItems => learnableByItems;
    public List<Evolution> Evolutions => evolutions;
}

[System.Serializable]

public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base { get { return moveBase; } }
    public int Level { get { return level; } }
}

[System.Serializable]
public class Evolution
{
    [SerializeField] UnitBase evolvesInto;
    [SerializeField] int requiredLevel;
    [SerializeField] EvolutionItem requireItem;

    public UnitBase EvolvesInto => evolvesInto;
    public int RequiredLevel => requiredLevel;
    public EvolutionItem RequireItem => requireItem;
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
    Normal,
    Fire,
    Water,
    Grass,
    Electric,
    Ice,
    Courage,
    Poison,
    Soil,
    Sky,
    Psycho,
    Wind,
    Stone,
    Ghost,
    Dragon,
    Devil,
    Steel,
    Strange,
}

public enum GrowthRate
{
    Fast, MediumFast
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    // 실수치가 아닌 %의 스텟
    Accuracy,
    Evasion
}

public class TypeChart
{
    static float[][] chart =
    {
        //                    없음, 불꽃,    물,   풀, 번개, 얼음,  용기,   독,   흙, 하늘,  마법, 바람,  바위, 유령,   용, 악마, 강철, 이상함
        /*없음*/new float[] {   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   0f,   1f,   1f, 0.5f,   1f },
        /*불꽃*/new float[] {   1f, 0.5f, 0.5f,   2f,   1f,   2f,   1f,   1f,   1f,   1f,   1f,   2f, 0.5f,   1f, 0.5f,   1f,   2f,   1f },
        /*물  */new float[] {   1f,   2f, 0.5f, 0.5f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f,   2f,   1f, 0.5f,   1f,   1f,   1f },
        /*풀  */new float[] {   1f, 0.5f,   2f, 0.5f,   1f,   1f,   1f, 0.5f,   2f, 0.5f,   1f, 0.5f,   2f,   1f, 0.5f,   1f, 0.5f,   1f },
        /*번개*/new float[] {   1f,   1f,   2f, 0.5f,   1f,   1f,   1f,   1f,   0f,   2f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   1f },
        /*얼음*/new float[] {   1f, 0.5f, 0.5f,   2f,   1f, 0.5f,   1f,   1f,   2f,   2f,   1f,   1f,   1f,   1f,   2f,   1f, 0.5f,   1f },
        /*용기*/new float[] {   2f,   1f,   1f,   1f,   1f,   2f,   1f, 0.5f,   1f, 0.5f, 0.5f, 0.5f,   2f,   0f,   1f,   2f,   2f, 0.5f },
        /*독  */new float[] {   1f,   1f,   1f,   2f,   1f,   1f,   1f, 0.5f, 0.5f,   1f,   1f,   1f, 0.5f, 0.5f,   1f,   1f,   0f,   2f },
        /*흙  */new float[] {   1f,   2f,   1f,   1f,   2f,   1f,   1f,   2f,   1f,   0f,   1f, 0.5f,   2f,   1f,   1f,   1f,   2f,   1f },
        /*하늘*/new float[] {   1f,   1f,   1f,   2f, 0.5f,   1f,   2f,   1f,   1f,   1f,   1f,   2f, 0.5f,   1f,   1f,   1f, 0.5f,   1f },
        /*마법*/new float[] {   1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   1f,   1f, 0.5f,   1f,   1f,   1f,   1f,   0f, 0.5f,   1f },
        /*바람*/new float[] {   1f, 0.5f,   1f,   2f,   1f,   1f, 0.5f, 0.5f,   1f, 0.5f,   2f,   1f,   1f, 0.5f,   1f,   2f, 0.5f, 0.5f },
        /*바위*/new float[] {   1f,   2f,   1f,   1f,   1f,   2f, 0.5f,   1f, 0.5f,   2f,   1f,   2f,   1f,   1f,   1f,   1f, 0.5f,   1f },
        /*유령*/new float[] {   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f, 0.5f,   1f,   1f },
        /*용  */new float[] {   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f, 0.5f,   0f },
        /*악마*/new float[] {   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f, 0.5f,   1f, 0.5f },
        /*강철*/new float[] {   1f, 0.5f, 0.5f,   1f, 0.5f,   2f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f, 0.5f,   2f },
        /*이상*/new float[] {   1f, 0.5f,   1f,   1f,   1f,   1f,   2f, 0.5f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f, 0.5f,   1f },
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

public class IndividualChart
{
    static string[][] chart =
    {   //             스텟치
        new string[] { "먹는 것을 제일 좋아한다", "힘자랑이 특기다", "몸이 튼튼하다", "호기심이 강하다", "기가 세다", "달리기를 좋아한다" },
        new string[] { "낮잠을 잘 잔다", "난동부리기를 좋아한다", "맷집이 강하다", "장난을 좋아한다", "조금 겉치레가 있다", "주위 소리에 민감하다" },
        new string[] { "느긋하다", "다혈질이다", "끈질기다", "빈틈이 없다", "오기가 세다", "촐랑댄다" },
        new string[] { "물건을 잘 어지른다", "싸움을 좋아한다", "인내심이 강하다", "걱정거리가 많다", "지기 싫어한다", "우쭐댄다" },
        new string[] { "여유를 즐긴다", "혈기왕성하다", "잘 참는다", "매우 꼼꼼하다", "조금 고집불통이다", "발이 빠르다" }
    };
    public static string GetIndividualText(int[] status)
    {
        // MathF.Max(status)
        var max = status.Select((n, i) => (Number: n, Index: i)).Max();
        return chart[max.Number][max.Index];
    }
}

public class PersonalityChart
{
    static string[][] chart =
    {   //            하락하는 능력치
        new string[] { "노력",   "외로움", "고집",       "개구쟁이", "용감" },
        new string[] { "대담",   "온순",   "장난꾸러기", "촐랑",    "무사태평" },
        new string[] { "조심",   "의젓",   "수줍음",     "덜렁",    "냉정" }, // 상승하는 능력치
        new string[] { "차분",   "얌전",   "신중",       "변덕",    "건방" },
        new string[] { "겁쟁이", "성급",   "명랑",       "천진난만", "성실" },
    };
    static string[][] tailerWord =
    {   //            하락하는 능력치
        new string[] { "하는", "을 타는", "스런", "같은", "한" },
        new string[] { "한", "한", "같은", "거리는", "한" },
        new string[] { "스런", "한", "을 타는", "거리는", "한" }, // 상승하는 능력치
        new string[] { "한", "한", "한", "스런", "진" },
        new string[] { "같은", "한", "한", "한", "한" },
    };
    static string[][] closingRemarks =
    {   //            하락하는 능력치
        new string[] { "한다", "을 탄다", "불통이다", "같다", "하다" },
        new string[] { "하다", "하다", "같다", "거린다", "하다" },
        new string[] { "스럽다", "하다", "을 탄다", "거린다", "하다" }, // 상승하는 능력치
        new string[] { "하다", "하다", "하다", "스럽다", "지다" },
        new string[] { "같다", "하다", "하다", "하다", "하다" },
    };
    public static string GetPersonalityText(int[] status)
    {
        return $"{chart[status[0]][status[1]]}{tailerWord[status[0]][status[1]]}";
    }
    public static string GetPersonalityClosingRemarks(int[] status)
    {
        return $"{chart[status[0]][status[1]]}{closingRemarks[status[0]][status[1]]}";
    }
    public static string[][] Chart => chart;
    public static string[][] TailerWord => tailerWord;
    public static string[][] ClosingRemarks => closingRemarks;
}