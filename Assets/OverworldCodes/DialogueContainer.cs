using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

public class DialogueContainer : MonoBehaviour
{
    public GameObject
        namePanel,
        dialoguePanel,
        picturePanel,
        questButton,
        storeButton;

    [SerializeField]
    private GameObject
        StoreUI;

    public RawImage
        npcPicture;

    XmlDocument dialogueDataXml;

    XmlNode text;

    public Text
        npcName,
        npcText;

    public int
        currentLine = 0;

    public CharacterControls player;

    public bool
        isActive,
        stopPlayerMovement = false,
        hasQuest = false,
        hasStore = false;

    public static int
        instanceNum = 0,
        instanceCap = 0;

    public static bool
        skipStart = false,
        updateInst = false,
        canTalk = true,
        acceptedQuest = false,
        closeStore = false;

    private bool
        storyBased,
        advanceStory;


    void Start()
    {
        if (!FindObjectOfType<TalkingNPC>())
        {
            this.gameObject.SetActive(false);
        }

        //creates a default xml search until a usable one is called
        TextAsset xmlTextAsset = Resources.Load<TextAsset>("XML/NPC_Text");
        dialogueDataXml = new XmlDocument();
        dialogueDataXml.LoadXml(xmlTextAsset.text);

        //starts by disabling the text box windows and hiding everything dialogue related on the screen

        if (isActive)
        {
            EnableTextBox();
        }
        if (!isActive)
        {
            DisableTextBox();
        }

        npcPicture.enabled = false;
    }
    private void LateUpdate()
    {
        if (player == null)
        {
            player = FindObjectOfType<CharacterControls>();
        }

        if (text == null)
        {
            text = dialogueDataXml.SelectSingleNode("/DialogueCollection/NPCDialogue/NPC[@ID='Randy']");
        }

        if (text == null)
        {
            Debug.LogError("Error couldn't find dialogue with name: Random in the xml document.");
            return;
        }

        if(acceptedQuest || closeStore)
        {
            DisableTextBox();
            acceptedQuest = closeStore = false;
        }
        
        //DialogueController newDialogueController = new DialogueController(text);
        
        //npcPicture.texture = newDialogueController.NPCImage;

        //brings up image of npc if the player is currently talking to a character
        //if (!player.canMove)
        //{
        //    npcPicture.enabled = true;
        //}
        //else if (player.canMove)
        //{
        //    npcPicture.enabled = false;
        //}

        //moves the conversation forward when the X key is pressed
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    print("the instance number is " + instanceNum);
        //    //if there are more lines to be read, shows the current line of text and prepares for the next one
        //    if (currentLine <= newDialogueController.EndAtLine)
        //    {
        //        npcText.text = newDialogueController.textLines[currentLine];
        //        currentLine++;
        //    }
        //    //if there are no more lines, closes the window and prepares the next instance of texts until it reaches the limit
        //    else
        //    {
                
        //        DisableTextBox();
        //        currentLine = 0;
        //    }
        //}

        //keep track of key press timing

    }

    //reading text
    public void ReadText()
    {
        DialogueController newDialogueController = new DialogueController(text, storyBased);

        if (!player.canMove)
        {
            npcPicture.enabled = true;
        }
        else
        {
            npcPicture.enabled = false;
        }

        if (newDialogueController.NPCImage != null)
        {
            npcPicture.texture = newDialogueController.NPCImage;
        }
        else
        {
            npcPicture.enabled = false;
            npcName.enabled = false;
            namePanel.SetActive(false);
        }

        PauseMenu.canPause = false;

        if (Input.GetKeyDown(PlayerData.controls[(int)CustomControls.Controls.Talk].key) || skipStart)
        {
            if(skipStart)
            {
                skipStart = false;
            }
            //if there are more lines to be read, shows the current line of text and prepares for the next one
            if (currentLine <= newDialogueController.EndAtLine)
            {
                npcText.text = newDialogueController.textLines[currentLine];
                currentLine++;
                if(currentLine > newDialogueController.EndAtLine && hasQuest)
                {
                    questButton.SetActive(true);
                }
                else if (currentLine > newDialogueController.EndAtLine && hasStore)
                {
                    storeButton.SetActive(true);
                }
            }
            //if there are no more lines, closes the window and prepares the next instance of texts until it reaches the limit
            else
            {
                if(hasStore && StoreUI.activeInHierarchy || questButton.activeInHierarchy == true)
                {
                    //do nothing
                }
                else
                {
                    DisableTextBox();
                    updateInst = true;
                }
            }
        }
    }

    //when passing information
    public void EnableTextBox(string name, int instNum, int instCap, XmlNode data, bool storyBased, bool advanceStory)
    {
        dialoguePanel.SetActive(true);
        namePanel.SetActive(true);
        picturePanel.SetActive(true);
        isActive = true;
        npcPicture.enabled = true;
        npcName.enabled = true;

        acceptedQuest = false;

        npcName.text = name;
        instanceNum = instNum;
        instanceCap = instCap;
        text = data;
        this.storyBased = storyBased;
        this.advanceStory = advanceStory;

        if (stopPlayerMovement)
        {
            player.canMove = false;
        }

        ReadText();
    }

    public void EnableTextBox()
    {
        //print("it came back on");
        dialoguePanel.SetActive(true);
        namePanel.SetActive(true);
        picturePanel.SetActive(true);
        isActive = true;
        
        if (stopPlayerMovement)
        {
            player.canMove = false;
        }
    }

    //hides all textbox values and updates a flag to change the npc instance value
    public void DisableTextBox()
    {
        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);
        picturePanel.SetActive(false);
        isActive = false;

        currentLine = 0;
        PauseMenu.canPause = true;

        questButton.SetActive(false);
        storeButton.SetActive(false);

        if (player != null)
            player.canMove = true;

        if(storyBased && advanceStory)
        {
            SceneLoader.storyInstance++;
        }
    }

    //public void ReloadScript(TextAsset npcText)
    //{

    //    if (npcText != null)
    //    {
    //        textLines = (text.Value.Split('\n'));
    //    }
    //}


    class DialogueController
    {
        public string NPCName { get; private set; }
        //public string storyInst { get; private set; }
        public string NPCDialogue { get; private set; }
        public Texture NPCImage { get; private set; }
        public string[] textLines;
        //public int CurrentLine { get; set; }
        public int EndAtLine { get; private set; }
        //public bool HasQuest { get; private set; }
        //public bool HasStore { get; private set; }
        //public bool diaChecker = true;

        public DialogueController(XmlNode DiaNode, bool storyBased)
        {
            int adder = 0;

            if(storyBased)
            {
                adder += SceneLoader.storyInstance;
            }

            NPCName = DiaNode.Attributes["ID"].Value;
            NPCDialogue = DiaNode["Instance" + (adder + DialogueContainer.instanceNum)].InnerText;

            if (NPCDialogue != null)
            {
                textLines = new string[3];
                textLines = (NPCDialogue.Split('_'));
                EndAtLine = textLines.Length - 1;
            }
            else
            {
                print("it fell through");
                return;
            }

            //for getting the npc images
            

            if(DiaNode["Image"].InnerText.ToLower() != "none")
            {
                string pathToImage = "NPC_Images/" + DiaNode["Image"].InnerText;
                NPCImage = Resources.Load<Texture2D>(pathToImage);
            }
            else
            {
                NPCImage = null;
            }
        }

        //gotta make a new inner class on here
    }


}