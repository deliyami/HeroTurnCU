using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;
    List<Unit> units;
    UnitParty party;

    int selection = 0;
    public Unit SelectedMember => units[selection];
    /// <summary>
    /// partyscreen 다른 상황일떄 불린다?
    /// </summary>
    public BattleState? CalledFrom { get; set; }

    PartyMemberUI[] memberSlots;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);

        party = UnitParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public void SetPartyData()
    {
        units = party.Units;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < units.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(units[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }
        UpdateMemberSelection(selection);

        messageText.text = "동료를 고르세요!";
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevSelection = selection;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            selection += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            selection -= 2;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            --selection;
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ++selection;

        selection = Mathf.Clamp(selection, 0, units.Count - 1);

        if (selection != prevSelection)
            UpdateMemberSelection(selection);

        if (Input.GetButtonDown("Submit"))
        {
            onSelected?.Invoke();
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            onBack?.Invoke();
        }
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i< units.Count; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    public void ShowIfTmIsUsable(TmItem tmItem)
    {
        for (int i = 0; i < units.Count; i++)
        {
            string message = tmItem.CanBeTaught(units[i])?"배울 수 있다!":"배울 수 없다!";
            memberSlots[i].SetMessage(message);
        }
    }
    public void ClearMemberSlotMessages()
    {
        for (int i = 0; i < units.Count; i++)
        {
            memberSlots[i].SetMessage("");
        }
    }
    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
