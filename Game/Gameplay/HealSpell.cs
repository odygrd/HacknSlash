using UnityEngine;
using System.Collections;

public class HealSpell : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        foreach (var collider in Physics.OverlapSphere(transform.position, 5.0f)) //create a sphere around particle
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                var healeffect = Instantiate(Resources.Load(GameSetting2.EFFECTS_PATH + "heal"), collider.gameObject.transform.position,
                                             transform.rotation) as GameObject;
                if (healeffect != null) healeffect.transform.parent = collider.gameObject.transform;
                int heal = Random.Range(400, 700);
                collider.gameObject.SendMessage("GetHeal",heal);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
