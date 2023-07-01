using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GDEUtils.StateMachine;
using UnityEngine;

public class UseItemState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    //  output
    public bool ItemUsed { get; private set; }
    public static UseItemState i { get; private set; }
    Inventory inventory;
    private void Awake()
    {
        i = this;
        inventory = Inventory.GetInventory();
    }
    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        ItemUsed = false;
        StartCoroutine(UseItem());
    }

    IEnumerator UseItem()
    {
        var item = inventoryUI.SelectedItem;
        var unit = partyScreen.SelectedMember;

        if (item is TmItem)
        {
            yield return HandleTmItems();
        }
        else
        {
            if (item is EvolutionItem)
            {
                var evolution = unit.CheckForEvolution(item);
                if (evolution != null)
                {
                    yield return EvolutionManager.i.Evolve(unit, evolution);
                }
                else
                {
                    gc.StateMachine.Pop();
                    yield break;
                }
            }
            var usedItem = inventory.UseItem(item, partyScreen.SelectedMember);
            if (usedItem != null)
            {
                ItemUsed = true;
                if (usedItem is RecoveryItem)
                    yield return DialogManager.Instance.ShowDialogText($"{usedItem.Name}을(를) 사용했다!");
                // onItemUsed?.Invoke(usedItem);
            }
            else
            {
                if (inventoryUI.SelectedCategory == (int)ItemCategory.Items)
                    yield return DialogManager.Instance.ShowDialogText($"그것을 사용 할 수 없다!");
            }
        }
        gc.StateMachine.Pop();
    }
    IEnumerator HandleTmItems()
    {
        var tmItem = inventoryUI.SelectedItem as TmItem;
        if (tmItem == null)
            yield break;
        var unit = partyScreen.SelectedMember;

        if (unit.HasMove(tmItem.Move))
        {
            yield return DialogManager.Instance.ShowDialogText($"{unit.Base.Name}은(는) 이미 알고있다!");
            yield break;
        }
        if (tmItem.CanBeTaught(unit))
        {
            yield return DialogManager.Instance.ShowDialogText($"{unit.Base.Name}은(는) 배울 수 없다!");
            yield break;
        }
        if (unit.Moves.Count < UnitBase.MaxNumOfMoves)
        {
            unit.LearnMove(tmItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{unit.Base.Name}은(는) {tmItem.Move.Name}을(를) 배웠다!");
            yield return DialogManager.Instance.ShowDialogText($"전투에는 전혀 도움 되지 않는다!");
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"{unit.Base.Name}은(는) {tmItem.Move.Name}을(를) 배우고 싶다!");
            yield return DialogManager.Instance.ShowDialogText($"하지만 이미 배울 만큼 배웠다!");
            yield return DialogManager.Instance.ShowDialogText($"기존 배우던 것을 잊어야 한다!");
            MoveToForgetState.i.NewMove = tmItem.Move;
            MoveToForgetState.i.CurrentMoves = unit.Moves.Select(m => m.Base).ToList();
            // 원래 moveIndex value에 따른 대사를 걸어두지 않음
            int moveIndex = MoveToForgetState.i.Selection;

            yield return new WaitForSeconds(2f);
        }

        // 예 아니오 선택지
        // yield return DialogManager.Instance.ShowDialogText($"{unit.Base.Name}은(는) {tmItem.Name}을(를) 사용하며 여유를 보냈다.", true, false);
        // yield return gc.StateMachine.PushAndWait(MoveToForgetState.i);
        yield return DialogManager.Instance.ShowDialogText($"{unit.Base.Name}은(는) {tmItem.Name}을(를) 사용하며 여유를 보냈다.");
        yield return DialogManager.Instance.ShowDialogText($"전투에는 전혀 도움 되지 않는다!");
    }

}
