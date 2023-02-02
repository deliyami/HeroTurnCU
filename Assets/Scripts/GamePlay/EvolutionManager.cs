using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image unitImage;
    [SerializeField] AudioClip evolutionMusic;

    public event Action OnStartEvolution;
    public event Action OnCompleteEvolution;

    public static EvolutionManager i { get; private set; }

    private void Awake() {
        i = this;
    }

    public IEnumerator Evolve(Unit unit, Evolution evolution)
    {
        OnStartEvolution?.Invoke();
        evolutionUI.SetActive(true);

        AudioManager.i.PlayMusic(evolutionMusic);

        unitImage.sprite = unit.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialogText($"보이면 안되는 창이야!");

        var oldUnit = unit.Base;
        unit.Evolve(evolution);
        unitImage.sprite = unit.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialogText($"연락주세요!");

        evolutionUI.SetActive(false);
        OnCompleteEvolution?.Invoke();
    }
}
