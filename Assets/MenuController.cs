using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject mainPanel;
    // Start is called before the first frame update
    void Start()
    {
        if(mainPanel.activeInHierarchy)
        {
            mainPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMenu(bool visibility)// can add in menu name as string later for multiple menus
    {
        mainPanel.SetActive(visibility);
    }
}
