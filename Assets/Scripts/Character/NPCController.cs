using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable, ISavable
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
    Healer healer;
    Merchant merchant;

    private void Awake() {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        unitGiver = GetComponent<UnitGiver>();
        healer = GetComponent<Healer>();
        merchant = GetComponent<Merchant>();
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
            else if (healer != null)
            {
                yield return healer.Heal(initiator, dialog);
            }
            else if (merchant != null)
            {
                yield return merchant.Trade();
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

    public object CaptureState()
    {
        var saveData = new NPCQuestSaveData();
        saveData.activeQuest = activeQuest?.GetSaveData();

        if (questToStart != null)
            saveData.questToStart = (new Quest(questToStart)).GetSaveData();

        if (questToComplete != null)
            saveData.questToComplete = (new Quest(questToComplete)).GetSaveData();

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as NPCQuestSaveData;
        if (saveData != null)
        {
            activeQuest = (saveData.activeQuest != null)?new Quest(saveData.activeQuest):null;
            questToStart = (saveData.questToStart != null)?new Quest(saveData.questToStart).Base:null;
            questToComplete = (saveData.questToComplete != null)?new Quest(saveData.questToComplete).Base:null;
        }
    }
}
[System.Serializable]
public class NPCQuestSaveData
{
    public QuestSaveData activeQuest;
    public QuestSaveData questToStart;
    public QuestSaveData questToComplete;
}
public enum NPCState { Idle, Walking, Dialog }