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

    public GameObject[] all_enemies = { };
    int counter = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cursor.transform.GetComponent<CursorMovementScript>().playerPhase)
        {
            myPlay = cursor.transform;
            counter = 0;
        }
        else
        {
            //enemy turn
            for (int i = 0; i < all_enemies.Length; i++) //all enemis go
            {
                if (all_enemies[i].activeSelf && all_enemies[i].GetComponent<StatsScript>().canMove)
                {
                    all_enemies[i].transform.GetComponent<AIMovement>().ActivateTurn();
                    myPlay = all_enemies[i].transform;
                    counter = i;
                    break;
                    
                }
            }
            //enemy turn over

        }

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
