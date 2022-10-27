using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleType : MonoBehaviour
{
    [SerializeField] TypeBase _base;
    [SerializeField] int level;


    // public Unit unit { get; set; }
    private Type type;
    public Type Type {
        get { return type; }
    }

    public void Setup(string typeText)
    {
        type = new Type(_base);
        // GetComponent<Image>().sprite = type.Base.Fire;

        switch(typeText){
            case "없음":
                GetComponent<Image>().sprite = type.Base.Normal;
                break;
            case "불꽃":
                GetComponent<Image>().sprite = type.Base.Fire;
                break;
            case "풀":
                GetComponent<Image>().sprite = type.Base.Grass;
                break;
            case "물":
                GetComponent<Image>().sprite = type.Base.Water;
                break;
            case "번개":
                GetComponent<Image>().sprite = type.Base.Electric;
                break;
            case "얼음":
                GetComponent<Image>().sprite = type.Base.Ice;
                break;
            case "용기":
                GetComponent<Image>().sprite = type.Base.Courage;
                break;
            case "독":
                GetComponent<Image>().sprite = type.Base.Poison;
                break;
            case "흙":
                GetComponent<Image>().sprite = type.Base.Soil;
                break;
            case "하늘":
                GetComponent<Image>().sprite = type.Base.Sky;
                break;
            case "마법":
                GetComponent<Image>().sprite = type.Base.Psycho;
                break;
            case "바람":
                GetComponent<Image>().sprite = type.Base.Wind;
                break;
            case "바위":
                GetComponent<Image>().sprite = type.Base.Stone;
                break;
            case "유령":
                GetComponent<Image>().sprite = type.Base.Ghost;
                break;
            case "용":
                GetComponent<Image>().sprite = type.Base.Dragon;
                break;
            case "악마":
                GetComponent<Image>().sprite = type.Base.Devil;
                break;
            case "강철":
                GetComponent<Image>().sprite = type.Base.Steel;
                break;
            case "이상함":
                GetComponent<Image>().sprite = type.Base.Strange;
                break;
            default:
                GetComponent<Image>().sprite = type.Base.None;
                break;
        }
    }

}
