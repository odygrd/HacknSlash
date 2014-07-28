using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {
    public GameObject destination;

	// Use this for initialization
	void Start () {
        if (destination == null)
        {
            destination = GameObject.Find("dz_Teleport Point");
        }
	
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
           // Debug.Log("Player Enterted");
            other.transform.position = destination.transform.position;
        }
    }
}
