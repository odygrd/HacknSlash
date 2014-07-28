using UnityEngine;
using System.Collections;

public class PlayerModelCustomization : MonoBehaviour
{
    public GUISkin guiSkin;

    //Character scale vars
    public float minCharHeight = 0.75f;
    public float maxCharHeight = 1.05f;

    public Texture2D[] skinTextures; //aray with the texture icons of skin color
    public Texture2D[] genderTextures; //aray with the texture icons for gender textures

    public int maxNumHairMeshes; //The total number of hair Meshes;
    public int maxNumHairTextures; // Thetotalnumber of the HairTextures
    public int maxNumHeadTypes; //The total number of head types
    
    public GameObject hairMesh; //gameobject to instatiate the hair mesh

    private float _charHeight = 1; //Used for gui scroll bar
    private bool _usingMaleModel = true;

    private int _modelIndex = 0; //the index for the current model, reset everytime we chnage sex
    private int _headIndex = 1; //index to head type
    private int _hairIndex = 0; //to keep track of the current hairmesh 

    private int _skinTextureIndex = 1; //skin texture index
    private int _hairTextureIndex = 0; //hair color texture pointer

    private Material _headMaterial;
    private Material _handsMaterial;

    private GameObject _characterMesh;

    public static int skinColor = 1;

    private bool _zoom = false;
    private string _zoomBtnText = "Zoom In";

    private Rect windowRect = new Rect(20,20,310,260);

    public Transform test;
    // Use  private const string MALE_MODEL_PATH = "Prefabs/Character Models Old/Male"; //male models path in Resources filethis for initialization
    void Start()
    {
       
        if (GameSetting2.maleModels.Length < 1)
            Debug.LogWarning("There are no male models");
        if (GameSetting2.femaleModels.Length < 1)
            Debug.LogWarning("There are no female models");

        InstatiatePlayerModel();
        
    }

    // Update is called once per frame
    void Update()
    {
        //if there is no char mesh return
        if (_characterMesh == null)
            return;

        _characterMesh.transform.localScale = new Vector3(1, _charHeight, 1); //this is used for height adjustment
    }

    void OnGUI()
    {
        GUI.skin = guiSkin;

        ChangeCharRotation();
        ChangePlayerGenderGUI();

        windowRect = GUI.Window(0, windowRect, DoMyWindow, "","MainMenuWindow");
    }

    void DoMyWindow(int windowID)
    {
        ChangePlayerBodyTypeGUI();
        ChangePlayerHeadGUI();
        ChangePlayerHairGUI();
        ChangePlayerSkinColorGUI();
        ChangePlayerHairColorGUI();
        ChangePlayerHeightGUI();
    }

    /// <summary>
    /// Method to save the char customization
    /// Uses the Gamesettings script
    /// </summary>
    private void SaveCustommizations()
    {
        GameSetting2.SaveGender(_usingMaleModel);
        GameSetting2.SaveCharacterModelIndex(_modelIndex);
        GameSetting2.SaveHead(_headIndex);
        GameSetting2.SaveHair(_hairIndex, _hairTextureIndex);
        GameSetting2.SaveSkinColor(_skinTextureIndex);
        GameSetting2.SaveCharacterHeight(_charHeight);
        GameSetting2.SaveGameVersion();
            
    }

    /// <summary>
    ///  Method to zoom in and out
    /// </summary>
    /// <param name="zoombutton"></param>
    private void CameraZoom(bool zoombutton)
    {
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        if (!zoombutton)
        {
            cam.transform.position = new Vector3(0, 1.41f, -4.35f);
            _zoomBtnText = "Zoom In";
        }
        else
        {
            
            if (_charHeight < 0.9f)
            cam.transform.position = new Vector3(0, 0.95f + _charHeight, -0.58f);
            else if (_charHeight > 1.01)
                cam.transform.position = new Vector3(0, 1.16f + _charHeight, -0.58f);
            else
              cam.transform.position = new Vector3(0, 1.12f + _charHeight, -0.58f);
            _zoomBtnText = "Zoom Out";
        }
    }

    #region Instantiates
    //Method to instatiate character model in the scene
    private void InstatiatePlayerModel()
    {
        //Destroy Previous Instaces
        if (transform.childCount > 0)
            foreach (Transform child in transform)
                Destroy(child.gameObject);

        //Instantiate the gameobject to current position
        if (_usingMaleModel)
            _characterMesh = Instantiate(Resources.Load(GameSetting2.MALE_MODEL_PATH + GameSetting2.maleModels[_modelIndex]), transform.position, transform.rotation) as GameObject;
        else
            _characterMesh = Instantiate(Resources.Load(GameSetting2.FEMALE_MODEL_PATH + GameSetting2.femaleModels[_modelIndex]), transform.position, transform.rotation) as GameObject;

        //Destroy the player input so the character cant move during the customization scene
        Destroy(_characterMesh.GetComponent<PlayerInput>());
        Destroy(_characterMesh.GetComponent<PlayerGUI>());
        Destroy(_characterMesh.GetComponent<ThirdPersonController>());
        Destroy(_characterMesh.GetComponent<NetworkCharacter>());
        Destroy(_characterMesh.GetComponent<PhotonView>());
        _characterMesh.transform.parent = transform; _characterMesh.transform.parent = transform; //Attach it as child to our gameobject - character mount

        //Meshoffset script - if the character model has one 
        MeshOffset mo = _characterMesh.GetComponent<MeshOffset>();
        if (mo != null)
        {
            //the the offset to the items current position
            mo.transform.position = new Vector3(
                mo.transform.position.x + mo.posOffset.x,
                mo.transform.position.y + mo.posOffset.y,
                mo.transform.position.z + mo.posOffset.z);
        }

        //If the character has animation attached and idle animation
        if (_characterMesh.GetComponent<Animation>())
        {
            if (_characterMesh.animation["idle1"])
            {
                _characterMesh.animation["idle1"].wrapMode = WrapMode.Loop; //animation plays repeatedly
                _characterMesh.animation.Play("idle1"); //play idle animaiton
            }
        }
        // Debug.Log("used");
        ChangePlayerFace();
        InstatitatePlayerHair();
        ChangePlayerHairColor();
        ChangePlayerSkinColor();

        Resources.UnloadUnusedAssets(); //to get rid apo the materials we are not using

    }

    //Method to instatiat players Hair
    private void InstatitatePlayerHair()
    {

        if (_hairIndex > maxNumHairMeshes - 1)
            _hairIndex = 0;
        else if (_hairIndex < 0)
            _hairIndex = maxNumHairMeshes - 1;

        //Delete previous isntances
        if (PlayerChar.Instance.hairMount.transform.childCount > 0)
            foreach (Transform child in PlayerChar.Instance.hairMount.transform)
                Destroy(child.gameObject);

        int hairset = _hairIndex / 5 + 1;
        int hairMeshNum = _hairIndex % 5 + 1;

        if (_usingMaleModel)
        hairMesh = Instantiate(Resources.Load(GameSetting2.MALE_MODEL_PATH + "Hair/Hair0" + hairset + "_" + hairMeshNum)) as GameObject;
        else
        hairMesh = Instantiate(Resources.Load(GameSetting2.FEMALE_MODEL_PATH + "Hair/Hair0" + hairset + "_" + hairMeshNum)) as GameObject;

        hairMesh.transform.parent = PlayerChar.Instance.hairMount.transform; //mount them to char hair mount point
        
        // Debug.Log(MALE_MODEL_PATH + "Hair/Hair0" + hairset + "_" + hairMeshNum);
        //reset local position
        hairMesh.transform.localPosition = Vector3.zero;
        hairMesh.transform.localRotation = Quaternion.identity;
        hairMesh.transform.localScale = new Vector3(4.2f, 4.2f, 4.2f);

        //check for meshoffset script n apply values
        MeshOffset meshOffsetScript = hairMesh.GetComponent<MeshOffset>();
        if (meshOffsetScript == null)
            return;
        hairMesh.transform.localPosition = meshOffsetScript.posOffset;
        hairMesh.transform.localRotation = Quaternion.Euler(meshOffsetScript.rotationOffset);
        hairMesh.transform.localScale = meshOffsetScript.scaleOffset;

    }
    #endregion

    #region Player Looks

    //Method to change the bool value of usingmalemodel var.
    //This method is called by the ChangeGender calls through the Messenger
    public void ToggleGender(bool ismale)
    {
        if (ismale)
            _usingMaleModel = true;
        else
            _usingMaleModel = false;
        _modelIndex = 0; //reset index to zero
        InstatiatePlayerModel();
    }

    //Method to change player skin color 
    private void ChangePlayerSkinColor()
    {
        //check for the index limits
        if (_skinTextureIndex > skinTextures.Length)
            _skinTextureIndex = 1;
        else if (_skinTextureIndex < 1)
            _skinTextureIndex = skinTextures.Length;

        //get reference to materials from the player model
     //   _handsMaterial = _characterMesh.GetComponent<PlayerCharacter>().armorMesh.renderer.materials[3];
      //  _headMaterial = _characterMesh.GetComponent<PlayerCharacter>().armorMesh.renderer.materials[4];

        //Debug.Log(HEAD_TEXTURE_PATH + "0" + _headIndex + "_" + _skinTextureIndex + ".head.human");
         PlayerChar.Instance.armorMesh.renderer.materials[4].mainTexture = Resources.Load(GameSetting2.MALE_TEXTURE_PATH + "Head/0" + _headIndex + "_" + _skinTextureIndex + ".head.human") as Texture2D;
         PlayerChar.Instance.armorMesh.renderer.materials[3].mainTexture = Resources.Load(GameSetting2.MALE_TEXTURE_PATH + "Hands/0" + _skinTextureIndex + ".hands") as Texture2D;
    }

    private void ChangePlayerHairColor()
    {
        // - for hair textures
        if (_hairTextureIndex > maxNumHairTextures - 1)
            _hairTextureIndex = 0;
        else if (_hairTextureIndex < 0)
            _hairTextureIndex = maxNumHairTextures - 1;

        hairMesh.renderer.material.mainTexture = Resources.Load(GameSetting2.MALE_TEXTURE_PATH + "Hair/Hair_" + _hairTextureIndex) as Texture2D;
    }

    private void ChangePlayerBodyType()
    {
        int maxModelNum;
        if (_usingMaleModel)
            maxModelNum = GameSetting2.maleModels.Length;
        else
            maxModelNum = GameSetting2.femaleModels.Length;

        if (_modelIndex > maxModelNum - 1)
            _modelIndex = 0;
        else if (_modelIndex < 0)
            _modelIndex = maxModelNum - 1;

        InstatiatePlayerModel();

    }

    private void ChangePlayerFace()
    {
        if (_headIndex > maxNumHeadTypes)
            _headIndex = 1;
        else if (_headIndex < 1)
            _headIndex = maxNumHeadTypes;

          PlayerChar.Instance.armorMesh.renderer.materials[4].mainTexture = Resources.Load(GameSetting2.MALE_TEXTURE_PATH + "Head/0" + _headIndex + "_" + _skinTextureIndex + ".head.human") as Texture2D;
    }

    #endregion

    #region GUI Controls
    //This method provides the gui controls to rotate our current char model
    private void ChangeCharRotation()
    {
        //Button - Left Character Rotate
        if (GUI.RepeatButton(new Rect(Screen.width * 0.5f - 85, Screen.height - 90, 50, 35), "<"))
        {
            _characterMesh.transform.Rotate(Vector3.up * Time.deltaTime * 100); //rotate on y axis - up
        }

        //Button - Zoom In - Out
        if (GUI.Button(new Rect(Screen.width * 0.5f - 50, Screen.height - 90, 100, 35), _zoomBtnText))
        {
            _zoom = !_zoom;
            CameraZoom(_zoom);
        }

        //Button - Right Character Rotate
        if (GUI.RepeatButton(new Rect(Screen.width * 0.5f + 35, Screen.height - 90, 50, 35), ">"))
        {
            _characterMesh.transform.Rotate(Vector3.down * Time.deltaTime * 100); //rotate on y axis - down
        }

        //Button - Create Char
        if (GUI.Button(new Rect(Screen.width * 0.5f - 55, Screen.height - 50, 110, 35), "Create"))
        {
            SaveCustommizations();
            Application.LoadLevel(GameSetting2.levelNames[5]);
        }
    }

    //This method displays the gui to change player body type
    private void ChangePlayerBodyTypeGUI()
    {
        GUI.Label(new Rect(30,25, 80, 35), "Body Type", "CustomCursedText");
        if (GUI.Button(new Rect(30 + 80, 20, 50, 35), "<"))
        {
            _modelIndex--;
            ChangePlayerBodyType();
        }

        if (_usingMaleModel)
            GUI.Label(new Rect(110+40, 20, 100, 35), GameSetting2.maleModels[_modelIndex], "Button");
        else
            GUI.Label(new Rect(110 + 40, 20, 100, 35), GameSetting2.femaleModels[_modelIndex], "Button");

        if (GUI.Button(new Rect(150+90, 20, 50, 35), ">"))
        {
            _modelIndex++;
            ChangePlayerBodyType();
        }
    }

    //This method displays the gui to change player head type
    private void ChangePlayerHeadGUI()
    {
        GUI.Label(new Rect(30, 60, 80, 35), "Face", "CustomCursedText");
        if (GUI.Button(new Rect(30+80, 55, 50, 35), "<"))
        {
            _headIndex--;
            ChangePlayerFace();
        }

        GUI.Label(new Rect(110 + 40, 55, 100, 35), _headIndex.ToString(), "Button");

        if (GUI.Button(new Rect(150 + 90, 55, 50, 35), ">"))
        {
            _headIndex++;
            ChangePlayerFace();
        }
    }

    //This method displays the gui to change player hair type
    private void ChangePlayerHairGUI()
    {
        GUI.Label(new Rect(30, 95, 80, 35), "Hair", "CustomCursedText");
        if (GUI.Button(new Rect(110, 90, 50, 35), "<"))
        {
            _hairIndex--;
            InstatitatePlayerHair();
        }

        GUI.Label(new Rect(150, 90, 100, 35), (_hairIndex + 1).ToString(), "Button");

        if (GUI.Button(new Rect(240, 90, 50, 35), ">"))
        {
            _hairIndex++;
            InstatitatePlayerHair();
        }
    }

    //This method displays the gui to change player skin color
    private void ChangePlayerSkinColorGUI()
    {
        GUI.Label(new Rect(30, 130, 80, 35), "Skin Color", "CustomCursedText");
        if (GUI.Button(new Rect(110, 125, 50, 35), "<"))
        {

            _skinTextureIndex--;
            ChangePlayerSkinColor();
        }

        GUI.Label(new Rect(175, 125, 50, 35), skinTextures[_skinTextureIndex - 1], "Button");

        if (GUI.Button(new Rect(240, 125, 50, 35), ">"))
        {
            _skinTextureIndex++;
            ChangePlayerSkinColor();
        }
    }

    private void ChangePlayerHairColorGUI()
    {
        GUI.Label(new Rect(30, 165, 80, 35), "Hair Color", "CustomCursedText");
        if (GUI.Button(new Rect(110, 160, 50, 35), "<"))
        {
            _hairTextureIndex--;
            ChangePlayerHairColor();
        }
        
        GUI.Label(new Rect(175, 160, 50, 35), Resources.Load(GameSetting2.MALE_TEXTURE_PATH + "Hair/Hair_" + _hairTextureIndex) as Texture2D, "Button");

        if (GUI.Button(new Rect(240, 160, 50, 35), ">"))
        {

            _hairTextureIndex++;
            ChangePlayerHairColor();

        }
    }

    //Gui for char height
    private void ChangePlayerHeightGUI()
    {
        float oldHeightValue = _charHeight;
        GUI.Label(new Rect(30, 200, 80, 35), "Height", "CustomCursedText");
        _charHeight = GUI.HorizontalSlider(new Rect(125, 205, 135, 20), _charHeight, minCharHeight, maxCharHeight);
       //call this when the slider changes to zoom out
        if (_charHeight != oldHeightValue)
        {
            _zoom = false;
            CameraZoom(_zoom);
        }
    }

    //Method for Gui Gender Change
    private void ChangePlayerGenderGUI()
    {
        if (GUI.Button(new Rect(Screen.width * .02f, Screen.height * .8f, 80, 80), genderTextures[0]))
            ToggleGender(true);


        if (GUI.Button(new Rect(Screen.width * .02f + 65, Screen.height * .8f, 80, 80), genderTextures[1]))
            ToggleGender(false);
    }


    #endregion



}

