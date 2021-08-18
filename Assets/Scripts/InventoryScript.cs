using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class InventoryScript
{
    public string name;

    public int
        minRange,
        maxRange,
        attack,
        durability,
        maxDirability; // and anything else needed for gear


    public InventoryScript(string name)
    {
        //creates and accesses the xml document for data
        XmlDocument gearDataXml;
        XmlNode gear;
        TextAsset xmlTextAsset = Resources.Load<TextAsset>("XML/ItemData");
        gearDataXml = new XmlDocument();
        gearDataXml.LoadXml(xmlTextAsset.text);

        //identifies the specific section of the document to retrieve the data from
        gear = gearDataXml.SelectSingleNode("/Inventory/Weapon[@ID='" + name + "']");

        //pulls data from the file and stores it in this instance of the inventory
        this.name = name;
        attack = int.Parse(gear["Attack"].InnerText);
        minRange = int.Parse(gear["MinRange"].InnerText);
        maxRange = int.Parse(gear["MaxRange"].InnerText);
        durability = maxDirability = int.Parse(gear["Durability"].InnerText);
        Debug.Log("The weapon is " + name + "\n" + minRange);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
