using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Unit> wildUnits;

    public Unit GetRandomWildUnit()
    {
        int randomInt = Random.Range(0, wildUnits.Count);
        var wildUnit =  wildUnits[randomInt];
        wildUnit.Init();
        Debug.Log("유닛 생성중");
        Debug.Log(wildUnit.Base.Name);
        Debug.Log(wildUnit.Level);
        return wildUnit;
    }
}
