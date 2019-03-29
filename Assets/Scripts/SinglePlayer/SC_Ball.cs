using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Ball : MonoBehaviour
{
    private float xRotation;
    private Rigidbody2D ballRigidBody;
    private Vector3 rotationVector;


    void Awake(){Init();}

    /// <summary>
    /// Initialization of required variables for proper ball behavior
    /// </summary>
    void Init()
    {
        xRotation = 0;
        ballRigidBody = GetComponent<Rigidbody2D>();
        rotationVector = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// 1st condition - If ball is moving & ball speed is less then 10.0f then: stop ball from moving.
    /// 2nd condition - If ball is moving & ball speed is more then 10.0f: rotate the ball on the x axis.
    /// </summary>
    void FixedUpdate()
    {
        if (ballRigidBody.IsAwake() && ballRigidBody.velocity.magnitude < 10.0f)
        {
            ballRigidBody.velocity = Vector2.zero;
        }

        if (ballRigidBody.IsAwake() && ballRigidBody.velocity.magnitude > 10.0f)
        {
            xRotation = xRotation + 5.0f;
            rotationVector.Set(xRotation, 0, 0);
            transform.eulerAngles = rotationVector;
        }
    }

    /// <summary>
    /// Playing ball hit sound when colliding with an object with one of the below tags.
    /// Also, if the collider is a puck, adding force in the direction of the hit to the puck.
    /// </summary>
    /// <param name="col">The collider the ball hit</param>
    void OnCollisionEnter2D(Collision2D col)
    {
        if (SC_GameManager.Instance.IsPucksInFormationPos == true)
        {
            if (col.gameObject.tag == "Player" || col.gameObject.tag == "Enemy" || col.gameObject.tag == "FieldBorder")
                SC_AudioManager.Instance.PlaySound("BallHit");

            if (col.gameObject.tag == "Player" || col.gameObject.tag == "Enemy")
            {
                Vector3 tmpAngle = col.gameObject.GetComponent<Transform>().position - transform.position;
                tmpAngle.Normalize();
                col.gameObject.GetComponent<Rigidbody2D>().AddForce(tmpAngle * GetComponent<Rigidbody2D>().velocity.magnitude * 10, ForceMode2D.Force);
            }
        }
    }
}
