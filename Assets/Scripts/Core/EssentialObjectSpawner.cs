using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EssentialObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectsPrefab;
    [SerializeField] GameObject soundObjectsPrefab;

    private void Awake()
    {
        var existingObjects = FindObjectsOfType<EssentialObject>();
        if (existingObjects.Length == 0)
        {
            // 중앙 생성
            var spawnPos = new Vector3(0, 0, 0);

            var grid = FindObjectOfType<Grid>();
            if (grid != null)
                spawnPos = grid.transform.position;
            Instantiate(essentialObjectsPrefab, spawnPos, Quaternion.identity);
        }
        var soundObjects = FindObjectsOfType<SoundObject>();
        if (soundObjects.Length == 0)
        {
            // 중앙 생성
            var spawnPos = new Vector3(0, 0, 0);

            var grid = FindObjectOfType<Grid>();
            if (grid != null)
                spawnPos = grid.transform.position;
            Instantiate(soundObjectsPrefab, spawnPos, Quaternion.identity);
        }
    }
}
