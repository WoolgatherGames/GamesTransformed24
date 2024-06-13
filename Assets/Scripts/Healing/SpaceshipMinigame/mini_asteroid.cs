using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mini_asteroid : mini_destructable
{
    //alternative to the enemy, spawn these in from beyond the screen bounds and have them fly in at a specific angle 

    //[SerializeField] float lifetime;
    float speed;
    //[SerializeField] AnimationCurve speedMult;

    spaceshipminigamemanager manager;
    public void SetValues(spaceshipminigamemanager m, float direction, Vector4 setBoundaries)
    {
        //call when spawned
        manager = m;

        transform.eulerAngles = new Vector3(0f, 0f, direction);
        boundaries = setBoundaries;

        speed = Random.Range(0.15f, 0.3f);
    }

    Vector4 boundaries;
    float lifeTimer;
    private void Update()
    {
        lifeTimer += Time.deltaTime;
        //float bonusSpeed = speedMult.Evaluate(lifeTimer);
        //if (bonusSpeed < 1f) { bonusSpeed = 1f; }
        float speedMult = Mathf.Clamp(11f - (lifeTimer * 2f), 1f, 4.5f);

        transform.position += transform.up * speed * Time.deltaTime * speedMult;
        if (transform.position.x < boundaries.x || transform.position.x > boundaries.y || transform.position.y < boundaries.z || transform.position.y > boundaries.w)
        {
            Destroy(gameObject);
        }

        //lifetime -= Time.deltaTime;
        //if (lifetime < 0) { Destroy(gameObject); }

        if (triggerEntered)
        {
            triggerTimer += Time.deltaTime;
            if (triggerTimer > 0.1f)
            {
                triggerEntered = false;
                manager.DestroyNodes(highestIndexFound);
            }
        }
    }

    bool triggerEntered;
    float triggerTimer;
    int highestIndexFound;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.GetComponent<Mini_Node>() != null)
        {
            gameObject.name = collision.GetComponent<Mini_Node>().myIndex.ToString();
            Debug.Log(collision.GetComponent<Mini_Node>().myIndex);
            if (!triggerEntered)
            {
                triggerEntered = true;
                triggerTimer = 0f;
                highestIndexFound = 0;
            }

            //manager.DestroyNodes();
            //manager.DestroyNodes(collision.GetComponent<Mini_Node>().myIndex);
            if (collision.GetComponent<Mini_Node>().myIndex > highestIndexFound)
            {
                highestIndexFound = collision.GetComponent<Mini_Node>().myIndex;
            }
        }
    }
}
