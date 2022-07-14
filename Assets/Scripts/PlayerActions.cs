using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public Animator anim;
    Rigidbody2D rigid;

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
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        bool hDown = Input.GetButtonDown("Horizontal");
        bool vDown = Input.GetButtonDown("Vertical");
        bool hUp = Input.GetButtonUp("Horizontal");
        bool vUp = Input.GetButtonUp("Vertical");
        bool shiftDown = Input.GetButtonDown("Run");

        // Direction
        if(vDown && v == 1) dirVec = Vector3.up;
        else if(vDown && v == -1) dirVec = Vector3.down;
        else if(hDown && h == -1) dirVec = Vector3.left;
        else if(hDown && h == 1) dirVec = Vector3.right;

        if(shiftDown) this.Speed = 5f;
        else this.Speed = 3f;

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
            Debug.Log(scanObject);
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
