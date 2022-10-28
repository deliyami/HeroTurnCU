using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI LevelText;
    [SerializeField] HPBar hpBar;

    Unit _unit;

    public virtual void SetData(Unit unit)
    {
        _unit = unit;

        nameText.text = unit.Base.Name;
        LevelText.text = "Lvl " + unit.Level;
        hpBar.SetHP((float) unit.HP / unit.MaxHP);
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float) _unit.HP / _unit.MaxHP);
    }
}
