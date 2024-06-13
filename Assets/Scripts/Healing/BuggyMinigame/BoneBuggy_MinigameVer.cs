using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BoneBuggy_MinigameVer : MonoBehaviour
{
    Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    [SerializeField] BuggyMinigameManager manager;
    private void OnTriggerExit2D(Collider2D collision)
    {
        //fell off the floor
        if (collision.GetComponent<TilemapCollider2D>() != null)
        {
            Transform moveTo = manager.ReturnLastCheckpointTransform();
            transform.position = moveTo.position;
            transform.rotation = moveTo.rotation;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    Vector2 movementInput;
    void OnDirectional(InputValue input)
    {
        movementInput = input.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    float accelerationTimer;
    void Movement()
    {
        //add a check to see if player WANTS to go max speed or not
        float movementSpeed = 15f;

        float accelerationPower = 0.75f;
        float turnSpeed = -0.25f;
        float maxAcceleration = 1.5f;
        float minAcceleration = -0.5f;

        float forwardInput = movementInput.y;
        float turnInput = movementInput.x;

        if (forwardInput > 0f)
        {
            float mod = accelerationTimer > 0f ? 1f : 3f;
            accelerationTimer += forwardInput * 0.02f * accelerationPower * mod;//note: 0.02f is basically delta time inside fixed update
        }
        else if (forwardInput < 0f)
        {
            float mod = accelerationTimer < 0f ? 1f : 3f;
            accelerationTimer += forwardInput * 0.02f * accelerationPower * mod;
        }
        else
        {
            //player is not inputting anything. move towards zero
            //accelerationTimer *= 0.85f;
            float mod = accelerationTimer > 0f ? -1f : 1f;
            accelerationTimer += mod * accelerationPower * 0.02f;
        }
        accelerationTimer = Mathf.Clamp(accelerationTimer, minAcceleration, maxAcceleration);
        float totalSpeed = accelerationTimer * movementSpeed;

        //test this:
        //increase turn speed IF the car isnt turning forward (drift?)
        if (forwardInput < 0f)
            turnInput *= 2f;
        else if (forwardInput == 0f)
            turnInput *= 1.25f;

        //if travelling at high speed, reduce turn speed
        //float turnDampner = Mathf.Lerp(1f, 0.75f, accelerationTimer / maxAcceleration);
        //turnInput *= turnDampner;

        rb.AddTorque(turnInput * turnSpeed);
        rb.AddForce(transform.up * totalSpeed);
    }
}
