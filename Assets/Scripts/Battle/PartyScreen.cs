using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GDE.GenericSelectionUI;

public class PartyScreen : SelectionUI<TextSlot>
{
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] GameObject typeObject1;
    [SerializeField] GameObject typeObject2;
    [SerializeField] TextMeshProUGUI ability1;
    [SerializeField] TextMeshProUGUI ability2;
    [SerializeField] GameObject moveObject;
    List<Unit> units;
    UnitParty party;
    public Unit SelectedMember => units[selectedItem];
    public int changedItem = -1;

    Image typeImage1;
    TextMeshProUGUI typeText1;
    Image typeImage2;
    TextMeshProUGUI typeText2;
    PartyScreenMove[] moves;

    PartyMemberUI[] memberSlots;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        SetSelectionSettings(SelectionType.List, 1);

        typeImage1 = typeObject1.GetComponentInChildren<Image>();
        typeText1 = typeObject1.GetComponentInChildren<TextMeshProUGUI>();
        typeImage2 = typeObject2.GetComponentInChildren<Image>();
        typeText2 = typeObject2.GetComponentInChildren<TextMeshProUGUI>();

        moves = GetComponentsInChildren<PartyScreenMove>();

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

        ResetMessage();
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
            if (i == changedItem) items[i].OnSeatChange(true);
            else if (i == selectedItem) items[i].OnSelectionChange(true);
            else items[i].OnSelectionChange(false);
            // if (i == selectedItem) items[i].OnSelectionChange(true);
            // else if (i == changedItem) items[i].OnSeatChange(true);
            // else items[i].OnSelectionChange(false);
        }
        TypeBase typeBase1 = TypeDB.GetObjectByName(units[selectedItem].Base.Type1.ToString());
        TypeBase typeBase2 = TypeDB.GetObjectByName(units[selectedItem].Base.Type2.ToString());
        typeImage1.sprite = typeBase1.Sprite;
        typeText1.text = typeBase1.Name;
        typeImage2.sprite = typeBase2.Sprite;
        typeText2.text = typeBase2.Name;

        for (int i = 0; i < units[selectedItem].Moves.Count; i++)
        {
            var partyScreenMove = moves[i];
            var unitMove = units[selectedItem].Moves[i];
            partyScreenMove.SetName(unitMove.Base.Name);
            partyScreenMove.SetPP($"{unitMove.PP}/{unitMove.Base.PP}");
            partyScreenMove.SetType(unitMove.Base.Type);
            partyScreenMove.SetSprite(unitMove.Base.Type);
        }
    }
    public void ResetUI()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].OnResetColor();
        }
    }
    public void ResetMessage()
    {
        messageText.text = "동료를 선택하세요!";
    }
}
