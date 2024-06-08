using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCameraAspectRatio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Camera>().aspect = 1.8f;
    }
}
