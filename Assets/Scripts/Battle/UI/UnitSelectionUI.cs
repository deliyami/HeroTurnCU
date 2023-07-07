using GDE.GenericSelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class UnitSelectionUI : SelectionUI<TextSlot>
{
    List<BattleUnit> playerUnits;
    List<BattleUnit> enemyUnits;
    public Move Move { get; set; }
    BattleSystem bs;
    public static UnitSelectionUI i { get; private set; }

    Dictionary<MoveTarget, Action> checkMoveTarget = new Dictionary<MoveTarget, Action>();

    void Start()
    {
        i = this;
        bs = BattleState.i.BattleSystem;

        // Foe, FoeAll, Team, TeamAnother, TeamAll, Self, Another, AnotherAll, All
        checkMoveTarget[MoveTarget.Foe] = selectFoeUnit;
        checkMoveTarget[MoveTarget.FoeAll] = selectFoeAllUnit;
        checkMoveTarget[MoveTarget.Team] = selectTeamUnit;
        checkMoveTarget[MoveTarget.TeamAnother] = selectTeamAnotherUnit;
        checkMoveTarget[MoveTarget.TeamAll] = selectTeamAllUnit;
        checkMoveTarget[MoveTarget.Self] = selectSelfUnit;
        checkMoveTarget[MoveTarget.Another] = selectAnotherUnit;
        checkMoveTarget[MoveTarget.AnotherAll] = selectAnotherAllUnit;
        checkMoveTarget[MoveTarget.All] = selectAllUnit;

        // BattleState.i.unitCount

        SetSelectionSettings(SelectionType.Grid, 2);
    }
    public void progressState()
    {
        playerUnits = bs.PlayerUnits;
        enemyUnits = bs.EnemyUnits;
        Move = bs.PlayerUnits[bs.ActionIndex].Unit.Moves[MoveSelectionState.i.currentMove];
        if (BattleState.i.unitCount == 1)
        {
            if (Move.Base.Target == MoveTarget.Foe || Move.Base.Target == MoveTarget.FoeAll || Move.Base.Target == MoveTarget.Another || Move.Base.Target == MoveTarget.AnotherAll) selectedItem = 0;
            if (Move.Base.Target == MoveTarget.Team || Move.Base.Target == MoveTarget.TeamAll || Move.Base.Target == MoveTarget.Self) selectedItem = 1;
            if (Move.Base.Target == MoveTarget.All) selectedItem = 0;
            if (Move.Base.Target == MoveTarget.TeamAnother) selectedItem = -1;
            HandleSubmit();
            return;
        }
        bs.DialogBox.SetDialog($"상대를 선택하세요.");
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

        checkMoveTarget[Move.Base.Target]();

        if (Input.GetButtonDown("Submit"))
        {
            HandleSubmit();
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            HandleCancel();
        }
    }
    // Foe, FoeAll, Team, TeamAnother, TeamAll, Self, Another, AnotherAll, All
    // 2이상 선택하는 것은 99로 정하려고 했는데 UnitSelectionState에서 인덱스 값 확인하니까 이건 0으로 해야겠다 나머지는 선택하는 것 하자... 무조건실패는 -1
    void selectFoeUnit()
    {
        // 적이 한명만 남았을 때 예외처리
        selectedItem = Mathf.Clamp(selectedItem, 0, enemyUnits.Count < bs.UnitCount ? 0 : 1);
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].SetSelected(i == selectedItem);
        }
    }
    void selectFoeAllUnit()
    {
        selectedItem = 0;
        enemyUnits.ForEach(e => e.SetSelected(true));
    }
    void selectTeamUnit()
    {
        selectedItem = Mathf.Clamp(selectedItem, 2, 3);
        for (int i = 0; i < playerUnits.Count; i++)
        {
            playerUnits[i].SetSelected(i + bs.UnitCount == selectedItem);
        }
    }
    void selectTeamAnotherUnit()
    {
        // 아군이 한명만 남았을 때 예외처리
        if (playerUnits.Count < bs.UnitCount)
        {
            selectSelfUnit();
            return;
        }
        selectedItem = bs.UnitCount + bs.ActionIndex == 0 ? 1 : 0;
        for (int i = 0; i < playerUnits.Count; i++)
        {
            playerUnits[i].SetSelected(i != bs.ActionIndex);
        }
    }
    void selectTeamAllUnit()
    {
        selectedItem = 0;
        playerUnits.ForEach(e => e.SetSelected(true));
    }
    void selectSelfUnit()
    {
        selectedItem = bs.UnitCount + bs.ActionIndex;
        playerUnits[bs.ActionIndex].SetSelected(true);
    }
    void selectAnotherUnit()
    {
        // 적이 한명만 남았을 때 예외처리
        // 적과 아군이 한명만 남았을 때 예외처리
        if ((playerUnits.Count < bs.UnitCount && enemyUnits.Count < bs.UnitCount) || playerUnits.Count < bs.UnitCount)
        {
            selectFoeUnit();
        }
        else
        {
            selectedItem = Mathf.Clamp(selectedItem, 0, enemyUnits.Count + playerUnits.Count - 2);
            if (enemyUnits.Count < bs.UnitCount)
            {
                if (selectedItem == 0)
                {
                    enemyUnits.ForEach(e => e.SetSelected(true));
                    playerUnits.ForEach(e => e.SetSelected(false));
                }
                else
                {
                    enemyUnits.ForEach(e => e.SetSelected(false));
                    foreach (var u in playerUnits)
                    {
                        u.SetSelected(u.Unit.Base.Name != bs.PlayerUnits[bs.ActionIndex].Unit.Base.Name);
                    }
                }
            }
            else
            {
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
            }
        }
    }
    void selectAnotherAllUnit()
    {
        selectedItem = 0;
        enemyUnits.ForEach(e => e.SetSelected(true));
        for (int i = 0; i < playerUnits.Count; i++)
        {
            playerUnits[i].SetSelected(i != bs.ActionIndex);
        }
    }
    void selectAllUnit()
    {
        selectedItem = 0;
        enemyUnits.ForEach(e => e.SetSelected(true));
        playerUnits.ForEach(e => e.SetSelected(true));
    }

    public override void ClearItems()
    {
        Debug.Log("use clear");
        enemyUnits.ForEach(e => e.SetSelected(false));
        playerUnits.ForEach(e => e.SetSelected(false));
        // for (int i = 0; i < BattleState.i.unitCount; i++)
        // {
        //     enemyUnits[i].SetSelected(false);
        //     playerUnits[i].SetSelected(false);
        // }
    }
}
