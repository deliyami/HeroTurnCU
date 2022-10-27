using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    Rigidbody2D rigid;
    public GameManager manager;

    public float moveSpeed = 3;
    public LayerMask solidObjectsLayer; //solidObjectsLayer
    public LayerMask grassLayer; //grassLayer
    public bool isMoving;
    private Vector2 input;
    GameObject scanObject;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Move Value
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // remove diagonal movement 대각 이동 제거
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;
                if (IsWalkable(targetPos)) 
                    StartCoroutine(Move(targetPos));
            }
        }

        animator.SetBool("isMoving", isMoving);

        // bool shiftDown = manager.isAction ? false : Input.GetButton("Run");
        
        // speed boots
        if(Input.GetButton("Run") && this.moveSpeed == 3) this.moveSpeed = 5;
        else if(!Input.GetButton("Run") && this.moveSpeed == 5) this.moveSpeed = 3;

        // Scan Object
        if(Input.GetButtonDown("Submit") && scanObject != null)
        {
            // Debug.Log(scanObject);
            manager.Action(scanObject);
        }
    }
    
    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        
        isMoving = false;

        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) != null)
        {
            return false;
        }
        return true;
    }

    private void CheckForEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            if (Random.Range(1, 101) <= 10)
            {
                Debug.Log("Encountered a wild pokemon");
            }
        }
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
}
