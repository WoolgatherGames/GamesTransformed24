using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class mini_destructable : MonoBehaviour
{
    //make sure this object has a collider

    //[SerializeField] float health;

    /*public void TakeDamage(float damage)
    {
        health -= damage;

        if (health < 0f)
        {
            Die();
        }
    }*/

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
