using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GDE.GenericSelectionUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DexDescriptionUI : SelectionUI<DexMoveSlotUI>
{
    [SerializeField] TextMeshProUGUI unitName;
    [SerializeField] TextMeshProUGUI categoryText;
    [Header("유닛 설명창")]
    [SerializeField] GameObject unitDescription;
    [SerializeField] TextMeshProUGUI unitDescriptionText;
    [SerializeField] Image portraitSprite;
    [SerializeField] Image battleSprite;
    [SerializeField] TextMeshProUGUI individualText;
    [SerializeField] TextMeshProUGUI personalityText;
    [Header("스킬 설명창")]
    [SerializeField] GameObject moveDescription;
    [SerializeField] TextMeshProUGUI moveDescriptionText;
    [SerializeField] GameObject itemList;
    [SerializeField] DexMoveSlotUI dexMoveSlotUI;
    [Header("도구 설명창")]
    [SerializeField] GameObject toolDescription;

    public int selectedCategory = 0;

    // 스킬 저장하는 곳
    List<DexMoveSlotUI> slotUIList;
    DexDescription dexDescription;
    UnitBase unit;
    private void Awake()
    {
        dexDescription = DexDescription.GetDexDescription();

        categoryText.text = DexDescription.DexDescriptionCategories[selectedCategory];
    }
    private void Start()
    {
        OnUpdated();
        // 도구 설명
        UpdateDexDescripon();
        DexState.i.DexDescriptionUIUpdate += OnUpdated;
    }
    private void OnUpdated()
    {
        // 유닛 설명
        UpdateUnitDescription();
        // 스킬 설명
        UpdateDexMoveList();
    }
    void UpdateUnitDescription()
    {
        unit = DexState.i.CurrentUnit;
        unitName.text = unit.Name;
        unitDescriptionText.text = unit.Description;
        portraitSprite.sprite = (unit.PortraitSprite == null && unit.PortraitSprite.Length > 0) ? null : unit.PortraitSprite[Random.Range(0, unit.PortraitSprite.Length - 1)];
        battleSprite.sprite = unit.FrontSprite;

        individualText.text = $"{unit.Name}은(는) {IndividualChart.GetIndividualText(unit.Individual)}.";
        personalityText.text = $"{unit.Name}은(는) {PersonalityChart.GetPersonalityText(unit.Personality)} 성격이다.";
    }
    void UpdateDexMoveList()
    {
        slotUIList = itemList.GetComponents<DexMoveSlotUI>().ToList();
        for (int i = 0; i < slotUIList.Count; i++)
        {
            slotUIList[i].SetData(unit.LearnableMoves[i].Base);
        }
        SetItems(slotUIList);

        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<DexMoveSlotUI>();
        foreach (var itemSlot in unit.LearnableMoves)
        {
            var slotUIObj = Instantiate(dexMoveSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot.Base);

            slotUIList.Add(slotUIObj);
        }

        SetItems(slotUIList.Select(s => s.GetComponent<DexMoveSlotUI>()).ToList());

        UpdateSelectionInUI();
    }
    void UpdateDexDescripon()
    {
        if (selectedCategory == 0)
        {

            unitDescription.SetActive(true);
            moveDescription.SetActive(false);
            toolDescription.SetActive(false);
        }
        else if (selectedCategory == 1)
        {
            unitDescription.SetActive(false);
            moveDescription.SetActive(true);
            toolDescription.SetActive(false);
        }
        else if (selectedCategory == 2)
        {
            unitDescription.SetActive(false);
            moveDescription.SetActive(false);
            toolDescription.SetActive(true);
        }
    }
    public override void HandleUpdate()
    {
        int prevCategry = selectedCategory;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            selectedCategory++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            selectedCategory--;

        if (selectedCategory > DexDescription.DexDescriptionCategories.Count - 1)
            selectedCategory = 0;
        else if (selectedCategory < 0)
            selectedCategory = DexDescription.DexDescriptionCategories.Count - 1;

        if (prevCategry != selectedCategory)
        {
            ResetSelection();
            categoryText.text = DexDescription.DexDescriptionCategories[selectedCategory];
            UpdateDexDescripon();
        }

        base.HandleUpdate();

        if (selectedCategory == 1)
        {
            moveDescriptionText.text = unit.LearnableMoves[selectedItem].Base.Description;
        }
    }

    void ResetSelection()
    {
        selectedItem = 0;
    }
}
