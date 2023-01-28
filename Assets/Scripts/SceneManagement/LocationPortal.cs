using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 화면 전환 없이 포탈 사용
public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{

    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;

    PlayerController player;
    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        StartCoroutine(Teleport());
    }
    Fader fader;
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator Teleport()
    {
        GameController.Instance.PauseGame(true);
        yield return fader.FadeIn(0.5f);

        // Debug.Log("포탈 사용");

        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x!= this && x.destinationPortal == this.destinationPortal);
        player.transform.position = destPortal.SpawnPoint.position;
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);
        
        yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);
    }

    public Transform SpawnPoint => spawnPoint;
}
