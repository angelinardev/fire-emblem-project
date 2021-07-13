using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

public class TextBoxManager : MonoBehaviour
{

    public GameObject 
        textBox,
        nameBox;

    public Text 
        npcText,
        npcName;

    public TextAsset textFile;
    public string[] textLines;

    public int 
        currentLine,
        endAtLine;

    //public CharacterControls player;

    public bool
        isActive = false,
        stopPlayerMovement = false;

    private bool
        changeName;


    private void Start()
    {
        changeName = true;

        //player = FindObjectOfType<CharacterControls>();

        if(textFile != null)
        {
            textLines = (textFile.text.Split('\n'));
        }

        if(endAtLine == 0)
        {
            endAtLine = textLines.Length - 1;
        }

        //if(isActive)
        //{
        //    EnableTextBox();
        //}
        //else
        //{
            DisableTextBox();
        //}
    }

    private void Update()
    {
        if(!isActive)
        {
            return;
        }

        if (changeName)
        {
            npcName.text = textLines[currentLine];
            currentLine++;
            changeName = false;
        }

        npcText.text = textLines[currentLine];

        if(Input.GetKeyDown(KeyCode.X))
        {
            currentLine++;
        }

        if(currentLine > endAtLine)
        {
            DisableTextBox();
        }
    }

    public void EnableTextBox()
    {
        textBox.SetActive(true);
        nameBox.SetActive(true);
        isActive = true;

        //if(stopPlayerMovement)
        //{
        //    player.canMove = false;
        //}
    }

    public void DisableTextBox()
    {
        textBox.SetActive(false);
        nameBox.SetActive(false);
        isActive = false;

        //player.canMove = true;
    }

    public void ReloadScript(TextAsset npcText)
    {
        
        if(npcText != null)
        {
            textLines = new string[1];
            textLines = (textFile.text.Split('\n'));
        }
    }
}
