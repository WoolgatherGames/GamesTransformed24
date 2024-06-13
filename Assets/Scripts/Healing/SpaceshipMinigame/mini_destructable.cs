using Healing;
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
        if (HealingManager.Instance != null)
        {
            float healPercentage = Random.Range(2f, 6f);
            HealingManager.Instance.MinigameHealPatient(healPercentage);
        }
        Destroy(gameObject);
    }
}
