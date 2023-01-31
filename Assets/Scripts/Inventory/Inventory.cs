using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory { Items, Balls, Tms }
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
        "회복 아이템", "귀환 아이템", "기술"
    };

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }
    public ItemBase UseItem (int itemIndex, Unit selectedUnit, int selectedCategory)
    {
        var currentSlots = GetSlotsByCategory(selectedCategory);
        var item = currentSlots[itemIndex].Item;
        bool itemUsed = item.Use(selectedUnit);
        if (itemUsed)
        {
            RemoveItem(item, selectedCategory);
            return item;
        }
        return null;
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

    public ItemBase Item => item;
    public int Count { 
        get => count;
        set => count = value;
    }
}