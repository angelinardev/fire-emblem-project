using System.Collections;
using UnityEngine;

public class ActiveTextAtLines : MonoBehaviour
{
    public TextAsset theText;

    //public int 
    //    startLine,
    //    endLine;

    public DialogueContainer theTextBox;

    public bool requireButtonPress;
    private bool waitForPress;

    public bool destroyWhenActivated;
	// Use this for initialization
	void Start ()
    {
        theTextBox = FindObjectOfType<DialogueContainer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(waitForPress && Input.GetKeyDown("Fire"/*PlayerData.controls[(int)CustomControls.Controls.Jump].key*/))
        {
            //theTextBox.ReloadScript(theText);
            //theTextBox.currentLine = startLine;
            //theTextBox.endAtLine = endLine;


            //theTextBox.EnableTextBox();

            if (destroyWhenActivated)
            {
                Destroy(gameObject);
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(requireButtonPress)
            {
                waitForPress = true;
                return;
            }

            //theTextBox.ReloadScript(theText);
            //theTextBox.currentLine = startLine;
            //theTextBox.endAtLine = endLine;

            //theTextBox.EnableTextBox();

            if (destroyWhenActivated)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            waitForPress = false;
        }
    }
}
//public class ActiveTextAtLines : MonoBehaviour
//{
//    public TextAsset theText;

//    public int
//        startLine,
//        endLine;

//    public TextBoxManager theTextBox;

//    public bool requireButtonPress;
//    private bool waitForPress;

//    public bool destroyWhenActivated;
//    // Use this for initialization
//    void Start()
//    {
//        theTextBox = FindObjectOfType<TextBoxManager>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (waitForPress && Input.GetKeyDown(KeyCode.X))
//        {
//            theTextBox.ReloadScript(theText);
//            theTextBox.currentLine = startLine;
//            theTextBox.endAtLine = endLine;
//            theTextBox.EnableTextBox();

//            if (destroyWhenActivated)
//            {
//                Destroy(gameObject);
//            }
//        }
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        if (other.tag == "Player")
//        {
//            if (requireButtonPress)
//            {
//                waitForPress = true;
//                return;
//            }

//            theTextBox.ReloadScript(theText);
//            theTextBox.currentLine = startLine;
//            theTextBox.endAtLine = endLine;
//            theTextBox.EnableTextBox();

//            if (destroyWhenActivated)
//            {
//                Destroy(gameObject);
//            }
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.tag == "Player")
//        {
//            waitForPress = false;
//        }
//    }
//}
