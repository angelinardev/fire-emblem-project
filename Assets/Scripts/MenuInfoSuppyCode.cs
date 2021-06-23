using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInfoSuppyCode : MonoBehaviour
{
    public Interaction interaction;

    public enum Interaction
    {
        Building,
        Terrain,
        Player,
        Enemy
    }

    public void FillMenu()
    {
        print(transform.name);
        if(interaction == Interaction.Player)
        {
            if(gameObject.GetComponent<StatsScript>())
            {
                print(gameObject.GetComponent<StatsScript>().name + " is currently at " + gameObject.GetComponent<StatsScript>().Lvl);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
