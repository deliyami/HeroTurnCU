using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyMemberUI : BattleHud
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI messageText;
    // Image image;
    public void Init(Unit unit)
    {
        image.sprite = unit.Base.SmallSprite;
        base.SetData(unit, false);
        // SetMessage("");
        // unit.OnHPChanged += UpdateData;
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            NameText.color = GlobalSettings.i.HighlightedColor;
        else
            NameText.color = Color.black;
    }
    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}


// public override void SetData(Unit unit)
//     {
//         this.unit = unit;
//         HPText.text = $"{unit.HP}";
//         MaxHPText.text = $"/{unit.MaxHP}";
//         base.SetData(unit);
//     }

//     public override IEnumerator UpdateHP()
//     {
//         HPText.text = $"{unit.HP}";
//         MaxHPText.text = $"/{unit.MaxHP}";
//         yield return base.UpdateHP();
//     }