using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;

    [Header("퀘스트")]
    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete;

    [Header("행동")]
    [SerializeField] bool isMoveable = true;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;
    Quest activeQuest;
    Character character;
    ItemGiver itemGiver;
    UnitGiver unitGiver;

    private void Awake() {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        unitGiver = GetComponent<UnitGiver>();
    }
    public IEnumerator Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);

            if (questToComplete != null)
            {
                var quest = new Quest(questToComplete);
                yield return quest.CompletedQuest(initiator);
                Debug.Log($"{questToComplete.Name}퀘스트 완료");
                questToComplete = null;

            }

            if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            if (unitGiver != null && unitGiver.CanBeGiven())
            {
                yield return unitGiver.GiveUnit(initiator.GetComponent<PlayerController>());
            }
            else if (questToStart != null)
            {
                activeQuest = new Quest(questToStart);
                yield return activeQuest.StartQuest();
                questToStart = null;
                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompletedQuest(initiator);
                    activeQuest = null;
                }
            }
            else if (activeQuest != null)
            {
                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompletedQuest(initiator);
                    activeQuest = null;
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialog(activeQuest.Base.InProgressDialogue);
                }
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog(dialog);
            }

            idleTimer = 0f;
            state = NPCState.Idle;
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
                if (isMoveable){
                    if (movementPattern.Count > 0)
                    {
                        StartCoroutine(Walk());
                    }
                    else
                    {
                        state = NPCState.Walking;
                        float plusMinus = Random.Range(-1f, 1f)>0?1:-1;
                        float horizon = Random.Range(0, 1f);
                        float vertical = Random.Range(0, 1f);
                        StartCoroutine(character.Move(new Vector2(
                            horizon>vertical?plusMinus:0,
                            horizon<=vertical?plusMinus:0)));
                        state = NPCState.Idle;
                    }
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