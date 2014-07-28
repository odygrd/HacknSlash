// MyGUI.cs
// August 16, 2012
// 
// This class will display all of the gui elements while the game is playing.
// This includes:
// Looting Window
// Inventory Window
// Character Panels (equipment, attributes, skils)
//
// This script needs to be attached to a gameobject.

using UnityEngine;

[AddComponentMenu("GUI/My Gui Script")] //Add script to main menu for easier reference

public class myGUI : MonoBehaviour
{
    public GUISkin mySkin;
    public float lootWindowHeight = 90;  //how height the loot window will be
    public float buttonWidth = 70;  //The button width
    public float buttonHeight = 70; //The button height
    public float closeButtonWidth = 20; //The close button width
    public float closeButtonHeight = 20; //The close button height
    public static Chest chest;
 

    /*Define GUI Skin names*/
    public string emptyInventorySlot;     //empty inventory slots
    public string closeButtonStyle;      //for button "x"
    public string commonInventorySlot;  //common item

    private const float _offset = 10; //offset from the sides of the screen
    private string _toolTip = string.Empty;

    private float _doubleClickTimer; //timer between two clicks to count them as double click
    private const float DOUBLECLICK_TIMER = 0.5f; //the timer between two clicks
    private Item _selectedItem; //store the user selected item
 

    #region Loot Window Variables*/
    private bool _displayLootWindow = false; //if true the loot window is shown
    private const int LOOT_WINDOWS_ID = 0;
    private Rect _lootWindowRect = new Rect(0, 0, 0, 0);
    private Vector2 _lootWindowSlider = Vector2.zero;
    #endregion

    #region Inventory Window Variables
    public static bool displayInventoryWindow = false;
    private const int INVENTORY_WINDOW_ID = 1;
    private Rect _inventoryWindowRect = new Rect(300, 100, 180, 295);
    private int _inventoryRows = 6;  //inventory rows
    private int _invetoryCols = 4; //inventory columns
    #endregion

    #region Character Window Variables
    public static bool displayCharacterWidnow = false;
    private const int CHARACTER_WINDOW_ID = 2;
    private Rect _characterWindowRect = new Rect(50, 80, 250, 350);

    private int _characterPanel = 0;
    private readonly string[] _characterPanelNames = new string[] { "Equipment", "Attributes"};
    #endregion

    #region Main Menu Widnow Variables
    private bool _displayMainMenu = false; //if true the loot window is shown
    private const int MAINMENU_WINDOWS_ID = 3;
    private Rect _MainMenuWindowRect = new Rect(Screen.width / 2 - 75, Screen.height / 2 - 100, 150, 200);
    #endregion

    // Use this for initialization
    void Awake()
    {
        PlayerChar.Instance.Initialise();
       
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Movement>().enabled = true;
        player.GetComponent<PlayerInput>().enabled = true;
        //Destroy the network scripts
        Destroy(player.GetComponent<ThirdPersonController>());
        Destroy(player.GetComponent<NetworkCharacter>());
        Destroy(player.GetComponent<PhotonView>());
    }
    #region Messenger Class Listeners
    //Methods required for messenger
    private void OnEnable()
    {
        Messenger.AddListener("DisplayLoot", DisplayLoot);
        Messenger.AddListener("CloseChest", ClearWindow);
        Messenger.AddListener("ToggleInventory", ToggleInventoryWindow);
        Messenger.AddListener("ToggleCharacterWindow", ToggleCharacterWindow);
        Messenger.AddListener("ToggleMainMenu", ToggleMainMenu);
    }

    private void onDisable()
    {
        Messenger.RemoveListener("DisplayLoot", DisplayLoot);
        Messenger.RemoveListener("CloseChest", ClearWindow);
        Messenger.RemoveListener("ToggleInventory", ToggleInventoryWindow);
        Messenger.RemoveListener("ToggleCharacterWindow", ToggleCharacterWindow);
        Messenger.RemoveListener("ToggleMainMenu", ToggleMainMenu);
    }
    #endregion

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        GUI.skin = mySkin;      //define the skin that we are going to be using for our window

        //display the character window if we are set to.
        if (displayCharacterWidnow)
            _characterWindowRect = GUI.Window(CHARACTER_WINDOW_ID, _characterWindowRect, CharacterWindow, "Character");

        //display the inventory window if we are set to.
        if (displayInventoryWindow)
            _inventoryWindowRect = GUI.Window(INVENTORY_WINDOW_ID, _inventoryWindowRect, InventoryWindow, "Inventory");

        //display the loot window if we are set to.
        if (_displayLootWindow)
            _lootWindowRect = GUI.Window(LOOT_WINDOWS_ID, new Rect(_offset, Screen.height - (_offset + lootWindowHeight), Screen.width - (_offset * 2), lootWindowHeight), LootWindow, "Loot Window");

        //display the main menu window - key Esc
        if (_displayMainMenu)
            _MainMenuWindowRect = GUI.Window(MAINMENU_WINDOWS_ID, _MainMenuWindowRect, MainMenuWindow, "Main Menu");

        //display the tooltip that we have
        DisplayTooltip();
    }


    private void DisplayLoot()
    {
        _displayLootWindow = true;
    }

    private void ClearWindow()
    {
        chest.OnMouseUp(); // on clear window call OnMouseUp function again to close the chest
        chest = null;
        _displayLootWindow = false;
    }

    private void SetToolTip()
    {
        if (Event.current.type == EventType.Repaint && GUI.tooltip != _toolTip)
        {
            if (!string.IsNullOrEmpty(_toolTip))
                _toolTip = string.Empty;
            if (!string.IsNullOrEmpty(GUI.tooltip))
                _toolTip = GUI.tooltip;
        }
    }

    private void DisplayTooltip()
    {
        if (!string.IsNullOrEmpty(_toolTip))
            GUI.Box(new Rect(Screen.width / 2 + 100, Screen.height / 2 - 100 , 150, 100), _toolTip);
    }

    #region Loot Window Content
    //Method used by the loot window, displaying the loot icons inside
    private void LootWindow(int id)
    {
        //add a close button to the window
        if (GUI.Button(new Rect(_lootWindowRect.width - _offset * 2, 0, closeButtonWidth, closeButtonHeight), "x"))
        {
            ClearWindow();
        }

        if (chest == null)
            return;

        if (chest.loot.Count == 0)
        {
            ClearWindow();
            return;
        }
        _lootWindowSlider = GUI.BeginScrollView(new Rect(_offset * .5f, 15, _lootWindowRect.width - _offset, 70), 
            _lootWindowSlider, new Rect(0, 0, (chest.loot.Count * buttonWidth) + _offset, buttonHeight + _offset));

        for (int cnt = 0; cnt < chest.loot.Count; cnt++)
            if (GUI.Button(new Rect(5 + (buttonWidth * cnt), _offset, buttonWidth, buttonHeight), new GUIContent(chest.loot[cnt].Icon, 
                chest.loot[cnt].Tooltip()), commonInventorySlot))
            {

                PlayerChar.Instance.Inventory.Add(chest.loot[cnt]); //add item to player inventory
                chest.loot.RemoveAt(cnt); // remove item from chest
            }


        GUI.EndScrollView();
        SetToolTip();
    }

    #endregion

    #region Inventory Window Content
    //Method used by the inventory Window, generating the buttons inside
    public void InventoryWindow(int id)
    {
        int cnt = 0;
        for (int y = 0; y < _inventoryRows; y++)
        {
            for (int x = 0; x < _invetoryCols; x++)
            {
                if (cnt < PlayerChar.Instance.Inventory.Count)
                {
                    //when user clicks an inventory item(button)
                    if (GUI.Button(new Rect(5 + (x * buttonWidth), 20 + (y * buttonHeight), buttonWidth, buttonHeight), new GUIContent(PlayerChar.Instance.Inventory[cnt].Icon, PlayerChar.Instance.Inventory[cnt].Tooltip()), commonInventorySlot))
                    {
                        //if no item is selected or a single click has occured
                        if (_doubleClickTimer != 0 && _selectedItem != null)
                        {
                            //if current time - doubleclicktime < 0.5 second. Double click occured
                            if (Time.time - _doubleClickTimer < DOUBLECLICK_TIMER)
                            {
                                //Debug.Log("Double Click" + PlayerCharacter.Inventory[cnt].Name); 
                                //if the selected itemtype is weapon and if player don't have a weapon equiped, equip it and remove the icon from the inventory
                                if (typeof(Weapon) == PlayerChar.Instance.Inventory[cnt].GetType())
                                {
                                    GameSetting2.SaveWeapon((Weapon)PlayerChar.Instance.Inventory[cnt]);
                                    GameSetting2.SaveEmptySlot((int)EquipmentSlot.MainHand, false);
                                    EquipItem("EquipedWeapon", cnt);                         
                                }
                                else if (typeof(Armor) == PlayerChar.Instance.Inventory[cnt].GetType())
                                {
                                    switch (((Armor)PlayerChar.Instance.Inventory[cnt]).Slot)
                                    {
                                        case EquipmentSlot.Helmet:
                                            GameSetting2.SaveArmor((Armor)PlayerChar.Instance.Inventory[cnt],(int)EquipmentSlot.Helmet);
                                            GameSetting2.SaveEmptySlot((int)EquipmentSlot.Helmet, false);
                                            EquipItem("EquipedHelmet", cnt);
                                            break;
                                        case EquipmentSlot.OffHand:
                                            GameSetting2.SaveArmor((Armor)PlayerChar.Instance.Inventory[cnt],(int)EquipmentSlot.OffHand);
                                            GameSetting2.SaveEmptySlot((int)EquipmentSlot.OffHand,false);
                                            EquipItem("EquipedShield", cnt);
                                            break;
                                        case EquipmentSlot.Body:
                                            GameSetting2.SaveArmor((Armor)PlayerChar.Instance.Inventory[cnt], (int)EquipmentSlot.Body);
                                            GameSetting2.SaveEmptySlot((int)EquipmentSlot.Body,false);
                                            EquipItem("EquipedBody", cnt);
                                            break;
                                        case EquipmentSlot.Boots:
                                            GameSetting2.SaveArmor((Armor)PlayerChar.Instance.Inventory[cnt], (int)EquipmentSlot.Boots);
                                            GameSetting2.SaveEmptySlot((int)EquipmentSlot.Boots,false);
                                            EquipItem("EquipedBoots", cnt);
                                            break;
                                        case EquipmentSlot.Hands:
                                            GameSetting2.SaveArmor((Armor)PlayerChar.Instance.Inventory[cnt], (int)EquipmentSlot.Hands);
                                            GameSetting2.SaveEmptySlot((int)EquipmentSlot.Hands,false);
                                            EquipItem("EquipedGloves", cnt);
                                            break;
                                        case EquipmentSlot.Legs:
                                            GameSetting2.SaveArmor((Armor)PlayerChar.Instance.Inventory[cnt], (int) EquipmentSlot.Legs);
                                            GameSetting2.SaveEmptySlot((int)EquipmentSlot.Legs,false);
                                            EquipItem("EquipedPants", cnt);
                                            break;
                                        default:
                                            Debug.LogWarning("No defined equipment slot");
                                            break;
                                    }
                                }
                                else if (typeof(Potion) == PlayerChar.Instance.Inventory[cnt].GetType())
                                {
                                    ((Potion)PlayerChar.Instance.Inventory[cnt]).UsePotion();
                                    PlayerChar.Instance.Inventory.RemoveAt(cnt);
                                    PlayerChar.Instance.GetComponent<PlayerGUI>().healPotionExists = false; //for player gui script
                                }
                                //reset selected item and timer.when doubleclicktimer is "0" it needs to get assinged on first first click
                                _doubleClickTimer = 0;
                                _selectedItem = null;
                            }
                            //if the difference is greater than 0.5 seconds
                            else
                            {
                                // Debug.Log("Reset Double Click Timer");
                                _doubleClickTimer = Time.time;
                            }
                        }
                        //If no click has occured, set the timer and the selected item on first button click
                        else
                        {
                            _doubleClickTimer = Time.time;
                            _selectedItem = PlayerChar.Instance.Inventory[cnt];
                        }
                    }
                }
                //display the empy inventory slots
                else
                {
                    GUI.Button(new Rect(5 + (x * buttonWidth), 20 + (y * buttonHeight), buttonWidth, buttonHeight), (x + y * _invetoryCols).ToString(), emptyInventorySlot);
                }
                cnt++;
            }
        }
        //display gold
        GUI.Label(new Rect(102, 260, 30, 30), new GUIContent(Resources.Load(GameSetting2.HUD_ICON_PATH + "coin") as Texture2D));
        GUI.Label(new Rect(180 - 45, 263, 40, 30), PlayerChar.Instance.Gold.ToString());
        GUI.DragWindow();
        SetToolTip();
    }

    private void EquipItem(string sItem, int cnt)
    {
        //Remove the stats from the players current equiped weapon
        PlayerChar.Instance.RemoveBuffEquipedStats(GetPropValue(sItem));

        //Add the selected wepaons stats
        PlayerChar.Instance.AddBuffEquipedStats(PlayerChar.Instance.Inventory[cnt]);

        if (GetPropValue(sItem) == null)
        {
            SetPropertyValue(sItem, PlayerChar.Instance.Inventory[cnt]);
            PlayerChar.Instance.Inventory.RemoveAt(cnt);
        }
        else //if player is already equiped with a weapon, swap them
        {
            Item temp = GetPropValue(sItem);
            SetPropertyValue(sItem, PlayerChar.Instance.Inventory[cnt]);
            PlayerChar.Instance.Inventory[cnt] = temp;
        }
    }

    //Method to get property value by reflection
    private Item GetPropValue(string propName)
    {
        return (Item)PlayerChar.Instance.GetType().GetProperty(propName).GetValue(PlayerChar.Instance, null);
    }

    //Method to set property value by reflection
    private void SetPropertyValue(string propName, Item value)
    {
        PlayerChar.Instance.GetType().GetProperty(propName).SetValue(PlayerChar.Instance, value, null);
    }

    #endregion

    #region Character Window Content
    //Method used by the character window
    public void CharacterWindow(int id)
    {
        _characterPanel = GUI.Toolbar(new Rect(5, 25, _characterWindowRect.width - 10, 50), _characterPanel, _characterPanelNames);

        switch (_characterPanel)
        {
            case 0:
                DisplayEquipment();
                break;
            case 1:
                DisplayAttributes();
                break;
        }
        GUI.DragWindow();
    }

    //Method to un-equip player items when he double clicks in the ui character window
    //Gui Buttons for Equipment
    private void DisplayEquipment()
    {
        // Debug.Log("Displaying Equipment");
        //check if it is empty first
        if (PlayerChar.Instance.EquipedWeapon == null)
        {
            GUI.Label(new Rect(70, 290, 40, 40), "", "Empty MainHand Slot");
        }
        else
        {
            //if the user presses the weapon button on the char window, type item display it's icon
            if (GUI.Button(new Rect(50, 290, 40, 40), new GUIContent(PlayerChar.Instance.EquipedWeapon.Icon, PlayerChar.Instance.EquipedWeapon.Tooltip())))
            {
                PlayerChar.Instance.UnEquipItem(PlayerChar.Instance.EquipedWeapon);
                PlayerChar.Instance.EquipedWeapon = null;
                GameSetting2.SaveEmptySlot((int)EquipmentSlot.MainHand,true);
            }
        }

        if (PlayerChar.Instance.EquipedShield == null)
        {
            GUI.Label(new Rect(120, 290, 40, 40), "", "Empty OffHand Slot");
        }
        else
        {
            if (GUI.Button(new Rect(100, 290, 40, 40), new GUIContent(PlayerChar.Instance.EquipedShield.Icon, PlayerChar.Instance.EquipedShield.Tooltip())))
            {
                PlayerChar.Instance.UnEquipItem(PlayerChar.Instance.EquipedShield);
                PlayerChar.Instance.EquipedShield = null;
                GameSetting2.SaveEmptySlot((int)EquipmentSlot.OffHand, true);
            }
        }

        if (PlayerChar.Instance.EquipedHelmet == null)
        {
            GUI.Label(new Rect(5, 100, 40, 40), "", "Empty Head Slot");
        }
        else
            if (GUI.Button(new Rect(5, 100, 40, 40), new GUIContent(PlayerChar.Instance.EquipedHelmet.Icon, PlayerChar.Instance.EquipedHelmet.Tooltip())))
            {
                PlayerChar.Instance.UnEquipItem(PlayerChar.Instance.EquipedHelmet);
                PlayerChar.Instance.EquipedHelmet = null;
                GameSetting2.SaveEmptySlot((int)EquipmentSlot.Helmet, true);
            }

        if (PlayerChar.Instance.EquipedBody == null)
        {
            GUI.Label(new Rect(5, 150, 40, 40), "", "Empty Chest Slot");
        }
        else
            if (GUI.Button(new Rect(5, 150, 40, 40), new GUIContent(PlayerChar.Instance.EquipedBody.Icon, PlayerChar.Instance.EquipedBody.Tooltip())))
            {
                PlayerChar.Instance.UnEquipItem(PlayerChar.Instance.EquipedBody);
                PlayerChar.Instance.EquipedBody = null;
                GameSetting2.SaveEmptySlot((int)EquipmentSlot.Body, true);
            }

        if (PlayerChar.Instance.EquipedBoots == null)
        {
            GUI.Label(new Rect(5, 200, 40, 40), "", "Empty Feet Slot");
        }
        else
            if (GUI.Button(new Rect(5, 200, 40, 40), new GUIContent(PlayerChar.Instance.EquipedBoots.Icon, PlayerChar.Instance.EquipedBoots.Tooltip())))
            {
                PlayerChar.Instance.UnEquipItem(PlayerChar.Instance.EquipedBoots);
                PlayerChar.Instance.EquipedBoots = null;
                GameSetting2.SaveEmptySlot((int)EquipmentSlot.Boots, true);
            }

        if (PlayerChar.Instance.EquipedGloves == null)
        {
            GUI.Label(new Rect(200, 125, 40, 40), "", "Empty Hands Slot");
        }
        else
            if (GUI.Button(new Rect(200, 125, 40, 40), new GUIContent(PlayerChar.Instance.EquipedGloves.Icon, PlayerChar.Instance.EquipedGloves.Tooltip())))
            {
                PlayerChar.Instance.UnEquipItem(PlayerChar.Instance.EquipedGloves);
                PlayerChar.Instance.EquipedGloves = null;
                GameSetting2.SaveEmptySlot((int)EquipmentSlot.Hands, true);
            }

        if (PlayerChar.Instance.EquipedPants == null)
        {
            GUI.Label(new Rect(200, 175, 40, 40), "", "Empty Legs Slot");
        }
        else
            if (GUI.Button(new Rect(200, 175, 40, 40), new GUIContent(PlayerChar.Instance.EquipedPants.Icon, PlayerChar.Instance.EquipedPants.Tooltip())))
            {
                PlayerChar.Instance.UnEquipItem(PlayerChar.Instance.EquipedPants);
                PlayerChar.Instance.EquipedPants = null;
                GameSetting2.SaveEmptySlot((int)EquipmentSlot.Legs, true);
            }

        const int lineHeight = 16;
        const int valuedisplaywidth = 50;

        //Display skills
        for (int i = 0; i < PlayerChar.Instance.skill.Length; i++)
        {
            GUI.Label(new Rect(45, i * lineHeight + 100, _characterWindowRect.width - (_offset * 2) - valuedisplaywidth - 5, 25), ((SkillName)i).ToString());
            GUI.Label(new Rect(_characterWindowRect.width - (_offset * 2) - valuedisplaywidth, i * lineHeight + 100, valuedisplaywidth, 25), (PlayerChar.Instance.GetSkill(i).AdjustedBaseValue.ToString()));
        }

        SetToolTip();
    }

    private void DisplayAttributes()
    {
        const int lineHeight = 16;
        const int valuedisplaywidth = 60;
        GUI.BeginGroup(new Rect(5, 73, _characterWindowRect.width - (_offset * 2), _characterWindowRect.height - 70));

        //Display attributes
        for (int i = 0; i < PlayerChar.Instance.primaryattribute.Length - 1; i++)
        {
            GUI.Label(new Rect(0, i * lineHeight, _characterWindowRect.width - (_offset * 2) - valuedisplaywidth - 5, 25), ((AttributeName)i).ToString());
            GUI.Label(new Rect(_characterWindowRect.width - (_offset * 2) - valuedisplaywidth, i * lineHeight, valuedisplaywidth, 25), (PlayerChar.Instance.GetPrimaryAttribute(i).BaseValue.ToString()));
        }

        //Display vitals
        for (int i = 0; i < PlayerChar.Instance.vital.Length; i++)
        {
            GUI.Label(new Rect(0, (i + PlayerChar.Instance.primaryattribute.Length) * lineHeight, _characterWindowRect.width - (_offset * 2) - valuedisplaywidth - 5, 25), ((VitalName)i).ToString());
            GUI.Label(new Rect(_characterWindowRect.width - (_offset * 2) - valuedisplaywidth, (i + PlayerChar.Instance.primaryattribute.Length) * lineHeight, valuedisplaywidth, 25), (PlayerChar.Instance.GetVital(i).CurValue + "/" + PlayerChar.Instance.GetVital(i).AdjustedBaseValue));
        }
        GUI.EndGroup();
    }


    #endregion

    #region Main Menu Window Content
    public void MainMenuWindow(int id)
    {
        GUI.Button(new Rect(_MainMenuWindowRect.width / 2 - 50, 50, 100, 30), "Options");
        if (GUI.Button(new Rect(_MainMenuWindowRect.width / 2 - 50, 90, 100, 30), "Enter Arena"))
        {
            Application.LoadLevel(GameSetting2.levelNames[6]);
        }
        if (GUI.Button(new Rect(_MainMenuWindowRect.width / 2 - 50, 130, 100, 30), "Exit Game"))
        {
            GameSetting2.SaveCharacterPosition(GameObject.FindGameObjectWithTag("Player").transform.position);
            Application.LoadLevel(GameSetting2.levelNames[0]);
        }
    }
    #endregion

    #region Toggle Windows Display
    //Those methods used by PlayerInput and they use the Messenger
    //Method to toggle inventory window on-off. "i" key
    public void ToggleInventoryWindow()
    {
        displayInventoryWindow = !displayInventoryWindow;
    }

    public void ToggleCharacterWindow()
    {
        displayCharacterWidnow = !displayCharacterWidnow;
    }

    public void ToggleMainMenu()
    {
        _displayMainMenu = !_displayMainMenu;
    }
    #endregion

}
