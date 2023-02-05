using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GDEUtils.StateMachine;
using System.Linq;

public class MoveSelectionUI : SelectionUI<TextSlot>
{
    [SerializeField] List<TextMeshProUGUI> moveTexts;

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i = 0; i < currentMoves.Count; ++i)
        {
            moveTexts[i].text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].text = newMove.Name;
        SetItems(moveTexts.Select(m => m.GetComponent<TextSlot>()).ToList());
    }

}
