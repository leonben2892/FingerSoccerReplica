using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;

public class SC_MenuLogic : MonoBehaviour
{
    private static Dictionary<string, GameObject> unityObjects;
    private static GlobalEnums.Screens currentScreen;
    public Sprite[] teamsLogos, formations;
    public string[] formationsNames;


    void Awake()
    {
        Init();
    }

    void Init()
    {
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] _unityObjects = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach (GameObject g in _unityObjects)
            unityObjects.Add(g.name, g);

        if (DefinedVariables.IsMenuButtonPressedInGame == true)
        {
            MainMenuButtonPressedInSinglePlayerGame();
            SC_Controller.onOptionsBackButtonClick = Back;
        }
        else
        {
            currentScreen = GlobalEnums.Screens.Login;
            unityObjects["Text_ErrorLogin"].SetActive(false);
            unityObjects["Screen_MainMenu"].SetActive(false);
            unityObjects["Loading"].SetActive(false);
            unityObjects["Screen_Multiplayer"].SetActive(false);
            unityObjects["Screen_GameTactics"].SetActive(false);
            unityObjects["Screen_StudentInfo"].SetActive(false);
            unityObjects["Screen_Options"].SetActive(false);
            unityObjects["MultiplayerGame"].SetActive(false);
        }

        EventSubs();
    }

    void EventSubs()
    {
        SC_Controller.onLoginButtonClick = Login;
        SC_Controller.onLogoutButtonClick = Logout;
        SC_Controller.onSinglePlayerButtonClick = SinglePlayer;
        SC_Controller.onMultiplayerButtonClick = Multiplayer;
        SC_Controller.onGameTacticsButtonClick = GameTactics;
        SC_Controller.onStudentInfoButtonClick = StudentInfo;
        SC_Controller.onPlayMultiplayerButtonClick = PlayMultiplayer;
        SC_Controller.onBackButtonClick = Back;
        SC_Controller.onOptionsButtonClick = Options;
        SC_Controller.onMultiplayerMoneySliderChange = OnMoneySliderChange;
        SC_Controller.onMultiplayerCloseStatusButtonClick = CloseStatusMessage;
        SC_Controller.onCVButtonClick = Cv;
        SC_Controller.onPreviousTeamButtonClick = PreviousTeam;
        SC_Controller.onNextTeamButtonClick = NextTeam;
        SC_Controller.onPreviousFormationButtonClick = PreviousFormation;
        SC_Controller.onNextFormationButtonClick = NextFormation;
        SC_Controller.onOptionsBackgroundMusicToggleChange = BackgroundVolumeToggleChange;
        SC_Controller.onOptionsSfxSoundToggleChange = SfxVolumeToggleChange;
        SC_Controller.onOptionsBackgroundVolumeSliderChange = BackgroundVolumeSliderChange;
        SC_Controller.onOptionsSfxVolumeSliderChange = SfxVolumeSliderChange;
    }

    void ChangeScreen(GlobalEnums.Screens _NewScreen)
    {
        unityObjects["Screen_" + currentScreen].SetActive(false);
        unityObjects["Screen_" + _NewScreen].SetActive(true);
        currentScreen = _NewScreen;
    }

    void Login()
    {
        PlayBtnAudio();
        string _inputUserName = unityObjects["InputField_UserName"].GetComponent<InputField>().text;
        string _password = unityObjects["InputField_Password"].GetComponent<InputField>().text;

        if ((_inputUserName == "leon" || _inputUserName == "moshe") && _password == DefinedVariables.password)
        {
            DefinedVariables.userName = _inputUserName;
            SC_Controller.onOptionsBackButtonClick = Back;
            if (unityObjects["Text_ErrorLogin"].activeSelf)
                unityObjects["Text_ErrorLogin"].SetActive(false);
            ChangeScreen(GlobalEnums.Screens.MainMenu);
        }
        else
            unityObjects["Text_ErrorLogin"].SetActive(true);
    }

    void Logout()
    {
        PlayBtnAudio();
        unityObjects["InputField_UserName"].GetComponent<InputField>().text = "";
        unityObjects["InputField_Password"].GetComponent<InputField>().text = "";
        ChangeScreen(GlobalEnums.Screens.Login);
    }

    void SinglePlayer()
    {
        PlayBtnAudio();
        DefinedVariables.IsMultiplayerOn = false;
        SceneManager.LoadScene("Scene_SinglePlayer");
    }

    void Multiplayer()
    {
        PlayBtnAudio();
        if (SC_Controller.onOptionsBackButtonClick != Multiplayer)
            SC_Controller.onOptionsBackButtonClick = Multiplayer;

        /** Setting a connection when a player enters multiplayer screen **/
        unityObjects["Image_StatusBackground"].SetActive(false);
        unityObjects["Button_Close"].SetActive(false);
        unityObjects["Button_Play"].GetComponent<Button>().interactable = false;
        MultiPlayerInit();

        ChangeScreen(GlobalEnums.Screens.Multiplayer);
    }

    void GameTactics()
    {
        PlayBtnAudio();
        if (SC_Controller.onOptionsBackButtonClick != GameTactics)
            SC_Controller.onOptionsBackButtonClick = GameTactics;
        ChangeScreen(GlobalEnums.Screens.GameTactics);
    }

    void StudentInfo()
    {
        PlayBtnAudio();
        if (SC_Controller.onOptionsBackButtonClick != StudentInfo)
            SC_Controller.onOptionsBackButtonClick = StudentInfo;
        ChangeScreen(GlobalEnums.Screens.StudentInfo);
    }

    void Back()
    {
        PlayBtnAudio();
        if (SC_Controller.onOptionsBackButtonClick != Back)
            SC_Controller.onOptionsBackButtonClick = Back;

        /** If back button pressed, disconnect the player from AppWarp server[Used when back button is pressed from the multiplayer screen] **/
        if (serverConnection != null && serverConnection.GetConnectionState() == 0)
        {
            unityObjects["Button_Play"].GetComponent<Button>().interactable = false;
            serverConnection.Disconnect();
        }

        ChangeScreen(GlobalEnums.Screens.MainMenu);
    }

    void Options()
    {
        PlayBtnAudio();
        ChangeScreen(GlobalEnums.Screens.Options);
    }

    void Cv()
    {
        PlayBtnAudio();
        Application.OpenURL("https://docs.google.com/document/d/1cFtc4tIjwR0RVBVCDnvr43mKTPrq6ybxbuY9XjKh03w/edit?usp=sharing");
    }

    void BackgroundVolumeSliderChange()
    {
        unityObjects["Audio_BackgroundMusic"].GetComponent<AudioSource>().volume = unityObjects["Slider_BackgroundVolume"].GetComponent<Slider>().value;
    }

    void SfxVolumeSliderChange()
    {
        unityObjects["Audio_BtnClickSound"].GetComponent<AudioSource>().volume = unityObjects["Slider_SfxVolume"].GetComponent<Slider>().value;
    }

    void BackgroundVolumeToggleChange()
    {
        if (unityObjects["Toggle_BackgroundVolume"].GetComponent<Toggle>().isOn == true)
        {
            unityObjects["Label_BackgroundVolume"].GetComponent<Text>().text = "Background Volume ON";
            unityObjects["Audio_BackgroundMusic"].GetComponent<AudioSource>().mute = false;
        }
        else
        {
            unityObjects["Label_BackgroundVolume"].GetComponent<Text>().text = "Background Volume OFF";
            unityObjects["Audio_BackgroundMusic"].GetComponent<AudioSource>().mute = true;
        }
    }

    void SfxVolumeToggleChange()
    {
        if (unityObjects["Toggle_SfxVolume"].GetComponent<Toggle>().isOn == true)
            unityObjects["Label_SfxVolume"].GetComponent<Text>().text = "SFX Volume ON";
        else
            unityObjects["Label_SfxVolume"].GetComponent<Text>().text = "SFX Volume OFF";
    }

    void PlayBtnAudio()
    {
        if (unityObjects["Toggle_SfxVolume"].GetComponent<Toggle>().isOn == true)
            unityObjects["Audio_BtnClickSound"].GetComponent<AudioSource>().Play();
    }

    void OnMoneySliderChange()
    {
        unityObjects["Text_MoneyValue"].GetComponent<Text>().text = unityObjects["Slider_Money"].GetComponent<Slider>().value.ToString() + " $";
    }

    private void CloseStatusMessage()
    {
        unityObjects["Image_StatusBackground"].SetActive(false);
        unityObjects["Button_Close"].SetActive(false);
        unityObjects["Button_Play"].GetComponent<Button>().interactable = false;
    }

    void PreviousTeam()
    {
        DefinedVariables.chosenTeam -= 1;
        if (DefinedVariables.chosenTeam == -1)
            DefinedVariables.chosenTeam = 13;
        unityObjects["Text_TeamName"].GetComponent<Text>().text = Enum.GetName(typeof(GlobalEnums.Team), DefinedVariables.chosenTeam);
        unityObjects["Image_TeamLogo"].GetComponent<Image>().sprite = teamsLogos[DefinedVariables.chosenTeam];
    }

    void NextTeam()
    {
        DefinedVariables.chosenTeam += 1;
        if (DefinedVariables.chosenTeam == 14)
            DefinedVariables.chosenTeam = 0;
        unityObjects["Text_TeamName"].GetComponent<Text>().text = Enum.GetName(typeof(GlobalEnums.Team), DefinedVariables.chosenTeam);
        unityObjects["Image_TeamLogo"].GetComponent<Image>().sprite = teamsLogos[DefinedVariables.chosenTeam];
    }

    void PreviousFormation()
    {
        DefinedVariables.chosenFormation -= 1;
        if (DefinedVariables.chosenFormation == -1)
            DefinedVariables.chosenFormation = 4;
        unityObjects["Text_Formation"].GetComponent<Text>().text = formationsNames[DefinedVariables.chosenFormation];
        unityObjects["Image_Formation"].GetComponent<Image>().sprite = formations[DefinedVariables.chosenFormation];
    }

    void NextFormation()
    {
        DefinedVariables.chosenFormation += 1;
        if (DefinedVariables.chosenFormation == 5)
            DefinedVariables.chosenFormation = 0;
        unityObjects["Text_Formation"].GetComponent<Text>().text = formationsNames[DefinedVariables.chosenFormation];
        unityObjects["Image_Formation"].GetComponent<Image>().sprite = formations[DefinedVariables.chosenFormation];
    }

    public static void MainMenuButtonPressedInSinglePlayerGame()
    {
        if (unityObjects["Menu"].activeSelf == false)
            unityObjects["Menu"].SetActive(true);

        unityObjects["Screen_Login"].SetActive(false);
        unityObjects["Screen_MainMenu"].SetActive(true);
        unityObjects["Loading"].SetActive(false);
        unityObjects["Screen_Multiplayer"].SetActive(false);
        unityObjects["Screen_GameTactics"].SetActive(false);
        unityObjects["Screen_StudentInfo"].SetActive(false);
        unityObjects["Screen_Options"].SetActive(false);
        unityObjects["MultiplayerGame"].SetActive(false);
        currentScreen = GlobalEnums.Screens.MainMenu;
        DefinedVariables.IsMultiplayerOn = true;
        DefinedVariables.IsMenuButtonPressedInGame = false;
    }


    /** Multiplayer code starts here **/

    private string apiKey;
    private string secretKey;
    private Listener listen;
    private List<string> rooms;
    private int roomIndex;
    private string roomId;
    private Dictionary<string, object> matchRoomData;
    WarpClient serverConnection;

    void OnEnable()
    {
        Listener.OnConnect += OnConnect;
        Listener.OnRoomsInRange += OnRoomsInRange;
        Listener.OnCreateRoom += OnCreateRoom;
        Listener.OnGetLiveRoomInfo += OnGetLiveRoomInfo;
        Listener.OnJoinRoom += OnJoinRoom;
        Listener.OnUserJoinRoom += OnUserJoinRoom;
        Listener.OnGameStarted += OnGameStarted;
    }

    void OnDisable()
    {
        Listener.OnConnect -= OnConnect;
        Listener.OnRoomsInRange -= OnRoomsInRange;
        Listener.OnCreateRoom -= OnCreateRoom;
        Listener.OnGetLiveRoomInfo -= OnGetLiveRoomInfo;
        Listener.OnJoinRoom -= OnJoinRoom;
        Listener.OnUserJoinRoom -= OnUserJoinRoom;
        Listener.OnGameStarted -= OnGameStarted;
    }

    void MultiPlayerInit()
    {
        apiKey = "58271c1a56f7b42d7a979d66e372b409ca81dabd1d1ea25c599dd1b7fd3a780b";
        secretKey = "9c38fcd3b67e65bb94a4c109ea0c2d50f0fd5f28d0bae2c66d6d8298898ac86f";
        roomIndex = 0;
        roomId = "";

        if (listen == null)
            listen = new Listener();

        WarpClient.initialize(apiKey, secretKey);
        serverConnection = WarpClient.GetInstance();
        serverConnection.AddConnectionRequestListener(listen);
        serverConnection.AddChatRequestListener(listen);
        serverConnection.AddUpdateRequestListener(listen);
        serverConnection.AddLobbyRequestListener(listen);
        serverConnection.AddNotificationListener(listen);
        serverConnection.AddRoomRequestListener(listen);
        serverConnection.AddZoneRequestListener(listen);
        serverConnection.AddTurnBasedRoomRequestListener(listen);



        matchRoomData = new Dictionary<string, object>();
        matchRoomData.Add("Password", "Shenkar");

        serverConnection.Connect(DefinedVariables.userName);
    }

    private void UpdateStatus(string _NewStatus)
    {
        unityObjects["Text_Status"].GetComponent<Text>().text = _NewStatus;
    }

    private void OnConnect(bool _IsSuccess)
    {
        //Debug.Log(_IsSuccess);
        if (_IsSuccess)
        {
            unityObjects["Button_Play"].GetComponent<Button>().interactable = true;
        }
        else
            UpdateError("Can't connect to the server. \n Please try again later.");
    }

    void PlayMultiplayer()
    {
        PlayBtnAudio();
        unityObjects["Image_StatusBackground"].SetActive(true);
        UpdateStatus("Searching for room..");
        serverConnection.GetRoomsInRange(1, 2);
    }

    public void OnRoomsInRange(bool _IsSuccess, MatchedRoomsEvent eventObj)
    {
        if (_IsSuccess)
        {
            UpdateStatus("Searching for available rooms..");
            rooms = new List<string>();
            foreach (var roomData in eventObj.getRoomsData())
            {
                //Debug.Log("Room Id: " + roomData.getId());
                //Debug.Log("Room Owner: " + roomData.getRoomOwner());
                rooms.Add(roomData.getId());
            }

            roomIndex = 0;
            DoRoomSearchLogic();
        }
        else
            UpdateError("Can't find rooms. \n Please try again later.");
    }

    public void DoRoomSearchLogic()
    {
        if (roomIndex < rooms.Count)
        {
            UpdateStatus("Getting room Details (" + rooms[roomIndex] + ")");
            serverConnection.GetLiveRoomInfo(rooms[roomIndex]);
        }
        else
        {
            UpdateStatus("Creating Room...");
            serverConnection.CreateTurnRoom(DefinedVariables.userName+ " Room", DefinedVariables.userName, 2, matchRoomData, 30);
        }
    }

    private void OnCreateRoom(bool _IsSuccess, string _RoomId)
    {
        if (_IsSuccess)
        {
            UpdateStatus("Room Created: " + _RoomId);
            roomId = _RoomId;
            JoinAndSubscribeRoom(roomId);
        }
        else
            UpdateError("Can't create a room. \n Please try again later.");
    }

    public void OnGetLiveRoomInfo(LiveRoomInfoEvent eventObj)
    {
        Dictionary<string, object> _parms = eventObj.getProperties();
        //Debug.Log(_parms["Password"]);
        if (_parms["Password"].ToString() == matchRoomData["Password"].ToString())
        {
            roomId = eventObj.getData().getId();
            JoinAndSubscribeRoom(roomId);
        }
        else
        {
            roomIndex++;
            DoRoomSearchLogic();
        }
    }

    private void JoinAndSubscribeRoom(string roomId)
    {
        serverConnection.JoinRoom(roomId);
        serverConnection.SubscribeRoom(roomId);
    }

    public void OnJoinRoom(bool _IsSuccess, string _RoomId)
    {
        if (_IsSuccess)
            UpdateStatus("Joined Room: " + _RoomId);
        else
            UpdateError("Can't join room. \n Please try again later.");
    }

    public void OnUserJoinRoom(RoomData eventObj, string _UserName)
    {
        UpdateStatus("User: " + _UserName + " Joined Room");
        if (_UserName != DefinedVariables.userName)
        {
            UpdateStatus("Starting game...");
            serverConnection.startGame();
        }
    }

    public void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        unityObjects["MultiplayerGame"].SetActive(true);
        unityObjects["Menu"].SetActive(false);
    }

    private void UpdateError(string error)
    {
        unityObjects["Image_StatusBackground"].SetActive(true);
        UpdateStatus(error);
        unityObjects["Button_Close"].SetActive(true);
    }
}
