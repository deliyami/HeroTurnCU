using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialog dialog)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);

        Fader.i.FadeIn(0.5f);

        var playerParty = player.GetComponent<UnitParty>();
        playerParty.Units.ForEach(p => p.Heal());
        playerParty.PartyUpdated();

        Fader.i.FadeOut(0.5f);
    }
}
