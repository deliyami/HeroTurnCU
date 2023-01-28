using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectsPrefab;

    private void Awake() 
    {
        var existingObjects = FindObjectsOfType<EssentialObject>();
        if (existingObjects.Length == 0)
        {
            Instantiate(essentialObjectsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {

        }
    }
}
