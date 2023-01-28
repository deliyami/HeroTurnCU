using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string name;
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
            Interact();
    }

    void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        // Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayer);

        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                character.Animator.IsMoving = false;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
        // CheckForEncounters();
        // CheckIfInTrainersView();
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
