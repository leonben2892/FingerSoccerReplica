using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;

public class SC_GameManager : MonoBehaviour
{
    //Player vars
    public Dictionary<string, GameObject> playerObject;
    private Vector2[] playerPucksFormation; 

    //Enemy vars
    public Dictionary<string, GameObject> enemyObject;
    private int enemyChosenTeamIndex, enemyChosenFormationIndex;
    private Vector2[] enemyPucksFormation; 

    //UI vars
    public Dictionary<string, GameObject> uiObject;

    //General vars
    private Vector2 puckCurrentPosition;
    private float initialPuckPositioningSpeed;
    public bool IsPucksInFormationPos, IsPlayerTurn, IsPuckMoving, IsGameOver;
    public Rigidbody2D ball;

    //Multiplayer vars
    Dictionary<string, object> _receivedPlayData;
    private bool IsMultiplayerInitiated;
    private int multiplayerGameRestartAcceptanceCounter;
    
    /// <summary>
    /// Implementing SC_GameManager as singleton design pattern.
    /// </summary>
    static SC_GameManager instance;
    public static SC_GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_GameManager").GetComponent<SC_GameManager>();
            return instance;
        }
    }

    void Awake(){Init();}

    /// <summary>
    /// Initialization of required variables for proper game manager behavior.
    /// </summary>
    void Init()
    {
        /** Player init **/
        playerObject = new Dictionary<string, GameObject>();
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject g in playerObjects)
            playerObject.Add(g.name, g);
        if (DefinedVariables.IsMultiplayerOn == false)
            playerPucksFormation = SC_GameTactics.ChosenFormation(DefinedVariables.chosenFormation);


        /** Enemy init **/
        enemyObject = new Dictionary<string, GameObject>();
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject g in enemyObjects)
            enemyObject.Add(g.name, g);
        if (DefinedVariables.IsMultiplayerOn == false)
        {
            ChooseEnemyTeamAndFormation();
            enemyPucksFormation = SC_GameTactics.EnemyFormation(enemyChosenFormationIndex);
        }

        /** UI init **/
        uiObject = new Dictionary<string, GameObject>();
        GameObject[] uiObjects = GameObject.FindGameObjectsWithTag("SinglePlayerUI");
        foreach (GameObject g in uiObjects)
            uiObject.Add(g.name, g);
        uiObject["Image_GameFinished"].SetActive(false);
        uiObject["Text_Username"].GetComponent<Text>().text = DefinedVariables.userName;

        /** General init **/            
        initialPuckPositioningSpeed = 480.0f;
        IsPuckMoving = false;
        IsGameOver = false;
        EventSubs();
        if (DefinedVariables.IsMultiplayerOn == false)
        {
            InitPucksTeamLogoImg();
            IsPlayerTurn = true;
            ResetRound();
        }
    }

    /// <summary>
    /// When the user is closing the game then unsubscribe to the multiplayer events and initiate stopGame method.
    /// Initiating stopGame method is designed to show the other client(that didnt close the game) that his opponent is no longer available.
    /// </summary>
    void OnDestroy()
    {
        if (DefinedVariables.IsMultiplayerOn == true && IsMultiplayerInitiated == true)
        {
            Listener.OnGameStarted -= OnGameStarted;
            Listener.OnMoveCompleted -= OnMoveCompleted;
            Listener.OnGameStopped -= OnGameStopped;
            Listener.OnChatReceived -= OnChatReceived;

            WarpClient.GetInstance().stopGame();
        }
    }

    /// <summary>
    /// Reseting the round.
    /// This means, the player and enemy pucks will go back to their starting formation and the ball will be in the middle of the field.
    /// </summary>
    public void ResetRound()
    {
        IsPuckMoving = true;
        IsPucksInFormationPos = false;
    }

    /// <summary>
    /// This method will manage 2 things:
    /// 1. If a round needs a reset, then it will run until the player and enemy pucks are in their starting formation and the ball is in the middle of the field.
    /// 2. After every round reset and turn played, it will run until all the pucks stopped moving and will notify about it by changing IsPuckMoving variable to false.
    /// </summary>
    void FixedUpdate()
    {
        if (IsGameOver == false)
        {
            if (IsPucksInFormationPos == false || IsPuckMoving == true)
            {
                if (IsPucksInFormationPos == false)
                {
                    PositionAllPucksInFormationPos();
                    ball.position = new Vector3(0, 0, -20);
                }
                if (CheckIfPucksStoppedMoving())
                {
                    IsPucksInFormationPos = true;
                    IsPuckMoving = false;
                }
            }
        }
    }

    /// <summary>
    /// Initializing the team logo picture on every puck based on the player and computer choices.
    /// </summary>
    void InitPucksTeamLogoImg()
    {
        for(int i = 0; i < DefinedVariables.maxPlayerPucks; i++)
        {
            playerObject["PlayerPuck_" + i].GetComponent<SC_Puck>().InitPuckImg(DefinedVariables.chosenTeam);
            enemyObject["EnemyPuck_" + i].GetComponent<SC_Puck>().InitPuckImg(enemyChosenTeamIndex);
        } 
    }

    /// <summary>
    /// Moving all pucks(player and computer) back to their starting position.
    /// Their starting position is based on the chosen formation.
    /// </summary>
    void PositionAllPucksInFormationPos()
    {
        for (int i = 0; i < DefinedVariables.maxPlayerPucks; i++)
        {
            if(playerPucksFormation != null && enemyPucksFormation != null)
            {
                //Position player pucks
                puckCurrentPosition = playerObject["PlayerPuck_" + i].GetComponent<Transform>().position;
                playerObject["PlayerPuck_" + i].GetComponent<Transform>().position = Vector2.MoveTowards(puckCurrentPosition, playerPucksFormation[i], initialPuckPositioningSpeed * Time.deltaTime);

                //Position enemy puck
                puckCurrentPosition = enemyObject["EnemyPuck_" + i].GetComponent<Transform>().position;
                enemyObject["EnemyPuck_" + i].GetComponent<Transform>().position = Vector2.MoveTowards(puckCurrentPosition, enemyPucksFormation[i], initialPuckPositioningSpeed * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// This method check if all the pucks in the game stopped moving.
    /// </summary>
    /// <returns>Return true if all the pucks stopped moving or false if not.</returns>
    public bool CheckIfPucksStoppedMoving()
    {
        for(int i = 0; i < DefinedVariables.maxPlayerPucks; i++)
        {
            if (playerObject["PlayerPuck_" + i].GetComponent<Rigidbody2D>().IsSleeping() && enemyObject["EnemyPuck_" + i].GetComponent<Rigidbody2D>().IsSleeping())
                continue;
            else
                return false;
        }
        return true;
    }

    /// <summary>
    /// Randomly decides the computer team and formation.
    /// This method ensure that the computer team will be different from the player chosen team.
    /// </summary>
    void ChooseEnemyTeamAndFormation()
    {
        enemyChosenTeamIndex = UnityEngine.Random.Range(0, 14);
        if (enemyChosenTeamIndex == DefinedVariables.chosenTeam)
        {
            if (enemyChosenTeamIndex == 0)
                enemyChosenTeamIndex += 1;
            if (enemyChosenTeamIndex == 13)
                enemyChosenTeamIndex -= 1;
            else
                enemyChosenTeamIndex += 1;
        }
        enemyChosenFormationIndex = UnityEngine.Random.Range(0, 5);
    }

    /// <summary>
    /// Check if there is a winner.
    /// Winner is the player who scored 2 goals to his opponent.
    /// </summary>
    /// <returns>The method return true if there is a winner or false if not</returns>
    public bool CheckWinner()
    {
        if (System.Int32.Parse(uiObject["Text_UsernameScore"].GetComponent<Text>().text) == 2)
        {
            uiObject["Text_WinnerName"].GetComponent<Text>().text = DefinedVariables.userName;
            EndGame();
            return true;
        }

        if (System.Int32.Parse(uiObject["Text_ComputerScore"].GetComponent<Text>().text) == 2)
        {
            uiObject["Text_WinnerName"].GetComponent<Text>().text = "Computer";
            EndGame();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Ending the game and presenting game-ending window
    /// </summary>
    void EndGame()
    {
        IsGameOver = true;
        uiObject["Image_GameFinished"].SetActive(true);
    }

    /// <summary>
    /// Simply changing turns between the player and the computer
    /// If IsPlayerTurn = true then its the player turn else its the computer turn
    /// </summary>
    public void PassTurn()
    {
        IsPlayerTurn = !IsPlayerTurn;
    }

    /// <summary>
    /// Subscribing for events
    /// </summary>
    void EventSubs()
    {
        SC_SPController.onRestartGameButtonClick = RestartGame;
        SC_SPController.onMainMenuButtonClick = MainMenu;
        if(DefinedVariables.IsMultiplayerOn == true)
        {
            SC_SPController.onSentChatMessageButtonClick = SendMessage;
            Listener.OnGameStarted += OnGameStarted;
            Listener.OnMoveCompleted += OnMoveCompleted;
            Listener.OnGameStopped += OnGameStopped;
            Listener.OnChatReceived += OnChatReceived;
        }
    }

    /// <summary>
    /// Singleplayer-
    /// Restarting the game completely.
    /// The method will change the player and enemy score back to 0.
    /// Also, will move the player and enemy pucks back to their starting formation and the ball to the middle of the field.
    /// 
    /// Multiplayer-
    /// if the game is in multiplayer mode & multiplayer acceptance counter is less then 2 then send a message: "I want to play again!"
    /// When the multiplayer acceptance counter is equal to 2, reset it and make restart game and send message buttons interactable again.
    /// Then, do the singleplayer routine that is written above.
    /// </summary>
    void RestartGame()
    {
        if(DefinedVariables.IsMultiplayerOn == true)
        {
            if(multiplayerGameRestartAcceptanceCounter < 2)
            {
                WarpClient.GetInstance().SendChat(DefinedVariables.userName + ": " + "I want to play again!");
                uiObject["Button_RestartGame"].GetComponent<Button>().interactable = false;
                return;
            }
            else
            {
                multiplayerGameRestartAcceptanceCounter = 0;
                uiObject["Button_RestartGame"].GetComponent<Button>().interactable = true;
                uiObject["Button_SendMessage"].GetComponent<Button>().interactable = true;
            }
        }

        IsGameOver = false;
        uiObject["Text_UsernameScore"].GetComponent<Text>().text = "0";
        uiObject["Text_ComputerScore"].GetComponent<Text>().text = "0";
        uiObject["Image_GameFinished"].SetActive(false);
        ResetRound();
        if(DefinedVariables.IsMultiplayerOn == false)
            IsPlayerTurn = true;
    }
 
    /// <summary>
    /// Singleplayer-
    /// Return to main menu screen
    /// 
    /// Multiplayer-
    /// Moving the ball and all the pucks back to the middle of the field.
    /// Then, initiating the stopGame method and returning to main menu screen.
    /// </summary>
    void MainMenu()
    {
        if (DefinedVariables.IsMultiplayerOn == true)
        {
            ball.position.Set(0, 0);
            for (int i = 0; i < DefinedVariables.maxPlayerPucks; i++)
            {
                playerObject["PlayerPuck_" + i].GetComponent<Transform>().position.Set(0, 0 ,0);
                enemyObject["EnemyPuck_" + i].GetComponent<Transform>().position.Set(0, 0, 0);
            }

            WarpClient.GetInstance().stopGame();
            IsMultiplayerInitiated = false;
            SC_MenuLogic.MainMenuButtonPressedInSinglePlayerGame();
        }
        else
        {
            DefinedVariables.IsMenuButtonPressedInGame = true;
            SceneManager.LoadScene("Scene_Menu");
        }
    }

    /** Multiplayer code section **/

    /// <summary>
    /// Initialize required variables for proper multiplayer game flow.
    /// Also, if its the player turn, send required data to the opponent for initializing the game with proper mirroring.
    /// </summary>
    /// <param name="_Sender"></param>
    /// <param name="_RoomId"></param>
    /// <param name="_NextTurn"></param>
    public void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        IsMultiplayerInitiated = false;
        multiplayerGameRestartAcceptanceCounter = 0;
        uiObject["Button_RestartGame"].GetComponent<Button>().interactable = true;
        uiObject["Button_SendMessage"].GetComponent<Button>().interactable = true;

        if (DefinedVariables.userName == _NextTurn)
        {
            IsPlayerTurn = true;
            SendGameInitializationData();
        }
        else
        {
            IsPlayerTurn = false;
        }
    }

    /// <summary>
    /// This method receives a move and changing turns between clients respectively.
    /// If a client didn't make any move in 30 seconds then end game and set the other client as the winner.
    /// If a move is made and the game is not initialize yet, then initialize the game.
    /// If a move is made and the game already initialized, then simulate a shot for the desirable client.
    /// </summary>
    /// <param name="_Move"></param>
    public void OnMoveCompleted(MoveEvent _Move)
    {
        //Debug.Log("OnMoveCompleted " + _Move.getMoveData() + " | " + _Move.getNextTurn());

        if (_Move.getNextTurn() == DefinedVariables.userName)
            IsPlayerTurn = true;
        else IsPlayerTurn = false;

        if (_Move.getMoveData() != null)
        {
            if (_Move.getSender() != DefinedVariables.userName && IsGameOver == false)
            {
                _receivedPlayData = MiniJSON.Json.Deserialize(_Move.getMoveData()) as Dictionary<string, object>;
                   
                if (_receivedPlayData != null)
                {
                    if(IsMultiplayerInitiated == false)
                    {
                        SendGameInitializationData();
                        if(_receivedPlayData.ContainsKey("i1") && _receivedPlayData.ContainsKey("i2") && _receivedPlayData.ContainsKey("i3"))
                            MultiplayerInit(int.Parse(_receivedPlayData["i1"].ToString()), int.Parse(_receivedPlayData["i2"].ToString()), _receivedPlayData["i3"].ToString());
                        IsMultiplayerInitiated = true;
                    }
                    else
                    {
                        if(_receivedPlayData.ContainsKey("v1") && _receivedPlayData.ContainsKey("v2") && _receivedPlayData.ContainsKey("v3"))
                        {
                            SimulateOpponentMove(int.Parse(_receivedPlayData["v1"].ToString()), StringToVector3(_receivedPlayData["v2"].ToString()), int.Parse(_receivedPlayData["v3"].ToString()));
                            IsPuckMoving = true;
                        }
                    }
                }
            }
        }
        else
        {
            if (_Move.getNextTurn() == DefinedVariables.userName)
                uiObject["Text_WinnerName"].GetComponent<Text>().text = DefinedVariables.userName;
            else
                uiObject["Text_WinnerName"].GetComponent<Text>().text = uiObject["Text_Computer"].GetComponent<Text>().text;

            EndGame();
        }
    }

    /// <summary>
    /// Ending the game (called when a client closes the game or the client pressed on main menu button).
    /// The method will set the winner(the client that is still in the game) and set restart game and send message buttons to not interactable.
    /// Also, disconnects the play from AppWarp server.
    /// </summary>
    /// <param name="_Sender"></param>
    /// <param name="_RoomId"></param>
    public void OnGameStopped(string _Sender, string _RoomId)
    {
        if (uiObject["Image_GameFinished"].activeSelf == false)
        {
            uiObject["Text_WinnerName"].GetComponent<Text>().text = DefinedVariables.userName;
            EndGame();
        }
        uiObject["Button_RestartGame"].GetComponent<Button>().interactable = false;
        uiObject["Button_SendMessage"].GetComponent<Button>().interactable = false;

        if (WarpClient.GetInstance() != null && WarpClient.GetInstance().GetConnectionState() == 0)
            WarpClient.GetInstance().Disconnect();
    }

    /// <summary>
    /// Receiving a message.
    /// If the game is over and the message contains "I want to play again!" then add 1 to multiplayer acceptance counter.
    /// If the multiplayer acceptance counter is equal to 2 then restart the game.
    /// </summary>
    /// <param name="eventObj">The data of the received message</param>
    public void OnChatReceived(ChatEvent eventObj)
    {
        uiObject["Text_ChatMessages"].GetComponent<Text>().text += eventObj.getMessage() + "\n";

        if (eventObj.getMessage().Contains("I want to play again!") && IsGameOver == true)
        {
            multiplayerGameRestartAcceptanceCounter += 1;
            if (multiplayerGameRestartAcceptanceCounter == 2)
                RestartGame();
        }
        
        uiObject["TextContainer"].GetComponent<ScrollRect>().verticalNormalizedPosition = 0.05f;
    }

    /// <summary>
    /// Sending the opponent a message.
    /// The message content is taken from the input field only if it is not empty.
    /// </summary>
    public void SendMessage()
    {
        string userMessage = uiObject["InputField_MessageContent"].GetComponent<InputField>().text;

        if (userMessage != "")
        {
            if (userMessage.Contains("I want to play again!") && IsGameOver == true)
                userMessage += " ";

            WarpClient.GetInstance().SendChat(DefinedVariables.userName + ": " + userMessage);
        }
            
        uiObject["InputField_MessageContent"].GetComponent<InputField>().text = "";
    }

    /// <summary>
    /// Initializing the game with data from the opponent to set up mirroring properly.
    /// </summary>
    /// <param name="enemyChosenTeam">The chosen team of the opponent</param>
    /// <param name="enemyChosenFormation">The chosen formation of the opponent</param>
    /// <param name="enemyUsername">The user name of the opponents</param>
    void MultiplayerInit(int enemyChosenTeam, int enemyChosenFormation, string enemyUsername)
    {
        enemyPucksFormation = SC_GameTactics.EnemyFormation(enemyChosenFormation);
        enemyChosenTeamIndex = enemyChosenTeam;
        playerPucksFormation = SC_GameTactics.ChosenFormation(DefinedVariables.chosenFormation);
        InitPucksTeamLogoImg();
        uiObject["Text_Username"].GetComponent<Text>().text = DefinedVariables.userName;
        uiObject["Text_Computer"].GetComponent<Text>().text = enemyUsername;
        ResetRound();       
    }

    /// <summary>
    /// Simulate the opponent shot
    /// </summary>
    /// <param name="puckIndex">The puck index the opponent did the shot on</param>
    /// <param name="angle">The angle of the opponent shot</param>
    /// <param name="mouseDragDistance">The mouse drag distance of the opponent(the power of the shot)</param>
    void SimulateOpponentMove(int puckIndex, Vector3 angle, int mouseDragDistance)
    {
        angle.Normalize();
        enemyObject["EnemyPuck_" + puckIndex].GetComponent<Rigidbody2D>().AddForce(-angle * mouseDragDistance * 300.0f, ForceMode2D.Force);
    }

    /// <summary>
    /// Sending to the other client required information to start the game and execute mirroring properly
    /// </summary>
    void SendGameInitializationData()
    {
        Dictionary<string, object> _toSend = new Dictionary<string, object>();
        _toSend.Add("i1", DefinedVariables.chosenTeam);
        _toSend.Add("i2", DefinedVariables.chosenFormation);
        _toSend.Add("i3", DefinedVariables.userName);
        string _jsonToSend = MiniJSON.Json.Serialize(_toSend);
        WarpClient.GetInstance().sendMove(_jsonToSend);
    }

    /// <summary>
    /// returning a Vector3 from a given string.
    /// </summary>
    /// <param name="stringVector">String of the desired vector3</param>
    /// <returns>Vector3 from the given string</returns>
    public Vector3 StringToVector3(string stringVector)
    {
        // Remove the parentheses
        if (stringVector.StartsWith("(") && stringVector.EndsWith(")"))
            stringVector = stringVector.Substring(1, stringVector.Length - 2);

        // Split the items
        string[] sArray = stringVector.Split(',');

        // Store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        // Return a Vector3 from the given string
        return result;
    }
}