using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quests/Create a new quest")]
public class QuestBase : ScriptableObject
{
    [SerializeField] string name;
    [TextArea]
    [SerializeField] string description;

    [SerializeField] Dialog startDialogue;
    [SerializeField] Dialog inProgressDialogue;
    [SerializeField] Dialog completedDialogue;

    [SerializeField] ItemBase requiredItem;
    [SerializeField] ItemBase rewardItem;

    public string Name { get => name; }
    public string Description { get => description; }
    public Dialog StartDialogue { get => startDialogue; }
    public Dialog InProgressDialogue { get => inProgressDialogue?.Lines.Count > 0?inProgressDialogue:startDialogue; }
    public Dialog CompletedDialogue { get => completedDialogue; }
    public ItemBase RequiredItem { get => requiredItem; }
    public ItemBase RewardItem { get => rewardItem; }
}
