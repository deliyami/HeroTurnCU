using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TalkManager talkManager;
    public GameObject talkPanel;

    public Image portraitImg;

    public Text talkText;
    public Text charName;
    public GameObject scanObject;

    public bool isAction;
    public int talkIndex;

    void setAction(bool onOff){
        isAction = onOff;
        talkPanel.SetActive(onOff);
    }
    // Update is called once per frame
    public void Action(GameObject scanObj)
    {
        
        scanObject = scanObj;
        // talkText.text = "이 오브젝트의 이름은" + scanObject.name;
        string thisName = "";
        ObjData objData = scanObject.GetComponent<ObjData>();
        Talk(objData.id, objData.isNpc);
        switch (objData.id)
        {
            case 0:
                // if문으로 이름 분기? 정해야 할 듯
                thisName = "주인공"; // 0
                break;
            case 1:
                thisName = "히나미"; // 1
                break;
            case 2:
                thisName = "미르"; // 2
                break;
            case 3:
                thisName = "흑유령"; // 3
                break;
            case 4:
                thisName = "마시로"; // 4
                break;
            case 5:
                thisName = "이름 기억이 안남..."; // 5
                break;
            case 6:
                thisName = "카를"; // 6
                break;
            case 7:
                thisName = "로드"; // 7
                break;
            case 8:
                thisName = "샐러맨더"; // 8
                break;
            case 9:
                thisName = "연화"; // 9
                break;
            case 10:
                thisName = "컨트롤러"; // 10
                break;
            case 11:
                thisName = "트레카"; // 11
                break;
            case 12:
                thisName = "왕"; // 12
                break;
            case 13:
                thisName = "여왕"; // 13
                break;
            case 100:
                thisName = "해바라기"; // 100
                break;
            case 14:
                thisName = "공주"; // 14
                break;
            case 101:
                thisName = "빈 박스"; // 101
                break;
            default:
                break;
        }
        charName.text = thisName;
        // setAction(true);
    }

    void Talk(int id, bool isNpc)
    {
        string talkData = talkManager.GetTalk(id, talkIndex);
        if(talkData == null)
        {
            talkIndex = 0;
            setAction(false);
            return;
        }
        if(isNpc)
        {
            talkText.text = talkData.Split(":")[0];
            
            portraitImg.sprite = talkManager.GetPortrait(id, int.Parse(talkData.Split(":")[1]));
            portraitImg.color = new Color(1, 1, 1, 1);
        }
        else {
            talkText.text = talkData;

            portraitImg.color = new Color(1, 1, 1, 0);
        }
        setAction(true);
        talkIndex++;
    }
}
