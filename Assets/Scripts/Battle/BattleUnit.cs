using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using System.Threading;

public class BattleUnit : MonoBehaviour
{
    // [SerializeField] UnitBase _base;
    // [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }

    public BattleHud Hud
    {
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

    private const float WAIT_TIME = 0.15f;
    private const float DISAPPEAR_TIME = 0.5f;

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
    public void SetSelected(bool selected)
    {
        Debug.Log($"hahaha {selected}");
        image.color = selected ? GlobalSettings.i.HighlightedColor : originalColor;
    }
    public bool HasUnit()
    {
        return Unit != null;
    }

    public (float, float) CreateElliptionOrbit(float paramSemiMajorAxis = 4f, float paramSemiMinorAxis = 1f, float paramScale = 1f)
    {
        float semiMajorAxis = paramSemiMajorAxis;
        float semiMinorAxis = paramSemiMinorAxis;
        float scale = paramScale;

        float angle = Random.Range(0, 360);

        float x = originalPos.x + semiMajorAxis * Mathf.Cos(angle) * scale;
        float y = originalPos.y + semiMinorAxis * Mathf.Sin(angle) * scale;

        if (y < originalPos.y)
        {
            y = originalPos.y + semiMinorAxis * Mathf.Sin(angle + Mathf.PI) * scale;
        }

        return (x, y);
    }
    public (float, float) CreateElliptionOrbitInnerPoint(float paramSemiMajorAxis = 4f, float paramSemiMinorAxis = 1f, float paramScale = 1f)
    {
        // float semiMajorAxis = paramSemiMajorAxis;
        // float semiMinorAxis = paramSemiMinorAxis;
        // float scale = paramScale;

        // float angle = Random.Range(0, 360);
        // float distance = Mathf.Sqrt(Random.Range(0f, 1f));

        // float x = originalPos.x + semiMajorAxis * Mathf.Cos(angle) * scale * distance;
        // float y = originalPos.y + semiMinorAxis * Mathf.Sin(angle) * scale * distance;

        // return (x, y);
        var (x, y) = CreateElliptionOrbit(paramSemiMajorAxis, paramSemiMinorAxis, paramScale);
        return (x * Mathf.Sqrt(Random.Range(0f, 1f)), y * Mathf.Sqrt(Random.Range(0f, 1f)));
    }
    public Vector3 CreateVector3(float xyz)
    {
        return new Vector3(xyz, xyz, xyz);
    }
    public Vector3 DefaultVector3()
    {
        if (isPlayerUnit)
            return CreateVector3(5);
        return CreateVector3(3);
    }
    public GameObject createEffectUnit(string name, AttackSpriteID attackSpriteID, float alpha = 1f, int firstScene = 0)
    {
        // effect 부모, 순서 정하기
        GameObject effect = new GameObject(name);
        Transform backgroundTransfrom = BattleSystem.i.BattleCanvas.transform;
        // Transform targetUnitTransform = this.transform;
        effect.transform.SetParent(backgroundTransfrom);
        effect.transform.SetSiblingIndex(4);

        // 위치 설정
        effect.transform.position = this.transform.position + (isPlayerUnit ? Vector3.down * 0.55f : Vector3.zero);
        // if (isPlayerUnit) effect.transform.localPosition = Vector3.down * 20f;

        // 이미지 수정
        Image imageComponent = effect.AddComponent<Image>();
        imageComponent.color = new Color(1, 1, 1, alpha);
        imageComponent.sprite = GlobalSettings.i.AttackSprites[attackSpriteID][firstScene];

        return effect;
    }
    public void DisappearEffect(GameObject effect, float time = DISAPPEAR_TIME)
    {
        effect.GetComponent<Image>().DOFade(0f, time).OnComplete(() => Destroy(effect));
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

    public void PlayDefaultAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public IEnumerator PlayDefaultHitAnimationBySprite(AttackSpriteID attackSpriteID, float time = WAIT_TIME, int start = 0, int end = 0, float disappearTime = DISAPPEAR_TIME)
    {
        GameObject effect = createEffectUnit("defaultHit", attackSpriteID);
        RectTransform rectTransform = effect.GetComponent<RectTransform>();
        rectTransform.localScale = DefaultVector3();
        Image imageComponent = effect.GetComponent<Image>();
        List<Sprite> attackSprite = GlobalSettings.i.AttackSprites[attackSpriteID];
        int count = end == 0 ? attackSprite.Count : end;
        yield return ChangeSprites(imageComponent, start, count, attackSprite, time);
        // for (int i = start; i < count; i++)
        // {
        //     imageComponent.sprite = attackSprite[i];
        //     yield return new WaitForSeconds(time);
        // }
        DisappearEffect(effect, disappearTime);
    }
    public IEnumerator PlayDefaultHitAnimationBySprite(AttackSpriteID attackSpriteID, float time)
    {
        yield return PlayDefaultHitAnimationBySprite(attackSpriteID, time, 0, 0, DISAPPEAR_TIME);
    }
    public IEnumerator PlayDefaultHitAnimationBySprite(AttackSpriteID attackSpriteID, float time, int end)
    {
        yield return PlayDefaultHitAnimationBySprite(attackSpriteID, time, 0, end, DISAPPEAR_TIME);
    }
    public IEnumerator PlayDefaultHitAnimationBySprite(AttackSpriteID attackSpriteID, int end)
    {
        yield return PlayDefaultHitAnimationBySprite(attackSpriteID, WAIT_TIME, 0, end, DISAPPEAR_TIME);
    }
    IEnumerator ChangeSprites(Image imageComponent, int start, int end, List<Sprite> sprites, float time = WAIT_TIME)
    {
        for (int i = start; i < end; i++)
        {
            imageComponent.sprite = sprites[i];
            yield return new WaitForSeconds(time);
        }
    }
    IEnumerator ReverseChangeSprites(Image imageComponent, int start, int end, List<Sprite> sprites, float time = WAIT_TIME)
    {
        for (int i = start; i > end; i--)
        {
            imageComponent.sprite = sprites[i];
            yield return new WaitForSeconds(time);
        }
    }
    public void Test1Attack()
    {

    }
    public void Test1Hit()
    {
        var sequence = DOTween.Sequence();
        int direction = isPlayerUnit ? -1 : 1;
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f * direction, 0.0625f));
        for (int i = 0; i < 20; i++)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - (100f - i * 5f) * direction, 0.125f));
            direction *= -1;
        }
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.0625f));
    }

    // 아군
    // 용사
    // bodyPress
    public void bodyPressAttack()
    {
        var sequence = DOTween.Sequence();

        int direction = isPlayerUnit ? 1 : -1;
        // sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f * direction, 0.0625f));
        float originalX = originalPos.x;
        float originalY = originalPos.y;
        float moveX = (originalX + 50f) * direction;
        float moveY = (originalY + 10f) * direction;
        sequence.Append(image.transform.DOLocalMove(new Vector2(moveX, moveY), 0.25f, false));
        sequence.Append(image.transform.DOLocalMove(new Vector2(originalX, originalY), 0.1f, false));
        sequence.WaitForCompletion();
    }
    public IEnumerator bodyPressHit()
    {
        yield return PlayDefaultHitAnimationBySprite(AttackSpriteID.Attack6, 6);
    }
    // doubleIronBash
    public IEnumerator doubleIronBashAttack()
    {
        yield break;
    }
    public IEnumerator doubleIronBashHit()
    {
        AttackSpriteID sword1ID = AttackSpriteID.Sword1;
        GameObject effect = createEffectUnit("doubleIronBash", sword1ID);
        RectTransform rectTransform = effect.GetComponent<RectTransform>();
        rectTransform.localScale = DefaultVector3();
        Image imageComponent = effect.GetComponent<Image>();
        List<Sprite> sword1 = GlobalSettings.i.AttackSprites[sword1ID];
        for (int i = 0; i < sword1.Count; i++)
        {
            yield return new WaitForSeconds(0.1f);

            imageComponent.sprite = sword1[i];
        }
        DisappearEffect(effect, 0.2f);
    }
    // headSmash
    public IEnumerator headSmashAttack()
    {
        AttackSpriteID state5ID = AttackSpriteID.State5;
        GameObject effect = createEffectUnit("doubleIronBash", state5ID);
        RectTransform rectTransform = effect.GetComponent<RectTransform>();
        rectTransform.localScale = DefaultVector3();
        Image imageComponent = effect.GetComponent<Image>();

        GameObject circleEffect = createEffectUnit("doubleIronBash", state5ID);
        RectTransform circleTransform = circleEffect.GetComponent<RectTransform>();
        circleTransform.localScale = DefaultVector3();

        List<Sprite> state5 = GlobalSettings.i.AttackSprites[state5ID];
        circleEffect.GetComponent<Image>().sprite = state5[state5.Count - 1];
        circleTransform.DOLocalRotate(new Vector3(0, 0, 180), 3f, RotateMode.LocalAxisAdd);
        for (int i = 1; i < state5.Count - 1; i++)
        {
            yield return new WaitForSeconds(WAIT_TIME);

            imageComponent.sprite = state5[i];
        }
        DisappearEffect(effect, 0.75f);
        DisappearEffect(circleEffect, DISAPPEAR_TIME);

        bodyPressAttack();
    }
    public IEnumerator headSmashHit()
    {
        Debug.Log("headSmashHit Start");
        int stoneCount = Random.Range(4, 13);
        int[] stoneSpriteIndexes = new int[stoneCount];
        for (int i = 0; i < stoneCount; i++)
        {
            stoneSpriteIndexes[i] = Random.Range(0, 6);
        }

        AttackSpriteID attackSpriteID = AttackSpriteID.StonePiece;
        // GameObject effect = createEffectUnit("headSmash", attackSpriteID);
        GameObject[] effects = new GameObject[stoneCount];
        var sequence = DOTween.Sequence();
        for (int i = 0; i < stoneCount; i++)
        {
            effects[i] = createEffectUnit("headSmash", attackSpriteID);
            Transform rectTransform = effects[i].transform;
            rectTransform.localScale = CreateVector3(0.5f);
            Debug.Log(effects[i].transform.position);
            var (x, y) = CreateElliptionOrbit(3f, 1f, 15f);
            rectTransform.localPosition = new Vector3(x, y - 130f, 0);
            Image imageComponent = effects[i].GetComponent<Image>();
            List<Sprite> attackSprite = GlobalSettings.i.AttackSprites[attackSpriteID];
            imageComponent.sprite = attackSprite[stoneSpriteIndexes[i]];
            // yield return new WaitForSeconds(0.001f);
            float addX = rectTransform.localPosition.x - originalPos.x;
            float jumpX = rectTransform.localPosition.x + addX;
            // sequence.Join(rectTransform.DOLocalJump(new Vector3(jumpX, rectTransform.localPosition.y, 0), 0.5f, 1, 3f, true));
            // effects[i].transform.DOLocalMove(new Vector3(effects[i].transform.position.x * 2 - this.transform.position.x, 2f, this.transform.localPosition.z), Random.Range(0.2f, 0.4f));
            sequence.Join(effects[i].transform.DOLocalMove(new Vector3(jumpX + addX * 2, rectTransform.localPosition.y + Random.Range(20f, 100f), 0), 1f));
            sequence.Join(effects[i].GetComponent<Image>().DOFade(0f, 1.5f));
            // var sequence = DOTween.Sequence();

            // int direction = isPlayerUnit ? 1 : -1;
            // // sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f * direction, 0.0625f));
            // float originalX = originalPos.x;
            // float originalY = originalPos.y;
            // float moveX = (originalX + 50f) * direction;
            // float moveY = (originalY + 10f) * direction;
            // sequence.Append(image.transform.DOLocalMove(new Vector2(moveX, moveY), 0.25f, false));
            // sequence.Append(image.transform.DOLocalMove(new Vector2(originalX, originalY), 0.1f, false));
            // sequence.WaitForCompletion();
        }

        // Image imageComponent = effect.AddComponent<Image>();
        // imageComponent.color = Color.white;
        // imageComponent.sprite = GlobalSettings.i.AttackSprites[attackSpriteID][0];

        // var sequence = DOTween.Sequence();
        for (int i = 0; i < stoneCount; i++)
        {
            // sequence.Append(effects[i].transform.DOJump(new Vector3(effects[i].transform.position.x - this.transform.position.x, 0, 0), Random.Range(0.2f, 0.4f), 1, 3f, true));
        }

        yield return bodyPressHit();
        Debug.Log("headSmashHit end");
        sequence.WaitForCompletion();
        for (int i = 0; i < stoneCount; i++)
        {
            Destroy(effects[i]);
        }
    }
    // ironDefense
    public IEnumerator ironDefenseAttack()
    {
        AttackSpriteID attackSpriteID = AttackSpriteID.Special11;
        GameObject effect = createEffectUnit("ironDefense", attackSpriteID);
        RectTransform rectTransform = effect.GetComponent<RectTransform>();
        rectTransform.localScale = DefaultVector3();
        Image imageComponent = effect.GetComponent<Image>();

        GameObject shieldEffect = createEffectUnit("ironDefense", attackSpriteID);
        RectTransform shieldTransform = shieldEffect.GetComponent<RectTransform>();
        shieldTransform.localScale = DefaultVector3();
        Image shieldImageComponent = shieldEffect.GetComponent<Image>();

        List<Sprite> attackSprite = GlobalSettings.i.AttackSprites[attackSpriteID];
        shieldImageComponent.sprite = attackSprite[attackSprite.Count - 1];
        shieldImageComponent.color = new Color(1, 1, 1, 0);
        shieldImageComponent.DOFade(1f, 1f);
        for (int i = 7; i < attackSprite.Count - 1; i++)
        {
            yield return new WaitForSeconds(0.125f);

            imageComponent.sprite = attackSprite[i];
        }
        DisappearEffect(effect, 0.2f);
        DisappearEffect(shieldEffect, 0.2f);

        yield return new WaitForSeconds(0.2f);
    }
    public IEnumerator ironDefenseHit()
    {
        yield break;
    }
    // 히나미
    // bulletPunch
    public IEnumerator bulletPunchAttack()
    {

        yield break;
    }
    public IEnumerator bulletPunchHit()
    {
        // AttackSpriteID attackSpriteID = AttackSpriteID.Attack4;

        // GameObject effect = createEffectUnit("bulletPunchi", attackSpriteID);
        // RectTransform rectTransform = effect.GetComponent<RectTransform>();
        // rectTransform.localScale = DefaultVector3();
        // Image imageComponent = effect.GetComponent<Image>();
        // List<Sprite> attack6 = GlobalSettings.i.AttackSprites[attackSpriteID];
        // for (int i = 0; i < attack6.Count; i++)
        // {
        //     yield return new WaitForSeconds(0.15f);
        //     imageComponent.sprite = attack6[i];
        // }
        // DisappearEffect(effect, 0.2f);
        yield return PlayDefaultHitAnimationBySprite(AttackSpriteID.Attack4, WAIT_TIME);
    }
    // rockBlast
    public IEnumerator rockBlastAttack()
    {
        yield break;
    }
    public IEnumerator rockBlastHit()
    {
        AttackSpriteID attackSpriteID = AttackSpriteID.Fire3;
        GameObject effect = createEffectUnit("ironDefense", attackSpriteID);
        RectTransform rectTransform = effect.GetComponent<RectTransform>();
        rectTransform.localScale = DefaultVector3();
        Image imageComponent = effect.GetComponent<Image>();

        List<Sprite> attackSprite = GlobalSettings.i.AttackSprites[attackSpriteID];

        ReverseChangeSprites(imageComponent, attackSprite.Count, 23, attackSprite, 0.175f);
        ChangeSprites(imageComponent, 0, 24, attackSprite, 0.85f);
        // for (int i = 0; i < 24; i++)
        // {
        //     imageComponent.sprite = attackSprite[i];
        //     yield return new WaitForSeconds(0.85f);
        // }
        DisappearEffect(effect, DISAPPEAR_TIME);

        yield return new WaitForSeconds(0.2f);
    }
    // snipeShot
    public IEnumerator snipeShotAttack()
    {
        yield break;
    }
    public IEnumerator snipeShotHit()
    {
        AttackSpriteID snipeSpriteID = AttackSpriteID.Gun2;
        AttackSpriteID shotSpriteID = AttackSpriteID.Gun1;
        GameObject snipe = createEffectUnit("ironDefense", snipeSpriteID);
        GameObject shot1 = createEffectUnit("ironDefense", shotSpriteID, 0f, 8);
        GameObject shot2 = createEffectUnit("ironDefense", shotSpriteID, 0f);
        // snipe
        RectTransform snipeTransform = snipe.GetComponent<RectTransform>();
        snipeTransform.localScale = DefaultVector3();
        Image snipeImageComponent = snipeTransform.GetComponent<Image>();
        // shot1
        RectTransform shot1Transform = shot1.GetComponent<RectTransform>();
        shot1Transform.localScale = DefaultVector3();
        Image shot1ImageComponent = shot1Transform.GetComponent<Image>();
        // shot2
        RectTransform shot2Transform = shot2.GetComponent<RectTransform>();
        shot2Transform.localScale = DefaultVector3();
        Image shot2ImageComponent = shot2Transform.GetComponent<Image>();

        List<Sprite> snipeSprite = GlobalSettings.i.AttackSprites[shotSpriteID];
        List<Sprite> shotSprite = GlobalSettings.i.AttackSprites[shotSpriteID];

        snipeImageComponent.sprite = snipeSprite[0];
        yield return new WaitForSeconds(0.3f);
        snipeImageComponent.sprite = snipeSprite[1];
        yield return new WaitForSeconds(0.75f);

        DisappearEffect(snipe, 3f);

        StartCoroutine(ChangeSprites(shot1ImageComponent, 0, 8, shotSprite, 0.25f));
        yield return new WaitForSeconds(WAIT_TIME);
        StartCoroutine(ChangeSprites(shot2ImageComponent, 8, shotSprite.Count, shotSprite, 0.25f));
        yield return new WaitForSeconds(0.4f);
        DisappearEffect(shot1, DISAPPEAR_TIME);
        yield return new WaitForSeconds(1.85f);
        // for (int i = 0; i < 8; i++)
        // {
        //     shot1ImageComponent.sprite = shotSprite[i];
        //     yield return new WaitForSeconds(0.85f);
        // }
        // for (int i = 8; i < shotSprite.Count; i++)
        // {
        //     shot2ImageComponent.sprite = shotSprite[i];
        //     yield return new WaitForSeconds(0.85f);
        // }
        DisappearEffect(shot2, DISAPPEAR_TIME);
        yield return new WaitForSeconds(0.2f);
    }

    // suckerPunch
    public IEnumerator suckerPunchAttack()
    {
        yield break;
    }
    public IEnumerator suckerPunchHit()
    {
        yield return PlayDefaultHitAnimationBySprite(AttackSpriteID.Blow1);
    }
    // 크라베
    // meteorBeam
    public IEnumerator meteorBeamAttack()
    {
        yield break;
    }
    public IEnumerator meteorBeamHit()
    {
        yield return PlayDefaultHitAnimationBySprite(AttackSpriteID.Blow3);
        yield break;
    }
    // meteorMash
    public IEnumerator meteorMashAttack()
    {
        yield break;
    }
    public IEnumerator meteorMashHit()
    {
        yield return PlayDefaultHitAnimationBySprite(AttackSpriteID.Fire3, 5);
    }
    // moonBlast
    public IEnumerator moonBlastAttack()
    {

        yield return PlayDefaultHitAnimationBySprite(AttackSpriteID.State2);
    }
    public void moonBlastHit()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOPunchPosition(new Vector2(originalPos.x + 50f, originalPos.y), 1.5f));
        else
            sequence.Append(image.transform.DOPunchPosition(new Vector2(originalPos.x - 50f, originalPos.y), 1.5f));
    }
    // aircutter
    public IEnumerator aircutterAttack()
    {
        yield break;
    }
    public IEnumerator aircutterHit()
    {
        AttackSpriteID swordSpriteID = AttackSpriteID.Sword3;
        List<Sprite> swordSprite = GlobalSettings.i.AttackSprites[swordSpriteID];
        int smallCount = 7;
        int bigCount = swordSprite.Count;

        int slashCount = Random.Range(4, 10);
        GameObject[] effects = new GameObject[slashCount];
        for (int i = 0; i < slashCount; i++)
        {
            effects[i] = createEffectUnit("aircutter", swordSpriteID);
            RectTransform rectTransform = effects[i].GetComponent<RectTransform>();
            rectTransform.localScale = DefaultVector3();

            Image imageComponent = effects[i].GetComponent<Image>();

            var (x, y) = CreateElliptionOrbitInnerPoint(2f, 2f, 15f);
            rectTransform.localPosition = new Vector3(x, y - 130f, 0);
            effects[i].transform.position = this.transform.position + (isPlayerUnit ? Vector3.down * 0.55f : Vector3.zero) + new Vector3(x, y);
            StartCoroutine(ChangeSprites(imageComponent, 0, smallCount, swordSprite, Random.Range(WAIT_TIME - 0.05f, WAIT_TIME + 0.05f)));
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }

        yield return PlayDefaultHitAnimationBySprite(AttackSpriteID.Sword3, WAIT_TIME, smallCount, bigCount);
        for (int i = 0; i < slashCount; i++)
        {
            DisappearEffect(effects[i]);
        }
        yield break;
    }
    // 흑유령
    // shadowBall
    public IEnumerator shadowBallAttack()
    {
        yield break;
    }
    public IEnumerator shadowBallHit()
    {
        AttackSpriteID shadowBallSpriteID = AttackSpriteID.Darkness3;
        GameObject effect = createEffectUnit("defaultHit", shadowBallSpriteID);
        RectTransform rectTransform = effect.GetComponent<RectTransform>();
        rectTransform.localScale = DefaultVector3();
        Image imageComponent = effect.GetComponent<Image>();
        List<Sprite> attackSprite = GlobalSettings.i.AttackSprites[shadowBallSpriteID];
        yield return ChangeSprites(imageComponent, 1, attackSprite.Count, attackSprite, DISAPPEAR_TIME);
        yield return ReverseChangeSprites(imageComponent, 9, 1, attackSprite, DISAPPEAR_TIME);
        DisappearEffect(effect);
    }
    // flameThrower
    public IEnumerator flameThrowerAttack()
    {
        yield break;
    }
    public IEnumerator flameThrowerHit()
    {
        yield return PlayDefaultHitAnimationBySprite(AttackSpriteID.Fire1, 6);
    }
    // protect
    public IEnumerator protectAttack()
    {
        image.DOFade(0.7f, 0.5f);

        AttackSpriteID protectSpriteID = AttackSpriteID.Heal5;
        GameObject protectComponent = createEffectUnit("protect", protectSpriteID);
        RectTransform rectTransform = protectComponent.GetComponent<RectTransform>();
        rectTransform.localScale = DefaultVector3();
        Image protectImageComponent = protectComponent.GetComponent<Image>();
        List<Sprite> protectSprite = GlobalSettings.i.AttackSprites[protectSpriteID];

        StartCoroutine(ChangeSprites(protectImageComponent, 0, protectSprite.Count - 1, protectSprite));
        yield return new WaitForSeconds(1.5f);

        image.DOFade(1f, 0.5f);
        DisappearEffect(protectComponent);
    }
    public IEnumerator protectHit()
    {
        yield break;
    }
    // calmMind
    public IEnumerator calmMindAttack()
    {
        AttackSpriteID calmSpriteID = AttackSpriteID.State3;
        GameObject effect = createEffectUnit("defaultHit", calmSpriteID);
        effect.transform.position = this.transform.position + Vector3.up * (isPlayerUnit ? 0.25f : 0.55f);
        RectTransform rectTransform = effect.GetComponent<RectTransform>();
        rectTransform.localScale = DefaultVector3();
        Image imageComponent = effect.GetComponent<Image>();
        List<Sprite> calmSprite = GlobalSettings.i.AttackSprites[calmSpriteID];
        yield return ChangeSprites(imageComponent, 0, 6, calmSprite);
        // for (int i = start; i < count; i++)
        // {
        //     imageComponent.sprite = attackSprite[i];
        //     yield return new WaitForSeconds(time);
        // }
        DisappearEffect(effect);
    }
    public IEnumerator calmMindHit()
    {
        yield break;
    }
    // 마시로
    // hyperVoice
    public IEnumerator hyperVoiceAttack()
    {
        AttackSpriteID hyperSpriteID = AttackSpriteID.State3;
        GameObject effect = createEffectUnit("defaultHit", hyperSpriteID);
        effect.transform.position = this.transform.position + Vector3.up * (isPlayerUnit ? 0.25f : 0.55f);
        RectTransform rectTransform = effect.GetComponent<RectTransform>();
        rectTransform.localScale = DefaultVector3();
        Image imageComponent = effect.GetComponent<Image>();
        List<Sprite> hyperSprite = GlobalSettings.i.AttackSprites[hyperSpriteID];
        yield return StartCoroutine(ChangeSprites(imageComponent, 6, hyperSprite.Count, hyperSprite));
        rectTransform.DOShakeScale(0.75f, 0.5f, 10, 90f, false);
        // for (int i = start; i < count; i++)
        // {
        //     imageComponent.sprite = attackSprite[i];
        //     yield return new WaitForSeconds(time);
        // }
        DisappearEffect(effect);
    }
    public IEnumerator hyperVoiceHit()
    {
        yield break;
    }
    // partingShot
    public IEnumerator partingShotAttack()
    {
        yield break;
    }
    public IEnumerator partingShotHit()
    {
        yield return PlayDefaultHitAnimationBySprite(AttackSpriteID.State1);
    }
    // tailWind
    public IEnumerator tailWindAttack()
    {
        yield break;
    }
    public IEnumerator tailWindHit()
    {
        yield break;
    }
    // sparklingAria
    public IEnumerator sparklingAriaAttack()
    {
        yield break;
    }
    public IEnumerator sparklingAriaHit()
    {
        yield return PlayDefaultHitAnimationBySprite(AttackSpriteID.Blow3);
    }
    // 잇쨩
    // citronVeil
    public IEnumerator citronVeilAttack(float x)
    {
        AttackSpriteID citronSpriteID = AttackSpriteID.Light5;
        GameObject effect = createEffectUnit("defaultHit", citronSpriteID);
        effect.transform.position = new Vector3(x, this.transform.position.y);
        RectTransform rectTransform = effect.GetComponent<RectTransform>();
        rectTransform.localScale = DefaultVector3();
        Image imageComponent = effect.GetComponent<Image>();
        List<Sprite> citronAttackSprite = GlobalSettings.i.AttackSprites[citronSpriteID];
        yield return ChangeSprites(imageComponent, 0, citronAttackSprite.Count, citronAttackSprite);
        // for (int i = start; i < count; i++)
        // {
        //     imageComponent.sprite = attackSprite[i];
        //     yield return new WaitForSeconds(time);
        // }
        DisappearEffect(effect);
    }
    public IEnumerator citronVeilHit()
    {
        yield break;
    }
    // nuzzle
    public IEnumerator nuzzleAttack()
    {
        yield break;
    }
    public IEnumerator nuzzleHit()
    {
        yield return PlayDefaultHitAnimationBySprite(AttackSpriteID.Sword4, WAIT_TIME, 6, 0);
    }
    // voltSwitch
    public IEnumerator voltSwitchAttack()
    {
        yield break;
    }
    public IEnumerator voltSwitchHit()
    {
        yield return PlayDefaultHitAnimationBySprite(AttackSpriteID.Spear1);
    }
    // skillSwap
    public IEnumerator skillSwapAttack()
    {
        yield break;
    }
    public IEnumerator skillSwapHit()
    {
        yield break;
    }

    // 적
    // 카를
    // phantomForce
    public IEnumerator phantomForceAttack()
    {
        yield break;
    }
    public IEnumerator phantomForceHit()
    {
        yield break;
    }
    // crunch
    public IEnumerator crunchAttack()
    {
        yield break;
    }
    public IEnumerator crunchHit()
    {
        yield break;
    }
    // shadowClaw
    public IEnumerator shadowClawAttack()
    {
        yield break;
    }
    public IEnumerator shadowClawHit()
    {
        yield break;
    }
    // willOWisop
    public IEnumerator willOWisopAttack()
    {
        yield break;
    }
    public IEnumerator willOWisopHit()
    {
        yield break;
    }
    // 로드
    // bellyDrum
    public IEnumerator bellyDrumAttack()
    {
        yield break;
    }
    public IEnumerator bellyDrumHit()
    {
        yield break;
    }
    // icePunch
    public IEnumerator icePunchAttack()
    {
        yield break;
    }
    public IEnumerator icePunchHit()
    {
        yield break;
    }
    // ironHead
    public IEnumerator ironHeadAttack()
    {
        yield break;
    }
    public IEnumerator ironHeadHit()
    {
        yield break;
    }
    // explosion
    public IEnumerator explosionAttack()
    {
        yield break;
    }
    public IEnumerator explosionHit()
    {
        yield break;
    }
    // 살라만다
    // flameThrower
    public IEnumerator flameThrowerAttack2()
    {
        yield break;
    }
    public IEnumerator flameThrowerHit2()
    {
        yield break;
    }
    // dracoMeteor
    public IEnumerator dracoMeteorAttack()
    {
        yield break;
    }
    public IEnumerator dracoMeteorHit()
    {
        yield break;
    }
    // flareBlitz
    public IEnumerator flareBlitzAttack()
    {
        yield break;
    }
    public IEnumerator flareBlitzHit()
    {
        yield break;
    }
    // earthquake
    public IEnumerator earthquakeAttack()
    {
        yield break;
    }
    public IEnumerator earthquakeHit()
    {
        yield break;
    }
    // 연화
    // overHeat
    public IEnumerator overHeatAttack()
    {
        yield break;
    }
    public IEnumerator overHeatHit()
    {
        yield break;
    }
    // leafStorm
    public IEnumerator leafStormAttack()
    {
        yield break;
    }
    public IEnumerator leafStormHit()
    {
        yield break;
    }
    // vacuumWave
    public IEnumerator vacuumWaveAttack()
    {
        yield break;
    }
    public IEnumerator vacuumWaveHit()
    {
        yield break;
    }
    // fakeOut
    public IEnumerator fakeOutAttack()
    {
        yield break;
    }
    public IEnumerator fakeOutHit()
    {
        yield break;
    }
    // 컨트롤러
    // ufoBeam
    public IEnumerator ufoBeamAttack()
    {
        yield break;
    }
    public IEnumerator ufoBeamHit()
    {
        yield break;
    }
    // bulldoze
    public IEnumerator bulldozeAttack()
    {
        yield break;
    }
    public IEnumerator bulldozeHit()
    {
        yield break;
    }
    // stoneEdge
    public IEnumerator stoneEdgeAttack()
    {
        yield break;
    }
    public IEnumerator stoneEdgeHit()
    {
        yield break;
    }
    // airSlach
    public IEnumerator airSlachAttack()
    {
        yield break;
    }
    public IEnumerator airSlachHit()
    {
        yield break;
    }
    // 트레카
    // swordsDance
    public IEnumerator swordsDanceAttack()
    {
        yield break;
    }
    public IEnumerator swordsDanceHit()
    {
        yield break;
    }
    // uTurn
    public IEnumerator uTurnAttack()
    {
        yield break;
    }
    public IEnumerator uTurnHit()
    {
        yield break;
    }
    // firstImpression
    public IEnumerator firstImpressionAttack()
    {
        yield break;
    }
    public IEnumerator firstImpressionHit()
    {
        yield break;
    }
    // spinOut
    public IEnumerator spinOutAttack()
    {
        yield break;
    }
    public IEnumerator spinOutHit()
    {
        yield break;
    }
    // 무너
    // scald
    public IEnumerator scaldAttack()
    {
        yield break;
    }
    public IEnumerator scaldHit()
    {
        yield break;
    }
    // iceBeam
    public IEnumerator iceBeamAttack()
    {
        yield break;
    }
    public IEnumerator iceBeamHit()
    {
        yield break;
    }
    // surf
    public IEnumerator surfAttack()
    {
        yield break;
    }
    public IEnumerator surfHit()
    {
        yield break;
    }
    // lightScreen
    public IEnumerator lightScreenAttack()
    {
        yield break;
    }
    public IEnumerator lightScreenHit()
    {
        yield break;
    }

    public void PlayerDamagedtAnimation()
    {
        var sequence = DOTween.Sequence();
        for (int i = 0; i < 2; i++)
        {
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
