using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Healing
{
    public class DirectionalInputDisplay : MonoBehaviour
    {
        [SerializeField] Sprite[] sprites;

        public void UpdateSprite(int spriteNumber)
        {
            //0 = quarter forward
            //1 = quarter back
            //2 = over forward
            //3 = over back
            //4 = zig forward
            //5 = zig back
            //6 = semi forward
            //7 = semi back
            GetComponent<SpriteRenderer>().sprite = sprites[spriteNumber];
        }
    }
}