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
    public Vector3 startPos;

    public Vector3 totalSteps = new Vector3(0, 0, 0);

    public RaycastHit2D unit;

    public GameObject[] all_units = { };
    int end_count = 0;

    public bool charaMenu = false;
    public bool charaMenu2 = false;
    public bool charaMenu3 = false;

    bool noMenu = false;

    public bool playerPhase = true;


    private float cursorDelay;
    /*
     *will go on characters later just for testing purposes 
     * 
     */
    public enum State
    {
        standing,
        moving,
        end
    }

    public State state;

    public Vector3
        currentPosition; //position while unit is moving 
                         // last position unit was moved to 

    private int
        keypressNum, // the "X" of movement
        keypressPlacement; // the "Y" of movement

    public float slideSpeed = 5f; // how fast characters move on screen
    /* 
    * 
    * 
    */

    public List<int[]> keypress = new List<int[]>();

    private void Start()
    {
        //runs function Blink at the start and reruns it every 0.4 seconds by default 
        InvokeRepeating("Blink", 0, blinkSpeed);
    }

    // Update is called once per frame 
    void Update()
    {
        //check for end turn
        for (int i=0; i < all_units.Length;i++)
        {
            if (all_units[i].activeSelf && all_units[i].GetComponent<StatsScript>().canMove) //also check if unit is in the scene
            {
                playerPhase = true;
                break;
            }
            playerPhase = false;
            
        }
        
        if (!playerPhase)
        {
            //player cant move?
            //lockMovement = true;
            //enable enemy phase movement
        }
        

        Controls();

        switch (state)
        {
            case State.standing:
                keypressPlacement = keypressNum = 0;
                break;
            case State.end:
                break;
            case State.moving:

                if (keypressNum >= keypress.Count)
                {
                    keypressPlacement = keypressNum = 0;
                    keypress.Clear();
                    state = State.end;
                }
                else if (keypressPlacement == 0)
                {
                    if (MoveUnit(new Vector3((currentPos.x + keypress[keypressNum][keypressPlacement]), currentPos.y, 0)))
                    {
                        keypressPlacement++;
                    }
                }
                else if (keypressPlacement == 1)
                {
                    if (MoveUnit(new Vector3(currentPos.x, (currentPos.y + keypress[keypressNum][keypressPlacement]), 0)))
                    {
                        keypressNum++;
                        keypressPlacement = 0;
                    }
                }

                //for (int i = 0; i < keypress.Count; i++)
                //{
                //    if (unit.transform.position.x >= desPos.x) //for real you'd  do a more precise check to see if it went past the des or not
                //    {
                //        keypress[i][0] = 0;
                //    }
                //    if (unit.transform.position.y >= desPos.y)
                //    {
                //        keypress[i][1] = 0;
                //    }
                //    print("here");
                //    unit.rigidbody.velocity.Set(keypress[i][0], keypress[i][1]);
                //}
                break;
        }
    }

    /// <summary>
    /// more testing
    /// 
    /// </summary>
    public bool MoveUnit(Vector3 route)
    {
        currentPosition = unit.transform.position;

        //moves the character torwards the next location

        unit.transform.position += (route - currentPosition) * slideSpeed * Time.deltaTime;
        //how close before unit snaps into place
        float reachedDistance = 0.1f;

        //if unit is close enough ends the movement
        if (Vector3.Distance(currentPosition, route) < reachedDistance)
        {
            //reached target
            unit.transform.position = route;
            //sets next new position for movement
            currentPos = unit.transform.position;
            return true;
        }

        return false;
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

    private bool Repeater()
    {
        if(cursorDelay > 0)
        {
            cursorDelay -= Time.deltaTime;
            return false;
        }
        else
        {
            cursorDelay = 0;
            return true;
        }
    }
    private void Controls()
    {
        if (charaMenu || charaMenu2)
        {
            lockMovement = false;
        }
        else if (charaMenu3)
        {
            lockMovement = true;
        }
        charaMenu = false;
        charaMenu2 = false;
        //sets left or right on
        if (Input.GetButtonDown("Horizontal") || Input.GetButton("Horizontal") && Repeater())
        {
            if(!lockMovement)
            {
                cursorDelay = .25f;
                horizontal = (int)Input.GetAxisRaw("Horizontal");
            }
        }
        //sets up or down on 
        if (Input.GetButtonDown("Vertical") || Input.GetButton("Vertical") && Repeater())
        {
            if (!lockMovement)
            {
                cursorDelay = .25f;
                vertical = (int)Input.GetAxisRaw("Vertical");
            }
        }
        if (horizontal != 0 || vertical != 0)
        {
            if (canvas.GetComponent<MenuController>().inUse && (canvas.GetComponent<MenuController>().currentMenu != MenuController.Menus.commands && canvas.GetComponent<MenuController>().currentMenu != MenuController.Menus.confirmation))
            {
                canvas.GetComponent<MenuController>().currentMenu = MenuController.Menus.detailedInfo;
                canvas.GetComponent<MenuController>().HideMenus();
            }
        }
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
                        if (unit.transform.GetComponent<StatsScript>().canMove)
                        {
                            //make unit avatar blink 
                            unit.transform.GetComponent<MenuInfoSuppyCode>().start_b();

                            //check if there is any key press, then close the menu 

                            //holds starting position
                            
                            unitSelected = true;
                            canvas.GetComponent<MenuController>().UpdateMenu(unit.transform.GetComponent<StatsScript>());

                            remainMov = unit.transform.GetComponent<StatsScript>().Mov;
                            //record the position 
                            startPos = currentPos = desPos = unit.transform.position;

                            charaMenu = true;
                            lockMovement = true;

                            return;
                        }
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
                lockMovement = true;
                //more complex  
            }
        }
        else if (Input.GetButtonDown("Confirm"))
        {
            if (unitSelected && charaMenu3)
            {
                EndTurn();
                lockMovement = false;
                charaMenu3 = false;
                charaMenu = false;
                charaMenu2 = false;
                //CLOSE MENUS
                canvas.GetComponent<MenuController>().CloseMenus();
                canvas.GetComponent<MenuController>().currentMenu = MenuController.Menus.basicinfo;
            }
            if (unitSelected && !charaMenu && !charaMenu2 && !charaMenu3)
            {
                //player wants to move
                //check if theyre trying to move to a position with already a player
                RaycastHit2D[] hitAll = Physics2D.RaycastAll(transform.position, Vector2.zero);
                for (int i = 0; i < hitAll.Length; i++)
                {
                    if (hitAll[i].collider != null && hitAll[i].transform.GetComponent<MenuInfoSuppyCode>())
                    {
                        if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Player)
                        {
                            noMenu = true;
                            return; //prevent further action
                        }
                    }
                }
                noMenu = false;
                currentPos = unit.transform.position;
                state = State.moving;
                charaMenu3 = true;
                totalSteps = new Vector3(0, 0, 0);
                //keypress.Clear();
                unit.transform.GetComponent<MenuInfoSuppyCode>().stop_b();

            }
            

            if (canvas.GetComponent<MenuController>().currentMenu == MenuController.Menus.basicinfo)
            {
                canvas.GetComponent<MenuController>().UpdateMenu(true);
            }
            else
            {
                //lockMovement = true;
                canvas.GetComponent<MenuController>().UpdateMenu(MenuController.Menus.confirmation);
            }
        }

        if (Input.GetButtonDown("Return"))
        {
            //closes the menus currently visible 
            noMenu = false;
            canvas.GetComponent<MenuController>().CloseMenus();
            canvas.GetComponent<MenuController>().currentMenu = MenuController.Menus.basicinfo;

            //enables cursor movement 
            lockMovement = false;
            print(unitSelected);
            if (unitSelected)
            {
                
                print("return");
                if (charaMenu)
                {
                    charaMenu = false;
                    unitSelected = false;
                    //totalSteps = 0;  
                    totalSteps = new Vector3(0, 0, 0);
                    keypress.Clear();
                    unit.transform.GetComponent<MenuInfoSuppyCode>().stop_b();
                    SnapBack();
                }
                else if (charaMenu2)
                {
                    charaMenu2 = false;
                    lockMovement = true;
                    
                }
                else if (charaMenu3)
                {
                    charaMenu3 = false;
                    charaMenu = true;
                    //pos reset  
                    SnapBack();
                    totalSteps = new Vector3(0, 0, 0);
                    keypress.Clear();
                }
                else if (!charaMenu && !charaMenu2 && !charaMenu3) //just a cancel movement
                {
                    unitSelected = false;
                    //totalSteps = 0;  
                    totalSteps = new Vector3(0, 0, 0);
                    keypress.Clear();
                    unit.transform.GetComponent<MenuInfoSuppyCode>().stop_b();
                    SnapBack();
                }
            }
        }
        
    }

    private void SnapBack()
    {
        unit.transform.position = gameObject.transform.position = startPos;
    }

    public void EndTurn()
    {
        //simply approach
        //unit.transform.position = desPos; //doesnt have animation for now
        //animation?

        unitSelected = false;
        unit.transform.GetComponent<StatsScript>().canMove = false;

        //change sprite
        unit.transform.GetComponent<SpriteRenderer>().sprite = unit.transform.GetComponent<StatsScript>().endTurn;
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
                //still need to check for enemies
                for (int i = 0; i < hitAll.Length; i++)
                {
                    if (hitAll[i].collider != null && hitAll[i].transform.GetComponent<MenuInfoSuppyCode>())
                    {
                        if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Enemy)
                        {
                            horizontal = 0;
                            vertical = 0;
                            break;
                        }
                    }
                }
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
                        //every condition where no one can traverse
                        if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Mountain || (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.House) || (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Enemy))
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
            if (horizontal != 0 || vertical != 0)
            {
                int[] presses = { horizontal, vertical };
                keypress.Add(presses);
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