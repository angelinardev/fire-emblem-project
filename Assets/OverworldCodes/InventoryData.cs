using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
    //for useable items
    public string name;
    public int HPAmount;
    public int SPAmount;
    public int value;
    public enum ItemType
    {
        Heal,
        Revive,
        Damage,
        Buff
    }
    public ItemType itemType;

    //for wearable items
    public enum SlotType
    {
        Head,
        Top,
        Bottom,
        Shoes,
        Weapon
    }
    public SlotType slotType;

    public enum CharacterType
    {
        Roger,
        Gerrison,
        Sharan,
        All
    }
    public CharacterType characterType;

    public int atk, def, crit, spec;

    public InventoryData(string name, int HPAmount, int SPAmount, int value, string type)
    {
        this.name = name;
        this.HPAmount = HPAmount;
        this.SPAmount = SPAmount;
        this.value = value;

        //turns string into an enum
        itemType = (ItemType)System.Enum.Parse(typeof (ItemType), type);
    }

    public InventoryData(string name, string slotType, string playerType, int def, int atk, int crit, int spec)
    {
        this.name = name;

        //turns string into an enum
        this.slotType = (SlotType)System.Enum.Parse(typeof(SlotType), slotType);
        characterType = (CharacterType)System.Enum.Parse(typeof(CharacterType), playerType);

        this.atk = atk;
        this.def = def;
        this.crit = crit;
        this.spec = spec;  
    }

}
