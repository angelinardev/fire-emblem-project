using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private CursorMovementScript cursor;

    public Menus currentMenu;

    public bool inUse;

    public enum Menus
    {
        basicinfo,
        detailedInfo,
        confirmation,
        commands,
    }

    [Serializable]
    public class MenuList
    {
        public GameObject[] menu;
        public Text[] information;
        public Image[] portrait;
    }
    public MenuList[] menus;

    [NonSerialized]
    public StatsScript charInfo;


    // Start is called before the first frame update
    void Start()
    {
        CloseMenus();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMenu()
    {
        CloseMenus();

        for (int i = 0; i < menus[(int)currentMenu].menu.Length; i++)
        {
            menus[(int)currentMenu].menu[i].SetActive(true);
        }

        if(currentMenu == Menus.basicinfo || currentMenu == Menus.detailedInfo)
        {
            UpdateInfo();
        }

        inUse = true;
        if(currentMenu == Menus.confirmation || currentMenu == Menus.commands)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(menus[(int)currentMenu].menu[0].transform.GetChild(0).gameObject);
        }
    }

    //updates menu with character specific data
    public void UpdateMenu(StatsScript character)
    {
        charInfo = character;

        OpenMenu();
    }

    //updates menu with speciic menu name for testing
    public void UpdateMenu(Menus location)
    {
        currentMenu = location;

        OpenMenu();
    }

    //updates menu by going forward or backward
    public void UpdateMenu(bool forward)// can add in menu name as string later for multiple menus
    {
        //int nextMenu = 0;

        if(forward)
        {
            //nextMenu = (int)currentMenu + 1;
            currentMenu++;
        }
        else
        {
            //nextMenu = (int)currentMenu - 1;
            currentMenu--;
        }

        OpenMenu();

        //if the current menu isn't the last in the list, open the next menu otherwise close them
        //if(onChar && (int)currentMenu < (int)Menus.end - 1)
        //{
        //    currentMenu++;
        //}
        //else if(currentMenu == Menus.end)
        //{
        //    currentMenu = 0;
        //}
        //else if(!onChar)
        //{
        //    currentMenu = Menus.commands;
        //}
        //else
        //{
        //    currentMenu = Menus.end;
        //    return;
        //}

        //sets the current opened menu if it should be open

        //for (int i = 0; i < menus[nextMenu].menu.Length; i++)
        //{
        //    menus[nextMenu].menu[i].SetActive(true);
        //}


    }

    public void UpdateInfo()
    {
        for (int i = 0; i < menus[(int)currentMenu].information.Length; i++)
        {
            if(menus[(int)currentMenu].information[i].name == "Name")
            {
                menus[(int)currentMenu].information[i].text = cursor.unit.transform.GetComponent<StatsScript>().name;
            }
            else if (menus[(int)currentMenu].information[i].name == "Class")
            {
                menus[(int)currentMenu].information[i].text = cursor.unit.transform.GetComponent<StatsScript>().classes.ToString();
            }
            else if (menus[(int)currentMenu].information[i].name == "Level")
            {
                menus[(int)currentMenu].information[i].text = "Lvl " + cursor.unit.transform.GetComponent<StatsScript>().Lvl.ToString();
            }
            else if (menus[(int)currentMenu].information[i].name == "Health")
            {
                menus[(int)currentMenu].information[i].text = "HP " + (cursor.unit.transform.GetComponent<StatsScript>().currentHp + "/" + cursor.unit.transform.GetComponent<StatsScript>().maxHp).ToString() ;
            }
            //else if (menus[(int)currentMenu].information[i].name == "Weapon1" && cursor.unit.transform.GetComponent<StatsScript>().inventory.Count > 0)
            //{
            //    menus[(int)currentMenu].information[i].text = cursor.unit.transform.GetComponent<StatsScript>().inventory[0].nametag + " " + cursor.unit.transform.GetComponent<StatsScript>().inventory[0].durability + "/" + cursor.unit.transform.GetComponent<StatsScript>().inventory[0].maxDurability;
            //}
            //else if (menus[(int)currentMenu].information[i].name == "Weapon2" && cursor.unit.transform.GetComponent<StatsScript>().inventory.Count > 1)
            //{
            //    menus[(int)currentMenu].information[i].text = cursor.unit.transform.GetComponent<StatsScript>().inventory[1].nametag + " " + cursor.unit.transform.GetComponent<StatsScript>().inventory[1].durability + "/" + cursor.unit.transform.GetComponent<StatsScript>().inventory[1].maxDurability;
            //}
            //else if (menus[(int)currentMenu].information[i].name == "Weapon3" && cursor.unit.transform.GetComponent<StatsScript>().inventory.Count > 2)
            //{
            //    menus[(int)currentMenu].information[i].text = cursor.unit.transform.GetComponent<StatsScript>().inventory[2].nametag + " " + cursor.unit.transform.GetComponent<StatsScript>().inventory[2].durability + "/" + cursor.unit.transform.GetComponent<StatsScript>().inventory[2].maxDurability;
            //}
            //else if (menus[(int)currentMenu].information[i].name == "Weapon4" && cursor.unit.transform.GetComponent<StatsScript>().inventory.Count == 4)
            //{
            //    menus[(int)currentMenu].information[i].text = cursor.unit.transform.GetComponent<StatsScript>().inventory[3].nametag + " " + cursor.unit.transform.GetComponent<StatsScript>().inventory[3].durability + "/" + cursor.unit.transform.GetComponent<StatsScript>().inventory[3].maxDurability;
            //}
            else if (menus[(int)currentMenu].information[i].name == "Strength")
            {
                menus[(int)currentMenu].information[i].text = "Str " + cursor.unit.transform.GetComponent<StatsScript>().Str.ToString();
            }
            else if (menus[(int)currentMenu].information[i].name == "Skill")
            {
                menus[(int)currentMenu].information[i].text = "Skl " + cursor.unit.transform.GetComponent<StatsScript>().Skl.ToString();
            }
            else if (menus[(int)currentMenu].information[i].name == "WeaponLevel")
            {
                menus[(int)currentMenu].information[i].text = "Wpn " + cursor.unit.transform.GetComponent<StatsScript>().Wpn.ToString();
            }
            else if (menus[(int)currentMenu].information[i].name == "Speed")
            {
                menus[(int)currentMenu].information[i].text = "Spd " + cursor.unit.transform.GetComponent<StatsScript>().Spd.ToString();
            }
            else if (menus[(int)currentMenu].information[i].name == "Luck")
            {
                menus[(int)currentMenu].information[i].text = "Lck " + cursor.unit.transform.GetComponent<StatsScript>().Lck.ToString();
            }
            else if (menus[(int)currentMenu].information[i].name == "Defense")
            {
                menus[(int)currentMenu].information[i].text = "Def " + cursor.unit.transform.GetComponent<StatsScript>().Def.ToString();
            }
            else if (menus[(int)currentMenu].information[i].name == "Movement")
            {
                menus[(int)currentMenu].information[i].text = "Mov " + cursor.unit.transform.GetComponent<StatsScript>().Mov.ToString();
            }
            else if (menus[(int)currentMenu].information[i].name == "Resistance")
            {
                menus[(int)currentMenu].information[i].text = "Res " + cursor.unit.transform.GetComponent<StatsScript>().Res.ToString();
            }
            else if (menus[(int)currentMenu].information[i].name == "Experience")
            {
                menus[(int)currentMenu].information[i].text = "Exp " +  cursor.unit.transform.GetComponent<StatsScript>().exp.ToString() + "/100";
            }
            else if (menus[(int)currentMenu].information[i].name.Contains("Weapon"))
            {
                for (int j = 0; j < cursor.unit.transform.GetComponent<StatsScript>().inventory.Count; j++)
                {
                    if (menus[(int)currentMenu].information[i].name == "Weapon" + (j + 1))
                    {
                        menus[(int)currentMenu].information[i].text = cursor.unit.transform.GetComponent<StatsScript>().inventory[j].nametag + " " + cursor.unit.transform.GetComponent<StatsScript>().inventory[j].durability + "/" + cursor.unit.transform.GetComponent<StatsScript>().inventory[j].maxDurability;
                        i++;
                    }
                }
            }
        }
        if(currentMenu == Menus.detailedInfo && cursor.unit.transform.GetComponent<StatsScript>().portrait != null)
        {
            menus[(int)currentMenu].portrait[0].sprite = cursor.unit.transform.GetComponent<StatsScript>().portrait;
        }
    }

    public void CloseMenus()
    {
        for (int i = 0; i < menus.Length; i++)
        {
            for (int j = 0; j < menus[i].menu.Length; j++)
            {
                menus[i].menu[j].SetActive(false);
            }
        }
        inUse = false;
    }

    public void List()
    {

    }

    public void Convoy()
    {

    }

    public void Funds()
    {

    }

    public void Suspend()
    {

    }

    public void Options()
    {

    }

    public void EndTurn()
    {
        CloseMenus();
        cursor.EndPhase();
        cursor.EndTurn();
        cursor.playerPhase = false;
        
    }

    public void Attack()
    {
        cursor.TargetEnemy();
        cursor.EndTurn();
        CloseMenus();
    }

    public void Items()
    {

    }

    public void Wait()
    {
        CloseMenus();
        cursor.EndTurn();
    }
}
