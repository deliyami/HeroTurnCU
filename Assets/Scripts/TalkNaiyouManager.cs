using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkNaiyouManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;
    // Start is called before the first frame update
    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        GenerateData();
    }

    // Update is called once per frame
    void GenerateData()
    {
        talkData.Add(15, new string[]{"나는 공주", "15번"});
        talkData.Add(100, new string[]{"나는 해바라기", "100번", "삶은 계란이다"});
        talkData.Add(101, new string[]{"나는 상자", "101번", "아무것도 안그리면 아무것도 안보이지", "정말 훌륭하다고 생각해", "그렇지 않니?"});
    }

    public string GetTalk(int id, int talkIndex)
    {
        print(talkIndex);
        print(talkData[id].Length);
        if(talkIndex == talkData[id].Length) return null;
        return talkData[id][talkIndex];
    }
}
