using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//was trying to do a lil bit of animation frame shennigans
namespace Movement
{
    public class PlayerSprite : MonoBehaviour
    {

        Transform playerTransform;
        private void Start()
        {
            transform.SetParent(null);
            playerTransform = PlayerController.Instance.transform;
        }

        float timer;
        void Update()
        {
            timer += Time.deltaTime;

            if (timer > 0.05f)
            {
                transform.position = playerTransform.position;
                timer = 0f;
            }
        }
    }
}