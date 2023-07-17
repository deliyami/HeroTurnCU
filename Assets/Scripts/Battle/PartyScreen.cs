using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using GDE.GenericSelectionUI;

public class PartyScreen : SelectionUI<TextSlot>
{
    [SerializeField] TextMeshProUGUI messageText;
    List<Unit> units;
    UnitParty party;
    public Unit SelectedMember => units[selectedItem];
    public int changedItem = -1;

    PartyMemberUI[] memberSlots;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        SetSelectionSettings(SelectionType.Grid, 2);

        party = UnitParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();
        UpdateUI();
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
        var textSlots = memberSlots.Select(m => m.GetComponent<TextSlot>());
        SetItems(textSlots.Take(units.Count).ToList());

        messageText.text = "동료를 선택하세요!";
    }

    public void ShowIfTmIsUsable(TmItem tmItem)
    {
        for (int i = 0; i < units.Count; i++)
        {
            string message = tmItem.CanBeTaught(units[i]) ? "배울 수 있다!" : "배울 수 없다!";
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
    public void UpdateStatusText()
    {
        for (int i = 0; i < units.Count; i++)
        {
            memberSlots[i].SetStatusText();
        }
    }
    public void UpdateUI()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (i == selectedItem) items[i].OnSelectionChange(true);
            else if (i == changedItem) items[i].OnSeatChange(true);
            else items[i].OnSelectionChange(false);
        }
    }
    public void ResetUI()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnResetColor();
        }
    }
}
