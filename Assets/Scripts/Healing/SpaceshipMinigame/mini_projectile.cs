using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mini_projectile : MonoBehaviour
{
    [SerializeField] float lifetime;
    [SerializeField] float speed;

    private void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
        lifetime -= Time.deltaTime;
        if ( lifetime < 0)
        {
            Destroy(gameObject);
        }
    }
}
