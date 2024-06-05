using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Healing
{

    [RequireComponent(typeof(SpriteRenderer))]
    public class JoystickArt : MonoBehaviour
    {
        [SerializeField] Sprite[] joystickSprites;

        SpriteRenderer my;

        public void UpdateJoystickSprite(int notation)
        {
            notation -= 1;
            if (my == null)
            {
                my = GetComponent<SpriteRenderer>();
            }

            my.sprite = joystickSprites[notation];
        }
    }
}