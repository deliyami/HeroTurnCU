using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable
{
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;

    private Vector2 input;
    
    private Character character;

    void Awake()
    {
        character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        // Move Value
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // remove diagonal movement 대각 이동 제거
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        // bool shiftDown = manager.isAction ? false : Input.GetButton("Run");

        // Scan Object
        // if(Input.GetButtonDown("Submit") && scanObject != null)
        // {
        //     manager.Action(scanObject);
        // }
        if(Input.GetButtonDown("Submit"))
            StartCoroutine(Interact());
    }

    IEnumerator Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        // Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if (collider != null)
        {
            yield return collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }
    IPlayerTriggerable currentlyInTrigger;
    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayer);
        IPlayerTriggerable triggerable = null;
        foreach (var collider in colliders)
        {
            triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                if (triggerable == currentlyInTrigger && !triggerable.TriggerRepeatedly)
                    break;
                triggerable.OnPlayerTriggered(this);
                currentlyInTrigger = triggerable;
                break;
            }
        }
        // CheckForEncounters();
        // CheckIfInTrainersView();
        if (colliders.Count() == 0 || triggerable != currentlyInTrigger)
        {
            currentlyInTrigger = null;
        }
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            units = GetComponent<UnitParty>().Units.Select(p => p.GetSaveData()).ToList(),
        };

        // float[] position = new float[] { transform.position.x, transform.position.y };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        // 위치 재생
        var position = saveData.position;
        transform.position = new Vector3(position[0], position[1]);

        // 유닛 재생
        GetComponent<UnitParty>().Units = saveData.units.Select(s => new Unit(s)).ToList();
    }

    // void FixedUpdate()
    // {
    //     // Move
    //     rigid.velocity = new Vector2(h, v) * moveSpeed;

    //     // Ray
    //     Debug.DrawRay(rigid.position, dirVec * 0.7f, new Color(0, 1, 0));
    //     RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 0.7f, LayerMask.GetMask("Object"));

    //     if(rayHit.collider != null)
    //     {
    //         scanObject = rayHit.collider.gameObject;
    //     }
    //     else scanObject = null;
    // }
    public string Name {
        get => name;
    }    

    public Sprite Sprite {
        get => sprite;
    }

    public Character Character => character;
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<UnitSaveData> units;
}