using UnityEngine;
using System.Collections.Generic;

public class PlayerChar : BaseCharacter
{
    private const float _levelExpMultiplier = 1.10f; //multiplier to increase the exp requirement for each level
    private static PlayerChar instance = null;
    private List<Item> _inventory = new List<Item>();
    private int _currentExp;                            //chars current level exp
    private int _expToNextLevel = 2000;                     //total exp required to level
    private int _gold;
    private static Transform _myTransform;
    private static GameObject selector;
    private static ErrorText _pointerror;

    public Transform selectedTarget = null; //Player's selected target
    public bool initialized = false;

    #region Properties
    public List<Item> Inventory
    {
        get { return _inventory; }
        set { _inventory = value; }
    }

    public int CurrentExp
    {
        get { return _currentExp; }
        set { _currentExp = value; }
    }

    public int ExpToNextLevel
    {
        get { return _expToNextLevel; }
        set { _expToNextLevel = value; }
    }

    public int Gold
    {
        get { return _gold; }
        set { _gold = value; }
    }
    #endregion

    public static PlayerChar Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("Instatiating a new Player Character");
                //Load the saved modelindex and the saved possition
                var go = Instantiate(Resources.Load(GameSetting2.MALE_MODEL_PATH + GameSetting2.maleModels[GameSetting2.LoadCharacterModelIndex()]), GameSetting2.LoadCharacterPosition(), Quaternion.identity) as GameObject;

                if (go.GetComponent<PlayerChar>() == null)
                    Debug.LogError("Player Prefab does not containt PlayerChar script");

                instance = go.GetComponent<PlayerChar>();

                _myTransform = go.transform;

                selector = GameObject.FindGameObjectWithTag("select");
                _pointerror = GameObject.Find("PointError").GetComponent<ErrorText>();
                //Set tag and name
                go.name = "Player Character";
                go.tag = "Player";
            }
            return instance;
        }
    }

    public void Initialise()
    {
        if (!initialized)
            LoadCharacter();
    }

    public new void Awake()
    {
        base.Awake(); //This makes sure that every awake in the base classes will called
        instance = this;
    }

    //Method to load Everything
    public void LoadCharacter()
    {
        //load player name
        name = GameSetting2.LoadName();

        LoadHair();
        LoadHeadnSkin();
        LoadScale();

        GameSetting2.LoadAttributes();
        ClearModifiers();
        GameSetting2.LoadVitals();
        GameSetting2.LoadSkills();
        GameSetting2.LoadCharacterLevel();
        GameSetting2.LoadCharacterGold();

        if (GameSetting2.LoadEmptySlot((int)EquipmentSlot.MainHand) == 0)
        {
            var weapon = GameSetting2.LoadWeapon();
            EquipedWeapon = weapon;
            //Add the selected wepaons stats
            AddBuffEquipedStats(weapon);
        }

        for (int i = 0; i < (int)EquipmentSlot.COUNT - 1; i++)
        {
            var slot = (EquipmentSlot)i;
            if (GameSetting2.LoadEmptySlot(i) == 0)
            {
                var armor = GameSetting2.LoadArmor(i);
                switch (slot)
                {
                    case EquipmentSlot.Helmet:
                        EquipedHelmet = armor;
                        break;
                    case EquipmentSlot.OffHand:
                        EquipedShield = armor;
                        break;
                    case EquipmentSlot.Body:
                        EquipedBody = armor;
                        break;
                    case EquipmentSlot.Legs:
                        EquipedPants = armor;
                        break;
                    case EquipmentSlot.Hands:
                        EquipedGloves = armor;
                        break;
                        case EquipmentSlot.Boots:
                        EquipedBoots = armor;
                        break;
                }

                AddBuffEquipedStats(armor);
            }
        }
        initialized = true;
    }

    #region Targeting

    //Method to target an enemy. Call this method with null to cancel the players target.
    public void targetEnemy(Transform target)
    {
        //select our target
        selectedTarget = target;

        //place the selector below
        selector.transform.parent = target.FindChild("Selector");
        selector.transform.localPosition = new Vector3(0, 0.25f);
    }

    //Methdo to cancel players target
    public void cancelTarget()
    {
        selectedTarget = null;
        selector.transform.parent = null;
        selector.transform.position = Vector3.zero;
    }
    //Method to cancel the from players current target
    public void deadTarget(GameObject mob)
    {
        if (selectedTarget == mob.transform)
            selectedTarget = null;
    }

    #endregion

    #region Equiped Gear Properties

    public Item EquipedBody
    {
        get { return _equipment[(int)EquipmentSlot.Body]; }
        set
        {
            _equipment[(int)EquipmentSlot.Body] = value;

            //if there is an armor put it on
            if (_equipment[(int)EquipmentSlot.Body] != null)
            {
                Instance.armorMesh.renderer.materials[2].mainTexture = Resources.Load(GameSetting2.ARMOR_TEXTURE_PATH + value.Name) as Texture2D;
                // Debug.Log(GameSetting2.ARMOR_TEXTURE_PATH + value.Name);
            }
            //if no armor use the starting textures.
            else
            {
                Instance.armorMesh.renderer.materials[2].mainTexture = Resources.Load(GameSetting2.ARMOR_TEXTURE_PATH + "Starting Armor") as Texture2D;
            }
        }
    }

    public Item EquipedBoots
    {
        get { return _equipment[(int)EquipmentSlot.Boots]; }
        set
        {
            _equipment[(int)EquipmentSlot.Boots] = value;

            //if there is an armor put it on
            if (_equipment[(int)EquipmentSlot.Boots] != null)
            {
                PlayerChar.Instance.armorMesh.renderer.materials[0].mainTexture = Resources.Load(GameSetting2.ARMOR_TEXTURE_PATH + value.Name) as Texture2D;
            }
            //if no armor use the starting textures.
            else
            {
                PlayerChar.Instance.armorMesh.renderer.materials[0].mainTexture = Resources.Load(GameSetting2.ARMOR_TEXTURE_PATH + "Starting Feet") as Texture2D;
            }
        }
    }

    public Item EquipedGloves
    {
        get { return _equipment[(int)EquipmentSlot.Hands]; }
        set
        {
            _equipment[(int)EquipmentSlot.Hands] = value;

            //if there is an armor put it on
            if (_equipment[(int)EquipmentSlot.Hands] != null)
            {
                PlayerChar.Instance.armorMesh.renderer.materials[3].mainTexture = Resources.Load(GameSetting2.ARMOR_TEXTURE_PATH + value.Name) as Texture2D;
            }
            //if no armor use the starting textures.
            else
            {
                PlayerChar.Instance.armorMesh.renderer.materials[3].mainTexture = Resources.Load(GameSetting2.ARMOR_TEXTURE_PATH + "Starting Gloves") as Texture2D;
            }
        }
    }

    public Item EquipedPants
    {
        get { return _equipment[(int)EquipmentSlot.Legs]; }
        set
        {
            _equipment[(int)EquipmentSlot.Legs] = value;

            //if there is an armor put it on
            if (_equipment[(int)EquipmentSlot.Legs] != null)
            {
                PlayerChar.Instance.armorMesh.renderer.materials[1].mainTexture = Resources.Load(GameSetting2.ARMOR_TEXTURE_PATH + value.Name) as Texture2D;
            }
            //if no armor use the starting textures.
            else
            {
                PlayerChar.Instance.armorMesh.renderer.materials[1].mainTexture = Resources.Load(GameSetting2.ARMOR_TEXTURE_PATH + "Starting Pants") as Texture2D;
            }
        }
    }
    #endregion

    #region Instantiate Shield

    public Item EquipedShield
    {
        get { return _equipment[(int)EquipmentSlot.OffHand]; }
        set
        {

            _equipment[(int)EquipmentSlot.OffHand] = value; //save the equiped weapon to the array spot
            //if no equiped weapon don't check below

            if (offhandMount.transform.childCount > 0)
                Destroy(offhandMount.transform.GetChild(0).gameObject);

            if (_equipment[(int)EquipmentSlot.OffHand] != null)
            {
                var mesh = Instantiate(Resources.Load(GameSetting2.SHIELD_MESH_PATH + _equipment[(int)EquipmentSlot.OffHand].Name), offhandMount.transform.position, offhandMount.transform.rotation) as GameObject;
                if (mesh != null) mesh.transform.parent = offhandMount.transform;
            }
        }
    }

    #endregion

    #region Instantiate Helmet

    public Item EquipedHelmet
    {
        get { return _equipment[(int)EquipmentSlot.Helmet]; }
        set
        {

            _equipment[(int)EquipmentSlot.Helmet] = value; //save the equiped weapon to the array spot
            //if no equiped weapon don't check below

            if (helmetMount.transform.childCount > 0)
                Destroy(helmetMount.transform.GetChild(0).gameObject);

            if (_equipment[(int)EquipmentSlot.Helmet] != null)
            {
                var mesh = Instantiate(Resources.Load(GameSetting2.ARMOR_MESH_PATH + "Helmets/" + _equipment[(int)EquipmentSlot.Helmet].Name), helmetMount.transform.position, helmetMount.transform.rotation) as GameObject;
                if (mesh != null)
                {
                    mesh.transform.parent = helmetMount.transform;

                    //scale the new helmet to the same scale as hair
                    mesh.transform.localScale = hairMount.transform.GetChild(0).localScale;
                }

                //hide hair
                // hairMount.transform.GetChild(0).gameObject.active = false;
            }
        }
    }

    #endregion

    #region Load Character Hair

    public void LoadHair()
    {
        LoadHairMesh();
        LoadHairColor();
    }
    public void LoadHairMesh()
    {
        int _hairIndex = GameSetting2.LoadHairMesh();
        int hairset = _hairIndex / 5 + 1;
        int hairMeshNum = _hairIndex % 5 + 1;

        //Delete previous isntances
        if (hairMount.transform.childCount > 0)
            foreach (Transform child in hairMount.transform)
                Destroy(child.gameObject);

        var hairMesh = Instantiate(Resources.Load(GameSetting2.MALE_MODEL_PATH + "Hair/Hair0" + hairset + "_" + hairMeshNum),
            hairMount.transform.position,
            hairMount.transform.rotation) as GameObject;

        if (hairMesh == null) return;

        hairMesh.transform.parent = hairMount.transform;

        // Debug.Log(MALE_MODEL_PATH + "Hair/Hair0" + hairset + "_" + hairMeshNum);
        //reset local position
        hairMesh.transform.localPosition = Vector3.zero;
        hairMesh.transform.localRotation = Quaternion.identity;
        hairMesh.transform.localScale = new Vector3(4.2f, 4.2f, 4.2f);

        LoadHairColor();

        //check for meshoffset script n apply values
        var meshOffsetScript = hairMesh.GetComponent<MeshOffset>();
        if (meshOffsetScript == null)
            return;
        hairMesh.transform.localPosition = meshOffsetScript.posOffset;
        hairMesh.transform.localRotation = Quaternion.Euler(meshOffsetScript.rotationOffset);
        hairMesh.transform.localScale = meshOffsetScript.scaleOffset;
    }

    public void LoadHairColor()
    {
        //Debug.Log(GameSetting2.MALE_TEXTURE_PATH + "Hair/Hair_" + GameSetting2.LoadHairColor());
        hairMount.transform.GetChild(0).renderer.material.mainTexture = Resources.Load(GameSetting2.MALE_TEXTURE_PATH + "Hair/Hair_" + GameSetting2.LoadHairColor()) as Texture2D;
    }
    #endregion

    #region Damage Done-Received

    //Method to choose which attack player will do depending on the weapon he is holding.
    public void PlayerAttack()
    {
        if (MeleeResetTimer <= 0)
        {
            if (EquipedWeapon == null)
            {
                //if no weapon equiped, return
                SendMessage("MeleeAttackAnim");
                MeleeResetTimer = MeleeAttackTimer;
                DamageMob(GameSetting2.BASE_MELEE_RANGE, SkillName.Melee_Attack_Power, 2f, 2.5f);
            }
            else if (((Weapon)EquipedWeapon).TypeofWeapon == WeaponType.OneHandMelee ||
                     ((Weapon)EquipedWeapon).TypeofWeapon == WeaponType.Magic)
            {
                SendMessage("MeleeAttackAnim");
                MeleeResetTimer = MeleeAttackTimer;
                DamageMob(GameSetting2.BASE_MELEE_RANGE, SkillName.Melee_Attack_Power, 2.5f, 3f);
            }
            else if (((Weapon)EquipedWeapon).TypeofWeapon == WeaponType.Ranged)
            {
                SendMessage("RangedAttackAnim");
                MeleeResetTimer = MeleeAttackTimer;
                DamageMob(GameSetting2.BASE_RANGED_RANGE, SkillName.Ranged_Attack_Power, 2f, 2.5f);
            }
        }
    }

    //Method to cast a spell without a target
    public void CastSpellNoTarget(string spellname)
    {
        SendMessage("SpellCastAnim");
        if (spellname == "Heal")
        {
            GetVital((int)VitalName.Mana).CurValue -= 320;
            Instantiate(Resources.Load(GameSetting2.SPELLS_PATH + spellname), _myTransform.FindChild("HealSpell").position, transform.rotation);
        }
        else
        {
            GetVital((int)VitalName.Mana).CurValue -= 170;
            var spell = Instantiate(Resources.Load(GameSetting2.SPELLS_PATH + spellname), _myTransform.FindChild("SpellDirection").position,
                            transform.rotation) as GameObject;
            if (spell != null) spell.gameObject.GetComponent<SpellDamage>().spellPower = GetSkill((int)SkillName.Spell_Power).AdjustedBaseValue; //add spell damage script to gameobject
        }
    }

    //Method to cast a spell to current target
    public void CastSpellWithTarget(string spellname, Transform spelltarget)
    {
        GetVital((int)VitalName.Mana).CurValue -= 60;
        Instantiate(Resources.Load(GameSetting2.SPELLS_PATH + spellname), spelltarget.position, Quaternion.identity);
        DamageMob(GameSetting2.BASE_MAGIC_RANGE, SkillName.Spell_Power, 10f, 15f);
    }

    //Method to damage the mob depending on the type of the attack
    //min,max are the skill multipliers
    private void DamageMob(float attackrange, SkillName attacktype, float min, float max)
    {
        if (selectedTarget == null) //if player has no target, return
            return;

        Vector3 direction = (selectedTarget.position - _myTransform.position).normalized;
        //Vector used to calculate direction, normalize it so it only has a value of 1
        float distance = Vector3.Distance(_myTransform.position, selectedTarget.position);
        //distance between mob and player

        if (distance < attackrange) //if player is in melee range
        {
            if (Vector3.Dot(direction, _myTransform.forward) > 0.85f) // check the direction, 1 when they are parallel
            {
                var mobScript = selectedTarget.GetComponent<Mob>();
                if (mobScript != null)
                {
                    var damage = (int)((GetSkill((int)attacktype).AdjustedBaseValue) * (Random.Range(min, max)));
                    mobScript.MobDamageReceived(damage);
                    //Create combat text
                    var PointTransform =
                        Instantiate(Resources.Load(GameSetting2.EFFECTS_PATH + "PointEffect"),
                                    selectedTarget.transform.position, _myTransform.rotation) as GameObject;
                    if (PointTransform != null)
                        PointTransform.gameObject.GetComponent<CombatText>().effectName = damage + " Damage";
                }
            }
        }
        else
        {
            _pointerror.DisplayerrorText("Out of range");
        }
    }

    //mitigate damage received depending on our armor
    public void DamageReceived(int damage)
    {
        var damageTaken = (int)(damage - (GetSkill((int)(SkillName.Armor)).AdjustedBaseValue * 1.2));
        if (damageTaken > 0)
        {
            GetVital((int)(VitalName.Health)).CurValue -= damageTaken;

            //Instatiate Special Effect
            var go = Instantiate(Resources.Load(GameSetting2.EFFECTS_PATH + "Blood"), _myTransform.position,
                                 Quaternion.identity) as GameObject;
            go.transform.parent = _myTransform;
            go.transform.localPosition = new Vector3(0, 0.75f, 0);

            if (GetVital((int)VitalName.Health).CurValue == 0) //check if player dies
            {
                gameObject.GetComponent<Movement>().Alive = false;
                gameObject.transform.FindChild("DieText").GetComponent<MeshRenderer>().enabled = true;
                gameObject.GetComponent<PlayerGUI>().deadgui = true;
            }
        }
    }

    public void PlayerRespawn()
    {
        gameObject.transform.FindChild("DieText").GetComponent<MeshRenderer>().enabled = false; //hide the death message

        GetVital((int)VitalName.Health).CurValue = (int)(GetVital((int)VitalName.Health).AdjustedBaseValue * 0.8f); //regen hp 
        gameObject.transform.position = new Vector3(74, 100, 456); //spawn player to the village
        gameObject.GetComponent<Movement>().Alive = true; //enable player movement

        //Instatiate Resurrection Effect
        var go = Instantiate(Resources.Load(GameSetting2.EFFECTS_PATH + "Resurrect"), gameObject.transform.position,
                             Quaternion.identity) as GameObject;
        go.transform.parent = gameObject.transform;
    }

    public void GetHeal(int heal)
    {
        GetVital((int)VitalName.Health).CurValue += heal;
        //Create Combat Text Point
        var PointTransform = Instantiate(Resources.Load(GameSetting2.EFFECTS_PATH + "PointEffect"), _myTransform.position, _myTransform.rotation) as GameObject;
        if (PointTransform != null) PointTransform.gameObject.GetComponent<CombatText>().effectName = heal + " Heal";
    }
    #endregion

    #region Item Stats Equip-Unequip
    //Method to add stats when a player unequips an ite
    public void AddBuffEquipedStats(Item item)
    {
        if (item.GetType() == typeof(Weapon))
        {
            switch (((Weapon)item).TypeofWeapon)
            {
                //add new weapon values
                case WeaponType.OneHandMelee:
                    GetSkill((int)(SkillName.Melee_Attack_Power)).BuffValue += ((Weapon)item).MaxDamage;
                    break;
                case WeaponType.Ranged:
                    GetSkill((int)(SkillName.Ranged_Attack_Power)).BuffValue += ((Weapon)item).MaxDamage;
                    break;
                case WeaponType.Magic:
                    GetSkill((int)(SkillName.Spell_Power)).BuffValue += ((Weapon)item).MaxDamage;
                    break;
            }
            //  Debug.Log("Added " + ((Weapon)item).MaxDamage + " to buff value");
        }
        else if (item.GetType() == typeof(Armor))
        {
            GetSkill((int)(SkillName.Armor)).BuffValue += ((Armor)item).ArmorLevel;
        }

    }

    //Method to remove stats when a player unequips an item
    public void RemoveBuffEquipedStats(Item item)
    {
        if (item != null)
        {
            if (item.GetType() == typeof(Weapon))
            {
                //reset the buffvalues - sub from current equiped weapon
                switch (((Weapon)item).TypeofWeapon)
                {
                    case WeaponType.OneHandMelee:
                        GetSkill((int)(SkillName.Melee_Attack_Power)).BuffValue -= ((Weapon)item).MaxDamage;
                        break;
                    case WeaponType.Ranged:
                        GetSkill((int)(SkillName.Ranged_Attack_Power)).BuffValue -= ((Weapon)item).MaxDamage;
                        break;
                    case WeaponType.Magic:
                        GetSkill((int)(SkillName.Spell_Power)).BuffValue -= ((Weapon)item).MaxDamage;
                        break;
                }
                // Debug.Log("Removed " + ((Weapon)EquipedWeapon).MaxDamage + " from buffvalue");
            }
            else if (item.GetType() == typeof(Armor))
            {
                GetSkill((int)(SkillName.Armor)).BuffValue -= ((Armor)item).ArmorLevel;
            }
        }
    }

    //Mehtod to remove an item from character window
    public void UnEquipItem(Item item)
    {
        Inventory.Add(item);
        RemoveBuffEquipedStats(item);
    }
    #endregion

    #region Experience - Leveling

    public void AddCharExp(int exp)
    {
        _currentExp += exp;
        if (_currentExp > _expToNextLevel) //Level up
        {
            //Instatiate Special Effect
            var go = Instantiate(Resources.Load(GameSetting2.EFFECTS_PATH + "Resurrect"), _myTransform.position,
                                 Quaternion.identity) as GameObject;
            go.transform.parent = _myTransform;

            //Increase Level and stats
            Level += 1;
            _currentExp = 0;
            _expToNextLevel = (int)(_expToNextLevel * 1.5f);
            IncreaseStats(Level);
            GameSetting2.SaveCharacterLevel(Level); //save value
        }
        GameSetting2.SaveCharacterExp(_currentExp, _expToNextLevel);  //Save new values
    }

    //Method to increase player stats each level
    private void IncreaseStats(int level)
    {
        if (level < 50)
        {
            for (int i = 0; i < (int)AttributeName.COUNT; i++)
                GetPrimaryAttribute(i).BaseValue += 1;
        }
        StatUpdate();
        GameSetting2.SaveAttributes(primaryattribute); //save new stats
    }
    #endregion

    public void LoadHeadnSkin()
    {
        armorMesh.renderer.materials[(int)CharacterMaterialIndex.Head].mainTexture = Resources.Load(GameSetting2.MALE_TEXTURE_PATH + "Head/0" + GameSetting2.LoadHead() + "_" + GameSetting2.LoadSkinColor() + ".head.human") as Texture2D;
        armorMesh.renderer.materials[(int)CharacterMaterialIndex.Hands].mainTexture = Resources.Load(GameSetting2.MALE_TEXTURE_PATH + "Hands/0" + GameSetting2.LoadSkinColor() + ".hands") as Texture2D;
    }

    public void LoadScale()
    {
        transform.localScale = new Vector3(transform.localScale.x,
            transform.localScale.y * GameSetting2.LoadCharacterHeight(),
            transform.localScale.z);
    }

    public void LoadBodyArmor()
    {
    }

    public void LoadHelmet()
    {
    }

    public void LoadShoulderPads()
    {
    }

    public void LoadGloves()
    {
    }

    public void LoadLegArmor()
    {
    }

    public void LoadBoots()
    {
    }
    public void LoadBackItem()
    {
    }

}

