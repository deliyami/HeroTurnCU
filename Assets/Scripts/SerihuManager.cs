using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SerihuManager : MonoBehaviour
{
    public GameObject serihuPanel;
    public bool isAction;

    public Text talkText;
    public Text charName;
    public GameObject scanObject;

    void setAction(bool onOff){
        isAction = onOff;
        serihuPanel.SetActive(onOff);
    }
    // Update is called once per frame
    public void Action(GameObject scanObj)
    {
        
        if(isAction) // exit Action
        {
            setAction(false);
        }
        else // enter Action
        {
            scanObject = scanObj;
            talkText.text = "이 오브젝트의 이름은" + scanObject.name;
            string thisName = "";
            switch (scanObject.name)
            {
                case "Hero":
                    // if문으로 이름 분기? 정해야 할 듯
                    thisName = "주인공";
                    break;
                case "Nun":
                    thisName = "수녀";
                    break;
                case "Mir":
                    thisName = "미르";
                    break;
                case "Ghost":
                    thisName = "흑유령";
                    break;
                case "Mashiro":
                    thisName = "마시로";
                    break;
                case "It":
                    thisName = "이름 기억이 안남...";
                    break;
                case "Karl":
                    thisName = "카를";
                    break;
                case "Lord":
                    thisName = "로드";
                    break;
                case "Salamandar":
                    thisName = "샐러맨더";
                    break;
                case "Violet":
                    thisName = "연화";
                    break;
                case "Controller":
                    thisName = "컨트롤러";
                    break;
                case "Treka":
                    thisName = "트레카";
                    break;
                case "King":
                    thisName = "왕";
                    break;
                case "Queen":
                    thisName = "여왕";
                    break;
                case "Sunflower_Lod":
                    thisName = "해바라기";
                    break;
                case "Princess":
                    thisName = "공주";
                    break;
                case "Another_Box":
                    thisName = "빈 박스";
                    break;
                default:
                    break;
            }
            charName.text = thisName;
            setAction(true);
        }
    }
}
