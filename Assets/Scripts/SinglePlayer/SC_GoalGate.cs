using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_GoalGate : MonoBehaviour
{

    public Image goalImg;
    public static bool IsTriggeredFinished;   //Used to make sure the goal count will be incremented by 1 each time a goal is scored and that player/enemy wont be able to shot the ball in the middle of goal scored routine.

    void Awake() { Init(); }

    /// <summary>
    ///  Initialization of required variables for proper goal gate behavior
    /// </summary>
    void Init()
    {
        IsTriggeredFinished = true;
    }

    /// <summary>
    /// If one of the gates goal lines is touched by the ball, check who scored and start goal routine.
    /// </summary>
    /// <param name="col">The collider that hits the gate goal line</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.gameObject.tag == "Ball") && (IsTriggeredFinished == true))
        {
            IsTriggeredFinished = false;
            WhoScored();
            StartCoroutine(GoalScored());
        }
    }

    /// <summary>
    /// Checks which goal line the ball touched and increments the player or enemy goal count.
    /// </summary>
    void WhoScored()
    {
        string tmpCurrentComputerScore = SC_GameManager.Instance.uiObject["Text_ComputerScore"].GetComponent<Text>().text;
        string tmpCurrentUserScore = SC_GameManager.Instance.uiObject["Text_UsernameScore"].GetComponent<Text>().text;

        if (gameObject.name == "Tex_LeftGoalGate")
            SC_GameManager.Instance.uiObject["Text_ComputerScore"].GetComponent<Text>().text = ((System.Int32.Parse(tmpCurrentComputerScore) + 1).ToString());
        else if(gameObject.name == "Tex_RightGoalGate")
            SC_GameManager.Instance.uiObject["Text_UsernameScore"].GetComponent<Text>().text = ((System.Int32.Parse(tmpCurrentUserScore) + 1).ToString());
    }

    /// <summary>
    /// Presents goal image for 3 seconds and plays "GOAL!" sound then the method is checking for a winner.
    /// If there is not a winner yet, reset the round.
    /// </summary>
    /// <returns></returns>
    IEnumerator GoalScored()
    {
        SC_AudioManager.Instance.PlaySound("GoalScored");
        goalImg.enabled = true;
        yield return new WaitForSeconds(3);
        goalImg.enabled = false;
        bool IsWinner = SC_GameManager.Instance.CheckWinner();
        if (IsWinner == false)
            SC_GameManager.Instance.ResetRound();
        IsTriggeredFinished = true;
    }
}
