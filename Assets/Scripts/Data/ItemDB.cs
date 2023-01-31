using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB
{
    static Dictionary<string, ItemBase> items;

    public static void Init()
    {
        items = new Dictionary<string, ItemBase>();
        var itemArray = Resources.LoadAll<ItemBase>("");
        foreach (var item in itemArray)
        {
            if (items.ContainsKey(item.Name))
            {
                Debug.LogError($"같은 아이템이 존재함 {item.Name}");
                continue;
            }
            items[item.Name] = item;
        }
    }

    public static ItemBase GetItemByName(string name)
    {
        if (!items.ContainsKey(name))
        {
            Debug.LogError($"해당 아아템은 없습니다{name}");
            return null;
        }

        return items[name];
    }
}
