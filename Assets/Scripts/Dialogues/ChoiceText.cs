using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoiceText : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetSelected(bool selected)
    {
        text.color = (selected)?GlobalSettings.i.HighlightedColor:Color.white;
    }
    public TextMeshProUGUI TextField => text;
}
