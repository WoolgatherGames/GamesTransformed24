using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Foraging;

namespace Movement
{
    public class PlayerController : MonoBehaviour
    {
        private static PlayerController instance;
        public static PlayerController Instance { get { return instance; } }

        PlayerInput playerInput;
        Rigidbody2D rb;

        [SerializeField] GameObject InventoryCanvas;

        [SerializeField] Transform InteractablePrompt;

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
            InventoryCanvas.SetActive(true);
            InteractablePrompt.gameObject.SetActive(false);
        }


        float checkForInteractionsTimer;
        private void Update()
        {
            PickupNearbyObjects();

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
            InventoryCanvas.SetActive(false);
        }
        public void EnableCharacterController()
        {
            disablePlayerInput = false;
            InventoryCanvas.SetActive(true);
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
        [SerializeField] LayerMask interactionRaycastLayers;
        IInteractable closestInteractable;

        public delegate void InteractionTargetChanged();
        public static event InteractionTargetChanged OnInteractionTargetChange;
        void CheckForInteractions()
        {
            if (disablePlayerInput) { return; }

            RaycastHit2D[] hits = Physics2D.CircleCastAll(interactionOrigin.position, interactionCastDistance, Vector2.up, 0f);
            foreach (RaycastHit2D hit in hits)
            {
                //check if it has line of sight to the player (so you cant interact through walls)
                Vector3 directionVector = hit.transform.position - interactionOrigin.position;
                RaycastHit2D doubleCheckHit = Physics2D.Raycast(interactionOrigin.position, directionVector.normalized, interactionCastDistance, interactionRaycastLayers);
                
                if (doubleCheckHit.collider != null)
                {
                    if (doubleCheckHit.collider.GetComponent<IInteractable>() != null)
                    {
                        closestInteractable = doubleCheckHit.collider.GetComponent<IInteractable>();
                        //show an object here to say "press [] to interact"
                        InteractablePrompt.gameObject.SetActive(true);
                        InteractablePrompt.position = closestInteractable.ReturnPositionForPromptIndicator();
                        return;//stop doing the loop, we've found an interactable
                    }
                }
            }

            if (closestInteractable != null) { OnInteractionTargetChange?.Invoke(); }

            closestInteractable = null;//we made it out the loop without finding anything
            InteractablePrompt.gameObject.SetActive(false);
        }
        void OnInteract(InputAction.CallbackContext context)
        {
            if (PauseMenu.Instance.GamePaused) { return; }
            if (playerIsDriving) { StepOutOfBuggy(); return; }

            if (disablePlayerInput) { return; }
            CheckForInteractions();

            closestInteractable?.Interact();
            InteractablePrompt.gameObject.SetActive(false);
        }

        [SerializeField] Transform colliderPosition;
        [SerializeField] float pickupDistance;
        float pickUpCheckTimer;
        void PickupNearbyObjects()
        {
            if (pickUpCheckTimer > 0f)
            {
                pickUpCheckTimer -= Time.deltaTime;
                return;
            }

            RaycastHit2D[] hits = Physics2D.CircleCastAll(interactionOrigin.position, pickupDistance, Vector2.up, 0f);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.GetComponent<ForagableResource>() != null)
                {
                    //move the hit object towards the players collider (and for
                    Vector3 directionVector = hit.collider.transform.position - colliderPosition.position;
                    hit.collider.transform.position -= directionVector.normalized * Time.deltaTime * 7f;//speed of pull

                    if (directionVector.sqrMagnitude < Mathf.Pow((pickupDistance * 0.1f), 2))
                    {
                        hit.collider.GetComponent<ForagableResource>().Pickup();
                    }

                    return;
                }
            }

            pickUpCheckTimer = 0.1f;//if we didnt pick anything up, dont check next frame, wait a lil while
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
