using GDE.GenericSelectionUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleSituationUI : SelectionUI<TextSlot>
{
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] Image unitImage;
    [SerializeField] GameObject typeObject1;
    [SerializeField] GameObject typeObject2;

    [SerializeField] TextMeshProUGUI weather;
    [SerializeField] TextMeshProUGUI field;
    [SerializeField] TextMeshProUGUI room;
    [SerializeField] TextMeshProUGUI teamSpecial; // 리플렉터 빛의장막 순풍
    [SerializeField] TextMeshProUGUI enemySpecial; // 리플렉터 빛의장막 순풍 중간중간 \n넣을 것
    [SerializeField] List<TextMeshProUGUI> Boost = new List<TextMeshProUGUI>(7); //
    // 0 = Attack
    // 1 = Defense
    // 2 = SpAttack
    // 3 = SpDefense
    // 4 = Speed
    // 5 = Accuracy
    // 6 = Evasion
    Image typeImage1;
    TextMeshProUGUI typeText1;
    Image typeImage2;
    TextMeshProUGUI typeText2;
    List<BattleUnit> units;
    public bool isFirstAccess = true;
    private void Start()
    {
        SetBattleSituation();
    }
    public void SetUnit(List<BattleUnit> units)
    {
        this.units = units;
        this.isFirstAccess = true;

        typeImage1 = typeObject1.GetComponentInChildren<Image>();
        typeText1 = typeObject1.GetComponentInChildren<TextMeshProUGUI>();
        typeImage2 = typeObject2.GetComponentInChildren<Image>();
        typeText2 = typeObject2.GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void HandleUpdate()
    {
        UpdateSelectionTimer();
        int prevSelection = selectedItem;
        HandleHorizonSelection();
        // selectedItem = Mathf.Clamp(selectedItem, 0, units.Count - 1);
        if (selectedItem > units.Count - 1)
            selectedItem = 0;
        else if (selectedItem < 0)
            selectedItem = units.Count - 1;

        if (selectedItem != prevSelection || isFirstAccess) UpdateSelectionInUI();

        // if (Input.GetKeyDown(KeyCode.C) && GameController.Instance.StateMachine.CurrentState != DexState.i && GameController.Instance.StateMachine.CurrentState != DexDescriptionState.i)
        // {
        //     // if (GameController.Instance.StateMachine.CurrentState == BattleState.i)
        //     Debug.Log("상태창 호출 중");
        //     GameController.Instance.StateMachine.Push(DexState.i);
        // }

        if (Input.GetButtonDown("Cancel"))
            HandleCancel();
    }
    protected void HandleHorizonSelection()
    {
        // TODO: 메뉴에는 이게 맞는데, 파티창같이 좌우로도 움직일 수 있는건 다른것으로 고쳐야 함
        float v = Input.GetAxis("Horizontal");
        if (selectionTimer == 0 && Mathf.Abs(v) > 0.2f)
        {
            selectedItem += -(int)Mathf.Sign(v);
            selectionTimer = 1 / selectionSpeed;
        }
    }
    public override void UpdateSelectionInUI()
    {
        isFirstAccess = false;
        SetBattleSituation();

        // base.UpdateSelectionInUI();
        name.text = units[selectedItem].Unit.Base.Name;
        typeImage1.sprite = TypeDB.GetObjectByName(units[selectedItem].Unit.Base.Type1.ToString()).Sprite;
        typeText1.text = TypeDB.GetObjectByName(units[selectedItem].Unit.Base.Type1.ToString()).Name;
        typeImage2.sprite = TypeDB.GetObjectByName(units[selectedItem].Unit.Base.Type2.ToString()).Sprite;
        typeText2.text = TypeDB.GetObjectByName(units[selectedItem].Unit.Base.Type2.ToString()).Name;
        unitImage.sprite = units[selectedItem].Unit.Base.FrontSprite;

        for (int i = 0; i < Boost.Count; i++)
        {
            Boost[i].text = units[selectedItem].Unit.GetStatBoost(i).ToString();
        }
    }
    private void SetBattleSituation()
    {
        List<string> teamSpecials = new List<string>();
        if (BattleSystem.i.Field.PlayerReflect != null) teamSpecials.Add("리플렉터 발동 중");
        if (BattleSystem.i.Field.PlayerLightScreen != null) teamSpecials.Add("빛의 장막 발동 중");
        if (BattleSystem.i.Field.PlayerTailwind != null) teamSpecials.Add("바람이 부는 중");
        string combinedTeamSpecial = string.Join("\n", teamSpecials);
        teamSpecial.text = combinedTeamSpecial;

        List<string> enemySpecials = new List<string>();
        if (BattleSystem.i.Field.EnemyReflect != null) enemySpecials.Add("리플렉터 발동 중");
        if (BattleSystem.i.Field.EnemyLightScreen != null) enemySpecials.Add("빛의 장막 발동 중");
        if (BattleSystem.i.Field.EnemyTailwind != null) enemySpecials.Add("바람이 부는 중");
        string combinedEnemySpecial = string.Join("\n", enemySpecials);
        enemySpecial.text = combinedEnemySpecial;

        SetField();
    }
    public void SetField()
    {
        weather.text = BattleSystem.i.Field.Weather?.condition.Name ?? "없음";
        field.text = BattleSystem.i.Field.field?.condition.Name ?? "없음";
        room.text = BattleSystem.i.Field.Room?.condition.Name ?? "없음";
    }
}
