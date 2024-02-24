using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGiver : MonoBehaviour, ISavable
{
    [SerializeField] Unit unitToGive;
    [SerializeField] Dialog dialog;
    [SerializeField] QuestBase questToCheck;
    QuestList questList;

    bool used = false;
    private void Start()
    {
        questList = QuestList.GetQuestList();
    }

    public IEnumerator GiveUnit(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);

        unitToGive.Init();
        player.GetComponent<UnitParty>().AddUnit(unitToGive);

        used = true;

        AudioManager.i.PlaySfx(AudioId.UnitObtained, pauseMusic: true);

        yield return DialogManager.Instance.ShowDialogText($"{unitToGive.Base.Name}와(과) 동료가 되었다!");
    }

    public bool CanBeGiven()
    {
        return unitToGive != null && !used && questList.IsStarted(questToCheck.Name);
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
