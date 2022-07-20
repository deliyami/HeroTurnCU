using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SerihuManager : MonoBehaviour
{
    public Text talkText;
    public GameObject scanObject;

    // Update is called once per frame
    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        talkText.text = "이 오브젝트의 이름은" + scanObject.name;
    }
}
