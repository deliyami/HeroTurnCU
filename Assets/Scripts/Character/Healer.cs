using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialog dialog)
    {
        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText("체력을 회복하시겠습니까",
            waitForInput: false,
            choices: new List<string>() { "네", "아니오" },
            onChoiceSelected: (choiceIndex) => selectedChoice = choiceIndex);
        if (selectedChoice == 0)
        {
            // 네
            yield return Fader.i.FadeIn(0.5f);

            var playerParty = player.GetComponent<UnitParty>();
            playerParty.Units.ForEach(p => p.Heal());
            playerParty.PartyUpdated();

            yield return Fader.i.FadeOut(0.5f);
            yield return DialogManager.Instance.ShowDialogText("회복 되었다!");
        }
        else if (selectedChoice == 1)
        {
            // 아니오
            yield return DialogManager.Instance.ShowDialogText("회복 안했다.");
        }

    }
}
