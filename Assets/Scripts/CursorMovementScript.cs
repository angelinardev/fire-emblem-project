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

    public bool unitSelected = false;

    public int remainMov = 0;

    public Vector3 currentPos;
    public Vector3 desPos;

    public Vector3 totalSteps = new Vector3(0, 0, 0);

    public RaycastHit2D unit;

    private void Start()
    {
        //runs function Blink at the start and reruns it every 0.4 seconds by default
        InvokeRepeating("Blink", 0, blinkSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Controls();
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

            //RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
            RaycastHit2D[] hitAll = Physics2D.RaycastAll(transform.position, Vector2.zero);

            for(int i = 0; i < hitAll.Length; i++)
            {
                if(hitAll[i].collider != null && hitAll[i].transform.GetComponent<MenuInfoSuppyCode>())
                {
                    if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Player)
                    {
                        unit = hitAll[i];
                        //make unit avatar blink
                        unit.transform.GetComponent<MenuInfoSuppyCode>().start_b();

                        //check if there is any key press, then close the menu

                        unitSelected = true;
                        remainMov = unit.transform.GetComponent<StatsScript>().Mov;
                        //record the position
                        currentPos = desPos = unit.transform.position;

                        print(hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().name);

                        return;
                    }
                }
            }
            //if no characters under the cursor continues

            //opens menus
            canvas.GetComponent<MenuController>().UpdateMenu(true);

            //disables cursor movement
            lockMovement = true;

            


        }

        if (Input.GetButtonDown("Return"))
        {
            //closes the menus currently visible
            canvas.GetComponent<MenuController>().UpdateMenu(false);

            //enables cursor movement
            lockMovement = false;
            if (unitSelected)
            {
                
                unitSelected = false;
                //totalSteps = 0;
                totalSteps = new Vector3(0, 0, 0);
                unit.transform.GetComponent<MenuInfoSuppyCode>().stop_b();  
            }
        }
    }

    private void MovementUpdate()
    {
        if (horizontal != 0 || vertical != 0)
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
        if (gameObject.transform.position.y + vertical <= -2.5 || gameObject.transform.position.y + vertical >= 13.5)
        {
            vertical = 0;
        }

        //check to see if movement range is possible
        if (unitSelected)
        {
            Vector3 newPos = new Vector3(horizontal, vertical, 0);
            RaycastHit2D[] hitAll = Physics2D.RaycastAll(transform.position + newPos, Vector2.zero);
            //check for special class properties
            //flying uniys bypass restrictions
            if (unit.transform.GetComponent<StatsScript>().classes == StatsScript.Classes.Pegasus_Knight || unit.transform.GetComponent<StatsScript>().classes == StatsScript.Classes.Wyvern_Knight)
            {
                //
            }
            else
            {//check for tiles that cant be traversed
                for (int i = 0; i < hitAll.Length; i++)
                {
                    if (hitAll[i].collider != null && hitAll[i].transform.GetComponent<MenuInfoSuppyCode>())
                    {
                        if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Water)
                        {
                            if (!(unit.transform.GetComponent<StatsScript>().classes == StatsScript.Classes.Pirate))
                            {
                                horizontal = 0;
                                vertical = 0;
                                break;
                            }

                        }
                        if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Hill)
                        {//horse units
                            if(unit.transform.GetComponent<StatsScript>().classes == StatsScript.Classes.Paladin || unit.transform.GetComponent<StatsScript>().classes == StatsScript.Classes.Cavalier)
                            {
                                horizontal = 0;
                                vertical = 0;
                                break;
                            }

                        }
                       if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Mountain)
                        {
                            horizontal = 0;
                            vertical = 0;
                            break;
                        }
                        if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Building)
                        {
                            horizontal = 0;
                            vertical = 0;
                            break;
                        }

                    }
                }
            }
            
            //determine the number of steps taken by taking the difference between the destination position and the current position
            totalSteps = desPos - currentPos;
            //some algorithm
            //check how many spaces can be moved
            if (!((Mathf.Abs(totalSteps.x+horizontal)  + Mathf.Abs(totalSteps.y+vertical) < remainMov)))
            {
                horizontal = 0;
                vertical = 0;
            }
            else //success
            {
                //increment to get to possible grid space
                desPos.x += horizontal;
                desPos.y += vertical;
                

            }
        }
        //moves the cursor once then stops additional movement
        gameObject.transform.position += new Vector3(horizontal, vertical, 0);


        horizontal = vertical = 0;


    }
}