using Healing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositToHospitalInventory : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        HospitalInventory.Instance.DepositAllResources();
    }

    [SerializeField] private Transform interactionPromptLocation;
    public Vector3 ReturnPositionForPromptIndicator()
    {
        return interactionPromptLocation.position;
    }
}
