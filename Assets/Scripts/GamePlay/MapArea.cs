using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Unit> wildUnits;

    public Unit GetRadomWildUnit()
    {
        int randomInt = Random.Range(0, wildUnits.Count);
        var wildUnit =  wildUnits[randomInt];
        wildUnit.Init();
        return wildUnit;
    }
}
