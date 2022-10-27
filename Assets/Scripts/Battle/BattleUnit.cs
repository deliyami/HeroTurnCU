using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] UnitBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    // 개체치 31 {HP, attack, defense, spAttack, spDefense, speed}
    [SerializeField] int[] tribe;
    // 노력치 0~252 {HP, attack, defense, spAttack, spDefense, speed} /4해서 더해야 하는데... 적혀있네
    [SerializeField] int[] effort;
    // 성격 int[] = {상승 스텟 index, 하락 스텟 index}
    [SerializeField] int[] personality;

    public Unit unit { get; set; }

    public Unit Unit {
        get { return unit; }
    }

    public void Setup()
    {
        unit = new Unit(_base, level, tribe, effort, personality);
        if (isPlayerUnit)
            GetComponent<Image>().sprite = unit.Base.BackSprite;
        else
            GetComponent<Image>().sprite = unit.Base.FrontSprite;
    }
}
