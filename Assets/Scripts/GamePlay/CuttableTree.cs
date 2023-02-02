using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CuttableTree : MonoBehaviour, Interactable
{
    [SerializeField] Move move;
    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogManager.Instance.ShowDialogText("이 나무는 보이면 안된다");

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

                gameObject.SetActive(false);
            }
        }
    }
}
