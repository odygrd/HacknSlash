using UnityEngine;
using System.Collections;

public class ErrorText : MonoBehaviour {
    private float _getHitEffect;
    private bool _enabled;
    private float _enabletime;
    public string effectName { get; set; }

    public GUISkin PointSkin;
    public GUISkin PointSkinShadow;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!_enabled)
            return;
        if (Time.time > _enabletime)      
            _enabled = false;

	}

    void OnGUI()
    {
        if (_enabled)
        {    
            _getHitEffect += Time.deltaTime*10;
            GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - (_getHitEffect - 40)/5);
            GUI.skin = PointSkinShadow;
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height*0.25f, 400, 70), effectName, "Label Error");
            GUI.skin = PointSkin;
            GUI.Label(new Rect(Screen.width/2 - 199, Screen.height*0.25f - 2, 400, 70), effectName, "Label Error");
        }
    }

    public void DisplayerrorText(string text)
    {
        _enabletime = Time.time + 2;
        _enabled = true;
        _getHitEffect = 0;
        effectName = text;
    }
}
