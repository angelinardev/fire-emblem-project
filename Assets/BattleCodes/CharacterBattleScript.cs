using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.SceneManagement;

public class CharacterBattleScript : MonoBehaviour
{

    private State state;
    private Vector3 moveTargetPosition;
    private Action onMoveComplete;
    public bool isPlayerTeam;
    public HealthSystem healthSystem;
    public bool canAttack = true;

    public new string name;
    public int health;
    public int attackDamage;
    public int defense;
    public int critChance;
    public int specialDamage;
    public int sp;
    public int exp;


    private int tracker =-1;

    private int buffResetTrigger = -1;
    private int defendResetTrigger = -1;

    public bool actionFinished = false;

    XmlDocument statXml;

    XmlNode text;

    private enum State
    {
        Idle,
        Moving,
        Busy
    }

    private bool skip = false;

    private void Awake()
    {
        //characterBase = GetComponent<character_Base>();
        state = State.Idle;
        
            //this.enabled = false;
    }

    void Start()
    {
        if (text == null)
        {
            return;
        }

        if (tracker == -1)
        {
            StatSetter newStatSetter = new StatSetter(text, !isPlayerTeam);

            health = newStatSetter.CharHp;
            attackDamage = newStatSetter.CharAttack;
            defense = newStatSetter.CharDefense;
            critChance = newStatSetter.CharCrit;
            specialDamage = newStatSetter.CharSpecialDamage;
            healthSystem = new HealthSystem(health);
            sp = newStatSetter.CharSp;

            if(!isPlayerTeam)
            {
                exp = newStatSetter.CharExp;
            }

            if (isPlayerTeam)
            {
                //PlayerData.teamStats.Add(new CharacterData(name, health, attackDamage, defense, critChance, specialDamage, sp , 1));

                tracker = PlayerData.teamStats.Count - 1;
            }
        }
    }
    

    public void Setup(bool isPlayerTeam, string name)
    {
        this.isPlayerTeam = isPlayerTeam;
        this.name = name;
        //this.health = health;

        if(isPlayerTeam)
        {

            if (!Contains())
            {
                TextAsset xmlTextAsset = Resources.Load<TextAsset>("XML/CharacterData");
                statXml = new XmlDocument();
                statXml.LoadXml(xmlTextAsset.text);

                text = statXml.SelectSingleNode("/CharacterCollection/TeamCharacters/Character[@ID='" + name + "']");
            }
            else
            {
                health = PlayerData.teamStats[tracker].currentHealth;
                attackDamage = PlayerData.teamStats[tracker].attack;
                defense = PlayerData.teamStats[tracker].defense;
                critChance = PlayerData.teamStats[tracker].crit;
                specialDamage = PlayerData.teamStats[tracker].specialAttack;
                sp = PlayerData.teamStats[tracker].currentSP;
                healthSystem = new HealthSystem(PlayerData.teamStats[tracker].currentHealth, PlayerData.teamStats[tracker].maxHealth, PlayerData.teamStats[tracker].currentSP, PlayerData.teamStats[tracker].maxSP);
            }
        }
        else
        {
            TextAsset xmlTextAsset = Resources.Load<TextAsset>("XML/EnemyData");
            statXml = new XmlDocument();
            statXml.LoadXml(xmlTextAsset.text);
            
            text = statXml.SelectSingleNode("/EnemyCollection/EnemyCharacters/Character[@ID='" + name + "']");
        }
    }

    private void Update()
    {
        if (skip)
            return;
        switch(state)
        {
            case State.Idle:
                break;
            case State.Busy:
                break;
            case State.Moving:
                float slideSpeed = 10f;
                transform.position += (moveTargetPosition - GetPosition()) * slideSpeed * Time.deltaTime;
                float reachedDistance = 1f;
                if(Vector3.Distance(GetPosition(), moveTargetPosition) < reachedDistance)
                {
                    //reached target
                    transform.position = moveTargetPosition;
                    onMoveComplete();
                }
                break;
        }
    }

    bool Contains()
    {
        int count = 0;
        foreach (CharacterData n in PlayerData.teamStats)
        {
            if (n.name == this.name)
            {
                tracker = count;
                return true;
            }
            count++;
        }
        return false;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void Damage(int damageAmount)
    {
        StatReset();
        healthSystem.Damage(damageAmount);
        SaveStats();
    }

    public void Heal(int healAmount, int healSP)
    {
        StatReset();
        healthSystem.Heal(healAmount, healSP);
        SaveStats();
    }

    public void Defend()
    {
        StatReset();
        defense *= 2;
        defendResetTrigger = 1;
    }

    public void StatReset()
    {
        if(buffResetTrigger > 0)
            buffResetTrigger -= 1;

        if(defendResetTrigger >= 0)
            defendResetTrigger -= 1;

        if (buffResetTrigger == 0)
        {
            //reset stat on said buff
        }

        if(defendResetTrigger == 0)
        {
            defense = PlayerData.teamStats[tracker].defense;
        }
    }

    public bool IsDead()
    {
        return healthSystem.IsDead();
    }

    public void Attack (CharacterBattleScript targetCharacter, Action onAttackComplete, bool isSpecial)
    {
        actionFinished = false;
        Vector3 moveTargetPosition = targetCharacter.GetPosition() + (GetPosition() - targetCharacter.GetPosition()).normalized * 10f;
        Vector3 startingPosition = GetPosition();

        int originalDamage;

        if(isSpecial)
        {
            originalDamage = specialDamage;
            print("Special");
        }
        else
        {
            originalDamage = attackDamage;
        }

        //move to target
        MoveToPosition(targetCharacter.GetPosition(), () => 
        {
            //arrived at target
            state = State.Busy;
            Vector3 attackDir = (targetCharacter.GetPosition() - GetPosition()).normalized;
            //characterBase.PlayAnimAttack(attackDir, null, () => {

            //gets damage to be applied to attack
            int damageAmount = UnityEngine.Random.Range(originalDamage, originalDamage+10);
            //factors in targets defense
            damageAmount -= targetCharacter.defense;
            //makes sure attacks do at least 1 damage
            if (damageAmount <= 1)
                damageAmount = 1;

            //gets the crit chance out of 100 and checks if attack will crit
            int critRoll = UnityEngine.Random.Range(1, 101);
            if (critRoll >= (100 - critChance))
                damageAmount *= 2;
            //sends damage to target
            targetCharacter.Damage(damageAmount);

            MoveToPosition(startingPosition, () =>
            {
                //slide back complete play idle
                state = State.Idle;
                //characterBase.PlayAnimIdle(attackDir);
                actionFinished = true;
                onAttackComplete();
            });
        });

        /*
        Vector3 attackDir = (targetCharacter.GetPosition() - GetPosition()).normalized;
        print("you attacked");
        onAttackComplete();*/
    }

    public void SpecialAttack(CharacterBattleScript targetCharacter, Action onSpecialComplete, int baseDamage)
    {
        actionFinished = false;
        Vector3 moveTargetPosition = targetCharacter.GetPosition() + (GetPosition() - targetCharacter.GetPosition()).normalized * 10f;
        Vector3 startingPosition = GetPosition();

        int originalDamage = specialDamage + baseDamage;
        

        //move to target
        MoveToPosition(targetCharacter.GetPosition(), () =>
        {
            //arrived at target
            state = State.Busy;
            Vector3 attackDir = (targetCharacter.GetPosition() - GetPosition()).normalized;
            //characterBase.PlayAnimAttack(attackDir, null, () => {

            //gets damage to be applied to attack
            int damageAmount = UnityEngine.Random.Range(originalDamage, originalDamage + 10);
            //factors in targets defense
            damageAmount -= targetCharacter.defense;
            //makes sure attacks do at least 1 damage
            if (damageAmount <= 1)
                damageAmount = 1;

            //gets the crit chance out of 100 and checks if attack will crit
            int critRoll = UnityEngine.Random.Range(1, 101);
            if (critRoll >= (100 - critChance))
                damageAmount *= 2;
            //sends damage to target
            targetCharacter.Damage(damageAmount);

            MoveToPosition(startingPosition, () =>
            {
                //slide back complete play idle
                state = State.Idle;
                //characterBase.PlayAnimIdle(attackDir);
                actionFinished = true;
                onSpecialComplete();
            });
        });
    }


    public void SaveStats()
    {
        if(isPlayerTeam)
            PlayerData.teamStats[tracker].currentHealth = (int)healthSystem.GetHealth();
    }

    private void MoveToPosition(Vector3 moveTargetPosition, Action onMoveComplete)
    {
        this.moveTargetPosition = moveTargetPosition;
        this.onMoveComplete = onMoveComplete;
        
        state = State.Moving;
    }

    class StatSetter
    {
        public string CharName { get; private set; }

        public int CharHp { get; private set; }
        public int CharAttack { get; private set; }
        public int CharDefense { get; private set; }
        public int CharCrit { get; private set; }
        public int CharSp { get; private set; }
        public string CharSpecialName { get; private set; }
        public int CharSpecialDamage { get; private set; }
        public int CharSpecialCost { get; private set; }
        public int CharExp { get; private set; }

        public StatSetter(XmlNode DiaNode, bool enemy)
        {
            CharName = DiaNode.Attributes["ID"].Value;
            CharHp = int.Parse(DiaNode["HP"].InnerText);
            CharAttack = int.Parse(DiaNode["Attack"].InnerText);
            CharDefense = int.Parse(DiaNode["Defense"].InnerText);
            CharCrit = int.Parse(DiaNode["CritChance"].InnerText);
            CharSp = int.Parse(DiaNode["SP"].InnerText);
            CharSpecialDamage = int.Parse(DiaNode["SpecialDamage"].InnerText);
            //CharSpecialName = DiaNode.SelectNodes("/SpecialAttack").Value;

            if(enemy)
            {
                CharExp = int.Parse(DiaNode["Exp"].InnerText);
            }
        }
    }
}
