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

    public bool charaMenu = false;
    public bool charaMenu2 = false;
    public bool charaMenu3 = false;

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
        if (horizontal != 0 || vertical != 0)
        {
            if (canvas.GetComponent<MenuController>().inUse && (canvas.GetComponent<MenuController>().currentMenu != MenuController.Menus.commands && canvas.GetComponent<MenuController>().currentMenu != MenuController.Menus.confirmation))
            {
                canvas.GetComponent<MenuController>().currentMenu = MenuController.Menus.detailedInfo;
                canvas.GetComponent<MenuController>().HideMenus();
            }
        }
        if (!charaMenu)
        {
            if (Input.GetButtonDown("Confirm") && !canvas.GetComponent<MenuController>().inUse)
            {

                //RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up); 
                RaycastHit2D[] hitAll = Physics2D.RaycastAll(transform.position, Vector2.zero);

                for (int i = 0; i < hitAll.Length; i++)
                {
                    if (hitAll[i].collider != null && hitAll[i].transform.GetComponent<MenuInfoSuppyCode>())
                    {
                        if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Player)
                        {
                            unit = hitAll[i];
                            //make unit avatar blink 
                            unit.transform.GetComponent<MenuInfoSuppyCode>().start_b();

                            //check if there is any key press, then close the menu 

                            unitSelected = true;
                            canvas.GetComponent<MenuController>().UpdateMenu(unit.transform.GetComponent<StatsScript>());

                            remainMov = unit.transform.GetComponent<StatsScript>().Mov;
                            //record the position 
                            currentPos = desPos = unit.transform.position;

                            print(hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().name);

                            return;
                        }

                    }
                }
                //if no characters under the cursor continues 

                if (!unitSelected)
                {
                    canvas.GetComponent<MenuController>().UpdateMenu(MenuController.Menus.commands);
                    //disables cursor movement 
                    lockMovement = true;
                }

                else if (charaMenu)
                {
                    charaMenu2 = true;
                    charaMenu = false;
                }
                if (charaMenu2)
                {
                    charaMenu2 = false;
                    charaMenu3 = true;
                }
                if (charaMenu3)
                {
                    //more complex  
                }

            }
            else if (Input.GetButtonDown("Confirm"))
            {
                if (canvas.GetComponent<MenuController>().currentMenu == MenuController.Menus.basicinfo)
                {
                    canvas.GetComponent<MenuController>().UpdateMenu(true);
                }
                else
                {
                    lockMovement = true;
                    canvas.GetComponent<MenuController>().UpdateMenu(MenuController.Menus.confirmation);
                }
            }
        }


        if (Input.GetButtonDown("Return"))
        {
            //closes the menus currently visible 

            canvas.GetComponent<MenuController>().CloseMenus();
            canvas.GetComponent<MenuController>().currentMenu = MenuController.Menus.basicinfo;

            //enables cursor movement 
            lockMovement = false;
            if (unitSelected)
            {
                if (charaMenu)
                {
                    charaMenu = false;
                    unitSelected = false;
                    //totalSteps = 0;  
                    totalSteps = new Vector3(0, 0, 0);
                    unit.transform.GetComponent<MenuInfoSuppyCode>().stop_b();
                    snapBack();
                }
                else if (charaMenu2)
                {
                    charaMenu2 = false;
                }
                else if (charaMenu3)
                {
                    charaMenu3 = false;
                    charaMenu = true;
                    //pos reset  
                    snapBack();
                    totalSteps = new Vector3(0, 0, 0);
                }
                else if (!charaMenu && !charaMenu2 && !charaMenu3)
                {
                    unitSelected = false;
                    //totalSteps = 0;  
                    totalSteps = new Vector3(0, 0, 0);
                    unit.transform.GetComponent<MenuInfoSuppyCode>().stop_b();
                    snapBack();
                }
            }
        }
    }

    private void snapBack()
    {
        gameObject.transform.position = currentPos;
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
                            if ((unit.transform.GetComponent<StatsScript>().classes != StatsScript.Classes.Pirate))
                            {
                                horizontal = 0;
                                vertical = 0;
                                break;
                            }

                        }
                        if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Hill)
                        {//horse units 
                            if (unit.transform.GetComponent<StatsScript>().classes == StatsScript.Classes.Paladin || unit.transform.GetComponent<StatsScript>().classes == StatsScript.Classes.Cavalier)
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
                        if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.House)
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
            if (!((Mathf.Abs(totalSteps.x + horizontal) + Mathf.Abs(totalSteps.y + vertical) <= remainMov)))
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


    ////notes 
    /* 
     * A - sub menu with basic character info 
     * name class level hp 
     * B- deselects character 
     * Arrows - hide menu 
     *  
     * A - again 
     * menu with specific stats/ char portrait 
     * B - hide menus 
     *  
     *  
     * Moving character -  
     * B - orginal character and cursor pos delselects character 
     *  
     *  
     *  
     * A - the continuation/ end movement 
     * /attack/ /special actions/ item wait 
     * B - return to original position and opens first menu 
     *  
     *  
     * */
}