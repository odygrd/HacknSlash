///<summary>
///Chest.cs
///August 23,2012
/// This Script requires a CharacterAsset script with character meshes defined
///</summary>
using UnityEngine;
using System.Collections;

public class ChangingRoom : MonoBehaviour
{

    public GameObject characterAssets; //The gameobject containing the script Character asset

    private GameObject _characterMesh;

    private CharacterAsset _caScript; //reference to character asset script
   // private PlayerCharacter _playercharScript; //reference to base char script

    private string _charModelName;  //stores the current char model name
    private string _weaponModelName; //store the current weapon model name

    //Mesh and Material Indexes
    private int _charModelIndex = 0; //index for character meshes
    private int _weaponIndex = 0; //index for weapons
    private int _hairMeshIndex = 0;     //index for hair styles
     
    private int _bodyMaterialIndex = 0; //index for body material
    private int _headMaterialIndex = 0; //Index for head material
    private int _pantsMaterialIndex = 0; //Index for pants material
    private int _handsMaterialIndex = 0; //Index for hands material
    private int _feetMaterialIndex = 0; //Index for feet material

    // Use this for initialization
    void Start()
    {
        _caScript = characterAssets.GetComponent<CharacterAsset>(); //get reference to the characterasset script from character asset manager gameobject
        InstantiateCharacterModel();
        RefreshCharacterMeshMaterials();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        ChangeCharModel();
        ChangeWeaponModel();
        ChangeCharRotation();
        ChangeBodyMaterialGUI();
        ChangeHeadMaterialGUI();
        ChangePantsMaterialGUI();
        ChangeHandsMaterialGUI();
        ChangeFeetMaterialGUI();
        ChangeHairModelGUI();
    }

    //This method is used to change the character current materials.
    //Our model has 5 materials attached.
    //In unity can't change the materials one by one. Have to store them all in an array, change the one we want and then reassign the whole array.
    private void ChangeMeshMaterial(CharacterMeshMaterial cmm)
    {
        Material[] mats = PlayerChar.Instance.armorMesh.renderer.materials;

        //save all our current materials to the array
        for (int i = 0; i < mats.Length; i++)
            mats[i] = PlayerChar.Instance.armorMesh.renderer.materials[i];

        switch (cmm)
        {
            case CharacterMeshMaterial.Body:
                if (_bodyMaterialIndex > _caScript.bodyMaterials.Length - 1)
                    _bodyMaterialIndex = 0;
                else if (_bodyMaterialIndex < 0)
                    _bodyMaterialIndex = _caScript.bodyMaterials.Length - 1;

                mats[(int)cmm] = _caScript.bodyMaterials[_bodyMaterialIndex];  //replace with the material we want to change

                break;
            case CharacterMeshMaterial.Head:
                 if (_headMaterialIndex > _caScript.headMaterials.Length - 1)
                     _headMaterialIndex = 0;
                else if (_headMaterialIndex < 0)
                     _headMaterialIndex = _caScript.headMaterials.Length - 1;

                 mats[(int)cmm] = _caScript.headMaterials[_headMaterialIndex];
                break;
            case CharacterMeshMaterial.Pants:
                if (_pantsMaterialIndex > _caScript.pantsMaterials.Length - 1)
                    _pantsMaterialIndex = 0;
                else if (_pantsMaterialIndex < 0)
                    _pantsMaterialIndex = _caScript.pantsMaterials.Length - 1;

                mats[(int)cmm] = _caScript.pantsMaterials[_pantsMaterialIndex];
                break;
            case CharacterMeshMaterial.Hands:
                if (_handsMaterialIndex > _caScript.handsMaterials.Length - 1)
                    _handsMaterialIndex = 0;
                else if (_handsMaterialIndex < 0)
                    _handsMaterialIndex = _caScript.handsMaterials.Length - 1;

                mats[(int)cmm] = _caScript.handsMaterials[_handsMaterialIndex];
                break;
            case CharacterMeshMaterial.Feet:
                if (_feetMaterialIndex > _caScript.feetMaterials.Length - 1)
                    _feetMaterialIndex = 0;
                else if (_feetMaterialIndex < 0)
                    _feetMaterialIndex = _caScript.feetMaterials.Length - 1;

                mats[(int)cmm] = _caScript.feetMaterials[_feetMaterialIndex];
                break;
        }

        DestroyImmediate(PlayerChar.Instance.armorMesh.renderer.materials[(int)cmm]); //Destroy not used material

        PlayerChar.Instance.armorMesh.renderer.materials = mats;      //assing the modified material array back to our character model

    }

    //This Method is used to refresh the materials when they are edited before we have changed the model
    private void RefreshCharacterMeshMaterials()
    {
        //this will update the new model with our new material indexes
        for (int i = 0; i < (int)CharacterMeshMaterial.COUNT; i++)
            ChangeMeshMaterial((CharacterMeshMaterial)i);
       // Debug.Log("refresh called");

    }

    #region GUI Controls - Equipment
    //This method displays the gui for the head edit buttons
    private void ChangeHeadMaterialGUI()
    {
        if (GUI.Button(new Rect(Screen.width * 0.5f - 95, 20, 30, 30), "<"))
        {
            _headMaterialIndex--;
            ChangeMeshMaterial(CharacterMeshMaterial.Head);
        }
        GUI.Box(new Rect(Screen.width / 2 - 60, 20, 120, 30), _headMaterialIndex.ToString());

        if (GUI.Button(new Rect(Screen.width * 0.5f + 65, 20, 30, 30), ">"))
        {
            _headMaterialIndex++;
            ChangeMeshMaterial(CharacterMeshMaterial.Head);
        }
    }

    //This method displays the gui for the body edit buttons
    private void ChangeBodyMaterialGUI()
    {
        if (GUI.Button(new Rect(Screen.width * 0.5f - 95, 55, 30, 30), "<"))
        {
            _bodyMaterialIndex--;
            ChangeMeshMaterial(CharacterMeshMaterial.Body);
        }
        GUI.Box(new Rect(Screen.width / 2 - 60, 55, 120, 30), _bodyMaterialIndex.ToString());

        if (GUI.Button(new Rect(Screen.width * 0.5f + 65, 55, 30, 30), ">"))
        {
            _bodyMaterialIndex++;
            ChangeMeshMaterial(CharacterMeshMaterial.Body);
        }
    }

    //This method displays the gui for the Pants edit buttons
    private void ChangePantsMaterialGUI()
    {
        if (GUI.Button(new Rect(Screen.width * 0.5f - 95, 90, 30, 30), "<"))
        {
            _pantsMaterialIndex--;
            ChangeMeshMaterial(CharacterMeshMaterial.Pants);
        }
        GUI.Box(new Rect(Screen.width / 2 - 60, 90, 120, 30),  _pantsMaterialIndex.ToString());

        if (GUI.Button(new Rect(Screen.width * 0.5f + 65, 90, 30, 30), ">"))
        {
            _pantsMaterialIndex++;
            ChangeMeshMaterial(CharacterMeshMaterial.Pants);
        }
    }

    //This method displays the gui for the Hands edit buttons
    private void ChangeHandsMaterialGUI()
    {
        if (GUI.Button(new Rect(Screen.width * 0.5f - 95, 125, 30, 30), "<"))
        {
            _handsMaterialIndex--;
            ChangeMeshMaterial(CharacterMeshMaterial.Hands);
        }
        GUI.Box(new Rect(Screen.width / 2 - 60, 125, 120, 30), _handsMaterialIndex.ToString());

        if (GUI.Button(new Rect(Screen.width * 0.5f + 65, 125, 30, 30), ">"))
        {
            _handsMaterialIndex++;
            ChangeMeshMaterial(CharacterMeshMaterial.Hands);
        }
    }

    //This method displays the gui for the Feet edit buttons
    private void ChangeFeetMaterialGUI()
    {
        if (GUI.Button(new Rect(Screen.width * 0.5f - 95, 160, 30, 30), "<"))
        {
            _feetMaterialIndex--;
            ChangeMeshMaterial(CharacterMeshMaterial.Feet);
        }
        GUI.Box(new Rect(Screen.width / 2 - 60, 160, 120, 30), _feetMaterialIndex.ToString());

        if (GUI.Button(new Rect(Screen.width * 0.5f + 65, 160, 30, 30), ">"))
        {
            _feetMaterialIndex++;
            ChangeMeshMaterial(CharacterMeshMaterial.Feet);
        }
    }

    #endregion

    #region GUI Controls Character and Weapon - Hair Models
    //This method displays the gui to change the whole character models
    private void ChangeCharModel()
    {
        //Button - Change character model
        if (GUI.Button(new Rect(Screen.width / 2 - 60, Screen.height - 70, 120, 30), _charModelName))
        {
            _charModelIndex++;
            InstantiateCharacterModel();
        }
    }

    //This method displays the gui to change the weapon model
    private void ChangeWeaponModel()
    {
        //Button - Change weapon model
        if (GUI.Button(new Rect(Screen.width / 2 - 60, Screen.height - 105, 120, 30), _weaponModelName))
        {
            _weaponIndex++;
            InstantiateWeaponModel();
        }
    }

    //This method displays the gui to change the hair style
    private void ChangeHairModelGUI()
    {
        //Button - Change weapon model
        if (GUI.Button(new Rect(Screen.width / 2 - 60, 195, 120, 30), _hairMeshIndex.ToString()))
        {
            _hairMeshIndex++;
            InstantiateHairModel();
        }
    }

    //This method provides the gui controls to rotate our current char model
    private void ChangeCharRotation()
    {
        //Button - Left Character Rotate
        if (GUI.RepeatButton(new Rect(Screen.width * 0.5f - 95, Screen.height - 70, 30, 30), "<"))
        {
            _characterMesh.transform.Rotate(Vector3.up * Time.deltaTime * 100); //rotate on y axis - up
        }

        //Button - Right Character Rotate
        if (GUI.RepeatButton(new Rect(Screen.width * 0.5f + 65, Screen.height - 70, 30, 30), ">"))
        {
            _characterMesh.transform.Rotate(Vector3.down * Time.deltaTime * 100); //rotate on y axis - down
        }
    }
    #endregion

    #region Model Instatiate Methods
    //Method to instantiate a character model in the scene
    private void InstantiateCharacterModel()
    {
        if (_charModelIndex > _caScript.characterMeshes.Length - 1) //reset the index
            _charModelIndex = 0;

        //Save the existing model rotation so the instatiated one has same
        Quaternion oldRotation;  //var to cache the rotation
        if (_characterMesh.Equals(null))
            oldRotation = transform.rotation;
        else
            oldRotation = _characterMesh.transform.rotation;

        //Destroy Previous Instaces
        if (transform.childCount > 0)
            foreach (Transform child in transform)
                Destroy(child.gameObject);

        //Instantiate the gameobject to current position
        _characterMesh = Instantiate(_caScript.characterMeshes[_charModelIndex], transform.position, Quaternion.identity) as GameObject;
        _characterMesh.transform.parent = transform; //Attach it as child to our gameobject - character mount
        _characterMesh.transform.rotation = oldRotation; //Adjust the new model rotation according to previous one
        _charModelName = _caScript.characterMeshes[_charModelIndex].name; //Get the game object name
        _characterMesh.animation["idle1"].wrapMode = WrapMode.Loop; //animation plays repeatedly
        _characterMesh.animation.Play("idle1"); //play idle animaiton

        //Instantiate the weapon
        //PlayerChar.Instance = _characterMesh.GetComponent<PlayerCharacter>(); //get the playercharacter script frm the instatiated object
        InstantiateWeaponModel();
        InstantiateHairModel();
        RefreshCharacterMeshMaterials();

        Resources.UnloadUnusedAssets(); //to get rid apo the materials we are not using
    }

    //Method to instantiate a weapon model in the scene
    private void InstantiateWeaponModel()
    {
        if (_weaponIndex > _caScript.weaponMeshes.Length - 1)
            _weaponIndex = 0;

        //Destroy Previous Instaces
        if (PlayerChar.Instance.weaponMount.transform.childCount > 0)
            foreach (Transform child in PlayerChar.Instance.weaponMount.transform)
                Destroy(child.gameObject);

        GameObject _weaponMesh = Instantiate(_caScript.weaponMeshes[_weaponIndex], PlayerChar.Instance.weaponMount.transform.position, Quaternion.identity) as GameObject;
        _weaponMesh.transform.parent = PlayerChar.Instance.weaponMount.transform; //Add the instantiated weapon as child to Weapon Mount object
        _weaponMesh.transform.rotation = new Quaternion(0, 0, 0, 0); //reset rotation
        _weaponModelName = _caScript.weaponMeshes[_weaponIndex].name; //get the weapon model name
    }

    //Method to instantiate a hair model in the scene
    private void InstantiateHairModel()
    {
        if (_hairMeshIndex > _caScript.hairMeshes.Length - 1)
           _hairMeshIndex = 0;

        //Destroy Previous Instaces
        if (PlayerChar.Instance.hairMount.transform.childCount > 0)
            foreach (Transform child in PlayerChar.Instance.hairMount.transform)
                Destroy(child.gameObject);

        GameObject _hairMesh = Instantiate(_caScript.hairMeshes[_hairMeshIndex], PlayerChar.Instance.hairMount.transform.position, Quaternion.identity) as GameObject;
        _hairMesh.transform.parent = PlayerChar.Instance.hairMount.transform; //Add the instantiated weapon as child to Weapon Mount object
        _hairMesh.transform.rotation = new Quaternion(0, 0, 0, 0); //reset rotation

        MeshOffset moScript = _hairMesh.GetComponent<MeshOffset>();
        //if there is no meshoffset attached we don't have to continue
        if (moScript == null)
            return;
        _hairMesh.transform.localPosition = moScript.posOffset;
    }
    #endregion

}

//Enum with our character model materials
public enum CharacterMeshMaterial{
    Feet,
    Pants,
    Body,
    Hands,
    Head,
    COUNT
}