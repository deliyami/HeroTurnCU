using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest
{
    public QuestBase Base { get; private set; }
    public QuestStatus Status { get; private set; }

    public Quest(QuestBase _base)
    {
        Base = _base;
    }

    public IEnumerator StartQuest()
    {
        Status = QuestStatus.Started;

        yield return DialogManager.Instance.ShowDialog(Base.StartDialogue);
    }

    public IEnumerator CompletedQuest(Transform player)
    {
        Status = QuestStatus.Completed;

        yield return DialogManager.Instance.ShowDialog(Base.CompletedDialogue);

        var inventory = Inventory.GetInventory();
        if (Base.RequiredItem != null)
        {
            inventory.RemoveItem(Base.RequiredItem);
            
        }

        if (Base.RewardItem != null)
        {
            inventory.AddItem(Base.RewardItem);
            
            yield return DialogManager.Instance.ShowDialogText($"{Base.RewardItem.Name}을(를) 얻었다!");
        }
    }

    public bool CanBeCompleted()
    {
        var inventory = Inventory.GetInventory();
        if (Base.RequiredItem != null)
        {
            if (!inventory.HasItem(Base.RequiredItem))
                return false;
        }
        return true;
    }
}

public enum QuestStatus { None, Started, Completed }
