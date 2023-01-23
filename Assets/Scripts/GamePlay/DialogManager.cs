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

    Dialog dialog;
    int currentLine = 0;
    bool isTyping;

    // TODO 유닛 portrait보이도록 하고 이름도 바꾸기, 포트레이트 감정표현도 0123으로 사용하기
    // 이름, 할 말, 표정 번호, 화면 좌우, 사진 뒤집기 onoff
    // string, string, int, string, bool
    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();

        this.dialog = dialog;
        dialogBox.SetActive(true);
        dialogName.text = dialog.Lines[0].Name;
        StartCoroutine(TypeDialog(dialog.Lines[0].Text));
    }

    public void HandleUpdate()
    {
        if (Input.GetButtonDown("Submit") && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine].Text));
            }
            else
            {
                currentLine = 0;
                dialogBox.SetActive(false);
                OnCloseDialog?.Invoke();
            }
        }
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}
