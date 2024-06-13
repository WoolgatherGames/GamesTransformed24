using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mini_enemy : mini_destructable
{
    [SerializeField] GameObject projectile;

    spaceshipminigamemanager manager;
    public void SetValues(spaceshipminigamemanager m)
    {
        //call when spawned
        manager = m;
    }

    public override void Die()
    {
        manager.SpawnEnemy();//spawn a replacement
        base.Die();
    }

    float projectileTimer;
    private void Update()
    {
        projectileTimer += Time.deltaTime;
        if (projectileTimer > 0.5f)
        {
            projectileTimer = 0f;
            if (Random.Range(0f, 1f) > 0.85f)
            {
                ShootProjectile();
            }
        }
    }

    void ShootProjectile()
    {
        //instantiate as a child of the manager, and randomise direction
        Instantiate(projectile, transform.position, Quaternion.identity, manager.transform).GetComponent<mini_projectile>().SetValues(manager);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("on trig");
        if (collision.GetComponent<Mini_Node>() != null)
        {
            manager.DestroyNodes(0);
        }
    }
}
