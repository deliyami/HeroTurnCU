using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDB
{
    static Dictionary<string, UnitBase> units;

    public static void Init()
    {
        units = new Dictionary<string, UnitBase>();
        var unitArray = Resources.LoadAll<UnitBase>("");
        foreach (var unit in unitArray)
        {
            if (units.ContainsKey(unit.Name))
            {
                Debug.LogError($"같은 이름의 유닛이 존재함 {unit.Name}");
                continue;
            }
            units[unit.Name] = unit;
        }
    }

    public static UnitBase GetUnitByName(string name)
    {
        if (!units.ContainsKey(name))
        {
            Debug.LogError($"해당 유닛은 없습니다{name}");
            return null;
        }

        return units[name];
    }
}
