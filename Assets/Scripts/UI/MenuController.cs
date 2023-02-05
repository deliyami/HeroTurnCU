using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GDEUtils.StateMachine;

public class MenuController : SelectionUI<TextSlot> {
    private void Start() {
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }
}
