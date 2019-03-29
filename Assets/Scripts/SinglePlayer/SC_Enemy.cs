using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Enemy : MonoBehaviour {

    public Transform ball;
    private Vector3 angle;
    private int closetPuckToBallIndex;


    /// <summary>
    /// Make the computer shoot when the following conditions are met: the game is not over & its the computer turn
    /// & none of the pucks are moving & the ball is not moving & goal routine is not currently in progress.
    /// After the shot, the turn is passed back to the player.
    /// </summary>
    void FixedUpdate ()
    {
        if (DefinedVariables.IsMultiplayerOn == false)
        {
            if (SC_GameManager.Instance.IsGameOver == false)
            {
                if ((SC_GameManager.Instance.IsPlayerTurn == false) && (SC_GameManager.Instance.IsPuckMoving == false) && (SC_GameManager.Instance.ball.IsSleeping() == true) && (SC_GoalGate.IsTriggeredFinished == true))
                {
                    closetPuckToBallIndex = CheckClosestPuckToBall();
                    Shoot(closetPuckToBallIndex);
                    SC_GameManager.Instance.PassTurn();
                }
            }
        }
	}

    /// <summary>
    /// Checks the closest puck to the ball
    /// </summary>
    /// <returns>closest puck index</returns>
    int CheckClosestPuckToBall()
    {
        Vector3 puckPosition = SC_GameManager.Instance.enemyObject["EnemyPuck_0"].GetComponent<Transform>().position;
        float minDistance = Vector3.Distance(ball.position, puckPosition);
        float tmpDistance;
        int indexOfClosestPuck = 0;

        for(int i = 1; i < DefinedVariables.maxPlayerPucks; i++)
        {
            puckPosition = SC_GameManager.Instance.enemyObject["EnemyPuck_" + i].GetComponent<Transform>().position;
            tmpDistance = Vector3.Distance(ball.position, puckPosition);
            if (tmpDistance < minDistance)
            {
                minDistance = tmpDistance;
                indexOfClosestPuck = i;
            } 
        }
        return indexOfClosestPuck;
    }

    /// <summary>
    /// Shooting the closest puck towards the ball
    /// </summary>
    /// <param name="_closestPuck">Index of the closest puck to the ball</param>
    void Shoot(int _closestPuck)
    {
        CheckAngleToBall(_closestPuck);
        SC_GameManager.Instance.enemyObject["EnemyPuck_" + _closestPuck].GetComponent<Rigidbody2D>().AddForce(angle * 100000.0f, ForceMode2D.Force);
        SC_GameManager.Instance.IsPuckMoving = true;
    }

    /// <summary>
    /// Check the angle from the closest puck to the ball
    /// </summary>
    /// <param name="_closestPuck">Index of the closest puck to the ball</param>
    void CheckAngleToBall(int _closestPuck)
    {
        angle = ball.position - SC_GameManager.Instance.enemyObject["EnemyPuck_" + _closestPuck].GetComponent<Transform>().position;
        angle.Normalize();
    }

}
