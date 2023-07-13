using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] UIMode screenMode;

    // TODO 글자 색, UI 전부 global세팅에 맞춰서
    // 게임 내에서 옵션 창 누르면 UI 바꿀 수 있도록 해야 함
    [Header("발악기")]
    [SerializeField] MoveBase strugglePhysical;
    [SerializeField] MoveBase struggleSpecial;
    [Header("글자 색")]
    [SerializeField] Color highLightedColor;
    [SerializeField] Color unchosenColor;
    public UIMode ScreenMode => screenMode;
    public MoveBase StrugglePhysical => strugglePhysical;
    public MoveBase StruggleSpecial => struggleSpecial;
    public Color HighlightedColor => highLightedColor;
    public Color UnchosenColor => unchosenColor;
    public static GlobalSettings i { get; private set; }
    private void Awake()
    {
        i = this;
    }
}
public enum UIMode { Light, Dark }