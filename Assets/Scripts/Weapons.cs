using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapons : Items
{
    public int
       might,
       weight,
       hit,
       wlvl,
       crt = 0; // and anything else needed for geardd

    

    public Weapons() : base()
    {

    }
    public Weapons(string n, int minr, int maxr, int m, int w, int h, int wl, int mx, int wor, int c)
    {
        nametag = n;
        minRange = minr;
        maxRange = maxr;
        might = m;
        weight = w;
        hit = h;
        wlvl = wl;
        durability = maxDurability = mx;
        worth = wor;
        crt = c;
    }
    
    void Equip(bool status)
    {
        equipped = status;
        //if (status)
        //{
        //    equipped = true;
        //}
        //else
        //{
        //    equipped = false;
        //}
    }
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
