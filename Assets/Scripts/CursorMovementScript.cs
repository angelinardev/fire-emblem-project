using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMovementScript : MonoBehaviour
{
    int horizontal,
        vertical;

    public float blinkSpeed = 0.4f;

    public GameObject canvas;

    public bool lockMovement = false;

    private void Start()
    {
        //runs function Blink at the start and reruns it every 0.4 seconds by default
        InvokeRepeating("Blink", 0, blinkSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Controls();
        Debug.DrawRay(transform.position, -Vector2.up, Color.green);
    }

    void Blink()
    {
        //turns cursor on and off
        gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
    }

    private void FixedUpdate()
    {
        //stops all cursor movement and removes stored movement
        if (lockMovement)
        {
            horizontal = vertical = 0;
            return;
        }

        MovementUpdate();
    }


    private void Controls()
    {
        //sets left or right on
        if (Input.GetButtonDown("Horizontal"))
        {
            horizontal = (int)Input.GetAxisRaw("Horizontal");
        }

        //sets up or down on
        if (Input.GetButtonDown("Vertical"))
        {
            vertical = (int)Input.GetAxisRaw("Vertical");
        }

        if (Input.GetButtonDown("Confirm"))
        {
            //opens menus
            canvas.GetComponent<MenuController>().UpdateMenu(true);

            //disables cursor movement
            lockMovement = true;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
            

            // If it hits something
            if (hit.collider != null && hit.transform.GetComponent<MenuInfoSuppyCode>())
            {
                //print()
                hit.transform.GetComponent<MenuInfoSuppyCode>().FillMenu();
            }
        }

        if (Input.GetButtonDown("Return"))
        {
            //closes the menus currently visible
            canvas.GetComponent<MenuController>().UpdateMenu(false);

            //enables cursor movement
            lockMovement = false;
        }
    }

    private void MovementUpdate()
    {
        if(horizontal != 0 || vertical != 0)
        {
            //turns cursor back on
            if (!gameObject.GetComponent<SpriteRenderer>().enabled)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        //don't move past boundaries
        if (gameObject.transform.position.x + horizontal <= -22.5 || gameObject.transform.position.x + horizontal >= 9.5)
        {
            horizontal = 0;
        }
        if (gameObject.transform.position.y + vertical <= -2.5 || gameObject.transform.position.y+vertical >= 13.5)
        {
            vertical = 0;
        }
        //moves the cursor once then stops additional movement
        gameObject.transform.position += new Vector3(horizontal, vertical, 0);
       
        
        horizontal = vertical = 0;

        
    }
}
