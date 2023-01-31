using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    public virtual string Name {
        get { return name; }
    }
    public string Description => description;
    public Sprite Icon => icon;

    public virtual bool Use(Unit unit)
    {
        return false;
    }
    public virtual bool IsReusable => false;
    public virtual bool CanBeUsedInBattle => true;
    public virtual bool CanBeUsedOutsideBattle => true;
}