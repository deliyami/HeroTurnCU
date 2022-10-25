using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] string frontSprite;
    [SerializeField] string backSprite;
}
