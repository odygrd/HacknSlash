/// <summary>
/// CharacterGenerator.cs
/// May 30,2012
/// 
/// This is the class that creates a new player character.
/// It is mainly the GUI and a method call at the end to save our settings
/// This script can be attached to any game object in the scene.
/// A GUI.skin link and a player prefab are required.
/// </summary>
using UnityEngine;
using System.Collections;
using System;

public class CharacterGenerator : MonoBehaviour
{
    //character points constants
    private const int STARTING_POINTS = 250; 			 //Total starting character points.
    private const int MIN_STARTING_ATT = 10; 			 //Free starting points for each attribute.
    private const int STARTING_VALUE = 50; 				 //Starting points for each attribute.

    private int _pointsleft = STARTING_POINTS; 			 //Remaining points that player has to spend.

    private float _lastClick = 0;                         //keep track how long it's been since increase or decrease of a stat
    private Rect windowRect;                             //gui - window's position
    private float _spikeCount;                           //used to display gui spikes
    private Vector2 _scrollPosition;                     //gui - scroll position for the text

    private GameObject _characterMesh;                   //storing players model - to get reference to destroy playerinput

    public GUISkin mySkin;                    			//To use custom GUI skin.
    public float delayTimer = 0.25f;                    //Delay timer for Menu - repeat button
   

    void Awake()
    {
        PlayerChar.Instance.Initialise();
    }

    /// <summary>
    /// Start this instance.
    /// Instantiate the player prefab and name it "Player Character".
    /// Then get a reference to PlayerCharacter class and set the BaseValue of each attribute.
    /// </summary>
    void Start()
    {
        windowRect = new Rect(0, 0, 1000, 600);
        for (int i = 0; i < Enum.GetValues(typeof(AttributeName)).Length-1; i++)
        {
            PlayerChar.Instance.GetPrimaryAttribute(i).BaseValue = STARTING_VALUE; //set up a starting value to each attribute
            _pointsleft -= (STARTING_VALUE - MIN_STARTING_ATT);
        }

        PlayerChar.Instance.StatUpdate(); //update vitals

        _characterMesh = GameObject.FindGameObjectWithTag("Player");
        Destroy(_characterMesh.GetComponent<PlayerInput>()); //destroy the controller
        Destroy(_characterMesh.GetComponent<PlayerGUI>());
        Destroy(_characterMesh.GetComponent<ThirdPersonController>());
        Destroy(_characterMesh.GetComponent<NetworkCharacter>());
        Destroy(_characterMesh.GetComponent<PhotonView>());
    }

    /// <summary>
    /// Raises the GUI event.
    /// OnGUI is called for rendering and handling GUI events.
    /// </summary>
    void OnGUI()
    {
        GUI.skin = mySkin;                 
        windowRect = GUI.Window(0, windowRect, DoMyWindow, "New Game");
    }

    void DoMyWindow(int windowID)
    {
        //Tittle
        //The the window spikes at the top
        AddSpikes(windowRect.width);

        GUILayout.BeginVertical();

        GUILayout.Space(15);
        GUILayout.Label("Character Creation");

        //Main Window

        Displayname();
        GUILayout.Space(20);
        DisplayAttributes();
        DisplayVitals();

        //Submit - check for player name and points
        GUILayout.Space(40);
        if (_pointsleft > 0 || string.IsNullOrEmpty(PlayerChar.Instance.name) )
            DisplayCreateLabel();
        else
            DisplayCreateButton();
    }

    private void AddSpikes(float winX)
    {

        _spikeCount = Mathf.Floor(winX - 152) / 22;
        GUILayout.BeginHorizontal();
        GUILayout.Label("", "SpikeLeft");
        for (int i = 0; i < _spikeCount; i++)
        {
            GUILayout.Label("", "SpikeMid");
        }
        GUILayout.Label("", "SpikeRight");
        GUILayout.EndHorizontal();
    }


    #region GUI Display Methods

    /// <summary>
    /// Method to display a label and a textfield for players char name.
    /// </summary>
    private void Displayname()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name:  ", "ShortLabelName", GUILayout.Width(130));
        PlayerChar.Instance.name = GUILayout.TextField(PlayerChar.Instance.name, 15, GUILayout.Width(130));
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Method to display attribute names,values and buttons for adjusting them.
    /// </summary>
    private void DisplayAttributes()
    {
        for (int i = 0; i < Enum.GetValues(typeof(AttributeName)).Length-1; i++)
        {
            GUILayout.BeginHorizontal();
            //Attribute Name Label
            GUILayout.Label(((AttributeName)i).ToString(), "ShortLabel", GUILayout.Width(130));
            // - button
            if (GUILayout.RepeatButton("-", "ShortButton"))
            {
                if (Time.time - _lastClick > delayTimer)  //if the timer is the delaytiemr allow stat update
                {
                    if (PlayerChar.Instance.GetPrimaryAttribute(i).BaseValue > MIN_STARTING_ATT)
                    {
                        PlayerChar.Instance.GetPrimaryAttribute(i).BaseValue -= 5; //Remove 5 points
                        _pointsleft += 5; //Add 5 points to remaining points
                        PlayerChar.Instance.StatUpdate();
                    }
                    _lastClick = Time.time;
                }
            }
            //Attribute Value Label
            GUILayout.Label(PlayerChar.Instance.GetPrimaryAttribute(i).AdjustedBaseValue.ToString(), "LightOutlineText");
            // + button
            if (GUILayout.RepeatButton("+", "ShortButton"))
            {
                if (Time.time - _lastClick > delayTimer)
                {
                    if (_pointsleft > 0)
                    {
                        PlayerChar.Instance.GetPrimaryAttribute(i).BaseValue += 5; //Add 5 points to that stat
                        _pointsleft -= 5; //Remove 5 points from remaining points
                        PlayerChar.Instance.StatUpdate();
                    }
                    _lastClick = Time.time;
                }
            }
			
			//check for i to be inside the index of array skill
			if (i<Enum.GetValues(typeof(SkillName)).Length)
			{
            //Display Skills
            GUILayout.Label(((SkillName)i).ToString(), "ShortLabel", GUILayout.Width(200));
            GUILayout.Label(PlayerChar.Instance.GetSkill(i).AdjustedBaseValue.ToString(), "BoldOutlineText");
			}
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Method to display player vitals and info text.
    /// </summary>
    private void DisplayVitals()
    {
        GUILayout.BeginArea(new Rect(570, 150, 300, 250));
        GUILayout.BeginVertical();
        GUILayout.Label("Points Left: " + _pointsleft, "LegendaryText");
        for (int i = 0; i < Enum.GetValues(typeof(VitalName)).Length; i++)
        {
            GUILayout.BeginHorizontal();
            //Label displays vital's name
            GUILayout.Label(((VitalName)i) + ": " + PlayerChar.Instance.GetVital(i).AdjustedBaseValue, "CursedText");
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.Label("", "Divider");
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, true, true);
        GUILayout.TextArea("Strength: Increases melee attack\nDexterity: Increases ranged attack\nIntelligence: Increases spell power and magic defence\nConstitution: Increases health\nCharisma: Increases mana and magic defence", "PlainText");
        GUILayout.EndScrollView();
        GUILayout.EndArea();

    }

    /// <summary>
    /// Method to display a label instead of a button when name or points are missing.
    /// </summary>
    private void DisplayCreateLabel()
    {
        GUILayout.Label("To continue spend the remaining points", "Button");
    }

    /// <summary>
    /// Method to display a create button
    /// </summary>
    private void DisplayCreateButton()
    {
        if (GUILayout.Button("Next"))
        {

            //Change the current value of the vitals, to the max modified value of that vital
            UpdateCurVitalValues();
            GameSetting2.SaveName(PlayerChar.Instance.name);
            GameSetting2.SaveAttributes(PlayerChar.Instance.primaryattribute);
            GameSetting2.SaveVitals(PlayerChar.Instance.vital);
            GameSetting2.SaveSkills(PlayerChar.Instance.skill);


            /***** Debug Section ******
            Skill[] temp = GameSetting2.LoadSkills(); 
            for (int i = 0; i < temp.Length; i++)
            {
                Debug.Log(+ temp[i].BaseValue +":" + temp[i].ExpToLevel);
            }
            //Debug.Log(GameSetting2.LoadAttribute(AttributeName.Might).BaseValue + "::" + GameSetting2.LoadAttribute(AttributeName.Might).ExpToLevel) ;
			*/
            //Load Next Level - Index 2 is "Tutorial" level
            Application.LoadLevel(GameSetting2.levelNames[2]);

        }
    }

    /// <summary>
    /// Updates the current vital values before saving them.
    /// We are displaying AdjustedBaseValue to the user above, so we have to save it to current value when he finishes creating the character.
    /// </summary>
    private void UpdateCurVitalValues()
    {
        for (int i = 0; i < Enum.GetValues(typeof(VitalName)).Length; i++)
        {
            PlayerChar.Instance.GetVital(i).CurValue = PlayerChar.Instance.GetVital(i).AdjustedBaseValue;
        }
    }
    #endregion
}
