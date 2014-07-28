//This script is used by the minimap camera to follow the player
using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public bool minimapcamera ;
    public GameObject target;

    // Use this for initialization
    void Start()
    {

    }

    public Texture2D BlipTex;

   // public GameObject Player;

    private void OnGUI()
    {
         if (target == null)
         {
             target = GameObject.Find("Player Character");
         }

        if (minimapcamera)
         transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

        var objPos = camera.WorldToViewportPoint(target.transform.position);

        if (minimapcamera)
        {
            GUI.DrawTexture(new Rect(
                                Screen.width*(camera.rect.x + (objPos.x*camera.rect.width)) - 2,
                                Screen.height*(1 - (camera.rect.y + (objPos.y*camera.rect.height))) - 2,
                                6, 6), BlipTex);
        }
        else
        {
            GUI.DrawTexture(new Rect(
                                Screen.width * (camera.rect.x + (objPos.x * camera.rect.width)) - 2,
                                Screen.height * (1 - (camera.rect.y + (objPos.y * camera.rect.height))) - 2,
                                12, 12), BlipTex);
        }

    }

    //void LateUpdate()
    //{
    //    if (target == null)
    //    {
    //        target = GameObject.Find("Player Character");
    //    }

    //    transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
    //}
}
