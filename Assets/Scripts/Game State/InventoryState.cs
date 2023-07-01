using System.Collections;
using System.Collections.Generic;
using GDEUtils.StateMachine;
using UnityEngine;

public class InventoryState : State<GameController>
{
    [SerializeField] InventoryUI inventoryUI;
    //  output
    public ItemBase SelectedItem { get; private set; }
    public static InventoryState i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    Inventory inventory;
    private void Start()
    {
        inventory = Inventory.GetInventory();
    }
    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        SelectedItem = null;
        inventoryUI.gameObject.SetActive(true);
        inventoryUI.OnSelected += OnItemSelected;
        inventoryUI.OnBack += OnBack;
    }
    public override void Execute()
    {
        inventoryUI.HandleUpdate();
    }
    public override void Exit()
    {
        inventoryUI.gameObject.SetActive(false);
        inventoryUI.OnSelected -= OnItemSelected;
        inventoryUI.OnBack -= OnBack;
    }
    void OnItemSelected(int selection)
    {
        SelectedItem = inventoryUI.SelectedItem;
        StartCoroutine(SelectUnitAndUseItem());
    }
    void OnBack()
    {
        SelectedItem = null;
        gc.StateMachine.Pop();
    }

    IEnumerator SelectUnitAndUseItem()
    {
        var prevState = gc.StateMachine.GetPrevState();
        if (prevState == BattleState.i)
        {
            //  in battle
            if (!SelectedItem.CanBeUsedInBattle)
            {
                yield return DialogManager.Instance.ShowDialogText("그 아이템은 여기서 사용 할 수 없다!");
                yield break;
            }
        }
        else
        {
            // outside battle
            if (!SelectedItem.CanBeUsedOutsideBattle)
            {
                yield return DialogManager.Instance.ShowDialogText("그 아이템은 여기서 사용 할 수 없다!");
                yield break;
            }
        }


        if (SelectedItem is BallItem)
        {
            inventory.UseItem(SelectedItem, null);
            gc.StateMachine.Pop();
            yield break;
        }

        yield return gc.StateMachine.PushAndWait(PartyState.i);

        if (prevState == BattleState.i)
        {
            if (UseItemState.i.ItemUsed)
                gc.StateMachine.Pop();
        }
    }
}
