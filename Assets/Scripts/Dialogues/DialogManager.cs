using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject playerSprite;
    [SerializeField] GameObject enemySprite;
    [SerializeField] GameObject playerNickName;
    [SerializeField] GameObject enemyNickName;

    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] TextMeshProUGUI dialogName;

    public event Action OnShowDialog;
    public event Action OnDialogFinished;

    public static DialogManager Instance { get; private set; }

    [SerializeField] float imageMoveTime;
    [SerializeField] float moveDistance;
    Vector3 playerNicknamePosition;
    Vector3 enemyNicknamePosition;
    private bool isActive = true;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        playerNicknamePosition = new Vector3(playerNickName.GetComponent<Image>().transform.position.x, playerNickName.GetComponent<Image>().transform.position.y);
        enemyNicknamePosition = new Vector3(enemyNickName.GetComponent<Image>().transform.position.x, enemyNickName.GetComponent<Image>().transform.position.y);
    }

    public bool IsShowing { get; private set; }

    // TODO 유닛 portrait보이도록 하고 이름도 바꾸기, 포트레이트 감정표현도 0123으로 사용하기
    // 이름, 할 말, 표정 번호, 화면 좌우, 사진 뒤집기 onoff
    // string, string, int, string, bool

    public IEnumerator ShowDialogText(string text, bool waitForInput = true, bool autoClose = true,
        List<string> choices = null, Action<int> onChoiceSelected = null)
    {
        playerSprite.GetComponent<Image>().color = GlobalSettings.i.Transparent;
        enemySprite.GetComponent<Image>().color = GlobalSettings.i.Transparent;
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);

        AudioManager.i.PlaySfx(AudioId.UISelect);

        dialogName.text = "시스템";
        yield return StartCoroutine(TypeDialog(text));
        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
        }
        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }
        if (autoClose)
        {
            CloseDialog();
        }
        OnDialogFinished?.Invoke();
    }
    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }
    public IEnumerator ShowDialog(Dialog dialog, List<string> choices = null,
        Action<int> onChoiceSelected = null)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);

        Image playerSprite = this.playerSprite.GetComponent<Image>();
        Image enemySprite = this.enemySprite.GetComponent<Image>();


        foreach (var line in dialog.Lines)
        {
            // TODO, isSystem이라고 적혀있는게 있음
            // system은 showDialogText를 쓰면 되는데 왜 굳이 여기로 들어오나?
            // 이벤트라던가, 아이템 사용 등에서 이 showdailog를 사용하는 경우가 있어서
            // 그 경우를 위해 isSystem이라는 변수가 dialog.lines의 line안에 들어가있음
            // 그래서 그것을 여기서 처리해야 함

            AudioManager.i.PlaySfx(AudioId.UISelect);
            dialogName.text = line.Name;
            // 사진 변경
            if (line.UnitID == UnitID.None)
            {
                // 128, 808080
                if (playerSprite.color.a == 1) playerSprite.color = Color.gray;
                if (enemySprite.color.a == 1) enemySprite.color = Color.gray;
            }
            else
            {
                if (line.IsLeft)
                {
                    playerSprite.color = Color.white;
                    var unitSprite = GlobalSettings.i.UnitSprites[line.UnitID];
                    if (unitSprite != null) playerSprite.sprite = GlobalSettings.i.UnitSprites[line.UnitID][line.Expression];
                    else playerSprite.color = Color.gray;
                    if (enemySprite.color.a == 1) enemySprite.color = Color.gray;

                }
                else
                {
                    enemySprite.color = Color.white;
                    var unitSprite = GlobalSettings.i.UnitSprites[line.UnitID];
                    if (unitSprite != null) enemySprite.sprite = GlobalSettings.i.UnitSprites[line.UnitID][line.Expression];
                    else enemySprite.color = Color.gray;
                    if (playerSprite.color.a == 1) playerSprite.color = Color.gray;
                }
                if (!GlobalSettings.i.CheckNaming[line.UnitID])
                {
                    isActive = false;
                    GlobalSettings.i.CheckNaming[line.UnitID] = true;
                    StartCoroutine(FlowName(line.UnitID, line.IsLeft));
                }
            }

            yield return TypeDialog(line.Text);
            yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
            yield return new WaitUntil(() => isActive);
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        playerSprite.GetComponent<Image>().color = GlobalSettings.i.Transparent;
        enemySprite.GetComponent<Image>().color = GlobalSettings.i.Transparent;

        dialogBox.SetActive(false);
        IsShowing = false;
        OnDialogFinished?.Invoke();
    }

    public void HandleUpdate()
    {
    }
    public void ClearDialog()
    {
        dialogText.text = "";
    }

    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }
    public IEnumerator FlowName(UnitID unitID, bool isLeft = true)
    {
        if (isLeft)
        {
            Image nickname = playerNickName.GetComponent<Image>();
            nickname.rectTransform.sizeDelta = GlobalSettings.i.UnitNicknameSprites[unitID].bounds.size * GlobalSettings.i.NicknameSize;
            nickname.sprite = GlobalSettings.i.UnitNicknameSprites[unitID];

            // TODO: globalsetting에 이미지를 저장하거나, textpro어쩌구를 글자 수정해야 할 듯
            float defaultX = playerNicknamePosition.x;
            float defaultY = playerNicknamePosition.y;
            while (nickname.color.a < 1.0f)
            {
                float deltaTime = Time.deltaTime / (imageMoveTime / 2);
                yield return FadeInImage(nickname, deltaTime);
                yield return MoveImage(nickname, 1, moveDistance / 2, deltaTime, defaultX, defaultY);
            }
            yield return new WaitForSeconds(1f);
            while (nickname.color.a > 0f)
            {
                float deltaTime = Time.deltaTime / (imageMoveTime / 2);
                yield return FadeOutImage(nickname, deltaTime);
                yield return MoveImage(nickname, 1, moveDistance, deltaTime, defaultX, defaultY);
            }
            nickname.transform.position = playerNicknamePosition;
        }
        else
        {
            Image nickname = enemyNickName.GetComponent<Image>();
            nickname.rectTransform.sizeDelta = GlobalSettings.i.UnitNicknameSprites[unitID].bounds.size;
            nickname.sprite = GlobalSettings.i.UnitNicknameSprites[unitID];

            enemyNickName.GetComponent<Image>().color = GlobalSettings.i.Transparent;
            float defaultX = enemyNicknamePosition.x;
            float defaultY = enemyNicknamePosition.y;
            while (nickname.color.a < 1.0f)
            {
                float deltaTime = Time.deltaTime / (imageMoveTime / 2);
                yield return FadeInImage(nickname, deltaTime);
                yield return MoveImage(nickname, 3, moveDistance / 2, deltaTime, defaultX, defaultY);
            }
            yield return new WaitForSeconds(1f);
            while (nickname.color.a > 0f)
            {
                float deltaTime = Time.deltaTime / (imageMoveTime / 2);
                yield return FadeOutImage(nickname, deltaTime);
                yield return MoveImage(nickname, 3, moveDistance, deltaTime, defaultX, defaultY);
            }
            nickname.transform.position = enemyNicknamePosition;
        }
        isActive = true;
    }
    public IEnumerator FadeInImage(Image image, float deltaTime)
    {
        image.color = new Color(1f, 1f, 1f, image.color.a + deltaTime);
        yield return null;
    }
    public IEnumerator FadeOutImage(Image image, float deltaTime)
    {
        image.color = new Color(1f, 1f, 1f, image.color.a - deltaTime);
        yield return null;
    }
    public IEnumerator MoveImage(Image image, int dir, float moveDistance, float deltaTime, float defaultX, float defaultY)
    // up 0, right 1, down 2, left 3
    {
        float x = 0;
        float y = 0;
        if (dir == 0)
            y = moveDistance * deltaTime;
        if (dir == 1)
            x = moveDistance * deltaTime;
        if (dir == 2)
            y = -moveDistance * deltaTime;
        if (dir == 3)
            x = -moveDistance * deltaTime;
        // image.transform.position += new Vector3(x, y);
        x = Mathf.Clamp(image.transform.position.x + x, defaultX - moveDistance, defaultX + moveDistance);
        y = Mathf.Clamp(image.transform.position.y + y, defaultY - moveDistance, defaultY + moveDistance);
        image.transform.position = new Vector3(x, y);
        yield return null;
    }
}