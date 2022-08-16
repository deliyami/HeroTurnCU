using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public Animator anim;
    Rigidbody2D rigid;

    public SerihuManager manager;

    public float Speed = 3;
    float h;
    float v;
    Vector3 dirVec; // direction
    GameObject scanObject;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Move Value
        h = manager.isAction ? 0 : Input.GetAxisRaw("Horizontal");
        v = manager.isAction ? 0 : Input.GetAxisRaw("Vertical");

        bool hDown = manager.isAction ? false : Input.GetButtonDown("Horizontal");
        bool vDown = manager.isAction ? false : Input.GetButtonDown("Vertical");
        bool hUp = manager.isAction ? false : Input.GetButtonUp("Horizontal");
        bool vUp = manager.isAction ? false : Input.GetButtonUp("Vertical");
        bool shiftDown = manager.isAction ? false : Input.GetButton("Run");

        if(shiftDown) this.Speed = 5;
        else this.Speed = 3;

        // Direction
        if(vDown && v == 1) dirVec = Vector3.up;
        else if(vDown && v == -1) dirVec = Vector3.down;
        else if(hDown && h == -1) dirVec = Vector3.left;
        else if(hDown && h == 1) dirVec = Vector3.right;

        // Animation
        if(anim.GetInteger("hAxisRaw") != h)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("hAxisRaw", (int)h);
        }
        else if (anim.GetInteger("vAxisRaw") != v)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("vAxisRaw", (int)v);
        }
        else
        {
            anim.SetBool("isChange", false);
        }

        // Scan Object
        if(Input.GetButtonDown("Submit") && scanObject != null)
        {
            // Debug.Log(scanObject);
            manager.Action(scanObject);
        }
    }

    void FixedUpdate()
    {
        // Move
        rigid.velocity = new Vector2(h, v) * Speed;

        // Ray
        Debug.DrawRay(rigid.position, dirVec * 0.7f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 0.7f, LayerMask.GetMask("Object"));

        if(rayHit.collider != null)
        {
            scanObject = rayHit.collider.gameObject;
        }
        else scanObject = null;
    }
}
