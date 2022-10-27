using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHud : BattleHud
{
    [SerializeField] TextMeshProUGUI HPText;
    [SerializeField] TextMeshProUGUI MaxHPText;
    
    public override void SetData(Unit unit)
    {
        base.SetData(unit);
        HPText.text = "" + unit.HP;
        MaxHPText.text = "/" + unit.MaxHP;
    }
}
