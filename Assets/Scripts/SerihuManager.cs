using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SerihuManager : MonoBehaviour
{
    public TalkNaiyouManager talkNaiyouManager;
    public GameObject serihuPanel;
    public bool isAction;
    public int talkIndex;

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
        
        scanObject = scanObj;
        // talkText.text = "이 오브젝트의 이름은" + scanObject.name;
        string thisName = "";
        ObjData objData = scanObject.GetComponent<ObjData>();
        Talk(objData.id, objData.isNpc);
        switch (scanObject.name)
        {
            case "Hero":
                // if문으로 이름 분기? 정해야 할 듯
                thisName = "주인공"; // 0
                break;
            case "Nun":
                thisName = "히나미"; // 1
                break;
            case "Mir":
                thisName = "미르"; // 2
                break;
            case "Ghost":
                thisName = "흑유령"; // 3
                break;
            case "Mashiro":
                thisName = "마시로"; // 4
                break;
            case "It":
                thisName = "이름 기억이 안남..."; // 5
                break;
            case "Karl":
                thisName = "카를"; // 6
                break;
            case "Lord":
                thisName = "로드"; // 7
                break;
            case "Salamandar":
                thisName = "샐러맨더"; // 8
                break;
            case "Violet":
                thisName = "연화"; // 9
                break;
            case "Controller":
                thisName = "컨트롤러"; // 10
                break;
            case "Treka":
                thisName = "트레카"; // 11
                break;
            case "King":
                thisName = "왕"; // 12
                break;
            case "Queen":
                thisName = "여왕"; // 13
                break;
            case "Sunflower_Lod":
                thisName = "해바라기"; // 100
                break;
            case "Princess":
                thisName = "공주"; // 15
                break;
            case "Another_Box":
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
        string talkData = talkNaiyouManager.GetTalk(id, talkIndex);
        if(talkData == null)
        {
            talkIndex = 0;
            setAction(false);
            return;
        }
        if(isNpc)
        {
            talkText.text = talkData;
        }
        else {
            talkText.text = talkData;
        }
        setAction(true);
        talkIndex++;
    }
}
