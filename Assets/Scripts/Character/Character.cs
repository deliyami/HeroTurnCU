using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveSpeed = 3;

    public bool IsMoving { get; private set; }

    CharacterAnimator animator;
    private void Awake() {
        animator = GetComponent<CharacterAnimator>();
    }
    public IEnumerator Move(Vector2 moreVec, Action OnMoveOver=null)
    {
        animator.MoveX = Mathf.Clamp(moreVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moreVec.y, -1f, 1f);
        
        var targetPos = transform.position;
        targetPos.x += moreVec.x;
        targetPos.y += moreVec.y;

        // speed boots
        // if(Input.GetButton("Run") && this.moveSpeed == 3) this.moveSpeed = 5;
        // else if(!Input.GetButton("Run") && this.moveSpeed == 5) this.moveSpeed = 3;

        if (!IsPathClear(targetPos))
            yield break;

        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        
        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer) == true)
        {
            return false;
        }
        return true;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null)
        {
            return false;
        }
        return true;
    }

    public void LookTowards(Vector3 targetPos)
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
            Debug.LogError("보는 방향 버그: 당신은 그 방향으로 대화를 걸 수 없습니다.");
    }

    public CharacterAnimator Animator {
        get => animator;
    }
}