using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    [HideInInspector]
    private Animator animator;
    [HideInInspector]
    private Rigidbody rb;
    public bool IsGrounded = true;

    [Header("Move")]
    public float Speed = 5;
    [Header("Inputs")]
    public KeyCode JumpKey;
    public KeyCode DashKey;
    [Header("Jump")]
    public float JumpForce=10;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiPlier = 2f;
    public float CheckGroundRadius = 0.5f;

    private bool isDashing = false;
    private Vector3 dashPoint;
    private Vector3 dashDir;
    private float dashTimer = 0;

    [Header("Dash")]
    public AnimationCurve dashCurve;
    public float DashDistance = 3;
    public float DashTime = 1;
    public float DashSpeed = 3;

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
        if (!isDashing)
        {
            Dash();
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            Dashing();
            return;
        }
        CheckGround();
        Move2d();
        
        //Move3d();
    }

    void CheckGround()
    {
        IsGrounded = Physics.CheckSphere(transform.position, CheckGroundRadius,LayerMask.GetMask("Ground"));
        if(IsGrounded)
            animator.SetBool("IsGrounded",true);
        else
            animator.SetBool("IsGrounded", false);
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
 

    void Dash()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(DashKey))
        {
            dashDir = new Vector3(h, v, 0).normalized;
            dashPoint = transform.position + dashDir * DashDistance;
            isDashing = true;
            dashTimer = 0;
        }
    }

    void Dashing()
    {
        dashTimer += Time.deltaTime;
        rb.useGravity = false;
        rb.MovePosition(transform.position+dashDir*Time.deltaTime*dashCurve.Evaluate(dashTimer)*DashSpeed);
        if (dashTimer>DashTime)
        {
            rb.useGravity = true;
            isDashing = false;
        }
    }


}
