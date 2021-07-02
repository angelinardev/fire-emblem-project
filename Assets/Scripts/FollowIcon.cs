using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowIcon : MonoBehaviour
{
    public Vector3 myPos;
    public Transform myPlay;

    //for moving camera
    public GameObject
        left,
        right,
        up,
        down,
        cursor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = myPlay.position + myPos.normalized;

        //if(left.GetComponent<CameraScreenScript>().move)
        //{
        //    transform.position = Vector3.left + myPos.normalized;
        //}
        //else if (right.GetComponent<CameraScreenScript>().move)
        //{
        //    transform.position = Vector3.right + myPos.normalized;
        //}
        //else if (up.GetComponent<CameraScreenScript>().move)
        //{
        //    transform.position = Vector3.up + myPos.normalized;
        //}
        //else if (down.GetComponent<CameraScreenScript>().move)
        //{
        //    transform.position = Vector3.down + myPos.normalized;
        //}
    }
}
