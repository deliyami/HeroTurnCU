using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;
    List<Unit> units;

    PartyMemberUI[] memberSlots;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Unit> units)
    {
        this.units = units;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < units.Count)
                memberSlots[i].SetData(units[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "동료를 고르세요!";
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

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
