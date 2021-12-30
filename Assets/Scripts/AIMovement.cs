using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public bool activated = false;

    public GameObject target;
    public GameObject cursor;

    private int horizontal,
        vertical = 0;

    private List<GameObject> openList = new List<GameObject>();
    private List<int> scores = new List<int>();
    private List<GameObject> closedList= new List<GameObject>();

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

            if (transform.GetComponent<StatsScript>().name.Equals("Gazzak"))
            {
                transform.position = new Vector3(-16.5F, 6.5F, 0);
                return;
            }
            //add current position to the closed list
            closedList.Add(gameObject);
            SetTarget(); //movement target

            print(target.name);

            Movement(); //do movement
            activated = false;
            transform.GetComponent<StatsScript>().canMove = false;

        }

        //and then do attack
    }
    public void CalculatePath(Transform square)
    {
        //check all adjacent squares
        checkAdjacent(square.position + new Vector3(0, 1), true);
        checkAdjacent(square.position + new Vector3(0, -1), true);
        checkAdjacent(square.position + new Vector3(1, 0), true);
        checkAdjacent(square.position + new Vector3(-1, 0), true);

        int lowest = 10000; //some arbitrary high number
        int index = 0;
        GameObject temp = gameObject;
        for (int i=0; i< openList.Count; i++)
        {
            //find the tile in the open list that has the lowest score
            if (scores[i] <= lowest)
            {
                lowest = scores[i];
                temp = openList[i];
                index = i;
            }
        }
        //remove from the open list
        if (openList.Contains(temp))
        {
            openList.RemoveAt(index);
            scores.RemoveAt(index);
            //add to the closed list
            closedList.Add(temp);
        }
        //check adjacent tiles of the one we just added
        //we can actually just do this by recursively calling
        movedTiles += 1;//increase the number we are going by, aka how far along we're going

        //we also want to check if any player characters are in range, because this will bypass the searching mechanism
        //check all adjacent squares
        checkAdjacent(square.position + new Vector3(0, 1), false);
        checkAdjacent(square.position + new Vector3(0, -1), false);
        checkAdjacent(square.position + new Vector3(1, 0), false);
        checkAdjacent(square.position + new Vector3(-1, 0), false);

        if (!attacking && movedTiles < gameObject.GetComponent<StatsScript>().Mov) //to continue moving, need to not be attacking AND have leftover movement
        {
            CalculatePath(temp.transform);
        }

    }
  
    public void Movement()
    {
        CalculatePath(transform);
        for (int i=0; i< closedList.Count; i++)
        {
            transform.position = closedList[i].transform.position;
            print(transform.position);
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
                    Attack(hitAll[i].transform.gameObject);
                    print("Attacking");
                    attacking = true;
                    break;

                }
                else if (checking) //PURELY FOR THE A* ALGORITHM
                {
                    //print("Checking tiles");
                    //check if its already in the closed list
                    if (closedList.Contains(hitAll[i].transform.gameObject))
                    {
                        print("Already in closed list");
                        return; //ignore it
                    }
                    //check if its already in the open list
                    if (openList.Contains(hitAll[i].transform.gameObject))
                    {
                        print("Already in open list");
                        //update score
                        int G = movedTiles + 1;
                        int H = (int)Mathf.Abs(((target.transform.position - closedList[movedTiles].transform.position).magnitude));
                        if (G + H < scores[openList.IndexOf(hitAll[i].transform.gameObject)]) //is our new score lower with the generated path
                        {
                            scores[openList.IndexOf(hitAll[i].transform.gameObject)] = G + H; //new score
                        }

                        return; //we don't need to do further calculations

                    }

                    if (gameObject.GetComponent<StatsScript>().classes == StatsScript.Classes.Pirate) //is a pirate
                    {
                        //pirate can walk on water, not on mountain
                        if ((hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Mountain))
                        {
                            //print("Unusable tile");
                            horizontal = 0;
                            vertical = 0;
                            break;
                        }
                        else //useable tile
                        {
                            //print("Useable tile");
                            openList.Add(hitAll[i].transform.gameObject);
                            int G = movedTiles + 1;
                            int H = (int)Mathf.Abs(((target.transform.position - closedList[movedTiles].transform.position).magnitude));
                            scores.Add(G + H);
                        }
                    }
                    else //not a pirate
                    {
                        //cant walk on water or mountain
                        if ((hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Mountain) || (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Water))
                        {
                            //print("Unusable tile");
                            horizontal = 0;
                            vertical = 0;
                            break;
                        }
                        else //useable tile
                        {
                           // print("Useable tile");
                            openList.Add(hitAll[i].transform.gameObject);
                            int G = movedTiles + 1;
                            int H = (int)Mathf.Abs(((target.transform.position - closedList[movedTiles].transform.position).magnitude));
                            scores.Add(G + H);
                        }
                    }
                }
            }
        }
    }

}
