using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractable
{
    //note, this needs to be able to tell the player controller to stop moving on interact, but also when the player can act again
    public void Interact();

    public Vector3 ReturnPositionForPromptIndicator();
}

