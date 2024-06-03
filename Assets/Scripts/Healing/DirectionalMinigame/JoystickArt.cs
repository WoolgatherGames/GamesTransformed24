using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Healing
{

    [RequireComponent(typeof(SpriteRenderer))]
    public class JoystickArt : MonoBehaviour
    {
        [SerializeField] Sprite[] joystickSprites;

        SpriteRenderer me;

        private void Start()
        {
            me = GetComponent<SpriteRenderer>();
        }

        public void UpdateJoystickSprite(int notation)
        {
            notation -= 1;
            me.sprite = joystickSprites[notation];
        }
    }
}