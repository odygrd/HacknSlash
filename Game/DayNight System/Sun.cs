///<summary>
///Sun.cs
///July 20,2012
///
/// This script is attached to every sun of our day light system
///</summary>
using UnityEngine;
using System.Collections;

[AddComponentMenu("Day Night Cycle/Sun")]

public class Sun : MonoBehaviour {
	public float maxLightBrightness;
	public float minLightBrightness;
	
	public float maxFlareBrightness;
	public float minFlareBrightness;
	
	public bool giveLight = false;
	
}
