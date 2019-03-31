using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove2DState :IBasePlayerState
{

    private Rigidbody rb;
    private Animator animator;

    private bool isDashing = false;
    private Vector3 dashPoint;
    private Vector3 dashDir;
    private float dashTimer = 0;

    private PlayerStateController controller;
    public PlayerMove2DState(PlayerStateController controller)
    {
        this.controller = controller;
        rb = controller.PlayerRb;
        animator = controller.PlayerAnimator;
    }
    public void OnStateEnter()
    {
        
    }
    public void HandleInput()
    {
        if (!isDashing&& Input.GetKeyDown(controller.AttackKey))
        {
            controller.SetPlayerState(new PlayerAttackState(controller));
        }
    }
    public void Update()
    {
        if (!isDashing)
        {
            Dash();
            Jump();
        }
    }

    public void FixedUpdate()
    {
        if (isDashing)
        {
            Dashing();
            return;
        }
        CheckGround();
        Move2d();
    }

    public void OnStateExit()
    {
        animator.SetFloat("Speed", 0);
    }

    void CheckGround()
    {
        controller.IsGrounded = Physics.CheckSphere(rb.transform.position, controller.CheckGroundRadius, LayerMask.GetMask("Ground"));


        if (controller.IsGrounded)
        {
            animator.SetBool("IsGrounded", true);

            //落地后可以冲刺
            if (isDashing)
            {
                controller.CanDash = false;
            }
            else
            {
                controller.CanDash = true;
            }
        }
        else
            animator.SetBool("IsGrounded", false);
    }


    private float currentSpeed = 0;
    void Move2d()
    {
        if (animator.IsInTransition(0)) { return; }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxis("Vertical");

        
        //Move

        Vector3 horizonDir = Camera.main.transform.right;
        horizonDir.Set(horizonDir.x, 0, horizonDir.z);
        horizonDir.Normalize();


        //transform.Translate((h * horizonDir) * Time.deltaTime * Speed, Space.World);
        rb.MovePosition(rb.transform.position + (h * horizonDir) * Time.deltaTime * controller.Speed);

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
            rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, Quaternion.Euler(new Vector3(0, Camera.main.transform.rotation.eulerAngles.y + angle, 0)), 0.6f);

        }

        currentSpeed = Mathf.Lerp(currentSpeed, h, 0.3f);
        animator.SetFloat("Speed", Mathf.Abs(currentSpeed));

        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
        //{
        //    currentSpeed = Mathf.Lerp(currentSpeed, h, 0.3f);
        //    animator.SetFloat("Speed", Mathf.Abs(currentSpeed));
        //}


    }

    void Jump()
    {
        if (rb.velocity.y <= 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (controller. fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(controller.JumpKey))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (controller.lowJumpMultiPlier - 1) * Time.deltaTime;
        }


        if (controller.IsGrounded)
        {
            if (Input.GetKeyDown(controller.JumpKey))
            {
                animator.SetTrigger("StartJump");
                //animator.SetFloat("Speed", 0);
                rb.velocity = rb.transform.up * controller.JumpForce;
            }

        }
    }

    void Dash()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(controller.DashKey)&&controller.CanDash)
        {
            if (h == 0 && v == 0)
            {
                dashDir = rb.transform.rotation.eulerAngles.y <180 ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
            }
            else
            {
                dashDir = new Vector3(h, controller.IsGrounded ? Mathf.Max(v, 0) : v, 0).normalized;
            }

            if (dashDir.sqrMagnitude != 0)
            {
                dashPoint = rb.transform.position + dashDir * controller.DashDistance;
                isDashing = true;
                dashTimer = 0;
                controller.CanDash = false;
                animator.SetTrigger("Dash");
            }
        }
    }

    void Dashing()
    {
        dashTimer += Time.deltaTime;
        rb.useGravity = false;
        rb.MovePosition(rb.transform.position + dashDir * Time.deltaTime * controller.dashCurve.Evaluate(dashTimer) * controller.DashSpeed);
        if (dashTimer > controller.DashTime)
        {
            rb.useGravity = true;
            isDashing = false;
        }
    }


}
