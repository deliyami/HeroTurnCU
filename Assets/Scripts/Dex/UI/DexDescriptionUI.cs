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
    [SerializeField] TextMeshProUGUI ability; // 특성
    [SerializeField] TextMeshProUGUI abilityDescription; // 특성
    [SerializeField] TextMeshProUGUI secondAbility; // 특성
    [SerializeField] TextMeshProUGUI secondAbilityDescription; // 특성

    public int selectedCategory = 0;
    public static DexDescriptionUI i { get; private set; }

    // 스킬 저장하는 곳
    List<DexMoveSlotUI> slotUIList;
    DexDescription dexDescription;
    UnitBase unit;
    private void Awake()
    {
        dexDescription = DexDescription.GetDexDescription();

        categoryText.text = DexDescription.DexDescriptionCategories[selectedCategory];

        i = this;
    }
    private void Start()
    {
        OnUpdated();
        // 도구 설명
        UpdateDexDescripon();
        DexState.i.DexDescriptionUIUpdate += OnUpdated;
    }
    private void OnDestroy()
    {
        DexState.i.DexDescriptionUIUpdate -= OnUpdated;
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
        int length = unit.PortraitSprite.Length;
        int randomPortraitSpriteIndex = Random.Range(0, 100) / ((unit.PortraitSprite.Length - 1) * 11);
        portraitSprite.sprite = (unit.PortraitSprite == null && length > 0) ? null : unit.PortraitSprite[randomPortraitSpriteIndex]; // 1% 확률로 네번째 얼굴이 나옴
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
            Debug.Log($"this is dex description ui {itemSlot?.Base?.Name}");
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
        else
        {
            unitDescription.SetActive(false);
            moveDescription.SetActive(false);
            toolDescription.SetActive(true);
            ability.text = unit.Ability;
            abilityDescription.text = unit.AbilityDescription;
            secondAbility.text = unit.SecondAbility;
            secondAbilityDescription.text = unit.SecondAbilityDescription;
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
