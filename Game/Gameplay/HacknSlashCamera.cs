/// <summary>
/// HackAndSlashCamera.cs
/// May 29, 2012
/// 
/// This is the camera script
/// This script should be attached to main camera.
/// The player character must have the tag "Player" so the camera can find it
/// 
/// For this script to work properly, the following axis must exist in Unity's Input Manger:
/// Rotate Camera Button				- the button we will press to allow us to rotate the camea with the mouse - Key / Mouse Button
/// Mouse X								- rotate the camera horizontally with the mouse (included by default) - Mouse Movement
/// Mouse Y								- rotate the camera vertically with the mouse (included by default) - Mouse Movement
/// Rotate Camera Horizontal Buttons	- the keyboard buttons to rotate the camera on the x axis - Axis
/// Rotate Camera Vertical Buttons		- the keyboard buttons to rotate the camera on the y axis - Axis
/// Reset Camera						- the button to reset the camera to the default position - Button
///
/// </summary>
using UnityEngine;
using System.Collections;
using System;

[AddComponentMenu("Camera/Rpg Camera")] //Add script to menu for easier access
public class HacknSlashCamera : MonoBehaviour
{
	public Transform target;
	public string cameraTargetTag = "Camera Target";
    public float minzoom = 6;
	public float maxzoom = 18;
	public float height= 1;
	public float xSpeed = 250f;
	public float ySpeed = 120f;
	public float heightDamping = 2f;
	public float rotationDamping = 3f;
	private Transform _myTransform;
	private float _x;
	private float _y;
	private bool _camButtonDown;
	private bool _rotateCameraKeyPressed;
    private float distance = 15;

	void Awake ()
	{
        _myTransform = transform; //cache our transform so we do not need to look it up all of the time
	}
	// Use this for initialization
	void Start ()
	{
		_camButtonDown = false;
		_rotateCameraKeyPressed = false;
        target = GameObject.FindGameObjectWithTag(cameraTargetTag).transform;
        if (target == null) //if we do not have a target, let them know, else set the camera up according to where our target it.
			Debug.LogWarning ("Main Camera has no target");
		else 
        {
			CameraSetup ();
		}

	}

	void Update ()
	{
        ////detect if the player has entered any input
        if (Input.GetButtonDown("Rotate Camera Button"))
        {  //Use the Input Manager to make this a user selectable button
            _camButtonDown = true;
        }
        if (Input.GetButtonUp("Rotate Camera Button"))
        {
            _x = 0;		//reset the x value
            _y = 0;		//reset the y value
            _camButtonDown = false;
        }
        if (Input.GetButtonDown("Rotate Camera Horizontal Buttons") || Input.GetButtonDown("Rotate Camera Vertical Buttons"))
        {
            _rotateCameraKeyPressed = true;
        }
        if (Input.GetButtonUp("Rotate Camera Horizontal Buttons") || Input.GetButtonUp("Rotate Camera Vertical Buttons"))
        {
            _x = 0;             //reset the x value
            _y = 0;             //reset the y value
            _rotateCameraKeyPressed = false;
        }
        
        // Change the zoom level of the camera
        distance = Mathf.Clamp(distance + Math.Sign(Input.GetAxis("Mouse ScrollWheel")) * -1f, minzoom, maxzoom);
		
	}

    //this function is called after all of the Update functions are done.
	void LateUpdate ()
	{	
		if (target != null) { //as long as we have a target, we can move the camera around them
			if (_rotateCameraKeyPressed) {
                _x += Input.GetAxis("Rotate Camera Horizontal Buttons") * xSpeed * 0.02f; //Use the Input Manager to make this a user selectable button
                _y -= Input.GetAxis("Rotate Camera Vertical Buttons") * ySpeed * 0.02f; //Use the Input Manager to make this a user selectable button
				RotateCamera ();
			} else if (_camButtonDown) { //if the button is being held down this frame
                _x += Input.GetAxis("Mouse X") * xSpeed * 0.02f; //Use the Input Manager to make this a user selectable button
                _y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f; //Use the Input Manager to make this a user selectable button
				RotateCamera ();
			} else {
				//	_myTransform.position = new Vector3 (target.position.x, target.position.y + height, target.position.z - walkDistance);
				//	_myTransform.LookAt (target);	
				
				// Calculate the current rotation angles
				float wantedRotationAngle = target.eulerAngles.y;
				float wantedHeight = target.position.y + height;
		
				float currentRotationAngle = _myTransform.eulerAngles.y;
				float currentHeight = _myTransform.position.y;
	
				// Damp the rotation around the y-axis
				currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

				// Damp the height
				currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);

				// Convert the angle into a rotation
				Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
	
				// Set the position of the camera on the x-z plane to:
				// distance meters behind the target
				_myTransform.position = target.position;
				_myTransform.position -= currentRotation * Vector3.forward * distance;

				// Set the height of the camera
				_myTransform.position = new Vector3 (_myTransform.position.x, currentHeight, _myTransform.position.z);
	
				// Always look at the target
				_myTransform.LookAt (target);
			}
		} else { //if we do not have a target, try to find it and assign it to the target variable
			GameObject go = GameObject.FindGameObjectWithTag (cameraTargetTag);
			if (go) {
				target = go.transform;
				Debug.LogWarning ("Main Camera target set to " + go.name);
			}
		}
	}

    //set the camera to a default position behind the player and facing them
	public void CameraSetup ()
	{
		_myTransform.position = new Vector3 (target.position.x, target.position.y + height, target.position.z - distance);
		_myTransform.LookAt (target);
	}
	
	private void RotateCamera ()
	{
		Quaternion rotation = Quaternion.Euler (_y, _x, 0);
		Vector3 position = rotation * new Vector3 (0f, 0f, -distance) + target.position;

        _myTransform.rotation = rotation; //set the rotation of the camera
        _myTransform.position = position; //set the position of the camera
	}
}
