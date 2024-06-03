using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    enum DoorToScene
    {
        Hospital,
        Forest,
    }

    [SerializeField] DoorToScene enterScene;

    public void Interact()
    {
        switch (enterScene)
        {
            case DoorToScene.Hospital:
                SceneManager.LoadScene("Hospital"); break;
            case DoorToScene.Forest:
                SceneManager.LoadScene("Forest"); break;
        }
    }
}
