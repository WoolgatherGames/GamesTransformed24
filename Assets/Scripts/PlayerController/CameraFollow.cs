using Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform player;
    float screenX;
    float screenY;
    Camera me;

    Vector2 highBoundary;
    Vector2 lowBoundary;

    float lerpVal;


    private void Start()
    {
        positionGoal = transform.position;

        me = GetComponent<Camera>();
        player = PlayerController.Instance.transform;
        screenX = Screen.width;
        screenY = Screen.height;

        highBoundary = new Vector2(screenX * 0.7f, screenY * 0.7f);
        lowBoundary = new Vector2(screenX * 0.3f, screenY * 0.3f);
        Debug.Log(highBoundary.x);
    }

    Vector3 positionGoal;
    float checkPlayerPosTime;

    private void Update()
    {
        if (lerpVal < 1f)
        {
            Debug.Log("lerp");
            lerpVal += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(transform.position, positionGoal, lerpVal);
        }



        if (Time.time > checkPlayerPosTime )
        {
            CheckPlayerPos();
            checkPlayerPosTime = Time.time + 0.2f;
        }
    }

    void CheckPlayerPos()
    {
        Vector3 playerScreenPoint = me.WorldToScreenPoint(player.position);

        Debug.Log(playerScreenPoint);

        if (playerScreenPoint.x > highBoundary.x)
        {
            JumpToPlayer();
        }
        else if (playerScreenPoint.x < lowBoundary.x)
        {
            JumpToLeft();
        }
        else if (playerScreenPoint.y > highBoundary.y)
        {
            JumpToPlayer();
        }
        else if (playerScreenPoint.y < lowBoundary.y)
        {
            JumpToPlayer();
        }
    }

    void JumpToPlayer()
    {
        //transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        positionGoal = new Vector3(player.position.x, player.position.y, transform.position.z);
        lerpVal = 0f;
    }

    void JumpToLeft()
    {
        positionGoal = new Vector3(me.ScreenToWorldPoint(new Vector3(lowBoundary.x, 0f, 0f)).x, transform.position.y, transform.position.z);
        lerpVal = 0f;
    }
    void JumpToRight()
    {

    }
}
