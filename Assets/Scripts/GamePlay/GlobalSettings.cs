using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] Color highLightedColor;
    public Color HighlightedColor => highLightedColor;
    public static GlobalSettings i { get; private set; }
    private void Awake() {
        i = this;
    }
}
