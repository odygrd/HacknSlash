// PlayerInput.cs
// June 27, 2012
// 
// This script manages the player's input
// The below keys have to be set in Unity's Input Manager
// 
// Toggle Inventory 			- Open and close the inventory window - Key
// Toggle Character Window		- Open and close the character information window - Key
// Move Forward				- Axis keys to move the character forward or backwards - Axis
// Rotate Player				- Axis keys to turn the player left and right - Axis
// Strafe						- Axis keys to have the character move side to side - Axis
// Jump						- Button to use to make the character jump - Key
// Run							- Toggle to use to have the character run or walk - Key

using UnityEngine;

[AddComponentMenu("Player/All Player Scripts")] //add the script to unity menu - for easier access
[RequireComponent(typeof(Movement))] //Movement script automaticly imported
[RequireComponent(typeof(PlayerChar))]

public class PlayerInput : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
  
            if (Input.GetButtonUp("Toggle Inventory"))
            {
                Messenger.Broadcast("ToggleInventory"); //listener toggle inventory in myGUI script
            }

            if (Input.GetButtonUp("Toggle Character Window"))
            {
                Messenger.Broadcast("ToggleCharacterWindow"); //listener toggle character window
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //first cancel players current target
                if (PlayerChar.Instance.selectedTarget != null)
                {
                    PlayerChar.Instance.cancelTarget();
                }
                else
                {
                    Messenger.Broadcast("ToggleMainMenu");
                }
            }

            if (Input.GetButton("Move Forward"))
            {
                if (Input.GetAxis("Move Forward") > 0)
                {
                    SendMessage("MoveMeForward", Movement.Forward.forward);
                }
                else
                {
                    SendMessage("MoveMeForward", Movement.Forward.back);
                }
            }

            if (Input.GetButtonUp("Move Forward"))
            {
                SendMessage("MoveMeForward", Movement.Forward.none);
            }

            if (Input.GetButton("Rotate Player"))
            {
                if (Input.GetAxis("Rotate Player") > 0)
                {
                    SendMessage("RotateMe", Movement.Turn.right);
                }
                else
                {
                    SendMessage("RotateMe", Movement.Turn.left);
                }
            }

            if (Input.GetButtonUp("Rotate Player"))
            {
                SendMessage("RotateMe", Movement.Turn.none);
            }

            if (Input.GetButtonUp("Strafe"))
            {
                SendMessage("Strafe", Movement.Turn.none);
            }

            if (Input.GetButton("Strafe"))
            {
                if (Input.GetAxis("Strafe") > 0)
                {
                    SendMessage("Strafe", Movement.Turn.right);
                }
                else
                {
                    SendMessage("Strafe", Movement.Turn.left);
                }
            }

            if (Input.GetButtonUp("Strafe"))
            {
                SendMessage("Strafe", Movement.Turn.none);
            }

            if (Input.GetButtonDown("Jump"))
            {
                SendMessage("JumpMe");
            }
            if (Input.GetButtonDown("Run"))
            {
                SendMessage("ToggleRun");
            }

            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        PlayerChar.Instance.targetEnemy(hit.transform.parent);
                    }
            }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
            SendMessage("IsSwimming", true);
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
            SendMessage("IsSwimming", false);
    }
}
