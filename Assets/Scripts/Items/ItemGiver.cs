using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour, ISavable
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;
    [SerializeField] Dialog dialog;
    [SerializeField] QuestBase questToCheck;
    QuestList questList;

    bool used = false;
    private void Start()
    {
        questList = QuestList.GetQuestList();
    }

    public IEnumerator GiveItem(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);
        player.GetComponent<Inventory>().AddItem(item, count);

        used = true;
        if (dialog.Lines.Count > 0)
        {
            AudioManager.i.PlaySfx(AudioId.ItemObtained, pauseMusic: true);
            yield return DialogManager.Instance.ShowDialogText($"{item.Name}을(를) 받았다!");
        }
    }

    public bool CanBeGiven()
    {

        return item != null && !used && questList.IsStarted(questToCheck.Name);
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
