using UnityEngine;
using System.Collections;

public class LobbyMenu : MonoBehaviour
{
    public GUISkin myskin;
    public static int playerWhoIsIt = 0;
    private static PhotonView ScenePhotonView;
    private PhotonView myMonsterPv;
    private Rect windowRect;
    void Awake()
    {
        //PhotonNetwork.logLevel = NetworkLogLevel.Full;

        //Connect to the main photon server. This is the only IP and port we ever need to set(!)
        if (!PhotonNetwork.connected)
            PhotonNetwork.ConnectUsingSettings("v1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)

        //Load name from PlayerPrefs
        PhotonNetwork.playerName = GameSetting2.LoadName();

        //Set camera clipping for nicer "main menu" background
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;

    }

    // Use this for initialization
    void Start()
    {
        windowRect = new Rect(0, 0, 1000, 600);
        ScenePhotonView = this.GetComponent<PhotonView>();
    }


    private string _roomName = "Room1";
    private Vector2 _scrollPos = Vector2.zero;

    void OnGUI()
    {
        GUI.skin = myskin;
        if (!PhotonNetwork.connected)
        {
            ShowConnectingGUI();
            return; //Wait for a connection
        }


        if (PhotonNetwork.room != null)
            return; //Only when we're not in a Room

        windowRect = GUI.Window(0, windowRect, DoMyWindow, "Enter Arena");
    }


    void DoMyWindow(int windowID)
    {
        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Game Lobby Menu");

        //Player name
        GUILayout.BeginHorizontal();
        GUILayout.Label("Player name: " + PhotonNetwork.playerName, "BoldOutlineText");
        GUILayout.EndHorizontal();

        GUILayout.Space(15);


        //Create a room (fails if exist!)
        GUILayout.BeginHorizontal();
        GUILayout.Label("CREATE ROOM:");
        _roomName = GUILayout.TextField(_roomName);
        if (GUILayout.Button("Create"))
        {
            PhotonNetwork.CreateRoom(_roomName, true, true, 10);
        }
        GUILayout.EndHorizontal();

        //Join room by title
        GUILayout.BeginHorizontal();
        GUILayout.Label("JOIN ROOM:");
        _roomName = GUILayout.TextField(_roomName);
        if (GUILayout.Button("Join"))
        {
            PhotonNetwork.JoinRoom(_roomName);
        }
        GUILayout.EndHorizontal();


        //Join random room
        GUILayout.BeginHorizontal();
        GUILayout.Label("JOIN RANDOM ROOM:");
        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            GUILayout.Label("no games available", "CursedText");
        }
        else
        {
            if (GUILayout.Button("Join"))
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.Label("ROOM LISTING:");
        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            GUILayout.Label("no games available", "CursedText");
        }
        else
        {
            //Room listing: simply call GetRoomList: no need to fetch/poll whatever!
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            foreach (RoomInfo game in PhotonNetwork.GetRoomList())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(game.name + " " + game.playerCount + "/" + game.maxPlayers);
                if (GUILayout.Button("Join"))
                {
                    PhotonNetwork.JoinRoom(game.name);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        GUILayout.EndArea();
    }


    void ShowConnectingGUI()
    {
        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Connecting to Photon server.");

        GUILayout.EndArea();
    }


    void OnJoinedRoom()
    {
        // game logic: if this is the only player, we're "it"
        if (PhotonNetwork.playerList.Length == 1)
        {
            playerWhoIsIt = PhotonNetwork.player.ID;
        }

        Debug.Log("playerWhoIsIt: " + playerWhoIsIt);
        Camera.main.farClipPlane = 1000f;
        GameObject monster = PhotonNetwork.Instantiate(GameSetting2.MALE_MODEL_PATH + "Muscular", new Vector3(860, 150, 870), Quaternion.identity, 0);
      //  monster.GetComponent<PlayerInput>().enabled = false;
      //  monster.GetComponent<Movement>().enabled = false;
        //monster.GetComponent<NetworkCharacter>().enabled = true;
       // gameObject.GetComponent<ChatVik>().enabled = true;
        ThirdPersonController contoller = monster.GetComponent<ThirdPersonController>();
        myMonsterPv = monster.GetComponent<PhotonView>();

        contoller.enabled = true;
        gameObject.GetComponent<ChatVik>().enabled = true;
    }

    void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerConnected: " + player);

        // when new players join, we send "who's it" to let them know
        // only one player will do this: the "master"

        if (PhotonNetwork.isMasterClient)
        {
            TagPlayer(playerWhoIsIt);
        }
    }

    public static void TagPlayer(int playerID)
    {
        Debug.Log("TagPlayer: " + playerID);

        ScenePhotonView.RPC("TaggedPlayer", PhotonTargets.All, playerID);
    }

    [RPC]
    void TaggedPlayer(int playerID)
    {
        Debug.Log("TaggedPlayerPre: " + playerID);
        playerWhoIsIt = playerID;
        Debug.Log("TaggedPlayer: " + playerID);
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerDisconnected: " + player);

        if (PhotonNetwork.isMasterClient)
        {
            if (player.ID == playerWhoIsIt)
            {
                // if the player who left was "it", the "master" is the new "it"
                TagPlayer(PhotonNetwork.player.ID);
            }
        }
    }

    void OnMasterClientSwitched()
    {
        Debug.Log("OnMasterClientSwitched");
    }
}
