using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour {

    
    IBasePlayerState OldPlayerState;

    IBasePlayerState CurrentState;

    #region 一些共有变量

    
    public Animator PlayerAnimator
    {
        get
        {
            return playerAnimator;
        }
    }

    public Camera PlayerCam
    {
        get
        {
            return playerCam;
        }

    }



    private Animator playerAnimator;

    private Camera playerCam;


    public bool isAttacking=false;
    #endregion

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerCam = Camera.main;
    }

    // Use this for initialization
    void Start () {
        //CurrentState = new PlayerStandState(this);
        //OldPlayerState = new PlayerStandState(this);
	}
	
	// Update is called once per frame
	void Update () {
        CurrentState.Update();
        CurrentState.HandleInput();
	}

    public void SetPlayerState(IBasePlayerState newState)
    {
        OldPlayerState = CurrentState;
        OldPlayerState.OnStateExit();
        CurrentState = newState;
        CurrentState.OnStateEnter();
    }

}
