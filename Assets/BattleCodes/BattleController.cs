using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleController : MonoBehaviour
{
    

    int enemyLayout;
    int enemyAtkOrder;
    int offTimer = 0;
    int attacker;
    int target;
    int pageNum = 1, maxPage;

    private bool battleStop = false;

    private bool[] canAttack = new bool[3];
    private bool[] actionFinished = new bool[3];

    bool reset = true;
    //Vector3 pos1, pos2, pos3, pos4, pos5;
    /*Vector3[] positions = new[]
    {
        //mid
        new Vector3(   0f, 0.5f,  5f),
        //mid left
        new Vector3(  -5f, 0.5f,  5f),
        //mid right
        new Vector3(   5f, 0.5f,  5f),
        //back left
        new Vector3(-2.5f, 0.5f, 10f),
        //back right
        new Vector3( 2.5f, 0.5f, 10f)
    };*/

    /*string[] enemyName = new[]
    {
        "Enemy",
        "Enemy",
        "Enemy",
        "Enemy Tall",
        "Enemy Tall"
    };*/

    //private Transform[] EnemyLoader;

    public GameObject enemyFinder;

    private Transform actionBoxOrigin;

    private Image[]
        playerBackdrop;

    private Text[]
        playerName,
        playerHP,
        item,
        special,
        defense,
        attack;

    private Navigation[] battleButtons = new Navigation[6];

    private Text charSp;

    public static BattleController instance;

    public static BattleController GetInstance()
    {
        return instance;
    }

    [SerializeField] private Transform pfCharacterBattle;
    [SerializeField] private Transform pfEnemyBattle;

    //private CharacterBattleScript[] playerCharacterBattle;
    //private CharacterBattleScript[] enemyCharacterBattle;
    private List<CharacterBattleScript> playerCharacterBattle = new List<CharacterBattleScript>();
    private List<CharacterBattleScript> enemyCharacterBattle = new List<CharacterBattleScript>();
    private CharacterBattleScript activeCharacterBattle;

    private GameObject 
        actionBox,
        itemBox,
        itemButton,
        specialBox,
        specialButton,
        defenseBox,
        defenseButton,
        attackBox,
        attackButton,
        attackLeft,
        attackRight,
        inventoryPage;

    private State state;
    private Turn turn;

    private enum Turn
    {
        Player1,
        Player2,
        Player3
    }
    

    private enum State
    {
        WaitingForPlayer,
        Busy
    }

    private List<string> DefenseList = new List<string>
    {
        "Defend",
        "Run"
    };
    //////////////////////////////
    public enum EnemyName
    {
        Tofu,
        Newfu,
        Oldfu
    }
    private EnemyName enemyName;
    //make battle transition code a static enum

    private void Awake()
    {
        if(BattleTransitionCode.Variance == 1 || BattleTransitionCode.Variance == 5)
        {
            enemyLayout = BattleTransitionCode.Variance;
        }
        else
        {
            enemyLayout = BattleTransitionCode.Variance + Random.Range(-1, 2);
        }

        if (GameObject.Find("Player and Camera"))
            Destroy(GameObject.Find("Player and Camera"));
        //EnemyLoader = new Transform[enemyLayout];
        //SpawnEnemies();
        //creates a default xml search until a usable one is called

        actionBox = GameObject.Find("ActionBox");
        itemBox = GameObject.Find("ItemBox");
        itemButton = GameObject.Find("ItemButton");
        specialBox = GameObject.Find("SpecialBox");
        specialButton = GameObject.Find("SpecialButton");
        defenseBox = GameObject.Find("DefenseBox");
        defenseButton = GameObject.Find("DefenseButton");
        attackBox = GameObject.Find("AttackBox");
        attackButton = GameObject.Find("AttackButton");
        attackLeft = GameObject.Find("Left");
        attackRight = GameObject.Find("Right");
        inventoryPage = GameObject.Find("Page");

        //itemButton.transform.GetChild(0).GetComponent<Text>().text = PlayerData.controls[9].key.ToString();
        //specialButton.transform.GetChild(0).GetComponent<Text>().text = PlayerData.controls[10].key.ToString();
        //defenseButton.transform.GetChild(0).GetComponent<Text>().text = PlayerData.controls[4].key.ToString();
        //attackButton.transform.GetChild(0).GetComponent<Text>().text = PlayerData.controls[5].key.ToString();
        //attackLeft.transform.GetChild(0).GetComponent<Text>().text = PlayerData.controls[6].key.ToString();
        //attackRight.transform.GetChild(0).GetComponent<Text>().text = PlayerData.controls[7].key.ToString();
    }

    //private void DeselectAll()
    //{
    //    for(int i = 0; i < enemyCharacterBattle.Count; i++)
    //    {
    //        GameObject.Find(enemyCharacterBattle[i].gameObject.name + "/Selected").SetActive(false);
    //    }
    //}

    private void Start()
    {
        playerName = new Text[3];
        playerHP = new Text[3];
        playerBackdrop = new Image[3];
        item = new Text[6];
        special = new Text[6];
        defense = new Text[6];
        attack = new Text[3];

        for(int i = 0; i < PlayerData.teamStats.Count && i < 3; i++)
        {
            playerCharacterBattle.Add(SpawnCharacter(true, i, PlayerData.teamStats[i].name));

            playerName[i] = GameObject.Find("Player" + (i + 1) + "/Name").GetComponent<Text>();
            playerHP[i] = GameObject.Find("Player" + (i + 1) + "/HP/HPAmount").GetComponent<Text>();
            playerBackdrop[i] = GameObject.Find("Player" + (i + 1) + "Backdrop").GetComponent<Image>();
        }

        for(int i = 3; i > playerCharacterBattle.Count; i--)
        {
            GameObject.Find("Player" + i).SetActive(false);
        }

        enemyName = (EnemyName)BattleTransitionCode.enemyNameNum;
        for(int i = 0; i<BattleTransitionCode.Variance; i++)
        {
            enemyCharacterBattle.Add(SpawnCharacter(false, i, enemyName.ToString()));//Tofu
        }

        turn = Turn.Player1;

        MenuOff();
        actionBox.SetActive(false);
    }
    /* 
     * set up a new battle system that creates a list of every character that can attack and if they should be able to
     * 
     * remove canAttack from battlescript
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */

    private void LateUpdate()
    {
        if (battleStop)
        {
            return;
        }
        TestBattleOver();
        if (reset)
        {
            ResetTurns();
        }
        /*
        if(attackBox.activeInHierarchy || itemBox.activeInHierarchy || defenseBox.activeInHierarchy || specialBox.activeInHierarchy)
        {
            //ButtonController.CheckInput();
        }

        for (int i = 0; i < playerCharacterBattle.Count; i++)
        {
            /////finds and assigns character names to the UI
            playerName[i].text = playerCharacterBattle[i].name;

            /////finds and assigns character hp values to the UI
            playerHP[i].text = playerCharacterBattle[i].healthSystem.GetHealth() + " / " + playerCharacterBattle[i].healthSystem.GetMaxHealth();
            
            if (i == (int)turn && state != State.Busy)
            {
                playerBackdrop[(int)turn].canvasRenderer.SetAlpha(1.50f);
            }
            else
            {
                playerBackdrop[i].canvasRenderer.SetAlpha(1);
            }
        }
        ///////finds and assigns enemy names to the UI

        //enemy1Name = GameObject.Find("Enemy1/Name").GetComponent<Text>();
        //enemy1Name.text = enemyCharacterBattle[0].name;

        ///////finds and assigns enemy hp values to the UI

        //enemy1Hp = GameObject.Find("Enemy1/HP/HPAmount").GetComponent<Text>();
        //enemy1Hp.text = enemyCharacterBattle[0].healthSystem.GetHealth() + " / " + enemyCharacterBattle[0].healthSystem.GetMaxHealth();
        if (state == State.Busy)
        {
            actionBox.SetActive(false);
            offTimer = 0;
        }
        if (state == State.WaitingForPlayer)
        {
            actionBox.SetActive(true);
            while (offTimer < 1)
            {
                MenuOff();
                offTimer++;
            }
            ActionBoxUpdater();
            if ((attackBox.activeInHierarchy || specialBox.activeInHierarchy) /*&& Input.GetButtonDown("ShiftAttack")/)
            {
                if (Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.CycleLeft].key) || Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Left].key))// cycle left
                {
                    target--;
                    if (target < 0)
                    {
                        target = enemyCharacterBattle.Count - 1;
                    }
                    if (attackBox.activeInHierarchy)
                    {
                        PlayerAttack();
                    }
                    else if (specialBox.activeInHierarchy)
                    {
                        PlayerSpecial();
                    }
                    else if (itemBox.activeInHierarchy)
                    {
                        PlayerItem();
                    }
                }
                else if (Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.CycleRight].key) || Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Right].key))// cycle right
                {
                    target++;
                    if (target > (enemyCharacterBattle.Count - 1))
                    {
                        target = 0;
                    }
                    if (attackBox.activeInHierarchy)
                    {
                        PlayerAttack();
                    }
                    else if (specialBox.activeInHierarchy)
                    {
                        PlayerSpecial();
                    }
                    else if (itemBox.activeInHierarchy)
                    {
                        PlayerItem();
                    }
                }
                

            }
            else if (itemBox.activeInHierarchy /*&& Input.GetButtonDown("ShiftAttack"))
            {
                if (Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.CycleLeft].key) || Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Left].key))// cycle left
                {
                    if (pageNum < maxPage)
                    {
                        pageNum = maxPage;
                    }
                    else
                    {
                        pageNum--;
                    }
                    PlayerItem();
                }
                else if (Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.CycleRight].key) || Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Right].key))// cycle right
                {
                    if (pageNum >= maxPage)
                    {
                        pageNum = 1;
                    }
                    else
                    {
                        pageNum++;
                    }
                    PlayerItem();
                }
            }
            if (Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Talk].key) && attackBox.activeInHierarchy)
            {
                AttackMenu();
            }
            else if (Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Talk].key))
            {
                PlayerAttack();
            }
            else if (Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Team].key) && specialBox.activeInHierarchy)// press A
            {
                SpecialMenu();
            }
            else if (Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Team].key))// press A
            {
                PlayerSpecial();
            }
            else if (Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Jump].key) && defenseBox.activeInHierarchy)
            {
                DefenseMenu();
            }
            else if (Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Jump].key))
            {
                PlayerDefense();
            }
            else if (Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Items].key) && itemBox.activeInHierarchy)
            {
                ItemMenu();
            }
            else if (Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Items].key))
            {
                PlayerItem();
            }
            //else if (Input.GetKeyDown(PlayerData.controls[5].key))
            //{
            //    MenuOff();
            //}
        }*/
    }

   

    public void PlayerAttack()
    {
        MenuOff();

        
    }

    public void AttackMenu()
    {
        state = State.Busy;
        
        // change this to a click onto the enemy later
        playerCharacterBattle[(int)turn].Attack(enemyCharacterBattle[target], () => { ChooseNextActiveCharacter(); }, false);
        
        MenuOff();
    }

    public CharacterBattleScript PlayerTarget()
    {
        bool targetSelected = false;
        while (!targetSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    if (hit.transform.GetComponent<CharacterBattleScript>().isPlayerTeam == false)
                    {
                        targetSelected = true;
                        return hit.transform.GetComponent<CharacterBattleScript>();

                    }
                }
            }
        }
        return null;
    }
    

    public void MenuOff()
    {
        itemButton.SetActive(true);
        specialButton.SetActive(true);
        defenseButton.SetActive(true);
        attackButton.SetActive(true);

        itemBox.SetActive(false);
        specialBox.SetActive(false);
        defenseBox.SetActive(false);
        attackBox.SetActive(false);
    }

    private CharacterBattleScript SpawnCharacter(bool isPlayerTeam, int charNum, string name)
    {
        Vector3 position;

        if (isPlayerTeam)
        {
            if(charNum == 0)
                position = new Vector3(3, 0, -10);
            else if (charNum == 1)
                position = new Vector3(9, 0, -10);
            else
                position = new Vector3(-3, 0, -10);
            Transform characterTransform = Instantiate(pfCharacterBattle, position, Quaternion.identity);
            characterTransform.name = name;
            CharacterBattleScript characterBattle = characterTransform.GetComponent<CharacterBattleScript>();
            characterBattle.Setup(isPlayerTeam, name);

            return characterBattle;
        }
        else
        {
            if (charNum == 0)
                position = new Vector3(3, 0, 5);
            else if (charNum == 1)
                position = new Vector3(9, 0, 5);
            else if (charNum == 2)
                position = new Vector3(-3, 0, 5);
            else if (charNum == 3)
                position = new Vector3(6, 0, 8);
            else
                position = new Vector3(0, 0, 8);

            Transform characterTransform = Instantiate(Resources.Load(enemyName.ToString(), typeof(Transform)), position, Quaternion.identity) as Transform;
            characterTransform.name = name;

            //////
            int nameAdder = 1;
            for (int i = 0; i<enemyCharacterBattle.Count;i++)
            {
                if(enemyCharacterBattle[i].gameObject.name == characterTransform.name)
                {
                    nameAdder++;
                    characterTransform.name = name + " " + nameAdder;
                    i = -1;
                }
            }
            //GameObject.Find(characterTransform.name + "/Selected").SetActive(false);
            CharacterBattleScript characterBattle = characterTransform.GetComponent<CharacterBattleScript>();
            characterBattle.Setup(isPlayerTeam, name);

            return characterBattle;
        }
        
        //    position = new Vector3(-5, 0);
        //    Transform characterTransform = Instantiate(pfCharacterBattle, position, Quaternion.identity);
        //    CharacterBattleScript characterBattle = characterTransform.GetComponent<CharacterBattleScript>();
        //    if (characterBattle == null)
        //    {
        //        print("giver is the problem");
        //    }
        //    return characterBattle;
        //}
        //else
        //{
        //    print("spawned enemy");
        //    position = new Vector3(5, 0);
        //    Transform characterTransform = Instantiate(pfEnemyBattle, position, Quaternion.identity);
        //    CharacterBattleScript characterBattle = characterTransform.GetComponent<CharacterBattleScript>();
        //    return characterBattle;
        //}
    }

    private void SetActiveCharacterBattle(CharacterBattleScript characterBattle)
    {
        if (characterBattle.canAttack)
        {
            activeCharacterBattle = characterBattle;
        }
        else
        {
            if (turn < (Turn)(playerCharacterBattle.Count - 1))
            {
                turn++;
                SetActiveCharacterBattle(playerCharacterBattle[(int)turn]);
            }
            else if (turn == (Turn)(playerCharacterBattle.Count - 1))
            {
                if(enemyCharacterBattle.Count > 0)
                {
                    SetActiveCharacterBattle(enemyCharacterBattle[attacker]);
                }
            }
        }
    }

    private void ChooseNextActiveCharacter()
    {
        //will have to change later for free character control
        //if (playerCharacterBattle[(int)turn].IsDead())
        //{
        //    playerCharacterBattle[(int)turn].canAttack = false;
        //}

        if (activeCharacterBattle == playerCharacterBattle[(int)turn] && turn <= (Turn)playerCharacterBattle.Count)
        {
            playerCharacterBattle[(int)turn].canAttack = false;
            turn++;

            if((int)turn > playerCharacterBattle.Count - 1)
            {
                turn = Turn.Player1;
            }

            if ((int)turn <= playerCharacterBattle.Count - 1)
            {
                SetActiveCharacterBattle(playerCharacterBattle[(int)turn]);
                state = State.WaitingForPlayer;
            }
        }

        for(int i = 0; i < playerCharacterBattle.Count; i++)
        {
            canAttack[i] = playerCharacterBattle[i].canAttack;
            actionFinished[i] = playerCharacterBattle[i].actionFinished;
        }
        if (!canAttack[0] && !canAttack[1] && !canAttack[2])
        {
            if(actionFinished[0] && actionFinished[1] && actionFinished[2])
            {
                state = State.Busy;
                SetActiveCharacterBattle(enemyCharacterBattle[0]);
            }
        }

        if (activeCharacterBattle == enemyCharacterBattle[attacker] && enemyCharacterBattle.Count > 0)
        {
            state = State.Busy;
            //sets the target the enemy will attack
            int targetPlayer;
            //do while to check that player is alive
            do
                targetPlayer = Random.Range(0, playerCharacterBattle.Count);

            while (playerCharacterBattle[targetPlayer].IsDead() && !TestBattleOver()) ;

            int specialUse = Random.Range(0, 11);
            bool attackType;

            //checks to see what kind of attack the enemy will use
            if (specialUse == 10)
            {
                attackType = true;
            }
            else
            {
                attackType = false;
            }

            enemyCharacterBattle[attacker].Attack(playerCharacterBattle[targetPlayer], () =>
            {
                //ChooseNextActiveCharacter();

                while (!enemyCharacterBattle[attacker].actionFinished) ;

                enemyCharacterBattle[attacker].canAttack = false;

                if(enemyCharacterBattle[attacker].actionFinished)
                {
                    if (attacker < enemyCharacterBattle.Count - 1)
                    {
                        attacker++;
                        SetActiveCharacterBattle(enemyCharacterBattle[attacker]);

                        ChooseNextActiveCharacter();
                    }
                    else
                    {
                        attacker++;
                        attacker = 0;
                        state = State.WaitingForPlayer;
                        turn = Turn.Player1;
                        reset = true;
                    }
                    //if (attacker < enemyCharacterBattle.Capacity - 4)
                    //{
                    //    attacker++;
                    //    print(attacker);
                    //    SetActiveCharacterBattle(enemyCharacterBattle[attacker]);
                    //    //ChooseNextActiveCharacter();
                    //}
                    //else
                    //{
                    //    print("here");
                    //    attacker = 0;
                    //    state = State.WaitingForPlayer;
                    //    turn = Turn.Player1;
                    //    reset = true;
                    //    return;
                    //}
                    //ChooseNextActiveCharacter();
                }

                
                //do
                //{
                //    attacker++;
                //    print(attacker);
                //    SetActiveCharacterBattle(enemyCharacterBattle[attacker]);
                //} while (attacker < enemyCharacterBattle.Capacity - 4);

                //SetActiveCharacterBattle(playerCharacterBattle[(int)turn]);
            }, attackType);
            

            //TestBattleOver();

            /* /if (attacker < enemyCharacterBattle.Capacity - 3)
            //{
            //    attacker++;
            //    print(attacker);
            //    //SetActiveCharacterBattle(enemyCharacterBattle[attacker]);
            //}
            //else
            //{
            //if (attacker == enemyCharacterBattle.Capacity - 4)
            //{
            //    attacker = 0;
            //    state = State.WaitingForPlayer;
            //    turn = Turn.Player1;
            //    reset = true;
            //}

            //} */
        }

    }

    private void ResetTurns()
    {
        for(int i = 0; i < playerCharacterBattle.Count; i++)
        {
            playerCharacterBattle[i].canAttack = canAttack[i] = !playerCharacterBattle[i].IsDead();
        }

        for(int i = 0; i < enemyCharacterBattle.Count; i++)
        {
            enemyCharacterBattle[i].canAttack = true;
        }

        for(int i = 0; i < playerCharacterBattle.Count; i++)
        {
            if(playerCharacterBattle[i].canAttack)
            {
                turn = (Turn)i;
                break;
            }
        }
        //if (playerCharacterBattle[0].canAttack)
        //    turn = Turn.Player1;
        //else if (playerCharacterBattle[1].canAttack)
        //    turn = Turn.Player2;
        //else
        //    turn = Turn.Player3;
        SetActiveCharacterBattle(playerCharacterBattle[(int)turn]);
        state = State.WaitingForPlayer;
        reset = false;
    }

    private bool TestBattleOver()
    {
        bool[] alive = new bool[3];
        for(int i = 0; i < playerCharacterBattle.Count; i++)
        {
            alive[i] = !playerCharacterBattle[i].IsDead();
        }
        if(!alive[0] && !alive[1] && !alive[2])
        {
            //player dead you lose
            print("You Lose!");
            //SceneLoader.GameOver();
            battleStop = true;
            return true;
        }
        //if(playerCharacterBattle[(int)turn].actionFinished)
        //{
        //    return false;
        //}
        for(int i = 0; i<enemyCharacterBattle.Count; i++)
        {
            if (enemyCharacterBattle[i].IsDead())
            {
                for(int j = 0; j < playerCharacterBattle.Count; j++)
                {
                    PlayerData.teamStats[j].GainExperience(enemyCharacterBattle[i].exp);
                    PlayerData.coins += enemyCharacterBattle[i].exp/2;
                    Debug.Log(PlayerData.teamStats[j].name + " gained " + enemyCharacterBattle[i].exp + " Exp");
                }
                //enemy dead you win
                //CharacterControls.questList[0].goal.EnemyKilled();
                //print(CharacterControls.questList[0].goal.currentAmount + " / " + CharacterControls.questList[0].goal.requiredAmount);
                Destroy(enemyCharacterBattle[i].gameObject);
                enemyCharacterBattle.Remove(enemyCharacterBattle[i]);
                i = -1;
                target = 0;
                //print("You Win!");
                
            }
        }
        if (enemyCharacterBattle.Count == 0)
        {
            BattleTransitionCode.enemyChecker[BattleTransitionCode.enemyRemoveNum] = true;
            

            for (int i = 0; i < PlayerData.teamStats.Count; i++)
            {
                if(PlayerData.teamStats[i].leveled)
                {
                    //SceneLoader.LevelUpScreen();
                    battleStop = true;
                    return true;
                }
            }

            //SceneLoader.LeaveBattle();

            battleStop = true;
            return true;
        }
        


        return false;
    }

    private void ActionBoxUpdater()
    {
        actionBox.SetActive(true);
        Transform holder = playerCharacterBattle[(int)turn].transform.Find("ActionBoxHolder");
        Vector3 boxPlacement = holder.transform.position;
        
        Vector3 actionBoxPos = Camera.main.WorldToScreenPoint(boxPlacement);
        actionBox.transform.position = actionBoxPos;
    }
    //to spawn in enemies
    //private void SpawnEnemies()
    //{
    //    for (int i = 0; i < enemyLayout; i++)
    //    {
    //        //finds prefab of enemy using it's name
    //        enemyFinder = (GameObject)Resources.Load(enemyName[i], typeof(GameObject));
    //        //gets the transform of the prefab
    //        EnemyLoader[i] = enemyFinder.transform;
    //        //loads in the enemy using their transform
    //        Instantiate(EnemyLoader[i], positions[i], Quaternion.identity);
    //    }
    //}
    ////////////////// UI ////////////////////
}