using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScreenScript : MonoBehaviour
{
    public bool move;
    void OnTriggerEnter(Collider other)
    {
        // other object is close
        print(other.name);
        if (other.GetComponent<CursorMovementScript>())
        {
            move = true;
        }
        else
            move = false;
    }
}
