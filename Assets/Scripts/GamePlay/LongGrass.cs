using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        if (UnityEngine.Random.Range(1, 10) <= 10)
        {
            player.Character.Animator.IsMoving = false;
            GameController.Instance.StartBattle(BattleTrigger.LongGrass);
        }

        // 100% 만남
        // GameController.Instance.StartBattle();
    }
    public bool TriggerRepeatedly => true;
}
