using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    private float Speed = 5;
    private Animator animator;
    private Rigidbody rb;
    public bool IsGrounded = true;
    public float CheckGroundRadius = 0.5f;
    public KeyCode JumpKey;
    public float JumpForce=10;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiPlier = 2f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, CheckGroundRadius);
    }

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        Jump();
    }

    private void FixedUpdate()
    {
        CheckGround();
        Move2d();
        
        //Move3d();
    }

    void CheckGround()
    {
        IsGrounded = Physics.CheckSphere(transform.position, CheckGroundRadius,LayerMask.GetMask("Ground"));
    }

    void Move2d()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        //Move

        Vector3 horizonDir = Camera.main.transform.right;
        horizonDir.Set(horizonDir.x, 0, horizonDir.z);
        horizonDir.Normalize();
        

        //transform.Translate((h * horizonDir) * Time.deltaTime * Speed, Space.World);
        rb.MovePosition(transform.position + (h * horizonDir) * Time.deltaTime * Speed);

        if (h != 0)
        {
            float angle = 0;
            if (h >= 0)
            {
                angle = Vector3.Angle(new Vector3(h, 0), Vector3.up);
            }
            else
            {
                angle = -Vector3.Angle(new Vector3(h, 0), Vector3.up);
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, Camera.main.transform.rotation.eulerAngles.y + angle, 0)), 0.6f);

        }

        //Anim
        animator.SetFloat("Speed", Mathf.Abs(h));


    }

    void Move3d()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");    


        //Move

        Vector3 horizonDir = Camera.main.transform.right;
        horizonDir.Set(horizonDir.x, 0, horizonDir.z);
        horizonDir.Normalize();

        #region 3d Move and Rot
        Vector3 verticalDir = Camera.main.transform.forward;
        verticalDir.Set(verticalDir.x, 0, verticalDir.z);
        verticalDir.Normalize();

        transform.Translate((h * horizonDir + v * verticalDir) * Time.deltaTime * Speed, Space.World);

        //Rotate
        if (h != 0 || v != 0)
        {
            float angle = 0;
            if (h >= 0)
            {
                angle = Vector3.Angle(new Vector3(h, v), Vector3.up);
            }
            else
            {
                angle = -Vector3.Angle(new Vector3(h, v), Vector3.up);
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, Camera.main.transform.rotation.eulerAngles.y + angle, 0)), 0.6f);

        }
        #endregion


        transform.Translate((h * horizonDir+v*verticalDir) * Time.deltaTime * Speed, Space.World);


        //Anim
        animator.SetFloat("Speed", Mathf.Min( Vector3.Magnitude(new Vector3(h,0,v)),1));


    }

    void Jump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }else if (rb.velocity.y > 0 && !Input.GetKey(JumpKey))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiPlier - 1) * Time.deltaTime;
        }

        if (IsGrounded)
        {
            if (Input.GetKeyDown(JumpKey))
            {
                rb.velocity = transform.up * JumpForce;
            }
        }
    }
}
