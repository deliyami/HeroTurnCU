using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory { Items, Balls, Tms, Quest }
public class Inventory : MonoBehaviour, ISavable
{
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> ballSlots;
    [SerializeField] List<ItemSlot> tmSlots;
    [SerializeField] List<ItemSlot> questSlots;
    List<List<ItemSlot>> allSlots;
    public event Action OnUpdated;
    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { slots, ballSlots, tmSlots, questSlots };
    }
    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "회복 아이템", "귀환 아이템", "즐길거리"
    };

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);
        return currentSlots[itemIndex].Item;
    }
    public ItemBase UseItem(int itemIndex, Unit selectedUnit, int selectedCategory)
    {
        var item = GetItem(itemIndex, selectedCategory);
        return UseItem(item, selectedUnit);
    }
    public ItemBase UseItem(ItemBase item, Unit selectedUnit)
    {
        bool itemUsed = item.Use(selectedUnit);
        if (itemUsed)
        {
            if (!item.IsReusable)
                RemoveItem(item);
            return item;
        }
        return null;
    }
    public void AddItem(ItemBase item, int count = 1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
        if (itemSlot != null)
        {
            itemSlot.Count += count;
        }
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }
        OnUpdated?.Invoke();
    }

    public int GetItemCount(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
        if (item != null)
            return itemSlot.Count;
        return 0;
    }

    public void RemoveItem(ItemBase item, int countToRemove = 1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);
        var itemSlot = currentSlots.First(slots => slots.Item == item);
        // 아이템 사용은 무제한, 사용 횟수에 따라 엔딩 패널티를 줄 것
        // 한 개 사용당 1턴으로 치고... 등등
        itemSlot.Count++;
        // itemSlot.countToRemove--;
        // if (itemSlot.countToRemove == 0)
        //     currentSlots.Remove(itemSlot);

        OnUpdated?.Invoke();
    }
    public bool HasItem(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        return currentSlots.Exists(slot => slot.Item == item);
    }
    ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem || item is EvolutionItem)
            return ItemCategory.Items;
        else if (item is BallItem)
            return ItemCategory.Balls;
        else if (item is TmItem)
            return ItemCategory.Tms;
        return ItemCategory.Quest;
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public object CaptureState()
    {
        var saveData = new InventorySaveData()
        {
            items = slots.Select(i => i.GetSaveData()).ToList(),
            balls = ballSlots.Select(i => i.GetSaveData()).ToList(),
            tms = tmSlots.Select(i => i.GetSaveData()).ToList(),
            quests = questSlots.Select(i => i.GetSaveData()).ToList(),
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;

        slots = saveData.items.Select(i => new ItemSlot(i)).ToList();
        ballSlots = saveData.balls.Select(i => new ItemSlot(i)).ToList();
        tmSlots = saveData.tms.Select(i => new ItemSlot(i)).ToList();
        questSlots = saveData.quests.Select(i => new ItemSlot(i)).ToList();

        allSlots = new List<List<ItemSlot>>() { slots, ballSlots, tmSlots, questSlots };

        OnUpdated?.Invoke();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;
    public ItemSlot()
    {

    }
    public ItemSlot(ItemSaveData saveData)
    {
        item = ItemDB.GetObjectByName(saveData.name);
        count = saveData.count;

    }
    public ItemSaveData GetSaveData()
    {
        var saveDate = new ItemSaveData()
        {
            name = item.name,
            count = this.count
        };
        return saveDate;
    }
    public ItemBase Item
    {
        get => item;
        set => item = value;
    }
    public int Count
    {
        get => count;
        set => count = value;
    }
}

[Serializable]
public class ItemSaveData
{
    public string name;
    public int count;
}

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items;
    public List<ItemSaveData> balls;
    public List<ItemSaveData> tms;
    public List<ItemSaveData> quests;
}