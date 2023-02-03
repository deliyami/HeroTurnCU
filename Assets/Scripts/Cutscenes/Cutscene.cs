using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Cutscene : MonoBehaviour, IPlayerTriggerable
{
    [SerializeReference]
    [SerializeField] List<CutsceneAction> actions;

    public bool TriggerRepeatedly => false;

    public IEnumerator Play()
    {
        GameController.Instance.StartCutsceneState();
        foreach (var action in actions)
        {
            if (action.WaitForCompletion)
                yield return action.Play();
            else
                StartCoroutine(action.Play());
        }
        GameController.Instance.StartFreeRoamState();
    }
    public void AddAction(CutsceneAction action)
    {
#if UNITY_EDIOTR
        Undo.RegisterCompleteObjectUndo(this, "컷신, 이벤트 ctrl z");
#endif
        action.Name = action.GetType().ToString();
        actions.Add(action);
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        StartCoroutine(Play());
    }
}
