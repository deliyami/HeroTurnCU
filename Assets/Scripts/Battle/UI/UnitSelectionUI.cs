using GDE.GenericSelectionUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class UnitSelectionUI : SelectionUI<TextSlot>
{
    List<BattleUnit> playerUnits;
    List<BattleUnit> enemyUnits;
    List<Move> Moves { get; set; }
    BattleSystem bs;
    public static UnitSelectionUI i { get; private set; }
    void Start()
    {
        i = this;
        bs = BattleState.i.BattleSystem;



        // BattleState.i.unitCount

        SetSelectionSettings(SelectionType.Grid, 2);
    }
    public void progressState()
    {
        playerUnits = bs.PlayerUnits;
        enemyUnits = bs.EnemyUnits;
        Moves = bs.PlayerUnits[bs.ActionIndex].Unit.Moves;
    }
    public override void HandleUpdate()
    {
        UpdateSelectionTimer();
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            selectedItem += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            selectedItem -= 2;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            selectedItem--;
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            selectedItem++;
        selectedItem = Mathf.Clamp(selectedItem, 0, enemyUnits.Count + playerUnits.Count - 2);
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].SetSelected(i == selectedItem);
        }
        if (selectedItem >= enemyUnits.Count)
            foreach (var u in playerUnits)
            {
                u.SetSelected(u.Unit.Base.Name != bs.PlayerUnits[bs.ActionIndex].Unit.Base.Name);
            }
        else
        {
            for (int i = 0; i < BattleState.i.unitCount; i++)
            {
                playerUnits[i].SetSelected(false);
            }
        }
        if (Input.GetButtonDown("Submit"))
        {
            HandleSubmit();
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            HandleCancel();
        }
    }
    public override void ClearItems()
    {
        Debug.Log("use clear");
        for (int i = 0; i < BattleState.i.unitCount; i++)
        {
            enemyUnits[i].SetSelected(false);
            playerUnits[i].SetSelected(false);
        }
    }
}
