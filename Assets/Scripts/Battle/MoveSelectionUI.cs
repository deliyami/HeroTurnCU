using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> moveTexts;
    [SerializeField] Color highlightedColor;
    int currentSelection = 0;

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i = 0; i < currentMoves.Count; ++i)
        {
            moveTexts[i].text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].text = newMove.Name;
    }

    public void HandleMoveSelection(Action<int> onSelected)
    {
        if (Input.GetButtonDown("Down") || Input.GetKeyDown("S"))
            ++currentSelection;
        else if (Input.GetButtonDown("Up") || Input.GetKeyDown("W"))
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, UnitBase.MaxNumOfMoves);
        UpdateMoveSelection(currentSelection);
        if (Input.GetButtonDown("Submit"))
            onSelected?.Invoke(currentSelection);
    }
    public void UpdateMoveSelection(int selection)
    {
        for (int i = 0; i < UnitBase.MaxNumOfMoves + 1; ++i)
        {
            if (i == selection)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.white;
        }

        currentSelection = Mathf.Clamp(currentSelection, 0, UnitBase.MaxNumOfMoves);
    }
}
