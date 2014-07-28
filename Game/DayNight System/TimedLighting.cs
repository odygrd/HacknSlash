///<summary>
/// TimedLighting.cs
/// July 21,2012
/// This script turns on and off the city lights during day-night
///</summary>
using UnityEngine;
using System.Collections;

[AddComponentMenu("Day Night Cycle/Timed Lighting")]

public class TimedLighting : MonoBehaviour {
	public void OnEnable(){
		Messenger<bool>.AddListener("Morning Light Time", OnToggleLight);
	}
	public void OnDisable(){
		Messenger<bool>.RemoveListener("Morning Light Time", OnToggleLight);
	}
	
	private void OnToggleLight(bool morning){
		if(morning){
			GetComponent<Light>().enabled = false;
		}
		else {
			GetComponent<Light>().enabled = true;
		}
			
	}
}
