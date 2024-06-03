using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    //attatch this to teh players camera in the stealth gameplay scene. 
    //camera follows the player but offsets slightly based on the mouse position on the screen. 

    Transform myTransform;
    Camera thisCamera;
    [SerializeField] Transform playerBody;

    [Tooltip("How far the camera can move left/right or up/down based on the mouse position (if the camera is at the edge of the screen, itll look the furthest")]
    [SerializeField] Vector2 cameraMaximumOffset;

    Vector2 screenBoundsX;
    Vector2 screenBoundsY;
    //[Tooltip("How far to the left of the screen in percentage should the mouse have to be before the game adjusts the camera angle to nothing"), Range(0,100)]
    //[SerializeField] float mousePosToCenterScreen = 50f;

    private void Start()
    {
        myTransform = this.transform;
        thisCamera = this.transform.GetComponent<Camera>();

        screenBoundsX = new Vector2(Screen.width * 0.15f, Screen.width * 0.15f);
        screenBoundsY = new Vector2(Screen.height * 0.15f, Screen.height * 0.15f);

        playerInput = new PlayerInput();
        playerInput.Input.Enable();
    }

    PlayerInput playerInput;

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        mousePos.x = Mathf.Clamp(mousePos.x, screenBoundsX.x, screenBoundsX.y);
        mousePos.y = Mathf.Clamp(mousePos.y, screenBoundsY.x, screenBoundsY.y);

        float screenWidth = (screenBoundsX.y - screenBoundsX.x);
        float screenHeight = (screenBoundsY.y - screenBoundsY.x);

        float xOffsetPercentage = (mousePos.x - screenBoundsX.x) / screenWidth;
        float yOffsetPercentage = (mousePos.y - screenBoundsY.x) / screenHeight;

        float xOffset = Mathf.Lerp(-cameraMaximumOffset.x, cameraMaximumOffset.x, xOffsetPercentage);
        float yOffset = Mathf.Lerp(-cameraMaximumOffset.y, cameraMaximumOffset.y, yOffsetPercentage);

        Vector3 newPos = new Vector3(playerBody.transform.position.x + xOffset, playerBody.transform.position.y + yOffset, -10f);
        myTransform.position = newPos;


    }


}