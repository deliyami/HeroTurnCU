using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopState { Menu, Buying, Selling, Busy }
public class ShopController : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;
    public event Action OnStart;
    public event Action OnFinish;
    ShopState state;
    public static ShopController i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    Inventory inventory;
    private void Start()
    {
        inventory = Inventory.GetInventory();
    }
    public IEnumerator StartTrading(Merchant merchant)
    {
        OnStart?.Invoke();
        yield return StartMenuState();
    }

    IEnumerator StartMenuState()
    {
        state = ShopState.Menu;
        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText("사고 싶은게 있어?",
            // waitForInput: false,
            choices: new List<string>() { "사기", "팔기", "닫기" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex );
        if (selectedChoice == 0)
        {
            state = ShopState.Buying;
        }
        else if (selectedChoice == 1)
        {
            state = ShopState.Selling;
            inventoryUI.gameObject.SetActive(true);
        }
        else if (selectedChoice == 2)
        {
            OnFinish?.Invoke();
            yield break;
        }
    }
    public void HandleUpdate()
    {
        if (state == ShopState.Selling)
        {
            inventoryUI.HandleUpdate(onBackFromSelling, (selectedItem) => StartCoroutine(SellItem(selectedItem)));
        }
    }

    void onBackFromSelling()
    {
        inventoryUI.gameObject.SetActive(false);
        StartCoroutine(StartMenuState());
    }
    IEnumerator SellItem(ItemBase item)
    {
        state = ShopState.Busy;

        if (!item.IsSellable)
        {
            yield return DialogManager.Instance.ShowDialogText("팔 수 없다!");
            state = ShopState.Selling;
            yield break;
        }
        float sellingPrice = item.Price;

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText($"가격: {sellingPrice}",
            // waitForInput: false,
            choices: new List<string>() { "판다", "거절한다" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex );
        if (selectedChoice == 0)
        {
            inventory.RemoveItem(item);
        }
        state = ShopState.Selling;
    }
}
