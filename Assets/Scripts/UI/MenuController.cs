using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GDE.GenericSelectionUI;

public class MenuController : SelectionUI<TextSlot>
{
    private void Start()
    {
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }
}
