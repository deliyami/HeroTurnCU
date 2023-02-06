using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Type", menuName = "Unit/Create new types")]
public class TypeBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] UnitType unitType;
    [SerializeField] Sprite sprite;

    public string Name => name;
    public UnitType UnitType => unitType;
    public Sprite Sprite => sprite;
}