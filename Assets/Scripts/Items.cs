using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Items 
{
    public string nametag;
    public int worth, durability,
        maxDurability, minRange, maxRange =0;
    public bool equipped = false;

    //whatever functions need to be here

    //public abstract void Use(GameObject player, GameObject target = null);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

