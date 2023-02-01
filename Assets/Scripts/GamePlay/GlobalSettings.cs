using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] UIMode screenMode;
    [Header("글자 색")]
    [SerializeField] Color highLightedColor;
    [SerializeField] Color unchosenColor;
    public UIMode ScreenMode => screenMode;
    public Color HighlightedColor => highLightedColor;
    public Color UnchosenColor => unchosenColor;
    public static GlobalSettings i { get; private set; }
    private void Awake() {
        i = this;
    }
}
public enum UIMode { Light, Dark }