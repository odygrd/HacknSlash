//Mob.cs
//July 25,2012
//
// This script is responsible for controlling the mob
// It should be attached to a mob, or a mob prefab
using UnityEngine;
using System.Collections;

[AddComponentMenu("Mob/All Mob Scripts")] //Add script to menu for easier access

//Automaticly add the following scripts
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Movement))]

public class Mob : BaseCharacter
{
    static public GameObject mycamera; //reference for our camera
    public bool showDebugConsole = false;

    private Ai _AiScript;
    private Movement _movementScript;
    private GameObject _selector; //reference to selector circle for script opt
    private bool _mobdied = false;
    private Transform displayName; //reference for the displayName text mesh compoment
    private Transform myTransform; //cache mytransform for better performacne
    private readonly GameObject[] loots = new GameObject[2]; //store prefabs
    new void Awake()
    {
        base.Awake();
        Spawn();
    }

    void Start()
    {
        //find the camera 
        mycamera = GameObject.Find("Main Camera");
        //find 3dtext name
        displayName = transform.FindChild("Name");
        myTransform = gameObject.transform;
        _AiScript = GetComponent<Ai>();
        _movementScript = GetComponent<Movement>();
        _selector = GameObject.FindGameObjectWithTag("select");
        loots[0] = Resources.Load(GameSetting2.MISC_MESH_PATH + "Money_Gold_Large") as GameObject;
        loots[1] = Resources.Load(GameSetting2.MISC_MESH_PATH + "RandomDrop") as GameObject;

        //if 3d text not exists
        if (displayName.Equals(null))
        {
            Debug.LogWarning("No 3D Text found in the mob");
            return;
        }

        //go.GetComponent<MeshRenderer>().enabled = false;

        //set the name to prefab name  
        // Debug.Log(displayname.transform.parent.name);
    
        displayName.GetComponent<TextMesh>().text = name;
    }

    public void Spawn()
    {
        //setup attributes and skills
        SetupStats();
        //setup gear
        SetupGear();
    }

    //Method to setup player stats
    private void SetupStats()
    {
        //**need to set mob levels
        //random setup each stat
        for (int i = 0; i < primaryattribute.Length; i++)
        {
            GetPrimaryAttribute(i).BaseValue = Random.Range(40, 80);
        }
        StatUpdate(); //update vitals and skills

        //Set up mobs starting health - 100%
        for (int i = 0; i < vital.Length; i++)
            GetVital(i).CurValue = GetVital(i).AdjustedBaseValue;
    }

    //Method to setup mob geaer
    private void SetupGear()
    {
        //Create new weapon and equip it
        EquipedWeapon = ItemGenerator.CreateItem(ItemType.Weapon);
    }

    //Methdo to instatiate loot when the mob dies
    private void DropLoot()
    {
        int dice = Random.Range(0, 10);

        if (dice > 1) //drop gold with 80% propability
        {
            var go = Instantiate(loots[0], myTransform.position, Quaternion.identity) as GameObject;
            if (go != null)
            {
                go.transform.Rotate(-90, 0, 0);
                go.transform.position = new Vector3(myTransform.position.x + Random.Range(0.6f, 2f),
                                                    myTransform.position.y + 0.07f,
                                                    myTransform.position.z + Random.Range(0.6f, 2f));

                go.GetComponent<Gold>().Value = Random.Range(50, GetVital((int)VitalName.Health).AdjustedBaseValue);
            }
        }

        dice = Random.Range(0, 10);

        //drop second item armor/weapon
        if (dice > 7)
        {
            var go = Instantiate(loots[1], myTransform.position, Quaternion.identity) as GameObject;
            if (go != null)
            {
                go.transform.position = new Vector3(myTransform.position.x + Random.Range(0.6f, 2f),
                                                    myTransform.position.y + 0.07f,
                                                    myTransform.position.z + Random.Range(0.6f, 2f));
            }
        }
    }

    void Update()
    {
        //if 3d text not exists-return
        if (displayName.Equals(null) || mycamera.Equals(null))
            return;

        displayName.LookAt(mycamera.transform);
        displayName.Rotate(new Vector3(0, 180, 0));
    }

    //mitigate damage received depending on our armor
    public void MobDamageReceived(int damage)
    {
        var damageTaken = (int)(damage - (GetSkill((int)(SkillName.Armor)).AdjustedBaseValue * 1.2));
        if (damageTaken > 0)
            GetVital((int)VitalName.Health).CurValue -= damageTaken;

        //mob dies
        if (GetVital((int)VitalName.Health).CurValue == 0 && !_mobdied)
        {
            KillMob();
        }

    }

    public void KillMob()
    {
        _movementScript.Alive = false;
        Destroy(_AiScript); // destroy Ai script
        SendMessage("DeathAnim"); //play death animation
        _mobdied = true;
        PlayerChar.Instance.InCombat = false;

        //reset the selctior position
        _selector.transform.parent = null;
        _selector.transform.position = Vector3.zero;

        //Add expersience to player
        PlayerChar.Instance.AddCharExp(2120);

        //drop loot
        DropLoot();

        //destroy mob after 10 seconds
        StartCoroutine(DestroyMob());
    }

    //co routine to wait for 10 seconds before destroying the mob
    IEnumerator DestroyMob()
    {
        yield return new WaitForSeconds(6);

        //remove the mob from our target list and if player still has it as target remove it
        PlayerChar.Instance.deadTarget(gameObject);

        //if the player reselected the dead mob remove it to avoid null reference
        if (_selector.transform.parent == gameObject.transform)
        {
            _selector.transform.parent = null;
            _selector.transform.position = Vector3.zero;
        }

        //finally destroy the gameobject
        Destroy(gameObject);
    }

    ////*******Debug Console*******
    //void OnGUI()
    //{
    //    if (showDebugConsole)
    //    {
    //        int lh = 20; //line height
    //        for (int i = 0; i < primaryattribute.Length; i++)
    //            GUI.Label(new Rect(10, 10 + (i * lh), 300, lh), ((AttributeName)i) + ":" + GetPrimaryAttribute(i).BaseValue);
    //        for (int i = 0; i <  vital.Length; i++)
    //            GUI.Label(new Rect(120, 10 + (i * lh), 300, lh), ((VitalName)i) + ":" + GetVital(i).CurValue + "/" + GetVital(i).AdjustedBaseValue);
    //        for (int i = 0; i < skill.Length; i++)
    //            GUI.Label(new Rect(120, 10 + (i * lh) + (vital.Length * lh), 300, lh), ((SkillName)i) + ":" + GetSkill(i).AdjustedBaseValue);
    //    }
    //}
}
