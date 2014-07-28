//Movement.cs
//July 25,2012
//
// This script is responsible for getting the players movement inputs and adjusting the characters animations accordindly.
// 
// This script will be automatically attached to your player or mob with the use of the PlayerInput.cs and AI.cs scripts.
// 
// This script assumes that you have these animation with the following names:
// Player:
// swim	- the swim forward animation (only needed if you want to swim in game)
// walk	- a walk animation
// run		- a run animations
// side	- for strafing
// jump	- a jump animation
// fall	- a falling animation
// idle	- an idle animation
// 
// Mob:
// run		- a run animations
// jump	- a jump animation
// fall	- a falling animation
// idle	- an idle animation
//


using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]

public class Movement : MonoBehaviour
{
    public enum State
    {
        Idle,
        Init,
        Setup,
        Run
    }

    public enum Turn
    {
        left = -1,
        none = 0,
        right = 1
    }

    public enum Forward
    {
        back = -1,
        none = 0,
        forward = 1
    }

    #region Animation Names

    //Simple Animation Names Set-up
    public string walkAnimName;
    public string runAnimName;
    public string jumpAnimName;
    public string idleAnimName;
    public string swimAnimName;
    public string strafeAnimName;
    public string fallAnimName;
    public string deathAnimName;
    public string meleeAttackAnim;
    public string rangedAttackAnim;
    public string idleAttackAnim;
    public string spellCastAnim;
    #endregion

    public float rotateSpeed = 200;             //the speed our character turns
    public float runMultiplier = 2;             //how fast character runs compared to walk speed
    public float walkSpeed = 3;                 //the character walk speed
    public float strafeSpeed = 2.5f;            //the speed our character strafes
    public float gravity = 20;                  //the value for gravity
    public float airTime = 0;                   //how long have we been in the air
    public float fallTime = 1;                  //The time we have to be failing before the system knows it is a fall.
    public float jumpHeight = 6.5f;             //How high the character jumps
    public float jumpTime = 1.5f;               //How long the jump lasts.

    private CollisionFlags _collisionFlags;     //collisionflags we have from last frame
    private Vector3 _moveDirection;             //the direction our char is moving
    private Transform _myTransform;             //our cached transform
    private CharacterController _controller;    //reference to player controller script
    private BaseCharacter _basecharScript;      //reference to basecharacter script

    private State _state = State.Init;           //FSM Current State
    private Turn _turn = Turn.none;
    private Turn _strafe = Turn.none;
    private Forward _forward = Forward.none;
    private bool _run = true;
    private bool _jump = false;
    private bool _isSwimming = false;
    private bool _died;
    private bool _isAlive = true;

    public bool Alive
    {
        get { return _isAlive; }
        set { _isAlive = value; }
    }

    void Awake()
    {
        _myTransform = transform;                                            //cache our transform
        _controller = GetComponent<CharacterController>();                   //reference to charactercontroller script
        _basecharScript = GetComponent<BaseCharacter>();          //reference to base character script

    }

    // Use this for initialization
    IEnumerator Start()
    {
        while (true)
        {
            switch (_state)
            {
                case State.Init:
                    Init();
                    break;
                case State.Setup:
                    SetUp();
                    break;
                case State.Run:
                    ActionPicker();
                    break;
            }
            yield return null;
        }
    }

    private void Init()
    {
        if (!GetComponent<CharacterController>() || !GetComponent<Animation>())
            return;

        _state = State.Setup;
    }

    //Setup the starting movement values
    private void SetUp()
    {
        animation.Stop();            //stop any animations
        animation.wrapMode = WrapMode.Loop; //make all animations loop by default

        _moveDirection = Vector3.zero; //zero the vector3 we will use for moving the player

        _state = State.Run;

        if (!string.IsNullOrEmpty(jumpAnimName))
        {
            animation[jumpAnimName].layer = -1; //move jump to a higher layer
            animation[jumpAnimName].wrapMode = WrapMode.Once; //play jump animation only once
        }

        if (!string.IsNullOrEmpty(idleAnimName))
            animation.Play(idleAnimName); //star the idle animation when the script starts

    }

    private void ActionPicker()
    {
        if (_isAlive)
        {
            _died = false;
            //allow player to rotate left or right, typecast _turn to -1 or 1 to rotate left or right instead of Input.GetAxis ("Rotate Player")
            _myTransform.Rotate(0, (int)_turn * Time.deltaTime * rotateSpeed, 0);

            //if we are on the ground let us move
            if (_controller.isGrounded || _isSwimming)
            {
                airTime = 0; //reset airTimer when we are on the ground
                //get user input depending where to move forward or sidewways,  typecast strafe instead Input.GetAxis("Strafe") 
                //typecast _forward instead of Input.GetAxis ("Move Forward")
                _moveDirection = new Vector3((int)_strafe, 0, (int)_forward);
                _moveDirection = _myTransform.TransformDirection(_moveDirection).normalized;
                _moveDirection *= walkSpeed;

                if (_forward != Forward.none)
                { //if forward is not none
                    if (_isSwimming)
                    {
                        Swim();
                    }
                    else
                    {
                        if (_run)
                        {     //and run key
                            _moveDirection *= runMultiplier; //move player at running speed
                            Run();  //run animation
                        }
                        else
                        {
                            Walk();  //walk animation
                        }
                    }
                }
                else if (_strafe != Turn.none)
                {
                    Strafe();  //strafe animation
                }
                else
                {
                    if (_isSwimming)
                    {
                    }
                    else
                    {
                        Idle();  //idle animaiton
                    }
                }
                if (_jump)
                { //if player presses jump key
                    if (airTime < jumpTime)
                    {  //if we are not on air
                        _moveDirection.y += jumpHeight; //move upwards
                        Jump(); //jump animation
                        _jump = false;
                    }
                }
            }
            else
            {
                //if we have collision flag and it is collide bellow 
                if ((_collisionFlags & CollisionFlags.CollidedBelow) == 0)
                {
                    airTime += Time.deltaTime; //increase airTime every frame
                    if (airTime > fallTime)
                    { //if in air for that long play fall animation
                        Fall();
                    }
                }
            }
            if (!_isSwimming)                              //if not swimming apply gravity
                _moveDirection.y -= gravity * Time.deltaTime;  //apply gravity
            _collisionFlags = _controller.Move(_moveDirection * Time.deltaTime); //move character and store any new collisionflags we get
        }
        else
        {
            if (!_died) //play dead animation once
            {
                _died = true;
                DeathAnim();
            }
        }
    }

    public void MoveMeForward(Forward z)
    {
        _forward = z;
    }

    public void RotateMe(Turn y)
    {
        _turn = y;
    }

    public void Strafe(Turn x)
    {
        _strafe = x;
    }
    public void ToggleRun()
    {
        _run = !_run;
    }

    public void JumpMe()
    {
        _jump = true;
    }


    public void IsSwimming(bool swim)
    {
        _isSwimming = swim;
    }

    #region AnimationList
    //Animations list
    public void Idle()
    {
        if (string.IsNullOrEmpty(idleAnimName))
            return;

        if (!_basecharScript.InCombat) //if not in comabt play normal animation
        {
            if (!animation[meleeAttackAnim].enabled) //when the mob not attacking
            {
                animation.CrossFade(idleAnimName);
            }
        }
        else
        {
            if (!animation[meleeAttackAnim].enabled) //when the mob not attacking
            {
                //   Debug.Log("playing idle attack");
                animation.CrossFade(idleAttackAnim);
            }
        }

    }

    public void Fall()
    {
        if (string.IsNullOrEmpty(fallAnimName))
            return;
        animation.CrossFade(fallAnimName);
    }

    public void Strafe()
    {
        if (string.IsNullOrEmpty(strafeAnimName))
            return;
        animation.CrossFade(strafeAnimName);
    }

    public void Walk()
    {
        if (string.IsNullOrEmpty(walkAnimName))
            return;
        animation.CrossFade(walkAnimName);
    }

    public void Run()
    {
        if (string.IsNullOrEmpty(runAnimName))
            return;
        animation[runAnimName].speed = 1.5f;
        animation.CrossFade(runAnimName);
    }

    public void Jump()
    {
        if (string.IsNullOrEmpty(jumpAnimName))
            return;
        animation.CrossFade(jumpAnimName);
    }

    public void Swim()
    {
        if (string.IsNullOrEmpty(swimAnimName))
            return;
        animation.CrossFade(swimAnimName);
    }

    public void DeathAnim()
    {
        if (string.IsNullOrEmpty(deathAnimName))
            return;

        animation[deathAnimName].wrapMode = WrapMode.Once;
        animation.CrossFade(deathAnimName);
    }

    public void MeleeAttackAnim()
    {
        if (string.IsNullOrEmpty(meleeAttackAnim))
        {
            Debug.LogWarning("No melee attack animation clip atached");
            return;
        }

        animation[meleeAttackAnim].wrapMode = WrapMode.Once;

        //  Debug.Log("Melee Attack Animation Length: " + attack.length);
        //  Debug.Log("Melee Attack Animaiton Speed: " + animation[attack.name].speed);

        animation[meleeAttackAnim].speed = animation[meleeAttackAnim].length / 2f; // Adjust Animation Length

        //  Debug.Log("Melee Attack Animaiton Speed Edited: " + animation[attack.name].speed);
        animation.Play(meleeAttackAnim);
    }

    public void RangedAttackAnim()
    {
        if (string.IsNullOrEmpty(rangedAttackAnim))
        {
            Debug.LogWarning("No ranged attack animation clip atached");
            return;
        }

        animation[rangedAttackAnim].wrapMode = WrapMode.Once;

        //  Debug.Log("Melee Attack Animation Length: " + attack.length);
        //  Debug.Log("Melee Attack Animaiton Speed: " + animation[attack.name].speed);

        animation[rangedAttackAnim].speed = animation[rangedAttackAnim].length / 20f; // Adjust Animation Length
       //   Debug.Log("Melee Attack Animaiton Speed Edited: " + animation[rangedAttackAnim].speed);
        animation.Play(rangedAttackAnim);
    }

    public void SpellCastAnim()
    {
        if(string.IsNullOrEmpty(spellCastAnim))
        {
            Debug.LogWarning("No spell animation clip attached");
            return;
        }

        animation[spellCastAnim].wrapMode = WrapMode.Once;
        animation[spellCastAnim].speed = animation[spellCastAnim].length/20f;
        animation.Play(spellCastAnim);
    }
    #endregion
}
