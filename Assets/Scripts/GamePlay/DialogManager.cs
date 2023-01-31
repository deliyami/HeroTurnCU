using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] TextMeshProUGUI dialogName;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    public static DialogManager Instance { get; private set; }
    private void Awake() {
        Instance = this;
    }

    public bool IsShowing { get; private set; }

    // TODO 유닛 portrait보이도록 하고 이름도 바꾸기, 포트레이트 감정표현도 0123으로 사용하기
    // 이름, 할 말, 표정 번호, 화면 좌우, 사진 뒤집기 onoff
    // string, string, int, string, bool

    public IEnumerator ShowDialogText(string text, bool waitForInput = true, bool autoClose = true)
    {
        IsShowing = true;
        dialogBox.SetActive(true);

        dialogName.text = "시스템";
        yield return StartCoroutine(TypeDialog(text));
        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
        }
        if (autoClose)
        {
            CloseDialog();
        }
        else
        {

        }
    }
    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }
    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);
        dialogName.text = dialog.Lines[0].Name;

        foreach (var line in dialog.Lines)
        {
            yield return TypeDialog(line.Text);
            yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
        }

        dialogBox.SetActive(false);
        IsShowing = false;
        OnCloseDialog?.Invoke();
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
