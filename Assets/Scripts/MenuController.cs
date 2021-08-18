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
        inUse = true;
        if(currentMenu == Menus.confirmation || currentMenu == Menus.commands)
        {
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



    //hides all menus
    public void HideMenus()
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

    public void CloseMenus()
    {
        HideMenus();
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
