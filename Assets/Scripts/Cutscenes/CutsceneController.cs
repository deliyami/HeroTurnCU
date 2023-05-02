using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public static CutsceneController i { get; private set; }
    public event Action OnStartCutscene;
    public event Action OnFinishCutscene;
    void Awake()
    {
        i = this;
    }
    public void StartCutscene()
    {
        OnStartCutscene?.Invoke();
    }
    public void FinishCutscene()
    {
        OnFinishCutscene?.Invoke();
    }
}
