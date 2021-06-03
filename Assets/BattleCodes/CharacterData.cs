using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string name;

    public int currentHealth, maxHealth, attack, defense, crit, currentSP, maxSP, specialAttack, specialDefense;

    public int currentLevel = 1;
    public int baseExp { get; private set; } = 100;
    public int currentExp { get; private set; } = 0;
    private int maxLevel = 99;
    public bool leveled;

    //public List<SpecialSkills> specialSkill = new List<SpecialSkills>();
    public List<SpecialSkills> currentSkills = new List<SpecialSkills>();
    public List<SpecialSkills> futureSkills = new List<SpecialSkills>();


    public List<InventoryData> equipment = new List<InventoryData>();

    public void AddGear(InventoryData slot, int slotOrigin)
    {
        //add in search that checks name for same slot position to switch first
        int i = 0;
        foreach (InventoryData n in equipment)
        {

            if (n.slotType == slot.slotType)
            {
                attack -= n.atk;
                defense -= n.def;
                crit -= n.crit;
                maxSP -= n.spec;
                equipment.RemoveAt(i);
                PlayerData.GearInventory.Add(n);

                break;
            }
            i++;
        }

        attack += slot.atk;
        defense += slot.def;
        crit += slot.crit;
        maxSP += slot.spec;

        if(currentSP > maxSP)
        {
            currentSP = maxSP;
        }

        equipment.Add(slot);
        PlayerData.GearInventory.RemoveAt(slotOrigin);
        MenuItemControl.updateInfo = true;
    }
    public void RemoveGear(string gearName)
    {
        for(int i = 0; i < equipment.Count; i++)
        {
            if(gearName == equipment[i].name)
            {
                attack -= equipment[i].atk;
                defense -= equipment[i].def;
                crit -= equipment[i].crit;
                maxSP -= equipment[i].spec;

                PlayerData.GearInventory.Add(equipment[i]);
                equipment.RemoveAt(i);

                MenuItemControl.updateInfo = true;
                return;
            }
        }
    }

    public void GainExperience(int exp)
    {
        if(currentLevel < maxLevel)
        {
            currentExp += exp;
        }
        else
        {
            currentExp = ExpToLevel();
        }

        if (currentExp >= ExpToLevel() && currentLevel < maxLevel)
        {
            currentExp -= ExpToLevel();
            currentLevel++;
            currentHealth = maxHealth += 5;
            currentSP += 5;
            maxSP += 5;
            attack++;
            defense++;
            specialDefense++;
            //crit += 1;
            specialAttack++;

            leveled = true;

            if(currentLevel == futureSkills[0].levelNeeded)
            {
                currentSkills.Add(futureSkills[0]);
                futureSkills.RemoveAt(0);
            }
        }
    }

    public void BonusStats(string statType, int amount)
    {
        if(statType == "Physical")
        {
            attack += amount;
            defense += amount;
        }
        else if(statType == "Magical")
        {
            specialAttack += amount;
            specialDefense += amount;
        }
        else if(statType == "Base")
        {
            currentHealth = maxHealth += amount;
            currentSP += amount;
            maxSP += amount;
        }
        leveled = false;
    }

    public int ExpToLevel()
    {
        return baseExp + ((baseExp * currentLevel) / 2);
    }

    public CharacterData(string name, int health, int attack, int defense, int crit, int specialAttack, int sp, int startLevel, string[] skillName, int[] skillCost, int[] skillDamage, string[] skillType, int[]skillLevel,
                         string[] skillInfo, string[] skillUse, string[] skillEle)
    {
        this.name = name;
        currentHealth = health;
        maxHealth = health;
        this.attack = attack;
        this.defense = defense;
        this.crit = crit;
        this.currentSP = sp;
        this.maxSP = sp;
        this.specialAttack = specialAttack;
        this.specialDefense = defense;
        this.currentLevel = startLevel;
        this.currentExp = 0; /*baseExp * (currentLevel - 1);*/

        for(int i = 0; i < skillName.Length; i++)
        {
            if(currentLevel >= skillLevel[i])
            {
                currentSkills.Add(new SpecialSkills(skillName[i], skillCost[i], skillDamage[i], skillType[i], skillLevel[i], skillInfo[i], skillUse[i], skillEle[i]));
            }
            else
            {
                futureSkills.Add(new SpecialSkills(skillName[i], skillCost[i], skillDamage[i], skillType[i], skillLevel[i], skillInfo[i], skillUse[i], skillEle[i]));
            }
        }
    }
}