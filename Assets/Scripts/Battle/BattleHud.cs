using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI HPText;
    [SerializeField] TextMeshProUGUI MaxHPText;
    [SerializeField] HPBar hpBar;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] Image statusSprite;
    [SerializeField] Color psnColor;
    [SerializeField] Sprite psnSprite;
    [SerializeField] Color brnColor;
    [SerializeField] Sprite brnSprite;
    [SerializeField] Color slpColor;
    [SerializeField] Sprite slpSprite;
    [SerializeField] Color parColor;
    [SerializeField] Sprite parSprite;
    [SerializeField] Color frzColor;
    [SerializeField] Sprite frzSprite;
    // [SerializeField] Status status;

    Unit _unit;
    Dictionary<ConditionID, Color> statusColors;
    Dictionary<ConditionID, Sprite> statusSprites;

    public TextMeshProUGUI NameText {
        get { return nameText; }
    }

    public virtual void SetData(Unit unit)
    {
        _unit = unit;

        nameText.text = unit.Base.Name;
        levelText.text = "Lvl " + unit.Level;
        hpBar.SetHP((float) unit.HP / unit.MaxHP);
        HPText.text = $"{unit.HP}";
        MaxHPText.text = $"/{unit.MaxHP}";

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor},
            {ConditionID.brn, brnColor},
            {ConditionID.slp, slpColor},
            {ConditionID.par, parColor},
            {ConditionID.frz, frzColor},
        };

        statusSprites = new Dictionary<ConditionID, Sprite>()
        {
            {ConditionID.psn, psnSprite},
            {ConditionID.brn, brnSprite},
            {ConditionID.slp, slpSprite},
            {ConditionID.par, parSprite},
            {ConditionID.frz, frzSprite},
        };

        SetStatusText();
        _unit.OnStatusChanged += SetStatusText;
    }

    void SetStatusText()
    {
        if (_unit.Status == null)
        {
            statusSprite.color = new Color(1f, 1f, 1f, 0f);
            statusText.text = "";
        }
        else
        {
            statusSprite.color = new Color(1f, 1f, 1f, 1f);
            // switch(_unit.Status.ID.ToString()){
            //     case "none":
            //         break;
            // }
            statusText.color = statusColors[_unit.Status.ID];
            statusText.text = ConditionDB.Conditions[_unit.Status.ID].Name;
            statusSprite.sprite = statusSprites[_unit.Status.ID];
        }
    }

    public virtual IEnumerator UpdateHP()
    {
        if (_unit.HPChanged)
        {
            yield return HPText.text = $"{_unit.HP}";
            MaxHPText.text = $"/{_unit.MaxHP}";
            yield return hpBar.SetHPSmooth((float) _unit.HP / _unit.MaxHP);
        }
    }
}
