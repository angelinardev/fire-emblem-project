using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsScript : MonoBehaviour
{
    new public string name = "";

    public int
        Lvl = 1,
        currentHp = 1,
        maxHp = 1,
        Str = 1,
        Skl = 1,
        Wpn = 1,
        Spd = 1,
        Lck = 1,
        Def = 1,
        Res = 1,
        Mov = 1;

    public float[] g_rates = {1,1,1,1,1,1,1,1};

    private int exp;

    public bool canMove = true;

    public Sprite endTurn;
    public Sprite normal;
    
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
    public Classes classes;

    public  List<string> items = new List<string>();

    [System.NonSerialized]
    public InventoryScript[] inventory = new InventoryScript[5];

    // Start is called before the first frame update
    void Start()
    {
        if(name == "")
        {
            name = gameObject.transform.name;
            maxHp = currentHp;
        }

        IDictionary<string, float> growths = new Dictionary<string, float>()
        {
            {"HP", g_rates[0]}, {"Str", g_rates[1]}, {"Skl", g_rates[2]}, {"Wlv", g_rates[3]}, {"Spd", g_rates[4]},{"Lck", g_rates[5]}, {"Def", g_rates[6]}, {"Res", g_rates[7]}
        };

        if(items.Count > 0 && items.Count <= inventory.Length)
        {
            for (int i = 0; i < items.Count; i++)
            {
                inventory[i] = new InventoryScript(items[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
