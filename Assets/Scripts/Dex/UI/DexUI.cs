using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GDE.GenericSelectionUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DexUI : SelectionUI<TextSlot>
{
    [SerializeField] TextMeshProUGUI categoryText;
    [SerializeField] GameObject itemList; // 밑에 dexSlotUI를 감싸고 있는 object
    [SerializeField] DexSlotUI dexSlotUI;
    [SerializeField] Image battleSprite;
    [SerializeField] Image fieldSprite;
    [SerializeField] RectTransform typeContainer;
    [SerializeField] GameObject typeObject1;
    [SerializeField] GameObject typeObject2;
    [SerializeField] TextMeshProUGUI unitDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    private Image typeSprite1;
    private TextMeshProUGUI typeText1;
    private Image typeSprite2;
    private TextMeshProUGUI typeText2;

    private const int TYPE_OBJECT_WIDTH = 200;
    private const int TYPE_OBJECT_SPACE = 15;

    int selectedCategory = 0;

    const int itemsInViewport = 6;
    List<DexSlotUI> slotUIList;
    Dex dex;
    RectTransform dexListRect;

    private void Awake()
    {
        dex = Dex.GetDex();
        dexListRect = itemList.GetComponent<RectTransform>();

        typeSprite1 = typeObject1.GetComponent<Image>();
        typeText1 = typeObject1.GetComponentInChildren<TextMeshProUGUI>();
        typeSprite2 = typeObject2.GetComponent<Image>();
        typeText2 = typeObject2.GetComponentInChildren<TextMeshProUGUI>();
    }
    private void Start()
    {
        UpdateDexList();

        // dex.OnUpdated += UpdateDexList;
    }
    void UpdateDexList()
    {
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<DexSlotUI>();
        foreach (var itemSlot in dex.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(dexSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        SetItems(slotUIList.Select(s => s.GetComponent<TextSlot>()).ToList());

        UpdateSelectionInUI();
    }
    public override void HandleUpdate()
    {
        int prevCategry = selectedCategory;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            selectedCategory++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            selectedCategory--;

        if (selectedCategory > Dex.DexCategories.Count - 1)
            selectedCategory = 0;
        else if (selectedCategory < 0)
            selectedCategory = Dex.DexCategories.Count - 1;

        if (prevCategry != selectedCategory)
        {
            ResetSelection();
            categoryText.text = Dex.DexCategories[selectedCategory];
            UpdateDexList();
        }
        base.HandleUpdate();
    }
    public override void UpdateSelectionInUI()
    {
        base.UpdateSelectionInUI();

        var units = dex.GetSlotsByCategory(selectedCategory);
        if (units.Count > 0)
        {
            var unit = units[selectedItem];
            var type1 = TypeDB.GetObjectByName(unit.Type1.ToString());
            var type2 = TypeDB.GetObjectByName(unit.Type2.ToString());

            battleSprite.sprite = unit.FrontSprite;
            fieldSprite.sprite = unit.SmallSprite;
            typeSprite1.sprite = type1.Sprite;
            typeText1.text = type1.Name;
            var sd = typeContainer.sizeDelta;
            if (type2.UnitType == UnitType.None)
            {
                if (typeObject2.activeSelf) typeObject2.SetActive(false);

                sd.x = TYPE_OBJECT_WIDTH;
                typeContainer.sizeDelta = sd;
            }
            else
            {
                if (!typeObject2.activeSelf) typeObject2.SetActive(true);

                sd.x = TYPE_OBJECT_WIDTH * 2 + TYPE_OBJECT_SPACE;
                typeContainer.sizeDelta = sd;

                typeSprite2.sprite = type2.Sprite;
                typeText2.text = type2.Name;
            }
            unitDescription.text = unit.Description;
        }
        HandleScrolling();
    }
    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewport) return;

        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport / 2, 0, selectedItem) * slotUIList[0].Height;
        // itemList.GetComponent<RectTransform>();
        dexListRect.localPosition = new Vector2(dexListRect.localPosition.x, scrollPos);

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

        // 스프라이트
        battleSprite.sprite = null;
        fieldSprite.sprite = null;
        typeSprite1.sprite = null;
        typeText1.text = "";
        typeSprite2.sprite = null;
        typeText2.text = "";
    }
    // public UnitBase SelectedUnit => dex.GetItem(selectedItem, selectedCategory);
    public int SelectedCategory => selectedCategory;
}
