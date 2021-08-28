using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;

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

    [System.NonSerialized]
    public int exp;

    public bool canMove = true;

    public Sprite endTurn;
    public Sprite normal;

    public Sprite portrait;
    
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
    public List<Items> inventory = new List<Items>();
    private int itemCap = 4;
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

        if (items.Count > 0 && items.Count <= itemCap && inventory.Count == 0)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Weapons s1 = AddGear(items[0]);
                s1.equipped = true;
                inventory.Add(s1);
            }
        }
    }

    private Weapons AddGear(string gearName)
    {
        //creates and accesses the xml document for data
        XmlDocument gearDataXml;
        XmlNode gear;
        TextAsset xmlTextAsset = Resources.Load<TextAsset>("XML/ItemData");
        gearDataXml = new XmlDocument();
        gearDataXml.LoadXml(xmlTextAsset.text);

        //identifies the specific section of the document to retrieve the data from
        gear = gearDataXml.SelectSingleNode("/Inventory/Weapon[@ID='" + gearName + "']");

        Debug.Log("The weapon added is " + gearName);
        
        //pulls data from the file and returns a weapon with the information back and into the inventory

        return new Weapons(gearName, int.Parse(gear["MinRange"].InnerText), int.Parse(gear["MaxRange"].InnerText), int.Parse(gear["Might"].InnerText), int.Parse(gear["Weight"].InnerText), int.Parse(gear["HitRate"].InnerText), int.Parse(gear["WeaponLevel"].InnerText), int.Parse(gear["Durability"].InnerText), int.Parse(gear["Value"].InnerText), int.Parse(gear["CritRate"].InnerText));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
