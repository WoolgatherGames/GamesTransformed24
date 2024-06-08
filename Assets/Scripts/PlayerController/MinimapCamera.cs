using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] Transform playerBody;
    float zPos;

    void Start()
    {
        zPos = transform.position.z;
        GetComponent<Camera>().aspect = 1f;
        //transform.SetParent(playerBody);
    }

    private void Update()
    {
        transform.position = new Vector3(playerBody.position.x, playerBody.position.y, zPos);
    }
}
