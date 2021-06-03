using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleTransitionCode : MonoBehaviour
{
    public GameObject levelLoader;

    public enum FieldType
    {
        standard,
        alt,
        training
    };
    public FieldType fieldtype;
    public static int formationType;
    public int EnemyCount;
    public static int Variance;
    public enum EnemyName
    {
        Tofu,
        Newfu,
        Oldfu
    }
    public EnemyName enemyName;
    public static int enemyNameNum;
    private float targetTime = 1.0f;

    

    //public bool defeated = false;

    private string sceneName = "Battle Scene ";

    private CharacterControls player;

    //creates a list of enemy numbers all set to don't delete
    public static List<bool> enemyChecker = new List<bool>(new bool[10]);
    public int enemyNum;
    public static int enemyRemoveNum;
    private void Awake()
    {
        if (enemyChecker[enemyNum])
            Destroy(gameObject);
    }

    void Start()
    {

        //list.Add(true);
        //enemyNum = list.Count - 1;
        

        
        //if (fieldType == 0)
        //{
        //    fieldType = (FieldType)1;
        //}
        //if (Variance == 0)
        //{
        //    Variance = 1;
        //}
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            player = FindObjectOfType<CharacterControls>();
        }
        targetTime -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (levelLoader == null)
        {
            levelLoader = GameObject.Find("LevelLoader");
        }

        if (targetTime<=0)
        {
            if (other.CompareTag("Player"))
            {
                Variance = EnemyCount + 1;
                enemyNameNum = (int)enemyName;
                enemyRemoveNum = enemyNum;
                //enemyChecker[enemyNum] = true;
                player.SavePlayerPosition();
                //player.transform.parent.DDOL.Kill();

                if (fieldtype == FieldType.training)
                {
                    levelLoader.GetComponent<SceneTransition>().LoadLevel((sceneName + 0), true);
                }
                else
                {
                    levelLoader.GetComponent<SceneTransition>().LoadLevel((sceneName + (int)(fieldtype + 1)), true);
                }
            }
        }
        
    }
}