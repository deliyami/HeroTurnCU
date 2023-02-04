using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
using TMPro;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;

    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject choiceBox;

    [SerializeField] List<TextMeshProUGUI> actionTexts;
    [SerializeField] List<TextMeshProUGUI> moveTexts;

    [SerializeField] TextMeshProUGUI ppText;
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI yesText;
    [SerializeField] TextMeshProUGUI noText;
    [SerializeField] BattleType typeSprite;

    Color highLightedColor;

    private void Start() {
        highLightedColor = GlobalSettings.i.HighlightedColor;
    }

    public void SetDialog(string dialog)
    {  
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void EnableChoiceBox(bool enabled)
    {
        choiceBox.SetActive(enabled);
    }


    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highLightedColor;
            else
                actionTexts[i].color = Color.white;
        }
    }
    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i == selectedMove)
                moveTexts[i].color = highLightedColor;
            else
                moveTexts[i].color = Color.white;
        }

        ppText.text = $"PP {move.PP}/ {move.Base.PP}";
        typeText.text = Type.GetType(move.Base.Type);
        // typeSprite.Type.Base.Courage
        typeSprite.Setup(move.Base.Type);

        if (move.PP == 0)
            ppText.color = Color.red;
        else if((float)move.PP / (float)move.Base.PP < 0.5f)
            ppText.color = new Color(1f, 0.6f, 0.2f, 1f);
        else
            ppText.color = Color.white;
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.Name;
            else
                moveTexts[i].text = "-";
        }
    }

    public void UpdateChoiceBox(bool yesSelected)
    {
        if (yesSelected)
        {
            yesText.color = highLightedColor;
            noText.color = Color.white;
        }
        else
        {
            yesText.color = Color.white;   
            noText.color = highLightedColor;
        }
    }

}
