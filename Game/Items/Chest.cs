///<summary>
///Chest.cs
///August 16,2012
///Script to manage opening-closing of chests.
///Chest part meshes need to be assinged in the Parts section for the highlight color
///</summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Objects/Chest")] //Add script to menu for easier access

//Required BoxCollider and AudioSource
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]

public class Chest : MonoBehaviour
{
    public enum State  //track chests states
    {
        Open,
        Close,
        Between
    }

    //Define animation names
    public string openAminName;
    public string closeAnimName;

    public AudioClip OpenSound; // The sound clip to play when the chest is opened
    public GameObject particleEffect; //The particle effect that will be shown when the chest is opened.
    public float maxDistance = 2; //The max distance that the player will be able to open the chest 
    public GameObject[] parts; //the parts(meshes) of our object.
    public List<Item> loot = new List<Item>(); //the loot in the chest

    public static float defaultLifeTime; // how long the chest will last, by default

    private float _lifetimer; //for how much time is the chest is already spawn.
    private Transform _myTransform; //current chest transform
    private Color[] _defaultColors; //Array to save the default material colors of the chest parts
    private bool _used; //Track if the chest has been used
    private GameObject _player; // current player using the chest
    private State _state; //Used to store the chest's current position
    private bool _inUse; //Is the chest in use?

    // Use this for initialization
    void Start()
    {
        _myTransform = transform; //chest transform cache
        _state = State.Close;
        _defaultColors = new Color[parts.Length]; //create the array with size same as the parts number we defined
        _inUse = false;
        _used = false;
        defaultLifeTime = 180;
        _lifetimer = 0;
        if (!particleEffect.Equals(null))
            particleEffect.active = false;
        //get the color of each part to defaultcolors array
        if (parts.Length > 0)
            for (int cnt = 0; cnt < _defaultColors.Length; cnt++)
                _defaultColors[cnt] = parts[cnt].renderer.material.GetColor("_Color");

    }

    void Update()
    {
        //increase the timer
        _lifetimer += Time.deltaTime;

        //Destroy the chest if is up longer than xx minutes and it is not open
        if (_lifetimer > defaultLifeTime && _state == State.Close)
            DestroyChest();

        if (!_inUse)
            return;

        if (_player == null)
            return;

        if (Vector3.Distance(_myTransform.position, _player.transform.position) > maxDistance) //check chest and player distance
            myGUI.chest.ForceClose();

    }

    public void OnMouseEnter()
    {
        //when mouse enters pass true for a highlight value
        HighLight(true);
    }

    public void OnMouseExit()
    {
        HighLight(false);
    }

    //This method called when the user clicks with his mouse
    public void OnMouseUp()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go == null)
            return;
        if (Vector3.Distance(_myTransform.position, go.transform.position) > maxDistance && !_inUse) //if player is away from chest and the chest is not in use return
            return;

        switch (_state)
        {
            case State.Open: //If the selected chest is open, close it
                _state = State.Between;
                ForceClose();
                break;
            case State.Close: //if the selected chest is closed, first check for any open chests then open it.
                if (myGUI.chest != null) //if an other chest is open already close it
                {
                    myGUI.chest.ForceClose();
                }
                _state = State.Between;
                StartCoroutine("Open");
                break;
        }

    }

    //Co-routine to Open the chest
    private IEnumerator Open()
    {
        myGUI.chest = this; //assing the chest
        _player = GameObject.FindGameObjectWithTag("Player"); //get the current player
        animation.Play(openAminName);
        audio.PlayOneShot(OpenSound);

        if (!_used) //if not used generate loot
            PopulateChest(5);

        yield return new WaitForSeconds(animation[openAminName].length); //wait for the animation to finish
        _state = State.Open;
        _inUse = true; //set the chest in use
        if (!particleEffect.Equals(null))
            particleEffect.active = true;  //enable the particle effect
        Messenger.Broadcast("DisplayLoot");

    }

    private IEnumerator Close()
    {
        _player = null;
        _inUse = false;
        animation.Play(closeAnimName);
        if (!particleEffect.Equals(null))
            particleEffect.active = false;
        audio.PlayOneShot(OpenSound);
        float tempTimer = animation[closeAnimName].length;
        //if sound is longer than animation, swap the timer to sound
        if (OpenSound.length > tempTimer)
            tempTimer = OpenSound.length;
        yield return new WaitForSeconds(tempTimer);
        _state = State.Close;

        if (loot.Count == 0)
            DestroyChest();
    }

    //Method to add a highlight color to a selected chest
    private void HighLight(bool glow)
    {
        if (glow)
        {
            //change the color of each part
            if (parts.Length > 0)
                for (int cnt = 0; cnt < _defaultColors.Length; cnt++)
                    for (int matcount = 0; matcount < parts[cnt].renderer.materials.Length; matcount++) //check how many materials the object has and change every color
                    parts[cnt].renderer.materials[matcount].SetColor("_Color", Color.yellow);
        }
        else
        {
            //restore the part color to it's first default value.
            if (parts.Length > 0)
                for (int cnt = 0; cnt < _defaultColors.Length; cnt++)
                    for (int matcount = 0; matcount < parts[cnt].renderer.materials.Length; matcount++)
                    parts[cnt].renderer.materials[matcount].SetColor("_Color", _defaultColors[cnt]);
        }
    }

    private void DestroyChest()
    {
        loot = null;
        Destroy(gameObject);
    }

    //Method to close the chest
    public void ForceClose()
    {
        Messenger.Broadcast("CloseChest");
        StopCoroutine("Open");
        StartCoroutine("Close");
    }

    //When called by chest script, it sets loot window display to true and generates items.
    private void PopulateChest(int x)
    {
        for (int cnt = 0; cnt < x; cnt++)
        {
            loot.Add(ItemGenerator.CreateItem());
        }
        _used = true;
    }

}
