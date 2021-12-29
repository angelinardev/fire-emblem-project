using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorMovementScript : MonoBehaviour
{
    private bool isBattle = false;//to tell when a battle is currently underway

    private int horizontal,
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
    //int end_count = 0;

    bool noMenu = false;
    private bool skipInput = false;

    public bool playerPhase = true;

    private float cursorDelay;

    public enum State
    {
        standing,
        moving,
        end
    }

    public State state;

    public Vector3
        currentPosition; //position while unit is moving 

    private int
        keypressNum, // the "X" of movement
        keypressPlacement; // the "Y" of movement

    public float slideSpeed = 5f; // how fast characters move on screen

    public List<int[]> keypress = new List<int[]>();

    private void Start()
    {
        //runs function Blink at the start and reruns it every 0.4 seconds by default 
        InvokeRepeating("Blink", 0, blinkSpeed);
    }

    public void EndPhase()
    {
        for (int i = 0; i < all_units.Length; i++)
        {
            all_units[i].GetComponent<StatsScript>().canMove = false;
        }
    }
    // Update is called once per frame 
    void Update()
    {
        //check for end turn
        for (int i = 0; i < all_units.Length; i++)
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
            lockMovement = true;
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
                    canvas.GetComponent<MenuController>().UpdateMenu(MenuController.Menus.confirmation);
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
                break;
        }
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

    void Blink()
    {
        //turns cursor on and off 
        gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
    }

    //lets cursor move by holding a direction
    private bool Repeater()
    {
        if (cursorDelay > 0)
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

    private void Controls()
    {
        if (canvas.GetComponent<MenuController>().currentMenu != MenuController.Menus.confirmation && canvas.GetComponent<MenuController>().currentMenu != MenuController.Menus.commands)
        {
            lockMovement = false;
        }
        else
        {
            lockMovement = true;
        }

        //sets left or right on
        if (Input.GetButtonDown("Horizontal") || Input.GetButton("Horizontal") && Repeater())
        {
            if (!lockMovement)
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
                canvas.GetComponent<MenuController>().CloseMenus();
            }
        }
        if (Input.GetButtonDown("Confirm") && !canvas.GetComponent<MenuController>().inUse && !unitSelected)
        {
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

                            //holds starting position

                            unitSelected = true;
                            canvas.GetComponent<MenuController>().UpdateMenu(unit.transform.GetComponent<StatsScript>());

                            remainMov = unit.transform.GetComponent<StatsScript>().Mov;
                            //record the position 
                            startPos = currentPos = desPos = unit.transform.position;

                            lockMovement = true;

                            return;
                        }
                    }
                }
            }
            //if no characters under the cursor continues
            if(!skipInput)
            {
                canvas.GetComponent<MenuController>().UpdateMenu(MenuController.Menus.commands);
                //disables cursor movement 
                lockMovement = true;
            }
        }
        else if (Input.GetButtonDown("Confirm"))
        {
            if (unitSelected && !canvas.GetComponent<MenuController>().inUse || unitSelected && canvas.GetComponent<MenuController>().currentMenu == MenuController.Menus.detailedInfo)
            {
                //player wants to move
                //check if theyre trying to move to a position with already a player
                RaycastHit2D[] hitAll = Physics2D.RaycastAll(transform.position, Vector2.zero);
                for (int i = 0; i < hitAll.Length; i++)
                {
                    if (hitAll[i].collider != null && hitAll[i].transform.GetComponent<MenuInfoSuppyCode>())
                    {
                        if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Player && hitAll[i] != unit)
                        {
                            noMenu = true;
                            return; //prevent further action
                        }
                    }
                }

                noMenu = false;
                currentPos = unit.transform.position;
                state = State.moving;
                totalSteps = new Vector3(0, 0, 0);
                unit.transform.GetComponent<MenuInfoSuppyCode>().stop_b();
                lockMovement = true;
            }
            else if (canvas.GetComponent<MenuController>().currentMenu == MenuController.Menus.basicinfo && canvas.GetComponent<MenuController>().inUse)
            {
                canvas.GetComponent<MenuController>().UpdateMenu(true);
            }
            else
            {
                lockMovement = true;
                canvas.GetComponent<MenuController>().UpdateMenu(MenuController.Menus.confirmation);
            }
        }

        if (Input.GetButtonDown("Return"))
        {
            //closes the menus currently visible 
            noMenu = false;
            canvas.GetComponent<MenuController>().CloseMenus();

            //enables cursor movement 
            lockMovement = false;

            if (unitSelected)
            {
                if (canvas.GetComponent<MenuController>().currentMenu == MenuController.Menus.basicinfo)
                {
                    unitSelected = false;
                    //totalSteps = 0;  
                    totalSteps = new Vector3(0, 0, 0);
                    keypress.Clear();
                    unit.transform.GetComponent<MenuInfoSuppyCode>().stop_b();
                    SnapBack();
                }
                else if (canvas.GetComponent<MenuController>().currentMenu == MenuController.Menus.detailedInfo)
                {
                    canvas.GetComponent<MenuController>().currentMenu = MenuController.Menus.basicinfo;
                    lockMovement = true;
                }
                else if (canvas.GetComponent<MenuController>().currentMenu == MenuController.Menus.confirmation)
                {
                    canvas.GetComponent<MenuController>().currentMenu = MenuController.Menus.basicinfo;
                    unit.transform.GetComponent<MenuInfoSuppyCode>().start_b();
                    lockMovement = true;
                    SnapBack();
                    totalSteps = new Vector3(0, 0, 0);
                    keypress.Clear();
                }
                else//just a cancel movement
                {
                    unitSelected = false;
                    //totalSteps = 0;  
                    totalSteps = new Vector3(0, 0, 0);
                    keypress.Clear();
                    unit.transform.GetComponent<MenuInfoSuppyCode>().stop_b();
                    SnapBack();
                }
            }
            else if(canvas.GetComponent<MenuController>().currentMenu == MenuController.Menus.commands)
            {
                canvas.GetComponent<MenuController>().currentMenu = MenuController.Menus.basicinfo;
            }

            if(isBattle)
            {
                isBattle = false;
                SceneManager.UnloadSceneAsync("BattleScene");
            }
        }
        skipInput = false;
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
        canvas.GetComponent<MenuController>().currentMenu = MenuController.Menus.basicinfo;
        skipInput = true;
        lockMovement = false;
        unitSelected = false;
        if(unit.transform != null)
        {
            unit.transform.GetComponent<StatsScript>().canMove = false;
            //change sprite
            unit.transform.GetComponent<SpriteRenderer>().sprite = unit.transform.GetComponent<StatsScript>().endTurn;
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

    public void TargetEnemy()
    {
        List<Vector3> enemyList = new List<Vector3>();//list of enemies POSITION you can attack
        List<GameObject> enemies = new List<GameObject>(); //actual enemy data

        List<Vector3> newPos = new List<Vector3>();// each position in your attack range
        List<RaycastHit2D[]> hitEverything = new List<RaycastHit2D[]>();

        List<Vector3> modifier = new List<Vector3>();

        Weapons thisWep = new Weapons();
        for (int i=0; i < unit.transform.GetComponent<StatsScript>().inventory.Count; i++)
        {
            if (unit.transform.GetComponent<StatsScript>().inventory[i].equipped)
            {
                thisWep = (Weapons)unit.transform.GetComponent<StatsScript>().inventory[i];
                break;
            }
            else if(thisWep != null)
            {
                thisWep = (Weapons)unit.transform.GetComponent<StatsScript>().inventory[0];
            }
        }

        if (thisWep.minRange == 1)
        {
            modifier.Add(new Vector3(-1, 0, 0));//left
            modifier.Add(new Vector3(0, -1, 0));//down
            modifier.Add(new Vector3(1, 0, 0));//right
            modifier.Add(new Vector3(0, 1, 0));//up
        }
        if (thisWep.minRange == 2 || thisWep.maxRange >= 2)
        {
            modifier.Add(new Vector3(-2, 0, 0)); modifier.Add(new Vector3(0, -2, 0)); modifier.Add(new Vector3(2, 0, 0)); modifier.Add(new Vector3(0, 2, 0));//straights
            modifier.Add(new Vector3(-1, 1, 0)); modifier.Add(new Vector3(1, -1, 0)); modifier.Add(new Vector3(1, 1, 0)); modifier.Add(new Vector3(1, 1, 0));//diagonals
        }
        if (thisWep.minRange == 3 || thisWep.maxRange >= 3)
        {
            modifier.Add(new Vector3(-3, 0, 0)); modifier.Add(new Vector3(0, -3, 0)); modifier.Add(new Vector3(3, 0, 0)); modifier.Add(new Vector3(0, 3, 0));//straights
            modifier.Add(new Vector3(-2, 1, 0)); modifier.Add(new Vector3(2, -1, 0)); modifier.Add(new Vector3(2, 1, 0)); modifier.Add(new Vector3(2, 1, 0));//diagonals
            modifier.Add(new Vector3(-1, 2, 0)); modifier.Add(new Vector3(1, -2, 0)); modifier.Add(new Vector3(1, 2, 0)); modifier.Add(new Vector3(1, 2, 0));//diagonals
        }
        
        for (int i = 0; i < modifier.Count; i++)
        {
            newPos.Add(new Vector3(transform.position.x + modifier[i].x, transform.position.y + modifier[i].y, 0));
            hitEverything.Add(Physics2D.RaycastAll(newPos[i], Vector2.zero));
        }

        for (int i = 0; i < hitEverything.Count; i++)
        {
            for (int j = 0; j < hitEverything[i].Length; j++)
            {
                if (hitEverything[i][j].collider != null && hitEverything[i][j].transform.GetComponent<MenuInfoSuppyCode>() && hitEverything[i][j].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Enemy)
                {
                    enemyList.Add(newPos[i]);
                    enemies.Add(hitEverything[i][j].transform.gameObject);
                }
            }
        }
        print("there are " + enemyList.Count + " enemies in range");
        if(enemyList.Count > 0)
        {
            SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
            isBattle = true;
        }
        //need to have cursor move to first enemy location and save locations to cycle through

        //enemy calculations
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