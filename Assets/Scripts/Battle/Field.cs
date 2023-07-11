using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
    public FieldBase Weather { get; set; }
    public FieldBase Room { get; set; }
    public FieldBase field { get; set; }
    public FieldBase Reflect { get; set; }
    public FieldBase LightScreen { get; set; }
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