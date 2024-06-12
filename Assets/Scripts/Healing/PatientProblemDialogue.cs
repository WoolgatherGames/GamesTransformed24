using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PatientProblemDialogue : ScriptableObject
{
    [SerializeField]
    string dialogue;
    [SerializeField]    
    ResourceTypes[] resources;

    public string Dialogue { get { return dialogue; } }
    public ResourceTypes[] Resources { get { return resources; } }
}
