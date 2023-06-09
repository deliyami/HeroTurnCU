using GDE.GenericSelectionUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class MoveSelectionUI : SelectionUI<TextSlot>
{
    [SerializeField] List<TextSlot> moveTexts;

    [SerializeField] TextMeshProUGUI ppText;
    [SerializeField] TextMeshProUGUI typeText;

    [SerializeField] BattleType typeSprite;

    private void Start()
    {
        SetSelectionSettings(SelectionType.Grid, 2);
    }

    List<Move> _moves;
    public void SetMoves(List<Move> moves)
    {
        _moves = moves;

        selectedItem = 0;

        SetItems(moveTexts.Take(moves.Count).ToList());

        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i < moves.Count)
                moveTexts[i].SetText(moves[i].Base.Name);
            else
                moveTexts[i].SetText("-");
        }
    }

    public override void UpdateSelectionInUI()
    {
        base.UpdateSelectionInUI();
        {
            base.UpdateSelectionInUI();

            var move = _moves[selectedItem];

            ppText.text = $"PP {move.PP}/ {move.Base.PP}";
            typeText.text = Type.GetType(move.Base.Type);
            // typeSprite.Type.Base.Courage
            typeSprite.Setup(move.Base.Type);

            if (move.PP == 0)
                ppText.color = Color.red;
            else if ((float)move.PP / (float)move.Base.PP < 0.5f)
                ppText.color = new Color(1f, 0.6f, 0.2f, 1f);
            else
                ppText.color = Color.white;
        }
    }
}
