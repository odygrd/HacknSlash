using UnityEngine;

public class DestroyThisTimed : MonoBehaviour
{
    public float destroyTime;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

}
