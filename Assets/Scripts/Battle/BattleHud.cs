using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI HPText;
    [SerializeField] TextMeshProUGUI MaxHPText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] Image statusSprite;
    [SerializeField] Color psnColor;
    [SerializeField] Sprite psnSprite;
    [SerializeField] Color brnColor;
    [SerializeField] Sprite brnSprite;
    [SerializeField] Color slpColor;
    [SerializeField] Sprite slpSprite;
    [SerializeField] Color parColor;
    [SerializeField] Sprite parSprite;
    [SerializeField] Color frzColor;
    [SerializeField] Sprite frzSprite;
    // [SerializeField] Status status;

    Unit _unit;
    Dictionary<ConditionID, Color> statusColors;
    Dictionary<ConditionID, Sprite> statusSprites;

    public TextMeshProUGUI NameText {
        get { return nameText; }
    }

    public virtual void SetData(Unit unit, bool parentComponent = true)
    {
        if (_unit != null)
        {
            _unit.OnStatusChanged -= SetStatusText;
            _unit.OnHPChanged -= parentComponent?UpdateHP:UpdateData;
        }

        _unit = unit;
        UpdateData();
        // Debug.Log($"여기는 battleHud {unit.Base.Name}: {unit.HP}/{unit.MaxHP}");

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor},
            {ConditionID.brn, brnColor},
            {ConditionID.slp, slpColor},
            {ConditionID.par, parColor},
            {ConditionID.frz, frzColor},
        };

        statusSprites = new Dictionary<ConditionID, Sprite>()
        {
            {ConditionID.psn, psnSprite},
            {ConditionID.brn, brnSprite},
            {ConditionID.slp, slpSprite},
            {ConditionID.par, parSprite},
            {ConditionID.frz, frzSprite},
        };

        SetStatusText();
        _unit.OnStatusChanged += SetStatusText;
        _unit.OnHPChanged += parentComponent?UpdateHP:UpdateData;
    }

    protected void UpdateData()
    {
        nameText.text = _unit.Base.Name;
        SetLevel();
        hpBar.SetHP((float) _unit.HP / _unit.MaxHP);
        HPText.text = $"{_unit.HP}";
        MaxHPText.text = $"/{_unit.MaxHP}";
    }

    void SetStatusText()
    {
        if (_unit.Status == null)
        {
            statusSprite.color = new Color(1f, 1f, 1f, 0f);
            statusText.text = "";
        }
        else
        {
            statusSprite.color = new Color(1f, 1f, 1f, 1f);
            // switch(_unit.Status.ID.ToString()){
            //     case "none":
            //         break;
            // }
            statusText.color = statusColors[_unit.Status.ID];
            statusText.text = ConditionDB.Conditions[_unit.Status.ID].Name;
            statusSprite.sprite = statusSprites[_unit.Status.ID];
        }
    }

    public void SetLevel()
    {
        levelText.text = "Lvl " + _unit.Level;
    }
    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1f, 1f);
    }

    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break;
        if (reset)
            expBar.transform.localScale = new Vector3(0, 1f, 1f);
        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp()
    {
        int currLevelExp = _unit.Base.GetExpForLevel(_unit.Level);
        int nextLevelExp = _unit.Base.GetExpForLevel(_unit.Level + 1);

        float normalizedExp = (_unit.Exp -currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }
    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }
    public virtual IEnumerator UpdateHPAsync()
    {
        HPText.text = $"{_unit.HP}";
        MaxHPText.text = $"/{_unit.MaxHP}";
        yield return hpBar.SetHPSmooth((float) _unit.HP / _unit.MaxHP);
    }

    public IEnumerator WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }
}
