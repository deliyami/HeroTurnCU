using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutToUseState : State<BattleSystem>
{
    public Unit NewUnit { get; set; }

    bool aboutToUseChoice;

    public static AboutToUseState i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    BattleSystem bs;
    public override void Enter(BattleSystem owner)
    {
        bs = owner;
        StartCoroutine(StartState());
    }

    IEnumerator StartState()
    {
        yield return bs.DialogBox.TypeDialog($"{NewUnit.Base.Name}(이)가 준비중이다! 팀원을 교체하겠습니까?");
        bs.DialogBox.EnableChoiceBox(true);
    }

    public override void Execute()
    {
        if (!bs.DialogBox.IsChoiceBoxEnabled) return;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            aboutToUseChoice = !aboutToUseChoice;
        bs.DialogBox.UpdateChoiceBox(aboutToUseChoice);
        if (Input.GetButtonDown("Submit"))
        {
            bs.DialogBox.EnableChoiceBox(false);
            if (aboutToUseChoice == true)
            {
                StartCoroutine(SwitchAndContinueBattle());
            }
            else
            {
                StartCoroutine(ContinueBattle());
            }
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            bs.DialogBox.EnableChoiceBox(false);
            StartCoroutine(ContinueBattle());
        }
    }
    IEnumerator SwitchAndContinueBattle()
    {
        yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
        var selectedUnit = PartyState.i.SelectedUnit;
        if (selectedUnit != null)
        {
            yield return bs.SwitchUnit(bs.PlayerUnits[bs.ActionIndex], selectedUnit);
        }

        yield return ContinueBattle();
    }
    IEnumerator ContinueBattle()
    {
        yield return bs.SendNextTrainerUnit();
        bs.StateMachine.Pop();
    }
}
