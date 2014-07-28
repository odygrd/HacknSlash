using UnityEngine;
using System.Collections;

public class RandomDrop : MonoBehaviour {
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //add a random item to player inventory
            PlayerChar.Instance.Inventory.Add((ItemGenerator.CreateItem()));
            Destroy(gameObject);
        }
    }
}
