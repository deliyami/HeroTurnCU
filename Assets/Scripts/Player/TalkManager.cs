using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;
    Dictionary<int, Sprite> portraitData;
    // Start is called before the first frame update

    public Sprite[] portraitArr;

    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, Sprite>();
        GenerateData();
    }

    // Update is called once per frame
    void GenerateData()
    {
        talkData.Add(2, new string[]{"나는 미르:0", "2번:0", "테스트 중:1"});
        talkData.Add(8, new string[]{"나는 샐러맨더:0", "8번:0", "걷고있어:1", "걷고있어걷고있어:2", "걷고있어걷고있어걷고있어:3", "걷고있어걷고있어걷고있어걷고있어:0", "걷고있어걷고있어걷고있어걷고있어걷고있어:1", "걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어:2", "걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어:3", "걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어:0", "걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어:1", "걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어걷고있어:2"});
        talkData.Add(14, new string[]{"나는 공주:0", "14번:3"});
        talkData.Add(100, new string[]{"나는 해바라기", "100번", "삶은 계란이다"});
        talkData.Add(101, new string[]{"나는 상자", "101번", "아무것도 안그리면 아무것도 안보이지", "정말 훌륭하다고 생각해", "그렇지 않니?"});

        portraitData.Add(2 + 0, portraitArr[2 * 4 + 0]);
        portraitData.Add(2 + 1, portraitArr[2 * 4 + 1]);
        portraitData.Add(2 + 2, portraitArr[2 * 4 + 2]);
        portraitData.Add(2 + 3, portraitArr[2 * 4 + 3]);

        portraitData.Add(8 + 0, portraitArr[8 * 4 + 0]);
        portraitData.Add(8 + 1, portraitArr[8 * 4 + 1]);
        portraitData.Add(8 + 2, portraitArr[8 * 4 + 2]);
        portraitData.Add(8 + 3, portraitArr[8 * 4 + 3]);

        portraitData.Add(14 + 0, portraitArr[14 * 4 + 0]);
        portraitData.Add(14 + 1, portraitArr[14 * 4 + 1]);
        portraitData.Add(14 + 2, portraitArr[14 * 4 + 2]);
        portraitData.Add(14 + 3, portraitArr[14 * 4 + 3]);
    }

    public string GetTalk(int id, int talkIndex)
    {
        // print(talkData[id].Length);
        if(talkIndex == talkData[id].Length) return null;
        return talkData[id][talkIndex];
    }

    /*
    
    **/
    public Sprite GetPortrait(int id, int portraitIndex) // id = unitid, portraitIndex = + 0~3
    {
        return portraitData[id + portraitIndex];
    }
}
