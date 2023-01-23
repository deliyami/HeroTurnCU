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
    [SerializeField] string text;
    [SerializeField] int expression;
    [SerializeField] string leftRight;
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
    public string LeftRight {
        get { return leftRight; }
    }
    public bool ImgReverse {
        get { return imgReverse; }
    }
}