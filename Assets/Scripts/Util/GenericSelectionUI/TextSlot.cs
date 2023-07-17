using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSlot : MonoBehaviour, ISelectableItem
{
    [SerializeField] TextMeshProUGUI text;
    Color originalColor;
    public void Init()
    {
        originalColor = text.color;
    }
    public void Clear()
    {
        text.color = originalColor;
    }
    public void OnSelectionChange(bool selected)
    {
        // text.color = (selected)?GlobalSettings.i.HighlightedColor:GlobalSettings.i.UnchosenColor;
        text.color = (selected) ? GlobalSettings.i.HighlightedColor : originalColor;
    }
    public void OnSeatChange(bool selected)
    {
        // text.color = (selected)?GlobalSettings.i.HighlightedColor:GlobalSettings.i.UnchosenColor;
        text.color = (selected) ? GlobalSettings.i.GreenlightedColor : originalColor;
    }
    public void OnResetColor()
    {
        text.color = originalColor;
    }
    public TextMeshProUGUI Text => text;

    public void SetText(string s)
    {
        text.text = s;
    }
}
