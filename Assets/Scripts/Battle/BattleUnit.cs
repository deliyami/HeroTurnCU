using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    // [SerializeField] UnitBase _base;
    // [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public bool IsPlayerUnit {
        get { return isPlayerUnit;}
    }

    public BattleHud Hud {
        get { return hud; }
    }

    // // 개체치 31 {HP, attack, defense, spAttack, spDefense, speed}
    // [SerializeField] int[] tribe;
    // // 노력치 0~252 {HP, attack, defense, spAttack, spDefense, speed} /4해서 더해야 하는데... 적혀있네
    // [SerializeField] int[] effort;
    // // 성격 int[] = {상승 스텟 index, 하락 스텟 index}
    // [SerializeField] int[] personality;

    public Unit Unit { get; set; }

    Image image;
    Vector3 originalPos;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition; 
        originalColor = image.color;
    }
    public void Setup(Unit unit)
    {

        Unit = unit;
        if (isPlayerUnit)
            image.sprite = unit.Base.BackSprite;
        else
            image.sprite = unit.Base.FrontSprite;
        hud.gameObject.SetActive(true);
        hud.SetData(unit);

        transform.localScale = new Vector3(1f, 1f, 1f);
        image.color = originalColor;
        PlayEnterAnimation();
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }
    public void SetSelected(bool selected) {
        image.color = selected?GlobalSettings.i.HighlightedColor:originalColor;
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            // image.transform.DOLocalMoveX(-200f, 1f);
            image.transform.localPosition = new Vector3(-600f, originalPos.y);
        else
            image.transform.localPosition = new Vector3(600f, originalPos.y);
            // image.transform.DOLocalMoveX(200f, 1f);

        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayerHitAnimation()
    {
        var sequence = DOTween.Sequence();
        for(int i = 0; i < 2; i++){
            sequence.Append(image.DOColor(Color.gray, 0.1f));
            sequence.Append(image.DOColor(originalColor, 0.1f));
        }
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        // sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 150f, 0.5f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
}
