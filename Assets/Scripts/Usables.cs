using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Usables : Items 
{


    public Usables(string n, int d, int w)
    {
        nametag = n;
        durability = maxDurability = d;
        worth = w;
    }

    void Use(GameObject target)
    {
        if (durability > 0)
        {
            switch (nametag)
            {
                case "door key":
                    //target.open
                    break;
                case "bridge key":
                    //target.open
                    break;
                case "master key":
                    //target.open
                    break;
                case "vulnerary":
                    target.GetComponent<StatsScript>().currentHp += 10;
                    if (target.GetComponent<StatsScript>().currentHp > target.GetComponent<StatsScript>().maxHp)
                    {
                        target.GetComponent<StatsScript>().currentHp = target.GetComponent<StatsScript>().maxHp;
                    }
                    break;
                case "pure water":
                    target.GetComponent<StatsScript>().Res += 7;
                    //make it temporary somehow
                    break;
                case "power ring":
                    target.GetComponent<StatsScript>().Str += 4;
                    break;
                case "secret book":
                    target.GetComponent<StatsScript>().Skl += 5;
                    break;
                case "speed ring":
                    target.GetComponent<StatsScript>().Spd += 6;
                    break;
                case "goddess icon":
                    target.GetComponent<StatsScript>().Lck += 7;
                    break;
                case "dracoshield":
                    target.GetComponent<StatsScript>().Def += 3;
                    break;
                case "talisman":
                    target.GetComponent<StatsScript>().Res += 7;
                    break;
                case "seraph robe":
                    target.GetComponent<StatsScript>().maxHp += 9;
                    break;
                case "manual":
                    target.GetComponent<StatsScript>().Wpn += 5;
                    break;
                case "boots":
                    target.GetComponent<StatsScript>().Mov += 4;
                    break;
                default:
                    break;
            }
            durability--;
        }
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
