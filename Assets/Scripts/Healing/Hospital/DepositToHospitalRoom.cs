using Movement;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Healing;

namespace Inventory
{
    public class DepositToHospitalRoom : MonoBehaviour, IInteractable
    {
        //give this an input object
        [SerializeField] HospitalRoom room;
        [SerializeField] GameObject inventoryCanvas;

        int highlightedResourceNum;
        [SerializeField] ResourceTypes[] resourceDisplayOrder;
        ResourceTypes highlightedResource;


        [SerializeField] GameObject[] highlightSelection;
        [SerializeField] TMP_Text[] playerInventoryResourceCount;
        [SerializeField] TMP_Text[] roomInventoryResourceCount;

        bool isActive;

        void Start()
        {
            //GetComponent<PlayerInput>().Disable();
            inventoryCanvas.SetActive(false);
        }

        bool playerPressingToDeposit;
        float holdToDepositTimer;

        void OnDirectional(InputValue input)
        {
            if (!isActive) { return; }
            Vector2 direction = input.Get<Vector2>().normalized;

            if (direction.y < -0.9f)
            {
                highlightedResourceNum += 1;
            }
            else if (direction.y > 0.9f)
            {
                highlightedResourceNum -= 1;
            }

            highlightedResourceNum = Mathf.Clamp(highlightedResourceNum, 0, resourceDisplayOrder.Length - 1);
            highlightedResource = resourceDisplayOrder[highlightedResourceNum];

            if (direction.x > 0.9f)
            {
                //oaef let this be held down to deposit faster
                room.DepositResource(highlightedResource);
                holdToDepositTimer = 0.4f;
                playerPressingToDeposit = true;
            }
            else
            {
                playerPressingToDeposit = false;
            }
            if (direction.x < -0.9f)
            {
                room.RetrieveResource(highlightedResource);
            }

            UpdateHighlightedResourceDisplay();
        }

        void UpdateHighlightedResourceDisplay()
        {
            if (!isActive) { return; }

            if (playerInventoryResourceCount.Length != resourceDisplayOrder.Length || roomInventoryResourceCount.Length != resourceDisplayOrder.Length || highlightSelection.Length != resourceDisplayOrder.Length)
            {
                Debug.LogError("oh no *dies* \n the lengths of these lists is different when they need to be the same");
                return;
            }

            for (int i = 0; i < resourceDisplayOrder.Length; i++)
            {
                switch (resourceDisplayOrder[i])
                {
                    case ResourceTypes.flower:
                        playerInventoryResourceCount[i].text = "x" + PlayerInventory.FlowerCount.ToString();
                        roomInventoryResourceCount[i].text = "x" + room.FlowerCount.ToString();
                        break;
                    case ResourceTypes.mushroom:
                        playerInventoryResourceCount[i].text = "x" + PlayerInventory.MushroomCount.ToString();
                        roomInventoryResourceCount[i].text = "x" + room.MushroomCount.ToString();
                        break;
                    case ResourceTypes.feathers:
                        playerInventoryResourceCount[i].text = "x" + PlayerInventory.FeathersCount.ToString();
                        roomInventoryResourceCount[i].text = "x" + room.FeathersCount.ToString();
                        break;
                    case ResourceTypes.treeSap:
                        playerInventoryResourceCount[i].text = "x" + PlayerInventory.TreeSapCount.ToString();
                        roomInventoryResourceCount[i].text = "x" + room.TreeSapCount.ToString();
                        break;
                    case ResourceTypes.conchShell:
                        playerInventoryResourceCount[i].text = "x" + PlayerInventory.ConchShellCount.ToString();
                        roomInventoryResourceCount[i].text = "x" + room.ConchShellCount.ToString();
                        break;
                }

                //do the glow thing
                if (i == highlightedResourceNum)
                {
                    highlightSelection[highlightedResourceNum].SetActive(true);
                }
                else
                {
                    highlightSelection[i].SetActive(false);
                }
            }
        }

        float preventDoubleClick;
        void Update()
        {
            if (preventDoubleClick >= 0f)
            {
                preventDoubleClick -= Time.deltaTime;
            }
            if (!isActive) { return; }

            if (playerPressingToDeposit && holdToDepositTimer <= 0f)
            {
                room.DepositResource(highlightedResource);
                holdToDepositTimer = 0.1f;
                playerPressingToDeposit = true;
                UpdateHighlightedResourceDisplay();
            }
            else
            {
                holdToDepositTimer -= Time.deltaTime;
            }
        }
        public void OnInteract(InputValue input)
        {
            if (!isActive) { return; }
            if (preventDoubleClick >= 0f)
            {
                return;
            }
            preventDoubleClick = 0.4f;
            //GetComponent<PlayerInput>().Disable();

            PlayerController.Instance.EnableCharacterController();
            inventoryCanvas.SetActive(false);

            isActive = false;
        }

        public void Interact()
        {
            if (isActive) { return; }
            if (preventDoubleClick >= 0f)
            {
                return;
            }

            //GetComponent<PlayerInput>().Enable();
            preventDoubleClick = 0.25f;
            PlayerController.Instance.DisableCharacterController();
            highlightedResourceNum = 0;
            isActive = true;
            playerPressingToDeposit = false;

            inventoryCanvas.SetActive(true);
            UpdateHighlightedResourceDisplay();
        }

        [SerializeField] Transform interactionPromptPos;
        public Vector3 ReturnPositionForPromptIndicator()
        {
            return interactionPromptPos.position;
        }
    }
}