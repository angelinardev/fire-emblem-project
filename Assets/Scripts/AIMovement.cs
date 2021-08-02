using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public bool activated = false;
    // Start is called before the first frame update
    void Start()
    {
        
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

            Movement();
            //check all adjacent squares
            checkAdjacent(transform.position + new Vector3(0, 1));
            checkAdjacent(transform.position + new Vector3(0, -1));
            checkAdjacent(transform.position + new Vector3(1, 0));
            checkAdjacent(transform.position + new Vector3(-1, 0));
            activated = false;
            transform.GetComponent<StatsScript>().canMove = false;

        }

        //and then do attack
    }
  
    public void Movement()
    {

    }

    public void Attack(GameObject player)
    {

    }

    public void checkAdjacent(Vector2 pos)
    {
        
        RaycastHit2D[] hitAll = Physics2D.RaycastAll(pos, Vector2.zero);

        for (int i = 0; i < hitAll.Length; i++)
        {
            if (hitAll[i].collider != null && hitAll[i].transform.GetComponent<MenuInfoSuppyCode>())
            {
                if (hitAll[i].transform.GetComponent<MenuInfoSuppyCode>().interaction == MenuInfoSuppyCode.Interaction.Player)
                {
                    Attack(hitAll[i].transform.gameObject);
                    break;

                }
            }
        }
    }

}
