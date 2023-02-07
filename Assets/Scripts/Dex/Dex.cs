using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DexCategory { Team, Enemy }
public class Dex : MonoBehaviour
{
    [SerializeField] List<UnitBase> team;
    [SerializeField] List<UnitBase> enemy;
    List<List<UnitBase>> allSlots;
    // public event Action OnUpdated;

    private void Awake()
    {
        allSlots = new List<List<UnitBase>>() { team, enemy };
        // OnUpdated?.Invoke();
    }
    public static List<string> DexCategories { get; set; } = new List<string>()
    {
        "사전 - 아군", "사전 - 적"
    };
    public List<UnitBase> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }
    public UnitBase GetItem(int unitIndex, int categoryIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);
        return currentSlots[unitIndex];
    }
    public static Dex GetDex()
    {
        return GameController.Instance.gameObject.GetComponent<Dex>();
        // return FindObjectOfType<GameController>().GetComponent<Dex>();
    }
}
