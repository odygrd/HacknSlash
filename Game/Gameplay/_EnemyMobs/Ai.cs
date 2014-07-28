//Ai.cs
//July 25,2012
//
//Ai script for the mobs

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Movement))] //Import Movement script because we will use the same controls for movement that we were using for uor player.
[RequireComponent(typeof(SphereCollider))] //Detect when a player is close enough to us
public class Ai : MonoBehaviour
{
    private enum State
    {
        Idle,              //do nothing
        Init,             //make sur ethat everyhtign we nee dis here
        Setup,            //assing the values to the things we need
        Search,           //find a player
        Decide            //Decide what to do with the targeted player
    }

    public float perceptionRadius = 10;  //the radius that mob will see us
    public Transform _target;  //Mob's target. Later get it from sphere collider.

    private Transform _myTransform; //my transform copy
    private const float ROTATION_DAMP = 0.3f; // rotation number for .dot calc
    private const float FORWARD_DAMP = 0.9f; //forward number for .dot calc

    private Transform _home;
    private SphereCollider _sphereCollider;
    private CharacterController _cc;
    private State _state = State.Init;
    private int _damageToDo;

    private Mob _mobScript;  //used to get reference to mob script


    void Awake()
    {
        //get reference to Mob Script
        _mobScript = gameObject.GetComponent<Mob>();
        //   Debug.Log("Mobs melee attack reset timer : " + _mobScript.MeleeResetTimer);
    }

    void Start()
    {
        StartCoroutine("FSM");
    }

    private IEnumerator FSM()
    {
        while (_state != State.Idle)
        {
            switch (_state)
            {
                case State.Init:
                    Init();
                    break;
                case State.Setup:
                    Setup();
                    break;
                case State.Search:
                    Search();
                    break;
                case State.Decide:
                    Decide();
                    break;
            }
            yield return null;
        }
    }

    private void Init()
    {
        _myTransform = transform;
        _home = transform.parent.transform;
        _sphereCollider = GetComponent<SphereCollider>();

        if (_sphereCollider == null)
        {
            Debug.Log("Sphere collider not present");
            return; //never change to setup state
        }

        _state = State.Setup;
    }

    private void Setup()
    {
        //put sphere collider in the center of the mob 
        _sphereCollider.center = GetComponent<CharacterController>().center;
        _sphereCollider.radius = perceptionRadius;
        _sphereCollider.isTrigger = true;
        //then stop and wait for player

        _state = State.Idle;
    }

    private void Search()
    {
        if (_target == null)
        {
            //     Debug.Log("search tagret null");
            _state = State.Idle;
            if (_mobScript.InCombat)
                _mobScript.InCombat = false;
        }
        else
        {   //do we detect the targted player yet?
            //if so set state to decide
            //     Debug.Log("search tagret found");
            _state = State.Decide;
            if (!_mobScript.InCombat)
                _mobScript.InCombat = true;
        }
    }

    private void Decide()
    {
        Move();
        //decide which attack to use
        int opt = 0;

        if (_target != null && _target.CompareTag("Player"))
        {
            if (PlayerChar.Instance.GetVital((int)VitalName.Health).CurValue > 0)
            {
            //  Debug.Log("Decide");
            //if the distance is less than the definded melee range, and if the attack timer is less than zero
            if (Vector3.Distance(_myTransform.position, _target.position) < GameSetting2.BASE_MELEE_RANGE &&
                _mobScript.MeleeResetTimer <= 0)
            {
                //   Debug.Log("In Melee Range");
                //opt = Random.Range(0, 3);
                opt = 0;
            }
            else //Mob can't use melee Attacks because it is away or attack timer is greater than zero
            {
                //Debug.Log("Not In Melee Range");
                if (_mobScript.MeleeResetTimer > 0)
                    _mobScript.MeleeResetTimer -= Time.deltaTime; //dont attack melee unless the timer reaches zero
                opt = Random.Range(1, 3);
            }

            switch (opt)
            {
                case 0:
                    MeleeAttack();
                    break;
                case 1:
                    RangedAttack();
                    break;
                case 2:
                    MagicAttack();
                    break;
                default:
                    break;
            }
        }
            else //if player is dead
            {
                _target = _home;
                PlayerChar.Instance.InCombat = false;
                if (_mobScript.InCombat)
                    _mobScript.InCombat = false;
            }
    }
        _state = State.Search;
    }

    private void MeleeAttack()
    {
        // Debug.Log("Melee Attack");
        //reset the melee timer back to melee attack timer
        _mobScript.MeleeResetTimer = _mobScript.MeleeAttackTimer;

        //deal with the animaiton
        //call MeleeAttackAnim method in Movement.cs
        SendMessage("MeleeAttackAnim");

        //decide if we hit or not
        //if hit
        if (true)
        {
            _damageToDo = Random.Range(7 * _mobScript.GetSkill((int)(SkillName.Melee_Attack_Power)).AdjustedBaseValue, 
                10 * _mobScript.GetSkill((int)(SkillName.Melee_Attack_Power)).AdjustedBaseValue);
            //   Debug.Log("We hit Melee!");
            PlayerChar.Instance.DamageReceived(_damageToDo);

        }
        else
        {
            Debug.Log("We missed");
        }
    }

    private void RangedAttack()
    {
        //  Debug.Log("Ranged Attack");
    }

    private void MagicAttack()
    {
        // Debug.Log("Magin Attack");
    }


    private void Move()
    {
        if (_target)
        {
            float distance = Vector3.Distance(_target.position, _myTransform.position); //find the distance between me and mob

            //On trigger exit mob target is Spawn Point
            if (_target.name == "Spawn Point")
            {
                //       Debug.LogWarning("Mob returning home");
                //         Debug.Log(distance);
                if (distance < GameSetting2.BASE_MELEE_RANGE)
                {
                    //     Debug.Log("Entereted if");
                    _target = null;
                    _state = State.Idle;
                    SendMessage("MoveMeForward", Movement.Forward.none);
                    SendMessage("RotateMe", Movement.Turn.none);
                    return;
                }
            }
            // Debug.Log("search target name");

            Quaternion rot = Quaternion.LookRotation(_target.transform.position - _myTransform.position);
            _myTransform.rotation = Quaternion.Slerp(_myTransform.rotation, rot, Time.deltaTime * 7.0f);

            Vector3 dir = (_target.position - _myTransform.position).normalized; //normalize it so it only has a value of 1
            float direction = Vector3.Dot(dir, _myTransform.forward); //For normalized vectors Dot returns 1 if they point in exactly the same direction; 
                                                                      //-1 if they point in completely opposite directions; 

            // Debug.Log(direction);
            if (direction > FORWARD_DAMP && distance > GameSetting2.BASE_MELEE_RANGE)
            {
                SendMessage("MoveMeForward", Movement.Forward.forward);
            }
            else
            {
                SendMessage("MoveMeForward", Movement.Forward.none);
            }

            //Example-
            //dir = (_target.position - _myTransform.position).normalized; //normalize it so it only has a value of 1
            //direction = Vector3.Dot(dir, transform.right); //1 for right, -1 for left

            //if (direction > ROTATION_DAMP)
            //{
            //    SendMessage("RotateMe", Movement.Turn.right);
            //}
            //else if (direction < -ROTATION_DAMP)
            //{
            //    SendMessage("RotateMe", Movement.Turn.left);
            //}
            //else
            //{
            //    SendMessage("RotateMe", Movement.Turn.none);
            //}
        }
        else
        {
            SendMessage("MoveMeForward", Movement.Forward.none);
            SendMessage("RotateMe", Movement.Turn.none);
        }
    }

    //when we collider with the sphere
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _target = other.transform;
            PlayerChar.Instance.InCombat = true;
            _state = State.Search;
            FSM();
            StartCoroutine("FSM");
        }
    }

    //when we leave the spheres range
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _target = _home;
            PlayerChar.Instance.InCombat = false;
            if (_mobScript.InCombat)
                _mobScript.InCombat = false;
        }
    }
}
