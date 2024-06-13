using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PatientProblemDialogue : ScriptableObject
{
    [SerializeField, TextArea(2,3)]
    string dialogue;
    [SerializeField]    
    ResourceTypes[] resources;

    public string Dialogue { get { return dialogue; } }
    public ResourceTypes[] Resources { get { return resources; } }
}
