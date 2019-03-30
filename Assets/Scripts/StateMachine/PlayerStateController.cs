using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour {

    
    IBasePlayerState OldPlayerState;

    IBasePlayerState CurrentState;

    #region Fields

    public bool isAttacking=false;
    public bool IsGrounded = true;
    [Header("Move")]
    public float Speed = 5;
    [Header("Inputs")]
    public KeyCode JumpKey;
    public KeyCode DashKey;
    public KeyCode AttackKey;
    [Header("Jump")]
    public float JumpForce = 10;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiPlier = 2f;
    public float CheckGroundRadius = 0.5f;
    [Header("Dash")]
    public AnimationCurve dashCurve;
    public float DashDistance = 3;
    public float DashTime = 1;
    public float DashSpeed = 3;
    
    #endregion


    #region Properties

    public Animator PlayerAnimator { get; private set; }
    public Rigidbody PlayerRb { get; private set; }
    public bool CanDash { get; set; }
    #endregion


    private void Awake()
    {
        PlayerAnimator = GetComponent<Animator>();
        PlayerRb = GetComponent<Rigidbody>();
        CanDash = false;
    }

    // Use this for initialization
    void Start () {
        CurrentState = new PlayerMove2DState(this);
        OldPlayerState = new PlayerMove2DState(this);
	}
	
	// Update is called once per frame
	void Update () {
        CurrentState.HandleInput();
        CurrentState.Update();

    }

    private void FixedUpdate()
    {
        CurrentState.FixedUpdate();
    }

    public void SetPlayerState(IBasePlayerState newState)
    {
        OldPlayerState = CurrentState;
        OldPlayerState.OnStateExit();
        CurrentState = newState;
        CurrentState.OnStateEnter();
    }

}
