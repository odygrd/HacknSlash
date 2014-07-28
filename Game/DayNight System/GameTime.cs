/// <summary>
/// GameTime.cs
/// July 20, 2012
/// This class is responsible for keeping track of in game time. It will also rotate the suns and moons in the sky based on the current in game time.
/// This class will also change the skybox from the day skybox to the night skybox as time progresses in game.
/// </summary>

using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour
{
  
	public enum TimeOfDay
	{
		Idle,
		SunRise,
		SunSet
	}
    public Transform[] sun;                      //an array to hold all of our suns
    public float dayCycleInMinutes = 1;          //how many real time minutes an in game day will last
	public float sunRise;                        //the time of day that we start the sunrise.
	public float sunSet;                         //the time of day that we start the sunset.
	public float skyboxBlendModifier;            //the speed at which the textures in the skybox blend.

    public Color ambLightMax;                    //the ambient light color at full day
    public Color ambLightMin;                    //the ambient light color at full night

	public float morningLight;                   //The time of the day morning light effects will happen
	public float nightLight;                     //The time of the day night light effects will happen

    private Sun[] _sunScript;                    //an array to hold all of the Sun.cs scripts attached to our sun
    private float _degreeRotation;               //how many degrees we rotate each unit of time
    private float _timeOfDay;                   //track the passage of time through out the day

    private float _dayCycleInSeconds;           //the number of real time seconds in an in game day
    private TimeOfDay _tod;                     //our time of day enumeration value;
    private float _noonTime;                    //this is the time of day when it is noon
    private float _morningLength;               //the length in seconds the morning last
    private float _eveningLength;               //the length is seconds the night lasts

	private bool _isMorning;
	
    //Constants
    private const float SECOND = 1;                           //constant for 1 second
    private const float MINUTE = 60 * SECOND;                //constant for how many seconds in a minute
	private const float HOUR = 60 * MINUTE;
	private const float DAY = 24 * HOUR;
    private const float DEGREE_PER_SECOND = 360 / DAY;      //constant for how many degrees we have to rotate per second a day to do 360 degrees
    
	
	// Use this for initialization
	void Start ()
	{
		_tod = TimeOfDay.Idle;
        _dayCycleInSeconds = dayCycleInMinutes * MINUTE;         //get the number of real time seconds in an in game day
        RenderSettings.skybox.SetFloat("_Blend", 0);             //change our blended skybox to be set to the first skybox in the list
        _sunScript = new Sun[sun.Length];                        //initialize the _sunScript array

        //make sure that all of our suns have the script, if not add it
		for (int i = 0; i<sun.Length; i++) {
			Sun temp = sun [i].GetComponent<Sun> ();
			if (temp == null) {
				Debug.Log ("Sun scipt not found. Adding it");
				sun [i].gameObject.AddComponent<Sun> ();
				temp = sun [i].GetComponent<Sun> ();			
			}
			_sunScript [i] = temp;
		}

        _timeOfDay = 0;                                                 //day starts at 0 seconds
        _degreeRotation = DEGREE_PER_SECOND * DAY / _dayCycleInSeconds; //set the _degreeRotation to the amount of degrees that have to rotate for our day
		
		sunRise *= _dayCycleInSeconds;
		sunSet *= _dayCycleInSeconds;
		morningLight *= _dayCycleInSeconds;
		nightLight *=_dayCycleInSeconds;
		_noonTime = _dayCycleInSeconds / 2;

		_morningLength = _noonTime - sunRise; //The length of the morning in seconds.
		_eveningLength = sunSet - _noonTime;  //The length of the evening in seconds.
		_isMorning = false;
		
		//setup MinLigthing values to start
		SetupLights ();
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		//update the time of the day
		_timeOfDay += Time.deltaTime;
       // Debug.Log(_timeOfDay);
		//if the day timer is over the limit of how long a day lasts, reset the day timer.
		if (_timeOfDay > _dayCycleInSeconds)
			_timeOfDay -= _dayCycleInSeconds;
		
		//control the outside lighting effects according to the time of day.
		if (!_isMorning && _timeOfDay >morningLight && _timeOfDay < nightLight){
			_isMorning=true;
			Messenger<bool>.Broadcast("Morning Light Time",true, MessengerMode.DONT_REQUIRE_LISTENER);
		}
		else if(_isMorning && _timeOfDay >nightLight){
			_isMorning=false;
            Messenger<bool>.Broadcast("Morning Light Time", false, MessengerMode.DONT_REQUIRE_LISTENER);
		}
		
		//position the sun the sky but adjusting the angle that the flare is shining from.
		for (int i = 0; i< sun.Length; i++)
			sun [i].Rotate (new Vector3 (_degreeRotation, 0, 0) * Time.deltaTime);
		
		if (_timeOfDay > sunRise && _timeOfDay < _noonTime) {
			AdjustLighting (true);
		} else if (_timeOfDay > _noonTime && _timeOfDay < sunSet) {
			AdjustLighting (false);
		}
		
		//the sun is past the sunrise point, before the sunset point and the day skybox has not yet fully faded in.
		if (_timeOfDay > sunRise && _timeOfDay < sunSet && RenderSettings.skybox.GetFloat ("_Blend") < 1) {
			_tod = TimeOfDay.SunRise;
			BlendSkyBox ();
		} else if (_timeOfDay > sunSet && RenderSettings.skybox.GetFloat ("_Blend") > 0) {
			_tod = TimeOfDay.SunSet;
			BlendSkyBox ();
		} else {
			_tod = TimeOfDay.Idle;
		}
		//Debug.Log (_timeOfDay.ToString());
	}
	
	private void BlendSkyBox ()
	{
		float temp = 0;
		switch (_tod) {
		case TimeOfDay.SunRise:
			temp = (_timeOfDay - sunRise) / _dayCycleInSeconds * skyboxBlendModifier;
			break;
		case TimeOfDay.SunSet:
			temp = (_timeOfDay - sunSet) / _dayCycleInSeconds * skyboxBlendModifier;
			temp = 1 - temp;
			break;
		}
		
		RenderSettings.skybox.SetFloat ("_Blend", temp);
		//Debug.Log(temp);
	}
	
	private void SetupLights ()
	{
		RenderSettings.ambientLight = ambLightMin;
		for (int i = 0; i<_sunScript.Length; i++) {
			if (_sunScript [i].giveLight) {
				sun [i].GetComponent<Light> ().intensity = _sunScript [i].minLightBrightness;
			}
		}
	}
	
	private void AdjustLighting (bool brighten)
	{
        float pos;                                          //get the position of the sun in the morning sky.

		if (brighten) {
			pos = (_timeOfDay - sunRise) / _morningLength;  //get the position of the sun in the morning sky.
		} else {
			pos = (sunSet - _timeOfDay) / _eveningLength;  //get the position of the sun in the evening sky.
		}

		RenderSettings.ambientLight = new Color(ambLightMin.r + ambLightMax.r *pos,
			                                    ambLightMin.g + ambLightMax.g *pos,
			                                    ambLightMin.b + ambLightMax.b *pos);
		
		for (int i = 0; i < _sunScript.Length; i++) {
			if (_sunScript [i].giveLight) {
				_sunScript [i].GetComponent<Light> ().intensity = _sunScript [i].maxLightBrightness * pos;
			}	
		}
	}
}