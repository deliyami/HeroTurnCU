using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Unit/Create new items")]
public class ItemBase : ScriptableObject
{
    [SerializeField] new string name;
    public string Name {
        get { return name; }
    }

}