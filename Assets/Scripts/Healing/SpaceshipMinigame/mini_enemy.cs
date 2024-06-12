using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mini_enemy : mini_destructable
{
    [SerializeField] GameObject projectile;

    //[SerializeField] //need to get the minigame manager
    public void SetValues()
    {
        //call when spawned
    }

    void ShootProjectile()
    {
        //instantiate as a child of the manager, and randomise direction
        Instantiate(projectile);
    }
}
