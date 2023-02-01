using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    // [SerializeField] List<string> lines;
    // 이름, 할 말, 표정 번호, 화면 좌우, 사진 뒤집기 onoff
    // string, string, int, string, bool
    [SerializeField] List<DialogObject> lines;

    public List<DialogObject> Lines {
        get { return lines; }
    }
}

[System.Serializable]
public class DialogObject
{
    [SerializeField] string name;
    [TextArea]
    [SerializeField] string text;
    [SerializeField] int expression;
    // 대화창에서 화면 왼쪽에 있는가?
    [SerializeField] bool isSystem;
    [SerializeField] bool isLeft;
    [SerializeField] bool imgReverse;

    public string Name {
        get { return name; }
    }
    public string Text {
        get { return text; }
    }
    public int Expression {
        get { return expression; }
    }
    public bool IsLeft {
        get { return isLeft; }
    }
    public bool ImgReverse {
        get { return imgReverse; }
    }
}