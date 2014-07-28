/// <summary>
/// MainMenu.cs
/// August 20,2012
/// 
/// The main Menu Script. 
/// The script checks to see if we have saved data to PlayerPrefs and the version of the saved data. 
/// If the saved version is the current one it checks if we have a character saved - check for a character name. 
/// If no char saved load the char generation scene, else let them load their character or delete and create a new one
/// </summary>
using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    private const string _wholeStory = "I escaped from the prison, six years ago \n I have spent half of my life in jail. \n I was living in snowy mountains alone until they found me .. They burned my tower. \n There is someone more powerful than him, that is what the king is worrying about. \n They tried to kill me, but they failed. Now it's time to revenge!";

    //Set up the Scenes names to load
    private readonly string _characterGenerationScene = GameSetting2.levelNames[1];
    private readonly string _firstLevelScene = GameSetting2.levelNames[5];
    private readonly string _arenaScene = GameSetting2.levelNames[6];

    public bool clearPrefs = false;

    private string _levelToLoad;                    //The level number to load
    private bool _charExists;                       //true if the player already has a created character
    private bool _displayOptions;                   //hide options if the user clicks a button
    private int _option = 0;
    private string story = "";

    private bool typing;
    private GUIStyle _centeredStyle;                //to center the label text 
    public Rect windowRect;                             //gui - window's position

    public GUISkin mySkin;
    // Use this for initialization
    void Start()
    {
        _charExists = false;
        _levelToLoad = string.Empty;
        _displayOptions = true;

        if (clearPrefs)  //clear player prefs option
            PlayerPrefs.DeleteAll();
        windowRect = new Rect(250, 100, 500, 400);

    }

    // Update is called once per frame
    void Update()
    {
        if (_levelToLoad.Equals(string.Empty))
            return;

        Application.LoadLevel(_levelToLoad);
    }

    void DoMyWindow(int windowID)
    {

        GUILayout.BeginVertical();
        switch (_option)
        {
            case 0:
                GUILayout.Label("Main Menu", GUILayout.Height(80));
                GUILayout.Space(30);
                typing = false;
                if (GUILayout.Button("New Story"))
                {
                    _option = 1;
                    NewGame();
                }
                if (GUILayout.Button("Enter Arena"))
                {
                    _option = 2;
                    NewGame();
                }
                if (GUILayout.Button("Controls"))
                {
                    _option = 3;
                    NewGame();
                }
                if (GUILayout.Button("Credits"))
                {
                    _option = 4;
                    NewGame();
                }
                break;
            case 1:
                if (_displayOptions)
                {
                    GUILayout.Label("Play Game", GUILayout.Height(80));
                    GUILayout.Space(20);
                    if (_charExists)
                    {
                        GUILayout.Label("A character already exists", "MainMenuCursedText");
                        GUILayout.Space(5);
                        if (GUILayout.Button("Load Character"))
                        {
                            _displayOptions = false;
                            _levelToLoad = _firstLevelScene;
                        }
                        if (GUILayout.Button("New Story"))
                        {
                            _charExists = false;
                            NewStory();
                        }
                        if (GUILayout.Button("Back"))
                        {
                            _option = 0;
                        }
                    }
                    else
                    {
                        NewStory();
                    }
                }
                break;
            case 2:
                if (_displayOptions)
                {
                    GUILayout.Label("Arena", GUILayout.Height(80));
                    GUILayout.Space(20);
                    if (_charExists)
                    {
                        _displayOptions = false;
                        _levelToLoad = _arenaScene;
                    }
                    else
                    {
                        GUILayout.Label("No saved character was found", "MainMenuCursedText");
                        GUILayout.Label("You need to play the game first before entering the arena", "MainMenuCursedText");
                        GUILayout.Space(10);
                        if (GUILayout.Button("Create Character"))
                        {
                            _displayOptions = false;
                            LoadCharacterGenerationScene();
                        }
                        if (GUILayout.Button("Back"))
                        {
                            _option = 0;
                        }
                    }
                }
                break;
            case 3:
                GUILayout.Label("Controls", GUILayout.Height(80));
                GUILayout.Label("Press W,S,A,D to move", "MainMenuCursedText");
                GUILayout.Label("Press space to jump", "MainMenuCursedText");
                GUILayout.Label("Press 1-5 to attack or use skills", "MainMenuCursedText");
                GUILayout.Label("Target enemies with mouse", "MainMenuCursedText");
                GUILayout.Space(10);
                if (GUILayout.Button("Back"))
                {
                    _option = 0;
                }
                break;
            case 4:
                GUILayout.Label("Controls", GUILayout.Height(80));
                GUILayout.Label("Odysseas Georgoudis", "MainMenuCursedText");
                GUILayout.Label("University of Piraeus, Digital Systems Department", "MainMenuCursedText");
                GUILayout.Label("Undergraduate Thesis", "MainMenuCursedText");
                GUILayout.Label("Year 2012", "MainMenuCursedText");
                GUILayout.Space(10);
                if (GUILayout.Button("Back"))
                {
                    _option = 0;
                }
                break;
        }
        GUILayout.EndVertical();
        //check if there is a level to load and if there is display the load percentage
        if (_levelToLoad.Equals(string.Empty))
            return;
        GUILayout.BeginVertical();
        GUILayout.Space(80);
        GUILayout.EndVertical();
    }
    void OnGUI()
    {
        GUI.skin = mySkin;
        windowRect = GUI.Window(0, windowRect, DoMyWindow, "", "MainMenuWindow");
    }

    //Method to load the next scene and clear any previous saved prefs.
    private void LoadCharacterGenerationScene()
    {
        PlayerPrefs.DeleteAll();
        GameSetting2.SaveGameVersion();
        _levelToLoad = _characterGenerationScene;
    }

    //Actually it only checks if a char exists or not
    private void NewGame()
    {
        if (PlayerPrefs.HasKey(GameSetting2.VERSION_NAME))
        {
            Debug.Log("There is a ver key");
            //check the version
            if (GameSetting2.LoadGameVersion() != GameSetting2.VERSION)
            {
                Debug.Log("Saved Version is not the same");
                /*Upgrade playerprefs here if we have to*/
            }
            else
            {
                Debug.Log("Saved Version is the same");
                //check for a player name. If there is one
                if (PlayerPrefs.HasKey("Player Name"))
                {
                    Debug.Log("There is a player name tag");
                    //check that the player name is not empty
                    if (string.IsNullOrEmpty(PlayerPrefs.GetString("Player Name")))
                    {
                        Debug.Log("The player name key is empty");
                        //        PlayerPrefs.DeleteAll();
                        //        _levelToLoad = _characterGenerationScene;
                    }
                    //if it has something
                    else
                    {
                        Debug.Log("The player name key has a value");
                        _charExists = true;
                    }
                }
                //If there is no player name
                else
                {
                    Debug.Log("There is no player name");
                    //  LoadCharacterGenerationScene();
                }
            }
        }
        //If there is no version key
        else
        {
            Debug.Log("There is no version key");
            LoadCharacterGenerationScene();
        }

    }

    private void NewStory()
    {
        // GUILayout.Label("No saved character was found", "MainMenuCursedText");
        GUILayout.Label(story, "MainMenuCursedText");
        if (!typing)
        {
            typing = true;
            StartCoroutine(TypeText());
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create Character"))
        {
            LoadCharacterGenerationScene();
        }
        if (GUILayout.Button("Back"))
        {
            _option = 0;
        }
    }

    IEnumerator TypeText()
    {
        foreach (char letter in _wholeStory)
        {
            story += letter;
            yield return new WaitForSeconds(0.12f);
        }
    }
}
