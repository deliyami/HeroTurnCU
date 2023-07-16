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
    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] TextMeshProUGUI dialogName;

    public event Action OnShowDialog;
    public event Action OnDialogFinished;

    Color originalColor = new Color(1, 1, 1, 1);

    public static DialogManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public bool IsShowing { get; private set; }

    // TODO 유닛 portrait보이도록 하고 이름도 바꾸기, 포트레이트 감정표현도 0123으로 사용하기
    // 이름, 할 말, 표정 번호, 화면 좌우, 사진 뒤집기 onoff
    // string, string, int, string, bool

    public IEnumerator ShowDialogText(string text, bool waitForInput = true, bool autoClose = true,
        List<string> choices = null, Action<int> onChoiceSelected = null)
    {
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
        playerSprite.GetComponent<Image>().color = GlobalSettings.i.Transparent;
        enemySprite.GetComponent<Image>().color = GlobalSettings.i.Transparent;
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
                if (playerSprite.color.a != 0) playerSprite.color = GlobalSettings.i.PortraitsHideColor;
                if (enemySprite.color.a != 0) enemySprite.color = GlobalSettings.i.PortraitsHideColor;
            }
            else
            {
                if (line.IsLeft)
                {
                    playerSprite.color = originalColor;
                    playerSprite.sprite = GlobalSettings.i.UnitSprites[line.UnitID][line.Expression];
                    if (enemySprite.color.a != 0) enemySprite.color = GlobalSettings.i.PortraitsHideColor;

                }
                else
                {
                    enemySprite.color = originalColor;
                    enemySprite.sprite = GlobalSettings.i.UnitSprites[line.UnitID][line.Expression];
                    if (playerSprite.color.a != 0) playerSprite.color = GlobalSettings.i.PortraitsHideColor;
                }
            }
            yield return TypeDialog(line.Text);
            yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        dialogBox.SetActive(false);
        IsShowing = false;
        OnDialogFinished?.Invoke();
    }

    public void HandleUpdate()
    {
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
}