using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGiver : MonoBehaviour, ISavable
{
    [SerializeField] Unit unitToGive;
    [SerializeField] Dialog dialog;

    bool used = false;

    public IEnumerator GiveUnit(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);

        unitToGive.Init();
        player.GetComponent<UnitParty>().AddUnit(unitToGive);

        used = true;
        
        yield return DialogManager.Instance.ShowDialogText($"{unitToGive.Base.Name}와(과) 동료가 되었다!");
    }

    public bool CanBeGiven()
    {
        return unitToGive != null && !used;
    }

    public object CaptureState()
    {
        return used;
    }

    public void RestoreState(object state)
    {
        used = (bool)state;
    }
}
