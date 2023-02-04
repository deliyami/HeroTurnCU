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

    public void Setup(UnitType unitType)
    {
        type = new Type(_base);
        // GetComponent<Image>().sprite = type.Base.Fire;
        if (unitType == UnitType.Normal)
            GetComponent<Image>().sprite = type.Base.Normal;
        else if (unitType == UnitType.Fire)
            GetComponent<Image>().sprite = type.Base.Fire;
        else if (unitType == UnitType.Water)
            GetComponent<Image>().sprite = type.Base.Water;
        else if (unitType == UnitType.Grass)
            GetComponent<Image>().sprite = type.Base.Grass;
        else if (unitType == UnitType.Electric)
            GetComponent<Image>().sprite = type.Base.Electric;
        else if (unitType == UnitType.Ice)
            GetComponent<Image>().sprite = type.Base.Ice;
        else if (unitType == UnitType.Courage)
            GetComponent<Image>().sprite = type.Base.Courage;
        else if (unitType == UnitType.Poison)
            GetComponent<Image>().sprite = type.Base.Poison;
        else if (unitType == UnitType.Soil)
            GetComponent<Image>().sprite = type.Base.Soil;
        else if (unitType == UnitType.Sky)
            GetComponent<Image>().sprite = type.Base.Sky;
        else if (unitType == UnitType.Psycho)
            GetComponent<Image>().sprite = type.Base.Psycho;
        else if (unitType == UnitType.Wind)
            GetComponent<Image>().sprite = type.Base.Wind;
        else if (unitType == UnitType.Stone)
            GetComponent<Image>().sprite = type.Base.Stone;
        else if (unitType == UnitType.Ghost)
            GetComponent<Image>().sprite = type.Base.Ghost;
        else if (unitType == UnitType.Dragon)
            GetComponent<Image>().sprite = type.Base.Dragon;
        else if (unitType == UnitType.Devil)
            GetComponent<Image>().sprite = type.Base.Devil;
        else if (unitType == UnitType.Steel)
            GetComponent<Image>().sprite = type.Base.Steel;
        else if (unitType == UnitType.Strange)
            GetComponent<Image>().sprite = type.Base.Strange;
        else
            GetComponent<Image>().sprite = type.Base.None;
    }
}
