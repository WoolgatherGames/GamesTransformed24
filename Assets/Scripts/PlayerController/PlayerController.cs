using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Movement
{
    public class PlayerController : MonoBehaviour
    {
        private static PlayerController instance;
        public static PlayerController Instance { get { return instance; } }

        PlayerInput playerInput;
        Rigidbody2D rb;

        private void Awake()
        {
            if (Instance != null) { Debug.Log("Bye Bye"); Destroy(this); }

            playerInput = new PlayerInput();
            playerInput.Input.Enable();
            playerInput.Input.Interact.performed += OnInteract;

            instance = this;
        }

        private void Start()
        {
            this.transform.SetParent(null);
            disablePlayerInput = false;
            rb = GetComponent<Rigidbody2D>();
        }


        float checkForInteractionsTimer;
        private void Update()
        {
            if (disablePlayerInput) { return; }

            checkForInteractionsTimer += Time.deltaTime;
            if (checkForInteractionsTimer > 0.05f)
            {
                CheckForInteractions();
            }
        }


        private void FixedUpdate()
        {
            if (disablePlayerInput) { return; };
            HandleWalking();
        }


        /// <summary>
        /// dont call this in the pause menu, create an extra script. 
        /// only one piece of code should be able to call this at a time
        /// </summary>
        bool disablePlayerInput;
        bool playerIsDriving;
        public void DisableCharacterController()
        {
            if (disablePlayerInput == true)
            {
                Debug.LogError("Player controller is already disabled");
            }

            disablePlayerInput = true;
        }
        public void EnableCharacterController()
        {
            disablePlayerInput = false;
        }
        public void SitInBuggy(Transform BuggySeat)
        {
            //disable collision
            GetComponent<CircleCollider2D>().enabled = false;
            rb.isKinematic = true;

            disablePlayerInput = true;
            playerIsDriving = true;

            //playerInput.Input.Interact.performed += OnInteractInsideBuggy;

            this.transform.SetParent(BuggySeat, false);
            this.transform.localPosition = new Vector3(0f, 0f, 0f);

        }
        public void StepOutOfBuggy()
        {
            //playerInput.Input.Interact.performed -= OnInteractInsideBuggy;
            GetComponentInParent<BuggyMovement>().StopDriving();//clean code who?

            disablePlayerInput = false;
            playerIsDriving = false;
            this.transform.SetParent(null);
            this.transform.rotation = Quaternion.identity;

            //re-enable collision
            GetComponent<CircleCollider2D>().enabled = true;
            rb.isKinematic = false;
        }
        void OnInteractInsideBuggy(InputAction.CallbackContext context)
        {
            StepOutOfBuggy();
        }


        [SerializeField] float interactionCastDistance;
        [SerializeField] Transform interactionOrigin;
        IInteractable closestInteractable;
        void CheckForInteractions()
        {
            if (disablePlayerInput) { return; }

            RaycastHit2D[] hits = Physics2D.CircleCastAll(interactionOrigin.position, interactionCastDistance, Vector2.up, 0f);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.GetComponent<IInteractable>() != null)
                {
                    closestInteractable = hit.collider.GetComponent<IInteractable>();
                    //show an object here to say "press [] to interact"
                    return;//stop doing the loop, we've found an interactable
                }
            }
            closestInteractable = null;//we made it out the loop without finding anything
        }
        void OnInteract(InputAction.CallbackContext context)
        {
            if (playerIsDriving) { StepOutOfBuggy(); return; }

            if (disablePlayerInput) { return; }
            CheckForInteractions();
            closestInteractable?.Interact();
        }


        #region Movement
        bool playerControllerActive;


        float stepTimer;
        void HandleWalking()
        {
            //other ideas:
            //bounce around like a frog (leap in target direction)?
            //skip (three faster steps followed by a small break)

            stepTimer += Time.deltaTime;
            if (stepTimer > 0.1f)
            {
                stepTimer -= 0.1f;
            }
            else
            {
                return;
            }

            float movementSpeed = 1f;
            Vector2 movementDirection = playerInput.Input.Directional.ReadValue<Vector2>().normalized * movementSpeed;

            rb.AddForce(movementDirection, ForceMode2D.Impulse);
        }
        #endregion


        private void OnDestroy()
        {
            playerInput.Dispose();
        }


        private void OnDrawGizmos()
        {
            if (interactionOrigin == null){ return; }
            Gizmos.color = Color.red;
            Gizmos.DrawLine(interactionOrigin.position, interactionOrigin.position + new Vector3(interactionCastDistance, 0f, 0f));
        }
    }
}
