using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public bool activated = false;

    public GameObject target;
    public GameObject cursor;

    private GameObject attackTarget;

    private List<Vector3> openList = new List<Vector3>();
    private List<int> scores = new List<int>();
    private List<Vector3> closedList= new List<Vector3>();

    private int movedTiles = 0;

    public bool attacking = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetTarget()
    {
        int highest = 0;
        int current;
        GameObject temp;
        GameObject[] players = cursor.GetComponent<CursorMovementScript>().all_units;
        //set default target to first unit
        temp = players[0];
        print(players.Length);
        for (int i=0; i< players.Length; i++)
        {
            if (players[i].activeSelf)
            {
                //calculate theoretical damage
                current =gameObject.GetComponent<StatsScript>().Str - players[i].GetComponent<StatsScript>().Def;
                if (current >= highest)
                {
                    //we want to target the player we can do the most damage to
                    temp = players[i];
                    highest = current;
                }

            }
        }
        target = temp;
    }
    public void ActivateTurn()
    {
        activated = true;
        transform.GetComponent<StatsScript>().canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        //do movement
        if (activated)
        {

            if (transform.GetComponent<StatsScript>().name.Equals("Gazzak")) //boss doesnt move
            {
                transform.position = new Vector3(-16.5F, 6.5F, 0);
                activated = false;
                transform.GetComponent<StatsScript>().canMove = false;
                return;
            }
            //add current position to the closed list
            closedList.Add(gameObject.transform.position);
            SetTarget(); //movement target

            //print(target.name);

            Movement(); //do movement
            activated = false;
            transform.GetComponent<StatsScript>().canMove = false;

        }

        //and then do attack
    }
    public void CalculatePath(Vector3 square)
    {
        //check all adjacent squares
        checkAdjacent(square + new Vector3(0, 1), true);
        checkAdjacent(square + new Vector3(0, -1), true);
        checkAdjacent(square + new Vector3(1, 0), true);
        checkAdjacent(square + new Vector3(-1, 0), true);

        int lowest = 100000; //some arbitrary high number
        int index = 0;
        Vector3 temp = gameObject.transform.position;

        //print(openList.Count);

        for (int i=0; i< openList.Count; i++)
        {
            //find the tile in the open list that has the lowest score
            if (scores[i] <= lowest)
            {
                //print("Swapped score");
                lowest = scores[i];
                temp = openList[i];
                index = i;
            }
        }
        //remove from the open list
        if (openList.Contains(temp))
        {
           // openList.RemoveAt(index);
            openList.Remove(openList[index]);
            //scores.RemoveAt(index);
            scores.Remove(scores[index]);
            //add to the closed list
            closedList.Add(temp);
            //print("Added to closed list");
        }
        //check adjacent tiles of the one we just added
        //we can actually just do this by recursively calling
        movedTiles += 1;//increase the number we are going by, aka how far along we're going

        //we also want to check if any player characters are in range, because this will bypass the searching mechanism
        //check all adjacent squares
        checkAdjacent(square+ new Vector3(0, 1), false);
        //we want only attack once per turn
        if (!attacking)
        {
            checkAdjacent(square + new Vector3(0, -1), false);
        }
        if (!attacking)
        {
            checkAdjacent(square + new Vector3(1, 0), false);
        }
        if (!attacking)
        {
            checkAdjacent(square + new Vector3(-1, 0), false);
        }

        if (!attacking && movedTiles < gameObject.GetComponent<StatsScript>().Mov) //to continue moving, need to not be attacking AND have leftover movement
        {
            CalculatePath(temp);
        }

    }
  
    public void Movement()
    {
        CalculatePath(transform.position);
        for (int i=0; i< closedList.Count; i++)
        {
            transform.position = closedList[i];
            //print(transform.position);
        }
        if (attacking)
        {
            Attack(attackTarget); //attack at the end of movement
        }
        //at the end
        movedTiles = 0;
        openList.Clear();
        closedList.Clear();
        scores.Clear();
        attacking = false;
    }

    public void Attack(GameObject player)
    {

    }

    public void checkAdjacent(Vector3 pos, bool checking)
    {
        //print("testing test");
        //print(pos);
        RaycastHit2D[] hitAll = Physics2D.RaycastAll(pos, Vector2.zero);

        for (int i = 0; i < hitAll.Length; i++)
        {
            if (hitAll[i].collider != null && hitAll[i].transform.GetComponent<MenuInfoSuppyCode>())
            {
               // print("is it going in");
                if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Player && !checking)
                {
                    attackTarget = hitAll[i].transform.gameObject;
                    //Attack(hitAll[i].transform.gameObject);
                    //print("Attacking");
                    attacking = true;
                    break;

                }
                else if (checking) //PURELY FOR THE A* ALGORITHM
                {
                    //print("Checking tiles");
                    //check if its already in the closed list
                    if (closedList.Contains(pos))
                    {
                        //print("Already in closed list");
                        return; //ignore it
                    }
                    //check if its already in the open list
                    if (openList.Contains(pos))
                    {
                       // print("Already in open list");
                        //update score
                        int G = movedTiles + 1;
                        int H = (int)Mathf.Abs((target.transform.position.y - pos.y) + (target.transform.position.x - pos.x));
                        if (G + H < scores[openList.IndexOf(pos)]) //is our new score lower with the generated path
                        {
                            scores[openList.IndexOf(pos)] = G + H; //new score
                        }

                        return; //we don't need to do further calculations

                    }
                    
                    if (gameObject.GetComponent<StatsScript>().classes == StatsScript.Classes.Pirate) //is a pirate
                    {
                        print(hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction);
                        //pirate can walk on water, not on mountain
                        if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Mountain)
                        {
                            print("Unusable tile");
                           
                            break;
                        }
                        else //useable tile
                        {
                            //print("Useable tile");
                            openList.Add(pos);
                            int G = movedTiles + 1;
                            int H = (int)Mathf.Abs((target.transform.position.y - pos.y) + (target.transform.position.x - pos.x));
                            scores.Add(G + H);
                        }
                    }
                    else //not a pirate
                    {
                        //cant walk on water or mountain
                        if ((hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Mountain) || (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Water))
                        {
                            //print("Unusable tile");
                            
                            break;
                        }
                        else //useable tile
                        {
                           // print("Useable tile");
                            openList.Add(pos);
                            int G = movedTiles + 1;
                            int H = (int)Mathf.Abs((target.transform.position.y - hitAll[i].transform.position.y) + (target.transform.position.x - hitAll[i].transform.position.x));
                            scores.Add(G + H);
                        }
                    }
                }
            }
        }
    }

}
