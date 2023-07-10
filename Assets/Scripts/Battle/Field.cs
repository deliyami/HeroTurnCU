using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
    public FieldBase Weather { get; set; } = new FieldBase();
    public FieldBase Room { get; set; } = new FieldBase();

    public FieldBase field { get; set; } = new FieldBase();

    public FieldBase Reflect { get; set; } = new FieldBase();
    public FieldBase LightScreen { get; set; } = new FieldBase();
}

public class FieldBase
{
    public Condition condition { get; set; }
    public int? duration { get; set; }
    public void SetCondition(ConditionID conditionID)
    {
        condition = ConditionDB.Conditions[conditionID];
        condition.ID = conditionID;
        condition.OnStart?.Invoke(null);
    }
}