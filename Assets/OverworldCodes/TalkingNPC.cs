using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class TalkingNPC : MonoBehaviour
{
    //to get the instance number for text strings and to set its specific cap
    public int 
        instanceNum,
        instanceCap;

    //to hold the name of the npc using the character's name ingame
    private string npcName;

    //public string questName;

    XmlDocument dialogueDataXml;

    //if the talking field should be kept after the first conversation
    public bool destroyWhenActivated;
    
    private DialogueContainer theTextBox;

    XmlNode text;

    public bool requireButtonPress;
    private bool waitForPress;

    private bool
        prompt = false;

    public bool
        loopText = false,
        stopPlayer = true,
        hasQuest = false,
        hasStore = false,
        recruitable = false,
        storyBased = false,
        advanceStory = false;
    
    // Use this for initialization
    void Start()
    {
        //finds the text box container to adjust as needed
        theTextBox = FindObjectOfType<DialogueContainer>();
        //theTextBox.Updater();
        //creates the actual xml search for when the specific npc is called on
        TextAsset xmlTextAsset = Resources.Load<TextAsset>("XML/NPC_Text");
        dialogueDataXml = new XmlDocument();
        dialogueDataXml.LoadXml(xmlTextAsset.text);

        //sets the npc name to pass into the dialogue container later on
        npcName = transform.parent.name;

        if(instanceNum == 0)
        {
            instanceNum++;
        }

        if(recruitable)
        {
            for(int i = 0; i < PlayerData.teamStats.Count; i++)
            {
                if(transform.parent.gameObject.name == PlayerData.teamStats[i].name)
                {
                    Destroy(transform.parent.gameObject);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        //gets the dialogue  from xml file using the name of the object this code is running on
        text = dialogueDataXml.SelectSingleNode("/DialogueCollection/NPCDialogue/NPC[@ID='" + npcName + "']");

        if (text == null)
        {
            Debug.LogError("Error couldn't find dialogue with name: " + npcName + " in the xml document.");
            return;
        }

        //starts the conversation after an x press
        if (waitForPress && Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Talk].key) && !DialogueContainer.updateInst && DialogueContainer.canTalk || prompt)
        {
            if(prompt)
            {
                prompt = false;
                DialogueContainer.skipStart = true;
                waitForPress = true;
            }

            //passes all relevant data to the dialogue container
            if (stopPlayer)
                theTextBox.stopPlayerMovement = true;
            else
                theTextBox.stopPlayerMovement = false;

            if(hasStore)
            {
                theTextBox.hasStore = true;
            }

            if (hasQuest)
            {
                if(PlayerData.activeQuestList.Count > 0)
                {
                    for(int i = 0; i < PlayerData.activeQuestList.Count; i++)
                    {
                        if((transform.parent.name + "_" + SceneLoader.sceneLocation[0] + "_" + SceneLoader.storyInstance + "_" + 1) == PlayerData.activeQuestList[i].id)
                        {
                            theTextBox.hasQuest = false;
                            break;
                        }
                        if(i == PlayerData.activeQuestList.Count -1)
                        {
                            theTextBox.hasQuest = true;
                        }
                    }
                }else if (PlayerData.completedQuestList.Count > 0)
                {
                    for (int i = 0; i < PlayerData.completedQuestList.Count; i++)
                    {
                        if ((transform.parent.name + "_" + SceneLoader.sceneLocation[0] + "_" + SceneLoader.storyInstance + "_" + 1) == PlayerData.completedQuestList[i].id)
                        {
                            theTextBox.hasQuest = false;
                            break;
                        }
                        if (i == PlayerData.completedQuestList.Count - 1)
                        {
                            theTextBox.hasQuest = true;
                        }
                    }
                }
                else
                {
                    theTextBox.hasQuest = true;
                }
            } else
                theTextBox.hasQuest = false;

            theTextBox.EnableTextBox(npcName, instanceNum, instanceCap, text, storyBased, advanceStory);
        }

        //updates the current npc's instance number if other dialogues with them are shown in the next conversation
        if (DialogueContainer.updateInst && instanceNum < instanceCap && !loopText)
        {
            instanceNum++;   
        }

        if (DialogueContainer.updateInst && instanceNum <= instanceCap && loopText)
        {
            instanceNum++;
        }

        if (DialogueContainer.updateInst && instanceNum > instanceCap && loopText)
        {
            if (loopText)
            {
                instanceNum = 1;
            }
        }


        if (DialogueContainer.updateInst && instanceNum >= instanceCap)
        {
            //if desired the npc will be unable to talk again while this is true
            if (destroyWhenActivated)
            {
                if(recruitable)
                {
                    GameObject.Find("Canvas").GetComponent<PauseMenu>().LoadStats(transform.parent.name);

                    //PauseMenu.nameList.Add(transform.parent.name);
                    PauseMenu.updateInfo = true;
                    Destroy(transform.parent.gameObject);
                }
                Destroy(gameObject);
            }
        }
            //resets update instance
            DialogueContainer.updateInst = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (requireButtonPress)
            {
                waitForPress = true;
                return;
            }
            else if(!requireButtonPress)
            {
                prompt = true;
            }

            //if(DialogueContainer.canTalk)
            //{
            //    print(DialogueContainer.canTalk);
            //    theTextBox.EnableTextBox(npcName, instanceNum, instanceCap, text);
            //}



            //if (destroyWhenActivated)
            //{
            //    Destroy(gameObject);
            //}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            waitForPress = false;
        }
    }


}
