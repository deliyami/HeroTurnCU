using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI LevelText;
    [SerializeField] HPBar hpBar;

    public virtual void SetData(Unit unit)
    {
        nameText.text = unit.Base.Name;
        LevelText.text = "Lvl " + unit.Level;
        hpBar.SetHP((float) unit.HP / unit.MaxHP);
    }
}
