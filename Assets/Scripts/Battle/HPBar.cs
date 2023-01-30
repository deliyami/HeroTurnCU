using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public bool IsUpdating { get; private set; }
    Color HPBarGreen = new Color(0f, 0.85f, 0.31f);
    Color HPBarYellow = new Color(1f, 0.82f, 0.18f);
    Color HPBarRed = new Color(1f, 0.18f, 0.18f);


    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);

        if(hpNormalized <= 0.2f)
            health.GetComponent<Image>().color = HPBarRed;
        else if(hpNormalized <= 0.5f)
            health.GetComponent<Image>().color = HPBarYellow;
        else
            health.GetComponent<Image>().color = HPBarGreen;
    }

    public IEnumerator SetHPSmooth(float newHP)
    {
        IsUpdating = true;

        float curHP = health.transform.localScale.x;
        float changeAmt = curHP - newHP;

        while (curHP - newHP > Mathf.Epsilon)
        {
            curHP -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(curHP, 1f);
            if(curHP <= 0.2f)
                health.GetComponent<Image>().color = HPBarRed;
            else if(curHP <= 0.5f)
                health.GetComponent<Image>().color = HPBarYellow;
            else
                health.GetComponent<Image>().color = HPBarGreen;
            yield return null;
        }
        health.transform.localScale = new Vector3(newHP, 1f);

        IsUpdating = false;
    }
}
