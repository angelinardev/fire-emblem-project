using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    public static List<InventoryData> ItemInventory = new List<InventoryData>();

    public static List<InventoryData> GearInventory = new List<InventoryData>();

    public static List<CharacterData> teamStats = new List<CharacterData>();

    //public static List<Quest> activeQuestList = new List<Quest>();

    //public static List<Quest> completedQuestList = new List<Quest>();

    //public static CustomControls.ControlName[] controls = new CustomControls.ControlName[9];

    //public CustomControls.ControlName[] savedControls = new CustomControls.ControlName[9];
    public static int coins = 200;

    public int coin = 0;

    public string mapLocation;
    public float time;

    public float[] position;

    public int storyInstance;

    //for saving the lists
    public List<InventoryData> items = new List<InventoryData>();

    public List<InventoryData> gear = new List<InventoryData>();

    public List<CharacterData> team = new List<CharacterData>();

    //public List<Quest> active = new List<Quest>();

    //public List<Quest> completed = new List<Quest>();



    public PlayerData(/*CharacterControls character*/)
    {
        mapLocation = SceneManager.GetActiveScene().name;

        //storyInstance = SceneLoader.storyInstance;

        //coin = coins;
        //time = TimeKeeperCode.timer;

        //if(controls.buttonKeys == null)
        //{
        //    controls.Default();
        //}
        //savedControls = controls;

        for (int i = 0; i < ItemInventory.Count; i++)
        {
            items.Add(ItemInventory[i]);
        }

        for (int i = 0; i < GearInventory.Count; i++)
        {
            gear.Add(GearInventory[i]);
        }

        for (int i = 0; i < teamStats.Count; i++)
        {
            team.Add(teamStats[i]);
        }

        //for (int i = 0; i < activeQuestList.Count; i++)
        //{
        //    active.Add(activeQuestList[i]);
        //}

        //for (int i = 0; i < completedQuestList.Count; i++)
        //{
        //    completed.Add(completedQuestList[i]);
        //}

        //character.SavePlayerPosition();
        //position = new float[3];
        //position[0] = CharacterControls.playerPos.x;
        //position[1] = CharacterControls.playerPos.y;
        //position[2] = CharacterControls.playerPos.z;
    }
}