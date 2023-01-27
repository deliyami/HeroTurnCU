using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;
    Character character;

    private void Awake() {
        character = GetComponent<Character>();
    }
    public void Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => {
                idleTimer = 0f;
                state = NPCState.Idle;
            }));
        }
        // StartCoroutine(character.Move(new Vector2(-2, 0)));
    }

    private void Update()
    {
        if (state == NPCState.Idle )
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                {
                    StartCoroutine(Walk());
                }
                else
                {
                    float plusMinus = Random.Range(-1f, 1f)>0?1:-1;
                    float horizon = Random.Range(0, 1f);
                    float vertical = Random.Range(0, 1f);
                    StartCoroutine(character.Move(new Vector2(
                        horizon>vertical?plusMinus:0,
                        horizon<=vertical?plusMinus:0)));
                }
            }
        }
        character.HandleUpdate();
    }
    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentPattern]);

        if (transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }
}

public enum NPCState { Idle, Walking, Dialog }