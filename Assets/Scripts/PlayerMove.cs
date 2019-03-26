using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    private float Speed = 5;
    private Animator animator;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");    


        //Move

        Vector3 horizonDir = Camera.main.transform.right;
        horizonDir.Set(horizonDir.x, 0, horizonDir.z);
        horizonDir.Normalize();

        #region 3d Move and Rot
        //Vector3 verticalDir = Camera.main.transform.forward;
        //verticalDir.Set(verticalDir.x, 0, verticalDir.z);
        //verticalDir.Normalize();

        //transform.Translate((h*horizonDir+v*verticalDir) * Time.deltaTime*Speed,Space.World);

        //Rotate
        //if (h != 0 || v != 0)
        //{
        //    float angle = 0;
        //    if (h >= 0)
        //    {
        //        angle = Vector3.Angle(new Vector3(h, v), Vector3.up);
        //    }
        //    else
        //    {
        //        angle = -Vector3.Angle(new Vector3(h, v), Vector3.up);
        //    }
        //    transform.rotation = Quaternion.Slerp( transform.rotation,Quaternion.Euler(new Vector3(0, Camera.main.transform.rotation.eulerAngles.y + angle, 0)),0.6f);

        //}
        #endregion


        transform.Translate((h * horizonDir) * Time.deltaTime * Speed, Space.World);

        if (h != 0 )
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
            transform.rotation = Quaternion.Slerp( transform.rotation,Quaternion.Euler(new Vector3(0, Camera.main.transform.rotation.eulerAngles.y + angle, 0)),0.6f);

        }

        //Anim
        animator.SetFloat("Speed", Mathf.Abs(h));

    }
}
