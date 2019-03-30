using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : IBasePlayerState
{
    private PlayerStateController controller;
    private Animator animator;

    public PlayerAttackState(PlayerStateController controller)
    {
        this.controller = controller;
        animator = controller.PlayerAnimator;
    }

    public void OnStateEnter()
    {
        Attack();
    }
    public void FixedUpdate()
    {

    }

    public void HandleInput()
    {
        if (animator.IsInTransition(0))
        {
            return;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Move"))
            {
                controller.SetPlayerState(new PlayerMove2DState(controller));
                Debug.Log("exit attack");
            }
        }
    }

    public void OnStateExit()
    {
        animator.ResetTrigger("Attack");
    }

    public void Update()
    {
        if (Input.GetKeyDown(controller.AttackKey))
        {
            Attack();
        }
        CheckGround();

    }

    void Attack()
    {
        animator.SetTrigger("Attack");
    }

    void CheckGround()
    {
        controller.IsGrounded = Physics.CheckSphere(controller.transform.position, controller.CheckGroundRadius, LayerMask.GetMask("Ground"));
        animator.SetBool("IsGrounded", controller.IsGrounded);
    }
}
