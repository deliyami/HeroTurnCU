using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;
    [SerializeField] Dialog dialog;

    bool used = false;

    public IEnumerator GiveItem(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);
        player.GetComponent<Inventory>().AddItem(item, count);

        used = true;
        
        yield return DialogManager.Instance.ShowDialogText($"{item.Name}을(를) 받았다!");
    }

    public bool CanBeGiven()
    {
        return item != null && count > 0 && !used;
    }
}
