using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type
{
    public TypeBase Base { get; set; }

    public Type(TypeBase uBase)
    {
        Base = uBase;
    }
    public static string GetType(UnitType unitType)
    {
        return TypeDB.GetObjectByName(unitType.ToString()).name;
        // string type;
        // if (unitType == UnitType.Normal)
        //     type = "없음";
        // else if (unitType == UnitType.Fire)
        //     type = "불꽃";
        // else if (unitType == UnitType.Water)
        //     type = "물";
        // else if (unitType == UnitType.Grass)
        //     type = "풀";
        // else if (unitType == UnitType.Electric)
        //     type = "번개";
        // else if (unitType == UnitType.Ice)
        //     type = "얼음";
        // else if (unitType == UnitType.Courage)
        //     type = "용기";
        // else if (unitType == UnitType.Poison)
        //     type = "독";
        // else if (unitType == UnitType.Soil)
        //     type = "흙";
        // else if (unitType == UnitType.Sky)
        //     type = "하늘";
        // else if (unitType == UnitType.Psycho)
        //     type = "마법";
        // else if (unitType == UnitType.Wind)
        //     type = "바람";
        // else if (unitType == UnitType.Stone)
        //     type = "바위";
        // else if (unitType == UnitType.Ghost)
        //     type = "유령";
        // else if (unitType == UnitType.Dragon)
        //     type = "용";
        // else if (unitType == UnitType.Devil)
        //     type = "악마";
        // else if (unitType == UnitType.Steel)
        //     type = "강철";
        // else if (unitType == UnitType.Strange)
        //     type = "이상함";
        // else
        //     type = "None";
        // return type;
    }
}