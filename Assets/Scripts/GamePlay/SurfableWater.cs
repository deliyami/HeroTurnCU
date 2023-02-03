using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class SurfableWater : MonoBehaviour, Interactable, IPlayerTriggerable
{
    bool isJumpingToWater = false;

    public bool TriggerRepeatedly => true;

    public IEnumerator Interact(Transform initiator)
    {
        var animaitor = initiator.GetComponent<CharacterAnimator>();
        if (animaitor.IsSurfing || isJumpingToWater)
            yield break;
        yield return DialogManager.Instance.ShowDialogText("물이 깊다");

        // var unitWithMove = initiator.GetComponent<UnitParty>().Units.FirstOrDefault(u => u.Moves.Any(m => m.Base.Name == move.Base.Name));
        bool unitWithMove;
        unitWithMove = true;
        // if (unitWithMove != null)
        if (unitWithMove)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"할거야?",
                choices: new List<string>() { "예", "아니오" },
                onChoiceSelected: (s) => selectedChoice = s);

            if (selectedChoice == 0)
            {
                yield return DialogManager.Instance.ShowDialogText("한다!");

                
                var dir = new Vector3(animaitor.MoveX, animaitor.MoveY);
                var targetPos = initiator.position + dir * 2;
                isJumpingToWater = true;
                yield return initiator.DOJump(targetPos, 0.3f, 1, 0.5f).WaitForCompletion();
                isJumpingToWater = false;
                animaitor.IsSurfing = true;
            }
        }
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        // if (UnityEngine.Random.Range(1, 101) <= 10)
        // {
            // player.Character.Animator.IsMoving = false;
            GameController.Instance.StartBattle(BattleTrigger.Water);
        // }
    }
}
