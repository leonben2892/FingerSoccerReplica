using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.shephertz.app42.gaming.multiplayer.client;
using System.Text.RegularExpressions;

public class SC_PlayerController : MonoBehaviour
{
    public Transform directionArrowTrans;
    public SpriteRenderer directionArrowSr, turnIndicatorSr; 

    private Vector3 startPos, draggedPos, relativePos, arrowScalableSize, arrowStartingSize;
    private float angle, puckMovingSpeed, lastDistance;
    private int finalDistance;
    private Rigidbody2D puckRigidBody;
    private bool IsMouseDownPosAvailable;

    void Awake(){Init();}

    /// <summary>
    /// Initialization of required variables for proper player controller behavior
    /// </summary>
    void Init()
    {
        directionArrowSr.enabled = false;
        puckMovingSpeed = 300.0f;
        puckRigidBody = GetComponent<Rigidbody2D>();
        lastDistance = 0;
        arrowScalableSize = new Vector3(0.08f,0f,0f);
        arrowStartingSize = new Vector3(2.5f, 2.5f, 1.0f);
        IsMouseDownPosAvailable = false;
    }

    /// <summary>
    /// Checks if the puck is still moving & if the puck speed is less then 30.0f. If so then, change puck speed to 0.
    /// Also, enable turn indicator if the game is not over & mouse isn't pressed & its the player turn & pucks and ball stopped moving & goal routine is not currently in progress. Else, disable it.
    /// </summary>
    void FixedUpdate()
    {
        if((SC_GameManager.Instance.IsPuckMoving == true) && (puckRigidBody.velocity.magnitude <= 30.0f))
            puckRigidBody.velocity = Vector3.zero;

        if ((SC_GameManager.Instance.IsGameOver == false) && (IsMouseDownPosAvailable == false) && (SC_GameManager.Instance.IsPlayerTurn == true) && (SC_GameManager.Instance.IsPuckMoving == false) && (SC_GameManager.Instance.ball.IsSleeping() == true) && (SC_GoalGate.IsTriggeredFinished == true))
        {
            if(turnIndicatorSr.enabled == false)
                turnIndicatorSr.enabled = true;
        }
        else
        {
            if(turnIndicatorSr.enabled == true)
                turnIndicatorSr.enabled = false;
        }
            
    }

    /// <summary>
    /// If the game is not over & its the player turn & pucks are not moving & ball is not moving then hold the position of the mouse down.
    /// Also, change IsMouseDownPosAvailable to true.
    /// </summary>
    void OnMouseDown()
    {
        if(SC_GameManager.Instance.IsGameOver == false)
        {
            if ((SC_GameManager.Instance.IsPlayerTurn == true) && (SC_GameManager.Instance.IsPuckMoving == false) && (SC_GameManager.Instance.ball.IsSleeping() == true) && (SC_GoalGate.IsTriggeredFinished == true))
            {
                startPos = Input.mousePosition;
                IsMouseDownPosAvailable = true;
            }
        } 
    }

    /// <summary>
    /// If IsMouseDownPosAvailable is true then hold position of dragged mouse.
    /// The method also continuously checks the angle from the the the mouse down position to the dragged mouse position.
    /// The size of the direction arrow will be according to the distance between the mouse down position and the dragged mouse position.
    /// </summary>
    void OnMouseDrag()
    {  
        if(IsMouseDownPosAvailable == true)
        {
            draggedPos = Input.mousePosition;
            relativePos = draggedPos - startPos;
            angle = Mathf.Atan2(-relativePos.y, -relativePos.x) * Mathf.Rad2Deg;

            directionArrowTrans.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (Vector3.Distance(startPos, draggedPos) < 35.0f)
                directionArrowSr.enabled = false;
            else
            {
                directionArrowSr.enabled = true;
                if (lastDistance < Vector3.Distance(startPos, draggedPos))
                {
                    lastDistance = Vector3.Distance(startPos, draggedPos);
                    if (directionArrowTrans.localScale.x < 6.0f)
                        directionArrowTrans.localScale += arrowScalableSize;
                }
                else if (lastDistance > Vector3.Distance(startPos, draggedPos))
                {
                    lastDistance = Vector3.Distance(startPos, draggedPos);
                    if (directionArrowTrans.localScale.x > 2.5f && Vector3.Distance(startPos, draggedPos) < 250.0f)
                        directionArrowTrans.localScale -= arrowScalableSize;
                }
            }
        }
    }

    /// <summary>
    /// Singleplayer-
    /// If IsMouseDownPosAvailable is true then:
    /// Check if the distance between mouse down and dragged mouse is greater then 35.0f. If so then check is the distance is greater then 350, 
    /// if its greater then 350.0f, set the distance to 350.0f(used to limit the power of the shot) then shoot the puck towards the angle 
    /// we calculated in the OnMouseDrag method.
    /// After shooting the puck, pass the turn back to the computer, change direction arrow size back to original size and disable it.
    /// 
    /// Multiplayer-
    /// Do the singleplayer routine written above & send required information to simulate for the opponent.
    /// </summary>
    void OnMouseUp()
    {
        if(IsMouseDownPosAvailable == true)
        {
            relativePos.Normalize();
            finalDistance = (int)Vector3.Distance(startPos, draggedPos);
            if (finalDistance > 35)
            {
                if (finalDistance > 350)
                    finalDistance = 350;
                GetComponent<Rigidbody2D>().AddForce(-SC_GameManager.Instance.StringToVector3(relativePos.ToString()) * finalDistance * puckMovingSpeed, ForceMode2D.Force);
                SC_GameManager.Instance.IsPuckMoving = true;
                if (DefinedVariables.IsMultiplayerOn == false)
                    SC_GameManager.Instance.PassTurn();

                if(DefinedVariables.IsMultiplayerOn == true)
                {
                    //Required data to simulate shot
                    Dictionary<string, object> _toSend = new Dictionary<string, object>();
                    _toSend.Add("v1", Regex.Replace(gameObject.name, "[^.0-9]", ""));
                    Vector3 tmpReflectVector = Vector3.Reflect(relativePos, Vector3.right);
                    _toSend.Add("v2", tmpReflectVector.ToString());
                    _toSend.Add("v3", finalDistance);
                    string _jsonToSend = MiniJSON.Json.Serialize(_toSend);
                    WarpClient.GetInstance().sendMove(_jsonToSend);
                }
            }
            directionArrowTrans.localScale = arrowStartingSize;
            directionArrowSr.enabled = false;
            IsMouseDownPosAvailable = false;
        }
    }

    /// <summary>
    /// Playing puck hit sound when colliding with an object with one of the below tags.
    /// </summary>
    /// <param name="col">The collider the puck hit</param>
    void OnCollisionEnter2D(Collision2D col)
    {
        if(SC_GameManager.Instance.IsPucksInFormationPos == true)
        {
            if (col.gameObject.tag == "Player" || col.gameObject.tag == "Enemy" || col.gameObject.tag == "FieldBorder")
                SC_AudioManager.Instance.PlaySound("PuckHit");
        }
    }
}
