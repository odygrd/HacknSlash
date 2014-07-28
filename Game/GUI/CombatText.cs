//Script to display combat text
//October 15, 2012
using UnityEngine;
using System.Collections;

public class CombatText : MonoBehaviour
{
    private float _getHitEffect;
    private float _targY;
    private Vector3 _pointPosition;

    public string effectName;
    public GUISkin PointSkin;
    public GUISkin PointSkinShadow;

    // Use this for initialization
    void Start()
    {
        //set a screen position
        _pointPosition = transform.position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        _targY = Screen.height / 2;
    }

    void Update()
    {
        _targY -= Time.deltaTime * 100;
    }

    void OnGUI()
    {
        Vector3 screenPos2 = Camera.main.camera.WorldToScreenPoint(_pointPosition);
        _getHitEffect += Time.deltaTime * 30;
        GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - (_getHitEffect - 50) / 5);
        if (effectName.Contains("Heal"))
        {
            GUI.skin = PointSkinShadow;
            GUI.Label(new Rect(screenPos2.x + 11, _targY + 2, 300, 70), effectName);
            GUI.skin = PointSkin;
            GUI.Label(new Rect(screenPos2.x + 10, _targY, 300, 70), effectName);
        }
        else if (effectName.Contains("Damage"))
        {
            GUI.skin = PointSkinShadow;
            GUI.Label(new Rect(screenPos2.x + 11, _targY + 2, 300, 70), effectName,"Label Dmg");
            GUI.skin = PointSkin;
            GUI.Label(new Rect(screenPos2.x + 10, _targY, 300, 70), effectName, "Label Dmg");
        }
    }

}
