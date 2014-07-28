using UnityEngine;
using System;
using System.Collections;

public class PlayerGUI : MonoBehaviour
{
    private Mob _enemytarget;
    private float _healthwidth;
    private float _manawidth;
    private float _expwidth;
    private float _enemyhealthhwidth;
    private Item _potionSelected;
    private float timeCounter;

    private static ErrorText _pointerror;

    //Player Skills Timers
    private float _nextFireball;
    private float _nextLighting;
    private float _nextHealing;

    //Player Skills Cooldowns In Seconds
    private const float _fireballCD = 2;
    private const float _lightningCD = 5;
    private const float _healingCD = 3;

    public bool deadgui { get; set; }
    public Texture charPortrait;
    public Texture playerAvantarBar;
    public Texture playerExpBar;
    public Texture enemyAvantarBar;
    public Texture healthImg;
    public Texture manaImg;
    public Texture2D[] numbers;
    public GUISkin mySkin;

    public bool healPotionExists { get; set; }

    void Start()
    {
        _pointerror = GameObject.Find("PointError").GetComponent<ErrorText>();
    }
    void Update()
    {
        if (!healPotionExists)
        {
            //Check players inventory for heal potions and update variables for GUI display
            foreach (Item t in PlayerChar.Instance.Inventory)
            {
                if (t.Name == "Super Healing Potion" || t.Name == "Greater Healing Potion" ||
                    t.Name == "Lesser Healing Potion")
                {
                    healPotionExists = true;
                    _potionSelected = t; //save the item temporaly to remove it or use it later
                }
            }
        }

        if (PlayerChar.Instance.MeleeResetTimer > 0)
            PlayerChar.Instance.MeleeResetTimer -= Time.deltaTime;
        //Regen Player Hp every some seconds
        RegenPlayerHp();
    }

    void OnGUI()
    {
        GUI.skin = mySkin;
        GUI.DrawTexture(new Rect(Screen.width * 0.45f - 180f, Screen.height * 0.8f - 4, 280, 60), Resources.Load(GameSetting2.HUD_ICON_PATH + "gamehudtest") as Texture2D);
        GUI.DrawTexture(new Rect(25, 20, 75, 75), charPortrait, ScaleMode.StretchToFill);
        GUI.DrawTexture(new Rect(20, 20, 224, 90), playerAvantarBar, ScaleMode.StretchToFill);

        //Player Hud  - Healthbar - Manabar
        _healthwidth = PlayerChar.Instance.GetVital((int)VitalName.Health).CurValue * 120 / PlayerChar.Instance.GetVital((int)VitalName.Health).AdjustedBaseValue;
        _manawidth = PlayerChar.Instance.GetVital((int)VitalName.Mana).CurValue * 120 / PlayerChar.Instance.GetVital((int)VitalName.Mana).AdjustedBaseValue;
        GUI.DrawTexture(new Rect(103, 33, _healthwidth, 8), healthImg, ScaleMode.StretchToFill);
        GUI.DrawTexture(new Rect(103, 47, _manawidth, 8), manaImg, ScaleMode.StretchToFill);

        GUI.Label(new Rect(130, 25, 75, 40), PlayerChar.Instance.GetVital((int)VitalName.Health).CurValue + "/" + PlayerChar.Instance.GetVital((int)VitalName.Health).AdjustedBaseValue);
        GUI.Label(new Rect(130, 40, 75, 40), PlayerChar.Instance.GetVital((int)VitalName.Mana).CurValue + "/" + PlayerChar.Instance.GetVital((int)VitalName.Mana).AdjustedBaseValue);

        //draw mob HUD- if there is a target
        if (PlayerChar.Instance.selectedTarget != null)
        {
            GUI.DrawTexture(new Rect(300, 20, 224, 90), enemyAvantarBar, ScaleMode.StretchToFill);
            //get the Mob script component from our selected target.
            _enemytarget = PlayerChar.Instance.selectedTarget.GetComponent<Mob>();

            _enemyhealthhwidth = _enemytarget.GetVital((int)VitalName.Health).CurValue * 120 / _enemytarget.GetVital((int)VitalName.Health).AdjustedBaseValue;
            //draw mobs hp bar
            GUI.DrawTexture(new Rect(320, 33, _enemyhealthhwidth, 8), healthImg, ScaleMode.StretchToFill);

            //display the mob hp
            GUI.Label(new Rect(350, 25, 75, 40), _enemytarget.GetVital((int)(VitalName.Health)).CurValue + "/" + _enemytarget.GetVital((int)(VitalName.Health)).AdjustedBaseValue);
            //display % 
            GUI.Label(new Rect(360, 40, 75, 40), Mathf.Round((float)(_enemytarget.GetVital((int)(VitalName.Health)).CurValue) / (_enemytarget.GetVital((int)(VitalName.Health)).AdjustedBaseValue) * 100) + "%"); ///_enemytarget.GetVital((int)(VitalName.Health)).AdjustedBaseValue) * 100) +"%");
        }

        if (GUI.Button(new Rect(Screen.width * 0.93f, Screen.height * 0.85f, 50, 50), new GUIContent(Resources.Load(GameSetting2.HUD_ICON_PATH + "Backpack") as Texture2D)))
            myGUI.displayInventoryWindow = !myGUI.displayInventoryWindow;

        if (GUI.Button(new Rect(Screen.width * 0.93f - 55, Screen.height * 0.85f, 50, 50), new GUIContent(Resources.Load(GameSetting2.HUD_ICON_PATH + "character") as Texture2D)))
            myGUI.displayCharacterWidnow = !myGUI.displayCharacterWidnow;

        if (GUI.Button(new Rect(Screen.width * 0.93f - 110, Screen.height * 0.85f, 50, 50), new GUIContent(Resources.Load(GameSetting2.HUD_ICON_PATH + "Map") as Texture2D)))
        {
            GameObject.Find("MapCamera").camera.enabled = !GameObject.Find("MapCamera").camera.enabled;
            GameObject.Find("MapCamera").GetComponent<CameraFollow>().enabled = !GameObject.Find("MapCamera").GetComponent<CameraFollow>().enabled;
        }
        if (!deadgui)
        {
            if (PlayerChar.Instance.MeleeResetTimer <= 0)
            {
                if (GUI.Button(new Rect(Screen.width*0.45f - 175f, Screen.height*0.8f, 50, 50), "", "Attack Button") ||
                    Input.GetKey(KeyCode.Alpha1))
                    PlayerChar.Instance.PlayerAttack();
            }
            else
            {
                GUI.Label(new Rect(Screen.width*0.45f - 175f, Screen.height*0.8f, 50, 50), "", "Hourglass Button");
            }

            if (Time.time > _nextFireball)
            {
                // To check the spell cooldown, compare nextfireball taht is time+2 with current game time.
                //then display fireball button
                if (
                    GUI.Button(new Rect(Screen.width*0.45f - 120f, Screen.height*0.8f, 50, 50), "", "Fireball Button") ||
                    Input.GetKey(KeyCode.Alpha2))
                {
                    if (PlayerChar.Instance.GetVital((int) VitalName.Mana).CurValue >= 170)
                    {
                        _nextFireball = Time.time + _fireballCD;
                        PlayerChar.Instance.CastSpellNoTarget("Fireball");
                    }
                    else
                    {
                        _pointerror.DisplayerrorText("I am out of mana!");
                    }
                }
            }
            else //display hourglass
            {
                GUI.Label(new Rect(Screen.width*0.45f - 120f, Screen.height*0.8f, 50, 50), "", "Hourglass Button");
            }

            if (Time.time > _nextLighting)
            {
                if (GUI.Button(new Rect(Screen.width*0.45f - 65f, Screen.height*0.8f, 50, 50), "", "Lighting Button") ||
                    Input.GetKey(KeyCode.Alpha3))
                {
                    if (PlayerChar.Instance.GetVital((int) VitalName.Mana).CurValue >= 60)
                    {
                        //if player has target
                        if (PlayerChar.Instance.selectedTarget != null)
                        {
                            float distance = Vector3.Distance(PlayerChar.Instance.transform.position,
                                                              PlayerChar.Instance.selectedTarget.position);
                            //distance between mob and player
                            if (distance < GameSetting2.BASE_MAGIC_RANGE)
                            {
                                Vector3 direction =
                                    (PlayerChar.Instance.selectedTarget.position -
                                     PlayerChar.Instance.transform.position).
                                        normalized;
                                if (Vector3.Dot(direction, PlayerChar.Instance.transform.forward) > 0.85f)
                                    // check the direction, 1 when they are parallel
                                {
                                    _nextLighting = Time.time + _lightningCD;
                                    PlayerChar.Instance.CastSpellWithTarget("Lightning",
                                                                            PlayerChar.Instance.selectedTarget);
                                }
                            }
                            else
                            {
                                _pointerror.DisplayerrorText("Out of range!");
                            }
                        }
                        else
                        {
                            _pointerror.DisplayerrorText("I need a target");
                        }
                    }
                    else
                    {
                        _pointerror.DisplayerrorText("Out of range");
                    }
                }
            }
            else //display hourglass
            {
                GUI.Label(new Rect(Screen.width*0.45f - 65f, Screen.height*0.8f, 50, 50), "", "Hourglass Button");
            }

            if (Time.time > _nextHealing)
            {
                if (GUI.Button(new Rect(Screen.width*0.45f - 10, Screen.height*0.8f, 50, 50), "", "Healing Button") ||
                    Input.GetKey(KeyCode.Alpha4))
                {
                    if (PlayerChar.Instance.GetVital((int) VitalName.Mana).CurValue >= 320)
                    {
                        _nextHealing = Time.time + _healingCD;
                        PlayerChar.Instance.CastSpellNoTarget("Heal");
                    }
                    else
                    {
                        _pointerror.DisplayerrorText("I am out of mana!");
                    }
                }
            }
            else //display hourglass
            {
                GUI.Label(new Rect(Screen.width*0.45f - 10, Screen.height*0.8f, 50, 50), "", "Hourglass Button");
            }

            //Heal potion Buttons
            if (healPotionExists)
            {
                if (_potionSelected.Name == "Super Healing Potion")
                {
                    if (
                        GUI.Button(new Rect(Screen.width*0.45f + 47, Screen.height*0.8f, 50, 50), "", "SuperHPPotion") ||
                        Input.GetKey(KeyCode.Alpha5))
                    {
                        UsePotion();
                    }
                }
                else if (_potionSelected.Name == "Greater Healing Potion")
                {
                    if (
                        GUI.Button(new Rect(Screen.width*0.45f + 47, Screen.height*0.8f, 50, 50), "", "GreaterHPPotion") ||
                        Input.GetKey(KeyCode.Alpha5))
                    {
                        UsePotion();
                    }
                }
                else if (_potionSelected.Name == "Lesser Healing Potion")
                {
                    if (
                        GUI.Button(new Rect(Screen.width*0.45f + 47, Screen.height*0.8f, 50, 50), "", "LesserHPPotion") ||
                        Input.GetKey(KeyCode.Alpha5))
                    {
                        UsePotion();
                    }
                }
            }
            else
            {
                GUI.Label(new Rect(Screen.width*0.45f + 48, Screen.height*0.8f + 5, 45, 45), "", "EmptyButton");
            }
        }

        GUI.Label(new Rect(50, 100, 75, 75), PlayerChar.Instance.name); //display player name
        DisplayPlayerLevel(); //display level
        _expwidth = PlayerChar.Instance.CurrentExp * 120 / PlayerChar.Instance.ExpToNextLevel;
        GUI.DrawTexture(new Rect(110, 70, _expwidth, 5), playerExpBar, ScaleMode.StretchToFill); //display exp bar

        DisplayDeadGui();
    }

    private void UsePotion()
    {
        ((Potion)_potionSelected).UsePotion(); //use potion
        PlayerChar.Instance.Inventory.Remove(_potionSelected); //remove it from player invectory
        healPotionExists = false;
    }
    private void DisplayDeadGui()
    {
        //if player is dead display a respawn button
        if (deadgui)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 2, 150, 50), "To nearest village"))
            {
                deadgui = false; //hide the death gui
                SendMessage("PlayerRespawn");
            }
        }
    }

    private void DisplayPlayerLevel()
    {
        //find of how many digits, level consists
        // Math.Floor(Math.Log10(PlayerChar.Instance.Level) + 1);
        //Pass the number digits to an array
        Int32 I = Convert.ToChar(PlayerChar.Instance.Level);
        var chars = I.ToString().ToCharArray();

        if (PlayerChar.Instance.Level < 10)
            GUI.DrawTexture(new Rect(8, 15, 20, 20), numbers[PlayerChar.Instance.Level], ScaleMode.StretchToFill);
        else if (PlayerChar.Instance.Level < 99)
        {
            GUI.DrawTexture(new Rect(8, 15, 20, 20), numbers[int.Parse(chars[0].ToString())], ScaleMode.StretchToFill);
            GUI.DrawTexture(new Rect(20, 15, 20, 20), numbers[int.Parse(chars[1].ToString())], ScaleMode.StretchToFill);
        }
    }

    private void RegenPlayerHp()
    {
        //check if player is in combat
        if (!PlayerChar.Instance.InCombat)
        {
            timeCounter += Time.deltaTime; //calculate the time passing in seconds
            //Debug.Log(timeCounter);

            if (timeCounter > 3) //Regening 2 player's hp and 5 mana every 3 seconds
            {
                if (PlayerChar.Instance.GetVital((int)VitalName.Health).CurValue < PlayerChar.Instance.GetVital((int)VitalName.Health).AdjustedBaseValue)
                    PlayerChar.Instance.GetVital((int)VitalName.Health).CurValue += 10;

                if (PlayerChar.Instance.GetVital((int)VitalName.Mana).CurValue < PlayerChar.Instance.GetVital((int)VitalName.Mana).AdjustedBaseValue)
                    PlayerChar.Instance.GetVital((int)VitalName.Mana).CurValue += 30;

                timeCounter = 0; //reset the counter
            }
        }
    }

}
