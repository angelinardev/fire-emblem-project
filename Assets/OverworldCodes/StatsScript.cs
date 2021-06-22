using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsScript : MonoBehaviour
{
    public string name = "";

    public int
        Lvl = 1,
        HP  = 1,
        Str = 1,
        Skl = 1,
        Wpn = 1,
        Spd = 1,
        Lck = 1,
        Def = 1,
        Res = 1,
        Mov = 1;

    private int exp;

    public Classes classes;

    public enum Classes
    {
        Lord,
        Mercenary,
        Hero,
        Thief,
        Freelancer,
        Fighter,
        Pirate,
        Archer,
        Sniper,
        Hunter,
        Horseman,
        Cavalier,
        Paladin,
        Knight,
        General,
        Pegasus_Knight,
        Wyvern_Knight,
        Ballistician,
        Mage,
        Curate,
        Bishop,
        Manakete
    }

    // Start is called before the first frame update
    void Start()
    {
        if(name == "")
        {
            name = gameObject.transform.name;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
