using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum InventoryUIState { ItemSelection, PartySelection, MoveToForget, Busy }

public class InventoryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI categoryText;
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveSelectionUI moveSelectionUI;

    Action<ItemBase> onItemUsed;

    int selectedItem = 0;
    int selectedCategory = 0;

    MoveBase moveToLearn;

    InventoryUIState state;

    const int itemsInViewport = 6;
    List<ItemSlotUI> slotUIList;
    Inventory inventory;
    RectTransform itemListRect;
    private void Awake() 
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }
    private void Start()
    {
        UpdateItemList();

        inventory.OnUpdated += UpdateItemList;
    }
    void UpdateItemList()
    {
        // 기존 내용물 지우기
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }
    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed = null)
    {
        this.onItemUsed = onItemUsed;

        if (state == InventoryUIState.ItemSelection)
        {
            int prevSelection = selectedItem;
            int prevCategry = selectedCategory;

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                --selectedItem;
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                ++selectedCategory;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                --selectedCategory;
            }
            if (selectedCategory > Inventory.ItemCategories.Count - 1)
                selectedCategory = 0;
            else if (selectedCategory < 0)
                selectedCategory = Inventory.ItemCategories.Count - 1;
            // selectedCategory = Mathf.Clamp(selectedCategory, 0, Inventory.ItemCategories.Count - 1);
            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);
            if (prevCategry != selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemList();
            } else if (prevSelection != selectedItem)
                UpdateItemSelection();
            if (Input.GetButtonDown("Submit"))
            {
                StartCoroutine(ItemSelected());
            }
            else if (Input.GetButtonDown("Cancel"))
            {
                onBack?.Invoke();
            }
        }
        else if (state == InventoryUIState.PartySelection)
        {
            // 파티 선택
            Action onSelected = () =>
            {
                // 아이템 사용하기
                StartCoroutine(UseItem());
            };
            Action onBackPartyScreen = () =>
            {
                // 뒤로 나가기
                ClosePartyScreen();
            };
            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
        else if (state == InventoryUIState.MoveToForget)
        {
            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            };
            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
    }

    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);
        var cancelDialog = "여기서 쓰는 물건이 아니다!";
        if (GameController.Instance.State == GameState.Battle)
        {
            // 전투에서
            if (!item.CanBeUsedInBattle)
            {
                yield return DialogManager.Instance.ShowDialogText(cancelDialog);
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            // 일반 메뉴
            if (!item.CanBeUsedOutsideBattle)
            {
                yield return DialogManager.Instance.ShowDialogText(cancelDialog);                
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        if (selectedCategory == (int)ItemCategory.Balls)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();
            // if (item is TmItem)
            //     partyScreen.ShowIfTmIsUsable(item as TmItem);
        }
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;
        
        yield return HandleTmItems();

        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if (usedItem != null)
        {
            if (usedItem is RecoveryItem)
            {
                yield return DialogManager.Instance.ShowDialogText($"{usedItem.Name}을(를) 사용했다!");
            }
            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            if (selectedCategory == (int)ItemCategory.Items)
                yield return DialogManager.Instance.ShowDialogText($"그것을 사용 할 수 없다!");
        }

        ClosePartyScreen();
    }

    IEnumerator HandleTmItems()
    {
        var tmItem = inventory.GetItem(selectedItem, selectedCategory) as TmItem;
        if (tmItem == null)
            yield break;
        var unit = partyScreen.SelectedMember;

        // if (unit.HasMove(tmItem.Move))
        // {
        //     yield return DialogManager.Instance.ShowDialogText($"{unit.Base.Name}은(는) 이미 알고있다!");
        //     yield break;
        // }
        // if (tmItem.CanBeTaught(unit))
        // {
        //     yield return DialogManager.Instance.ShowDialogText($"{unit.Base.Name}은(는) 배울 수 없다!");
        //     yield break;
        // }
        // if (unit.Moves.Count < UnitBase.MaxNumOfMoves)
        // {
        //     unit.LearnMove(tmItem.Move);
        //     yield return DialogManager.Instance.ShowDialogText($"{unit.Base.Name}은(는) {tmItem.Move.Name}을(를) 배웠다!");
        //     yield return DialogManager.Instance.ShowDialogText($"전투에는 전혀 도움 되지 않는다!");
        // }
        // else
        // {
        //     yield return DialogManager.Instance.ShowDialogText($"{unit.Base.Name}은(는) {tmItem.Move.Name}을(를) 배우고 싶다!");
        //     yield return DialogManager.Instance.ShowDialogText($"하지만 이미 배울 만큼 배웠다!");
        //     yield return DialogManager.Instance.ShowDialogText($"기존 배우던 것을 잊어야 한다!");
        //     yield return ChooseMoveToForget(unit, tmItem.Move);
        //     yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
        //     yield return new WaitForSeconds(2f);
        // }

        yield return DialogManager.Instance.ShowDialogText($"{unit.Base.Name}은(는) {tmItem.Name}을(를) 사용하며 여유를 보냈다.");
        yield return DialogManager.Instance.ShowDialogText($"전투에는 전혀 도움 되지 않는다!");
    }

    IEnumerator ChooseMoveToForget(Unit unit, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogManager.Instance.ShowDialogText($"기술 배우는 창, 보인다면 버그입니다.", true, false);
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(unit.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = InventoryUIState.MoveToForget;
    }

    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);
        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }

        if (slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }
        HandleScrolling();
    }
    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewport) return;
        
        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport / 2, 0, selectedItem) * slotUIList[0].Height;
        // itemList.GetComponent<RectTransform>();
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > itemsInViewport / 2;
        upArrow.gameObject.SetActive(showUpArrow);
        bool showDownArrow = selectedItem + itemsInViewport / 2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }
    void ResetSelection()
    {
        selectedItem = 0;
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
    }
    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }
    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;

        // partyScreen.ClearMemberSlotMessages();
        partyScreen.gameObject.SetActive(false);
    }

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var unit = partyScreen.SelectedMember;

        DialogManager.Instance.CloseDialog();
        moveSelectionUI.gameObject.SetActive(false);
        if (moveIndex == UnitBase.MaxNumOfMoves)
        {
            // 배우지 않음
            yield return DialogManager.Instance.ShowDialogText($"배우지 않는다. 이것도 보이면 버그다...");
        }
        else
        {
            // 새로운 스킬 배움
            // var selectedMove = playerUnit.Unit.Moves[moveIndex].Base;
            unit.Moves[moveIndex] = new Move(moveToLearn);
            yield return DialogManager.Instance.ShowDialogText($"아님 핵을 썼던가...");
        }
        moveToLearn = null;
        state = InventoryUIState.ItemSelection;
    }
}
