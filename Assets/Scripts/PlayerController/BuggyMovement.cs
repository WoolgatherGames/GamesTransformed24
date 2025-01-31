using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Movement
{
    public class BuggyMovement : MonoBehaviour, IInteractable
    {
        bool playerDriving;
        Rigidbody2D rb;

        PlayerInput playerInput;

        [SerializeField] Transform playerSeat;
        [SerializeField] GameObject buggyVFXTrail;
        [SerializeField] Transform buggyVFXSpawnPos;

        //would be good to move camera control to its own script 
        [SerializeField] CinemachineVirtualCamera virtualCamera;//zoom camera out whilst inside buggy

        private void Awake()
        {
            playerInput = new PlayerInput();
            playerInput.Input.Enable();
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            cameraStartZoom = virtualCamera.m_Lens.OrthographicSize;
        }

        float cameraStartZoom;
        float cameraLerpVal;
        [SerializeField] float cameraZoomGoal;
        void Update()
        {
            if (virtualCamera != null)
            {
                float lerpSpeed = Time.deltaTime * 0.3f;
                if (playerDriving)
                {
                    cameraLerpVal = Mathf.Clamp(cameraLerpVal + lerpSpeed, 0f, 1f);
                    float newCameraZoom = Mathf.Lerp(cameraStartZoom, cameraZoomGoal, cameraLerpVal);
                    virtualCamera.m_Lens.OrthographicSize = newCameraZoom;
                }
                else if (cameraLerpVal > 0f)
                {
                    cameraLerpVal = Mathf.Clamp(cameraLerpVal - (lerpSpeed * 2f), 0f, 1f);
                    float newCameraZoom = Mathf.Lerp(cameraStartZoom, cameraZoomGoal, cameraLerpVal);
                    virtualCamera.m_Lens.OrthographicSize = newCameraZoom;
                }
            }
        }

        public Vector3 ReturnPositionForPromptIndicator()
        {
            return transform.position + new Vector3(0f, 0.65f, 0f);
        }

        public void Interact()
        {
            StartDriving();
            PlayerController.Instance.SitInBuggy(playerSeat);
        }
        void StartDriving()
        {
            playerDriving = true;
            rb.isKinematic = false;
            rb.velocity = Vector3.zero; accelerationTimer = 0f;
        }
        bool disableMovement;
        public void StopDriving()
        {
            playerDriving = false;
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0;
        }


        private void FixedUpdate()
        {
            if (playerDriving)
            {
                Movement();
            }
        }

        float accelerationTimer;
        float VFXSpawnTimer;
        void Movement()
        {
            //add a check to see if player WANTS to go max speed or not
            float movementSpeed = 27f;

            float accelerationPower = 0.75f;
            float turnSpeed = -0.9f;
            float maxAcceleration = 1.5f;
            float minAcceleration = -0.5f;

            Vector2 movementInput = playerInput.Input.Directional.ReadValue<Vector2>();
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


            //VFX
            if (accelerationTimer > maxAcceleration * 0.1f)
            {
                VFXSpawnTimer += Time.fixedDeltaTime * accelerationTimer;
            }
            if (VFXSpawnTimer > 1f)//rate at which buggy vfx spawns
            {
                Debug.Log("yippee");
                VFXSpawnTimer = 0f;
                //Instantiate(buggyVFXTrail, buggyVFXSpawnPos.position, transform.rotation);
                Instantiate(buggyVFXTrail, buggyVFXSpawnPos);
            }
        }

        private void OnDestroy()
        {
            playerInput.Dispose();
        }
    }
}