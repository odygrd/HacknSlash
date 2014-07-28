///<summary>
///MobGenerator.cs
///July 25,2012
/// This script is responsible for making sure that there is a mob for each spawn point.
/// 
/// It should be attached in an empty GameObject called 'Mob Generator'
///</summary>

using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Managers/MobGenerator")] //Add the script to menu for easier access

public class MobGenerator : MonoBehaviour
{
    public GameObject[] mobPrefabs; //an array to store mob prefabs that are going to spawn.
    public GameObject[] spawnPoints; //an array to store the spawn points of the mobs.

    void Start()
    {
        StartCoroutine(test(90)); //call co-routine every 30 seconds, If spawn spots have no child it will spawn new mobs
    }

    /// <summary>
    /// Start this instance.
    /// </summary>
    IEnumerator test(float respawnTime)
    {
        while (true)
        {
            SpawnMob();
            yield return new WaitForSeconds(respawnTime);
        }
    }


    /// <summary>
    /// Spawn a mob if there is a spawn point.
    /// </summary>
    private void SpawnMob()
    {
        if (mobPrefabs.Length > 0 && spawnPoints.Length > 0) //check for mobs and spawnpoints
        {
            //Debug.Log ("SpawnMob function called");
            IEnumerable<GameObject> emptySpawnPoint = AvailableSpawnPoints();

            //for (int i = 0; i < emptySpawnPoint.Length; i++)
            //{
            //    GameObject go = Instantiate(mobPrefabs[Random.Range(0, mobPrefabs.Length)], emptySpawnPoint[i].transform.position,
            //                    Quaternion.identity) as GameObject; //Instantiate a random mob
            //    go.transform.parent = emptySpawnPoint[i].transform;

            //}

            foreach (GameObject spawnPoint in emptySpawnPoint)
            {
                GameObject go =
                    Instantiate(mobPrefabs[Random.Range(0, mobPrefabs.Length)], spawnPoint.transform.position,
                                Quaternion.identity) as GameObject;
                if (go != null)
                    go.transform.parent = spawnPoint.transform;
            }
        }
    }

    /// <summary>
    /// generate list of availbale spawn points that do not have any mobs chiled to them.
    /// </summary>
    /// <returns>
    /// The available spawn points.
    /// </returns>
    private IEnumerable<GameObject> AvailableSpawnPoints()
    {
        //iterate though our spawn points and add the ones that do not have a mob under it to the list
        return spawnPoints.Where(t => t.transform.childCount == 0).ToArray();
    }

}
