using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    PlayerMove playerMove;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)||Input.GetKeyDown(KeyCode.J))
        {
            animator.SetTrigger("Attack");
        }
    }
}
