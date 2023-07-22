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
    [SerializeField] GameObject individualObject;
    [SerializeField] GameObject personalityObject;
    [Header("스킬 설명창")]
    [SerializeField] GameObject moveDescription;
    [SerializeField] TextMeshProUGUI power;
    [SerializeField] TextMeshProUGUI accuracy;
    [SerializeField] TextMeshProUGUI category;
    [SerializeField] TextMeshProUGUI pp;
    [SerializeField] TextMeshProUGUI moveDescriptionText;
    [SerializeField] GameObject itemList;
    [SerializeField] DexMoveSlotUI dexMoveSlotUI;
    [Header("도구 설명창")]
    [SerializeField] GameObject toolDescription;
    [SerializeField] TextMeshProUGUI ability; // 특성
    [SerializeField] TextMeshProUGUI abilityDescription; // 특성
    [SerializeField] TextMeshProUGUI secondAbility; // 특성
    [SerializeField] TextMeshProUGUI secondAbilityDescription; // 특성
    const int UNIT_SPRITE_LENGTH = 4;
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
        int randomPortraitSpriteIndex = Random.Range(0, 100) / ((UNIT_SPRITE_LENGTH - 1) * 11);
        // int randomPortraitSpriteIndex = 98 / ((UNIT_SPRITE_LENGTH - 1) * 11);
        // Debug.Log($"rand: {randomPortraitSpriteIndex}, unit id {unit.UnitID}");
        portraitSprite.sprite = GlobalSettings.i.UnitSprites[unit.UnitID][randomPortraitSpriteIndex]; // 1% 확률로 네번째 얼굴이 나옴
        battleSprite.sprite = unit.FrontSprite;

        var individualText = individualObject.GetComponentInChildren<TextMeshProUGUI>();
        var personalityText = personalityObject.GetComponentInChildren<TextMeshProUGUI>();
        var individualSprite = individualObject.GetComponent<Image>();
        var personalitySprite = personalityObject.GetComponent<Image>();
        if (unit.Individual[0] == 0) individualSprite.sprite = TypeDB.GetObjectByName(UnitType.Courage.ToString()).Sprite;
        else if (unit.Individual[0] == 1) individualSprite.sprite = TypeDB.GetObjectByName(UnitType.Soil.ToString()).Sprite;
        else if (unit.Individual[0] == 2) individualSprite.sprite = TypeDB.GetObjectByName(UnitType.Wind.ToString()).Sprite;
        else if (unit.Individual[0] == 3) individualSprite.sprite = TypeDB.GetObjectByName(UnitType.Strange.ToString()).Sprite;
        else individualSprite.sprite = TypeDB.GetObjectByName(UnitType.Ice.ToString()).Sprite;

        if (unit.Personality[0] == unit.Personality[1])
        {
            // 중앙값
            personalitySprite.sprite = TypeDB.GetObjectByName(UnitType.Normal.ToString()).Sprite;

        }
        else if (unit.Personality[0] % 2 == 1)
        {
            if (unit.Personality[1] % 2 == 0)
            {
                if (unit.Personality[1] > 3)
                {
                    // 무사태평, 건방
                    personalitySprite.sprite = TypeDB.GetObjectByName(UnitType.Grass.ToString()).Sprite;
                }
                else
                {
                    // 대담, 장난, 차분, 신중
                    personalitySprite.sprite = TypeDB.GetObjectByName(UnitType.Water.ToString()).Sprite;
                }
            }
            else
            {
                // 얌전, 촐랑
                personalitySprite.sprite = TypeDB.GetObjectByName(UnitType.Devil.ToString()).Sprite;
            }
        }
        else if (unit.Personality[1] % 2 == 1)
        {
            if (unit.Personality[0] > 3)
            {
                // 외로움 개구쟁이, 의젓 덜렁
                personalitySprite.sprite = TypeDB.GetObjectByName(UnitType.Psycho.ToString()).Sprite;
            }
            else
            {
                // 성급 천진난만
                personalitySprite.sprite = TypeDB.GetObjectByName(UnitType.Stone.ToString()).Sprite;
            }
        }
        else if (unit.Personality[0] >= 4)
        {
            // 겁쟁이 명랑
            personalitySprite.sprite = TypeDB.GetObjectByName(UnitType.Electric.ToString()).Sprite;
        }
        else if (unit.Personality[1] >= 4)
        {
            // 냉정 용감
            personalitySprite.sprite = TypeDB.GetObjectByName(UnitType.Sky.ToString()).Sprite;
        }
        else
        {
            // 고집 조심
            personalitySprite.sprite = TypeDB.GetObjectByName(UnitType.Fire.ToString()).Sprite;
        }
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
            ability.text = unit.Ability.Name;
            abilityDescription.text = unit.Ability.Description;
            secondAbility.text = unit.SecondAbility.Name;
            secondAbilityDescription.text = unit.SecondAbility.Description;
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
            MoveBase move = unit.LearnableMoves[selectedItem].Base;
            moveDescriptionText.text = unit.LearnableMoves[selectedItem].Base.Description;
            power.text = move.Category == MoveCategory.Status || move.Power <= 0 ? "-" : move.Power.ToString();
            accuracy.text = move.AlwaysHits ? "-" : move.Accuracy.ToString();
            category.text = move.Category == MoveCategory.Physical ? "물리" : move.Category == MoveCategory.Special ? "마법" : "특수";
            pp.text = move.PP.ToString();
        }
    }

    void ResetSelection()
    {
        selectedItem = 0;
    }
}
