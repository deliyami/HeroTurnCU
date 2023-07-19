using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalObject : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
