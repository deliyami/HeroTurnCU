using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DexMoveSlotUI : TextSlot
{
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] Image typeSprite;
    public void SetData(MoveBase move)
    {
        base.Text.text = move.Name;
        var type = TypeDB.GetObjectByName(move.Type.ToString());
        typeText.text = type.Name;
        typeSprite.sprite = type.Sprite;
    }
}
