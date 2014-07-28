using UnityEngine;
using System.Collections;
using System;

public static class GameSetting2
{
    public const string VERSION_NAME = "VER";
    public const float VERSION = 1;

    #region Attack Modifiers

    //Base values for different Attacks
    public const float BASE_MELEE_ATTACK_TIMER = 0.7f;
    public const float BASE_MELEE_ATTACK_SPEED = 0.7f;

    //Base Ranges for attacks
    public const float BASE_MELEE_RANGE = 3.5f;
    public const float BASE_RANGED_RANGE = 15f;
    public const float BASE_MAGIC_RANGE = 10f;

    #endregion

    #region Player Preferences Constants

    //Strings that will be used to save values
    private const string NAME = "Player Name";
    private const string CHARACTER_GENDER = "Character Gender";
    private const string CHARACTER_MODEL_INDEX = "Character Model Index";
    private const string PLAYER_HEAD_INDEX = "Character Head Index";
    private const string SKIN_COLOR = "Character Skin Color";
    private const string HAIR_COLOR = "Character Hair Color";
    private const string HAIR_MESH = "Character Hair Mesh";
    private const string CHARACTER_HEIGHT = "Character Height";
    private const string CHARACTER_POSITION = "Character Position";
    private const string BASE_VALUE = " - BASE VALUE";
    private const string CUR_VALUE = " - Cur Value";
    private const string PLAYER_LEVEL = "Player Level";
    private const string EXP_TO_LEVEL = "Exp To Next Level";
    private const string CUR_EXP = "Current Level Exp";
    private const string PLAYER_GOLD = "Player Gold";

    private const string ITEM_NAME = "Item Name";
    private const string ITEM_VALUE = "Item Value";
    private const string ITEM_CUR_DUR = "Item Current Dur";
    private const string ITEM_MAX_DUR = "Item Max Dur";
    private const string ITEM_RARE = "Item Rarity";

    private const string WEAPON_MAX_DMG = "Weapon Max Damage";
    private const string WEAPON_DMGVAR = "Weapon Damage Var";
    private const string WEAPON_TYPE = "Weapon Type";

    private const string ARMOR_LEVEL = "Armor Level";
    private const string ARMOR_SLOT = "Armor Slot";

    private const string SLOT_EMPTY = "Slot Empty";
    #endregion

    #region Resource File Paths

    public const string MALE_MODEL_PATH = "Prefabs/Character Models Old/Male/"; //male models path in Resources file
    public const string FEMALE_MODEL_PATH = "Prefabs/Character Models Old/Female/"; //male models path in Resources file

    public const string MALE_TEXTURE_PATH = "Characters/Male/Human/Textures/";

    public const string WEAPON_ICON_PATH = "GUI Icons/Items/Weapons/";
    public const string WEAPON_MESH_PATH = "Prefabs/Items/Weapons/";

    public const string SHIELD_ICON_PATH = "GUI Icons/Items/Armor/Shields/";
    public const string SHIELD_MESH_PATH = "Prefabs/Items/Armor/Shields/";

    public const string ARMOR_ICON_PATH = "GUI Icons/Items/Armor/";
    public const string ARMOR_MESH_PATH = "Prefabs/Items/Armor/";

    public const string ARMOR_TEXTURE_PATH = "Textures/Character Armor Textures/";

    public const string HUD_ICON_PATH = "GUI Icons/HUD/";

    public const string MISC_ICON_PATH = "GUI Icons/Items/Misc/";
    public const string MISC_MESH_PATH = "Prefabs/Items/Misc/";

    public const string EFFECTS_PATH = "Prefabs/Effects/";
    public const string SPELLS_PATH = "Prefabs/Spells/";

    #endregion

    public static Vector3 charStartingPoint = new Vector3(74, 100, 456);
    public static string[] maleModels = { "Muscular", "Fat" }; //string array containing the names of our male models
    public static string[] femaleModels = { "Hot Cube", "Tall Chick" };

    //Game Levels
    //index 0 = mainmenu
    //index 1 = character creation screen
    //index 2 = character customization screen
    //index 3 = starting area level
    //index 4 = arena
    //index 5 = loading starting area
    public static string[] levelNames = {
		"Main Menu",
		"Character Generation",
		"Character Customization",
		"Starting Area",
        "Arena",
        "Loading Starting Area",
        "Loading Arena"
	};

    /// <summary>
    /// Default Constructor
    /// </summary>
    static GameSetting2()
    {
    }

    #region Save-Load Character Name
    public static void SaveName(string name)
    {
        PlayerPrefs.SetString(NAME, name);
    }

    public static string LoadName()
    {
        return PlayerPrefs.GetString(NAME, "Guest" + UnityEngine.Random.Range(1, 1000));
    }
    #endregion

    #region Save-Load Character Gender
    /// <summary>
    /// Saves "1" is the character is male, else saves "0" for female
    /// </summary>
    /// <param name="index"></param>
    public static void SaveGender(bool index)
    {
        int ismale = 1;
        if (!index)
            ismale = 0;
        PlayerPrefs.SetInt(CHARACTER_GENDER, ismale);

    }

    public static int LoadGender()
    {
        return PlayerPrefs.GetInt(CHARACTER_GENDER, 1);
    }
    #endregion

    #region Save-Load Character Model Index
    public static void SaveCharacterModelIndex(int index)
    {
        PlayerPrefs.SetInt(CHARACTER_MODEL_INDEX, index);
    }

    public static int LoadCharacterModelIndex()
    {
        return PlayerPrefs.GetInt(CHARACTER_MODEL_INDEX, 0);
    }

    #endregion

    #region Save-Load Character Head
    public static void SaveHead(int index)
    {
        PlayerPrefs.SetInt(PLAYER_HEAD_INDEX, index);
    }

    public static int LoadHead()
    {
        return PlayerPrefs.GetInt(PLAYER_HEAD_INDEX, 1);
    }
    #endregion

    #region Save-Load Character Hair
    public static void SaveHair(int mesh, int color)
    {
        SaveHairMesh(mesh);
        SaveHairColor(color);
    }

    /// <summary>
    /// Store the index of the hair color as an int
    /// </summary>
    /// <param name="index"></param>
    public static void SaveHairColor(int index)
    {
        PlayerPrefs.SetInt(HAIR_COLOR, index);
    }

    /// <summary>
    /// Loads the players selected index for the hair color they have selected
    /// </summary>
    /// <returns></returns>
    public static int LoadHairColor()
    {
        return PlayerPrefs.GetInt(HAIR_COLOR, 0);
    }

    /// <summary>
    /// Save the selected index for the hair mesh as an int
    /// </summary>
    /// <param name="index"></param>
    public static void SaveHairMesh(int index)
    {
        PlayerPrefs.SetInt(HAIR_MESH, index);
    }


    public static int LoadHairMesh()
    {
        return PlayerPrefs.GetInt(HAIR_MESH, 0);
    }



    #endregion

    #region Save-Load Character Skin Color

    public static void SaveSkinColor(int index)
    {
        PlayerPrefs.SetInt(SKIN_COLOR, index);
    }

    public static int LoadSkinColor()
    {
        return PlayerPrefs.GetInt(SKIN_COLOR, 0);
    }
    #endregion

    #region Save-Load Character Height

    public static void SaveCharacterHeight(float height)
    {
        PlayerPrefs.SetFloat(CHARACTER_HEIGHT, height);
    }

    public static float LoadCharacterHeight()
    {
        return PlayerPrefs.GetFloat(CHARACTER_HEIGHT, 1);
    }
    #endregion

    #region Save-Load Character Position

    public static void SaveCharacterPosition(Vector3 pos)
    {
        PlayerPrefs.SetFloat(CHARACTER_POSITION + "X", pos.x);
        PlayerPrefs.SetFloat(CHARACTER_POSITION + "Y", pos.y);
        PlayerPrefs.SetFloat(CHARACTER_POSITION + "Z", pos.z);
    }

    public static Vector3 LoadCharacterPosition()
    {
        Vector3 vec = new Vector3(PlayerPrefs.GetFloat(CHARACTER_POSITION + "X", charStartingPoint.x),
            PlayerPrefs.GetFloat(CHARACTER_POSITION + "Y", charStartingPoint.y),
            PlayerPrefs.GetFloat(CHARACTER_POSITION + "Z", charStartingPoint.z));
        return vec;
    }
    #endregion

    #region Save-Load Level
    public static void SaveCharacterLevel(int level)
    {
        PlayerPrefs.SetInt(PLAYER_LEVEL, level);
    }

    public static void SaveCharacterExp(int curexp, int exptolvl)
    {
        PlayerPrefs.SetInt(EXP_TO_LEVEL, exptolvl);
        PlayerPrefs.SetInt(CUR_EXP, curexp);
    }

    public static void LoadCharacterLevel()
    {
        PlayerChar.Instance.Level = PlayerPrefs.GetInt(PLAYER_LEVEL, 1);
        PlayerChar.Instance.CurrentExp = PlayerPrefs.GetInt(CUR_EXP, 0);
        PlayerChar.Instance.ExpToNextLevel = PlayerPrefs.GetInt(EXP_TO_LEVEL, 2000);
    }
    #endregion

    #region Save-Load Attributes

    public static void SaveAttributes(Attribute[] attribute)
    {
        for (int i = 0; i < attribute.Length; i++)
            SaveAttribute((AttributeName)i, attribute[i]);
    }

    public static void LoadAttributes()
    {
        for (int i = 0; i < Enum.GetValues(typeof(AttributeName)).Length; i++)
            LoadAttribute((AttributeName)i);
    }

    public static void SaveAttribute(AttributeName name, Attribute attribute)
    {
        PlayerPrefs.SetInt(name + BASE_VALUE, attribute.BaseValue);
    }

    public static void LoadAttribute(AttributeName name)
    {
        PlayerChar.Instance.GetPrimaryAttribute((int)name).BaseValue = PlayerPrefs.GetInt(name + BASE_VALUE, 0);
    }

    #endregion

    #region Save-Load Vitals

    public static void SaveVitals(Vital[] vital)
    {
        for (int i = 0; i < vital.Length; i++)
        {
            SaveVital((VitalName)i, vital[i]);
        }
    }

    public static void LoadVitals()
    {
        for (int i = 0; i < Enum.GetValues(typeof(VitalName)).Length; i++)
        {
            LoadVital((VitalName)i);
            //   Debug.Log(PlayerChar.Instance.vital[i].CurValue);
        }
    }

    public static void SaveVital(VitalName name, Vital vital)
    {
        PlayerPrefs.SetInt(name + BASE_VALUE, vital.BaseValue);
        PlayerPrefs.SetInt(name + CUR_VALUE, vital.CurValue);
    }

    public static void LoadVital(VitalName name)
    {
        PlayerChar.Instance.GetVital((int)name).BaseValue = PlayerPrefs.GetInt(name + BASE_VALUE, 1000);

        ////make sure you call this so that the AjustedBaseValue will be updated before you try to call to get the curValue
        PlayerChar.Instance.GetVital((int)name).Update();

        PlayerChar.Instance.GetVital((int)name).CurValue = PlayerPrefs.GetInt(name + CUR_VALUE, 1);
        PlayerChar.Instance.GetVital((int)name);
    }

    #endregion

    #region Save-Load Skills

    public static void SaveSkills(Skill[] skill)
    {
        for (int i = 0; i < skill.Length; i++)
            SaveSkill((SkillName)i, skill[i]);
    }

    public static void LoadSkills()
    {
        for (int i = 0; i < Enum.GetValues(typeof(SkillName)).Length; i++)
            LoadSkill((SkillName)i);
    }

    public static void SaveSkill(SkillName name, Skill skill)
    {
        PlayerPrefs.SetInt(name + BASE_VALUE, skill.BaseValue);
    }

    public static void LoadSkill(SkillName name)
    {
        PlayerChar.Instance.GetSkill((int)name).BaseValue = PlayerPrefs.GetInt(name + BASE_VALUE, 0);
        PlayerChar.Instance.GetSkill((int)name).Update();
    }

    #endregion

    #region Save-Load Items
    public static void SaveCharacterGold(int gold)
    {
        PlayerPrefs.SetInt(PLAYER_GOLD, gold);
    }

    public static void LoadCharacterGold()
    {
        PlayerChar.Instance.Gold = PlayerPrefs.GetInt(PLAYER_GOLD, 0);
    }

    public static void SaveEmptySlot(int slot, bool isempty)
    {
        if (isempty)
            PlayerPrefs.SetInt(SLOT_EMPTY+slot, 1);
        else
            PlayerPrefs.SetInt(SLOT_EMPTY+slot, 0);
    }

    public static int LoadEmptySlot(int slot)
    {
        return PlayerPrefs.GetInt(SLOT_EMPTY + slot, 1);
    }

    public static void SaveItem(Item item, int slot)
    {
        PlayerPrefs.SetString(ITEM_NAME + slot, item.Name);
        PlayerPrefs.SetInt(ITEM_VALUE + slot, item.Ivalue);
        PlayerPrefs.SetInt(ITEM_CUR_DUR + slot, item.CurDurability);
        PlayerPrefs.SetInt(ITEM_MAX_DUR + slot, item.MaxDurability);
        PlayerPrefs.SetInt(ITEM_RARE + slot, (int)item.Rarity);
    }

    public static void SaveWeapon(Weapon weapon)
    {
        SaveItem(weapon, (int)EquipmentSlot.MainHand);
        PlayerPrefs.SetInt(WEAPON_MAX_DMG, weapon.MaxDamage);
        PlayerPrefs.SetFloat(WEAPON_DMGVAR, weapon.DamageVariance);
        PlayerPrefs.SetInt(WEAPON_TYPE, (int)weapon.TypeofWeapon);
    }

    public static void SaveArmor(Armor armor,  int slot)
    {
        SaveItem(armor, slot);
        PlayerPrefs.SetInt(ARMOR_LEVEL + slot,armor.ArmorLevel);
        PlayerPrefs.SetInt(ARMOR_SLOT + slot, (int)armor.Slot);
    }

    public static Armor LoadArmor(int slot)
    {
        var armor = new Armor
                        {
                            Name = PlayerPrefs.GetString(ITEM_NAME + slot),
                            Ivalue = PlayerPrefs.GetInt(ITEM_VALUE + slot),
                            CurDurability = PlayerPrefs.GetInt(ITEM_CUR_DUR + slot),
                            MaxDurability = PlayerPrefs.GetInt(ITEM_MAX_DUR + slot),
                            Rarity = (RarityType) PlayerPrefs.GetInt(ITEM_RARE + slot),
                            ArmorLevel = PlayerPrefs.GetInt(ARMOR_LEVEL + slot),
                            Slot = (EquipmentSlot) PlayerPrefs.GetInt(ARMOR_SLOT + slot)
                        };
        armor.Icon = ItemGenerator.LoadItemTexture((EquipmentSlot) slot, armor.Name);
        return armor;
    }

    public static Weapon LoadWeapon()
    {
        const int slot = (int) EquipmentSlot.MainHand;
        var weapon = new Weapon
                         {
                             Name = PlayerPrefs.GetString(ITEM_NAME + slot),
                             Ivalue = PlayerPrefs.GetInt(ITEM_VALUE + slot),
                             CurDurability = PlayerPrefs.GetInt(ITEM_CUR_DUR + slot),
                             MaxDurability = PlayerPrefs.GetInt(ITEM_MAX_DUR + slot),
                             Rarity = (RarityType)PlayerPrefs.GetInt(ITEM_RARE + slot),
                             MaxDamage = PlayerPrefs.GetInt(WEAPON_MAX_DMG),
                             DamageVariance = PlayerPrefs.GetFloat(WEAPON_DMGVAR),
                             TypeofWeapon = (WeaponType)PlayerPrefs.GetInt(WEAPON_TYPE),
                         };
        weapon.Icon = ItemGenerator.LoadItemTexture(EquipmentSlot.MainHand, weapon.Name);
        return weapon;
    }
    #endregion

    #region Save- Load Game Version
    public static void SaveGameVersion()
    {
        PlayerPrefs.SetFloat(VERSION_NAME, VERSION);
    }

    public static float LoadGameVersion()
    {
        return PlayerPrefs.GetFloat(VERSION_NAME, 0);
    }
    #endregion
}

public enum CharacterMaterialIndex
{
    Feet = 0,
    Pants = 1,
    Body = 2,
    Hands = 3,
    Head = 4
}
