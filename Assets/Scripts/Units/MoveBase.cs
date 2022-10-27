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
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;
    [SerializeField] int priority; // 선공기
    // [SerializeField] int strange; // 상태이상
    // [SerializeField] int strangePercentage;
    // [SerializeField] string spStat?; 이거 특수 기능 삽입해야하는데...

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
    public int PP {
        get { return pp; }
    }
    public int Priority {
        get { return priority; }
    }
}
