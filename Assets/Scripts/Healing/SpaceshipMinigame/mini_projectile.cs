using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mini_projectile : MonoBehaviour
{
    //wanna change this so projectiles come from the sides of the screen and travel inwards
    [SerializeField] float lifetime;
    [SerializeField] float speed;


    spaceshipminigamemanager manager;


    public void SetValues(spaceshipminigamemanager m)
    {
        //call when spawned
        manager = m;

        transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
    }


    private void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime * Mathf.Clamp(lifetime, 0f, 2f);
        lifetime -= Time.deltaTime;
        if ( lifetime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("on trig");
        if (collision.GetComponent<Mini_Node>() != null)
        {
            manager.DestroyNodes(0);
            Destroy(gameObject);
        }
    }
}
