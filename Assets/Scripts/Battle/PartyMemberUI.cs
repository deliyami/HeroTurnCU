using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : PlayerHud
{
    [SerializeField] Image image;
    // Image image;
    public override void SetData(Unit unit)
    {
        image.sprite = unit.Base.SmallSprite;
        base.SetData(unit);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            NameText.color = GlobalSettings.i.HighlightedColor;
        else
            NameText.color = Color.black;
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