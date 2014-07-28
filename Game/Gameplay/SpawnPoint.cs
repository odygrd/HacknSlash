using UnityEngine;
using System.Collections;

[AddComponentMenu("Objects/Spawn Point")] //Add the script to menu for easier access
public class SpawnPoint : MonoBehaviour {

    //check this flag to see if we can spawn a new mob or not
    public bool available = true;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2);
    }
}
