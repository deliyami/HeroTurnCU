using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory { None, Items, Balls, Tms }
public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> ballSlots;
    [SerializeField] List<ItemSlot> tmSlots;
    List<List<ItemSlot>> allSlots;
    public event Action OnUpdated;
    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { slots, ballSlots, tmSlots };
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
        bool itemUsed = item.Use(selectedUnit);
        if (itemUsed)
        {
            if (!item.IsReusable)
                RemoveItem(item, selectedCategory);
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

    public void RemoveItem(ItemBase item, int category)
    {
        var currentSlots = GetSlotsByCategory(category);
        var itemSlot = currentSlots.First(slots => slots.Item == item);
        // 아이템 사용은 무제한, 사용 횟수에 따라 엔딩 패널티를 줄 것
        // 한 개 사용당 1턴으로 치고... 등등
        itemSlot.Count++;
        // itemSlot.Count--;
        // if (itemSlot.Count == 0)
        //     currentSlots.Remove(itemSlot);

        OnUpdated?.Invoke();
    }
    ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem)
            return ItemCategory.Items;
        else if (item is BallItem)
            return ItemCategory.Balls;
        else if (item is TmItem)
            return ItemCategory.Tms;
        return ItemCategory.None;
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemBase Item {
        get => item;
        set => item = value;
    }
    public int Count { 
        get => count;
        set => count = value;
    }
}