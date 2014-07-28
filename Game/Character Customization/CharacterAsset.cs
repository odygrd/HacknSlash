using UnityEngine;
using System.Collections;

public class CharacterAsset : MonoBehaviour {
    public GameObject[] characterMeshes;             //Contains the different character models prefabs
    public GameObject[] weaponMeshes;               //Contains the weapons prefeabs
    public GameObject[] hairMeshes;                 //Contains hair prefeabs
    
    //Materials Arrays
    public Material[] headMaterials;    
    public Material[] bodyMaterials;
    public Material[] pantsMaterials;
    public Material[] handsMaterials;
    public Material[] feetMaterials;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
