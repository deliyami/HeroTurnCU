using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    [SerializeField] float price = 1f;
    [SerializeField] bool isSellable;
    public virtual string Name
    {
        get { return name; }
    }
    public string Description => description;
    public Sprite Icon => icon;
    public float Price => price;
    public bool IsSellable => isSellable;
    public virtual bool isUsable(Unit unit)
    {
        return false;
    }
    public virtual bool Use(Unit unit)
    {
        return false;
    }
    public virtual bool IsReusable => false;
    public virtual bool CanBeUsedInBattle => true;
    public virtual bool CanBeUsedOutsideBattle => true;
}