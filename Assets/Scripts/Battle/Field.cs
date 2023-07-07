using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
    public Condition Weather { get; set; }
    public int? WeatherDuration { get; set; }
    public void SetWeather(ConditionID conditionID)
    {
        Weather = ConditionDB.Conditions[conditionID];
        Weather.ID = conditionID;
        Weather.OnStart?.Invoke(null);
    }

    public Condition Room { get; set; }
    public int? RoomDuration { get; set; }
    public void SetRoom(ConditionID conditionID)
    {
        Room = ConditionDB.Conditions[conditionID];
        Room.ID = conditionID;
        Room.OnStart?.Invoke(null);
    }

    public Condition field { get; set; }
    public int? FieldDuration { get; set; }
    public void SetField(ConditionID conditionID)
    {
        field = ConditionDB.Conditions[conditionID];
        field.ID = conditionID;
        field.OnStart?.Invoke(null);
    }
}
