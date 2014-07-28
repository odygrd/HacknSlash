// BaseCharacter.cs
// May 30, 2012
// 
// This is the BaseCharacter class

using UnityEngine;
using System;

public class BaseCharacter : MonoBehaviour
{
    public new string name;
    public GameObject weaponMount;             //Weapon mount position for players weapon
    public GameObject offhandMount;             //Offhand Mount posiition
    public GameObject hairMount;               //Hair mount position for players weapon
    public GameObject helmetMount;             //Helmet mount position for players weapon
    public GameObject armorMesh;               //Reference to the mesh that actualy holds the armor materials - "muscle" or "fat" in our models

    public Attribute[] primaryattribute;     //Array with character attributes.
    public Vital[] vital;                    //Array with character vital elements.
    public Skill[] skill;                    //Array with character skills.

    private int _level;                        //The character starting level.

    private bool _incombat = false;                                            //checks if the current char is in combat or not
    private float _meleeAttackTimer = GameSetting2.BASE_MELEE_ATTACK_TIMER;    //The time it takes between attacks
    private float _meleeAttackSpeed = GameSetting2.BASE_MELEE_ATTACK_SPEED;   //The time left before we can attack again
    private float _meleeResetTimer = 0;                                       //The speed the attack happens at

    protected Item[] _equipment = new Item[(int)EquipmentSlot.COUNT];

    /// <summary>
    /// Awake sets the starting values for character creation
    /// </summary>
    public virtual void Awake()
    {
        _level = 1;
        primaryattribute = new Attribute[Enum.GetValues(typeof(AttributeName)).Length]; //get the length of the attribute struct and create an array.
        vital = new Vital[Enum.GetValues(typeof(VitalName)).Length];
        skill = new Skill[Enum.GetValues(typeof(SkillName)).Length];

        SetupPrimaryAttributes();
        SetupVitals();
        SetupSkills();
    }

    #region Properties

    /// <summary>
    /// Gets or sets the _level.
    /// </summary>
    /// <value>
    /// The _level.
    /// </value>
    public int Level
    {
        get { return _level; }
        set { _level = value; }
    }

    public float MeleeAttackTimer
    {
        get { return _meleeAttackTimer; }
        set { _meleeAttackTimer = value; }
    }

    public float MeleeAttackSpeed
    {
        get { return _meleeAttackSpeed; }
        set { _meleeAttackSpeed = value; }
    }

    public float MeleeResetTimer
    {
        get { return _meleeResetTimer; }
        set { _meleeResetTimer = value; }
    }

    public bool InCombat
    {
        get { return _incombat; }
        set { _incombat = value; }
    }
    #endregion

    #region private methods
    /// <summary>
    /// Setups the primary attributes array.
    /// </summary>
    private void SetupPrimaryAttributes()
    {
        for (int i = 0; i < primaryattribute.Length; i++)
        {
            primaryattribute[i] = new Attribute();
        }

    }

    /// <summary>
    /// Setups the vitals array and the vital modifiers for each vital element.
    /// </summary>
    private void SetupVitals()
    {
        for (int i = 0; i < vital.Length; i++)
        {
            vital[i] = new Vital();
        }

        SetupVitalModifiers();

    }

    /// <summary>
    /// Setups the skills array and the modifiers for each skill.
    /// </summary>
    private void SetupSkills()
    {
        for (int i = 0; i < skill.Length; i++)
        {
            skill[i] = new Skill();
        }

        SetupSkillModifiers();

    }

    public void ClearModifiers()
    {
        foreach (Vital t in vital)
            t.ClearModifiers();

        foreach (Skill t in skill)
            t.ClearModifiers();

        SetupVitalModifiers();
        SetupSkillModifiers();
    }

    /// <summary>
    /// Setups the modifiers for each vital element. 
    /// For example health : Go to the vital array position which represents "health" and add a modifying attribute.
    /// Modifyingattribute is a struct containing the primary attribute we want to use to modify and a ratio.
    /// </summary>
    private void SetupVitalModifiers()
    {
        //Health Modifiers
           
        GetVital((int)VitalName.Health).AddModifier(new ModifyingAttribute { attribute = GetPrimaryAttribute((int)AttributeName.Constitution), ratio = 10f });
        GetVital((int)VitalName.Health).AddModifier(new ModifyingAttribute { attribute = GetPrimaryAttribute((int)AttributeName.Strength), ratio = 0.2f });
        //Debug.Log(GetVital((int)VitalName.Health).BaseValue.ToString());
        //Debug.Log(GetVital((int)VitalName.Health).AdjustedBaseValue.ToString());

        //Mana Modifiers
        GetVital((int)VitalName.Mana).AddModifier(new ModifyingAttribute { attribute = GetPrimaryAttribute((int)AttributeName.Charisma), ratio = 0.8f });
        GetVital((int)VitalName.Mana).AddModifier(new ModifyingAttribute { attribute = GetPrimaryAttribute((int)AttributeName.Intelligence), ratio = 0.5f });


    }

    /// <summary>
    /// Setups the skill modifiers.
    /// Go to the vital array position which represents the skill and add a modifying attribute.
    /// Modifyingattribute is a struct containing the primary attribute we want to use to modify and a ratio.
    /// </summary>
    private void SetupSkillModifiers()
    {
		
        //Melee Attack Power Modifiers		
        GetSkill((int)SkillName.Melee_Attack_Power).AddModifier(new ModifyingAttribute { attribute = GetPrimaryAttribute((int)AttributeName.Strength), ratio = 0.33f });
        GetSkill((int)SkillName.Melee_Attack_Power).AddModifier(new ModifyingAttribute { attribute = GetPrimaryAttribute((int)AttributeName.Dexterity), ratio = 0.11f });

        //Ranged Attack Power Modifiers
        GetSkill((int)SkillName.Ranged_Attack_Power).AddModifier(new ModifyingAttribute { attribute = GetPrimaryAttribute((int)AttributeName.Dexterity), ratio = 0.33f });
        GetSkill((int)SkillName.Ranged_Attack_Power).AddModifier(new ModifyingAttribute { attribute = GetPrimaryAttribute((int)AttributeName.Strength), ratio = 0.11f });

        //Spell Power Modifiers
        GetSkill((int)SkillName.Spell_Power).AddModifier(new ModifyingAttribute { attribute = GetPrimaryAttribute((int)AttributeName.Intelligence), ratio = 0.33f });

        //Armor Modifiers
        GetSkill((int)SkillName.Armor).AddModifier(new ModifyingAttribute { attribute = GetPrimaryAttribute((int)AttributeName.Strength), ratio = 0.20f });
        GetSkill((int)SkillName.Armor).AddModifier(new ModifyingAttribute { attribute = GetPrimaryAttribute((int)AttributeName.Dexterity), ratio = 0.10f });

        //Magic Defence Modifiers
        GetSkill((int)SkillName.Magic_Defence).AddModifier(new ModifyingAttribute { attribute = GetPrimaryAttribute((int)AttributeName.Intelligence), ratio = 0.20f });
        GetSkill((int)SkillName.Magic_Defence).AddModifier(new ModifyingAttribute { attribute = GetPrimaryAttribute((int)AttributeName.Charisma), ratio = 0.10f });
    }
    #endregion

    #region public methods
    /// <summary>
    /// Gets the attribute array value.
    /// </summary>
    /// <returns>
    /// The _primaryattribute.
    /// </returns>
    /// <param name='index'>
    /// Index.
    /// </param>
    public Attribute GetPrimaryAttribute(int index)
    {
        return primaryattribute[index];
    }

    /// <summary>
    /// Gets the vital array value.
    /// </summary>
    /// <returns>
    /// The _vital.
    /// </returns>
    /// <param name='index'>
    /// Index.
    /// </param>
    public Vital GetVital(int index)
    {
        return vital[index];
    }

    /// <summary>
    /// Gets the skill array value.
    /// </summary>
    /// <returns>
    /// The _skill.
    /// </returns>
    /// <param name='index'>
    /// Index.
    /// </param>
    public Skill GetSkill(int index)
    {
        return skill[index];
    }

    /// <summary>
    /// Update players vital and skill values based on modified attributes we used.
    /// attribute.AdjustedBaseValue * att.ratio
    /// </summary>
    public void StatUpdate()
    {
        for (int i = 0; i < vital.Length; i++)
            vital[i].Update();

        for (int i = 0; i < skill.Length; i++)
            skill[i].Update();
    }

    #region Instatiate Weapon

    public Item EquipedWeapon
    {
        get { return _equipment[(int)EquipmentSlot.MainHand]; }
        set
        {
            _equipment[(int)EquipmentSlot.MainHand] = value; //save the equiped weapon to the array spot
            //if no equiped weapon don't check below

            if (weaponMount.transform.childCount > 0)
                Destroy(weaponMount.transform.GetChild(0).gameObject);

            if (_equipment[(int)EquipmentSlot.MainHand] != null)
            {
                GameObject currentWeapon = Instantiate(Resources.Load(GameSetting2.WEAPON_MESH_PATH + _equipment[(int)EquipmentSlot.MainHand].Name), weaponMount.transform.position, weaponMount.transform.rotation) as GameObject;
                currentWeapon.transform.parent = weaponMount.transform;

            }
            
        }
    }
    #endregion

    //function to calculate the melee attack speed based on the stats selection
    public void CalculateMeleeAttackSpeed()
    {
        //todo
    }
    #endregion



}

public enum EquipmentSlot
{
    Helmet,
    Shoulders,
    Body,
    Legs,
    Hands,
    Boots,
    Back,
    OffHand,
    MainHand,
    COUNT
}

public enum AttackType
{
    Melee,
    Ranged,
    Magic
}

