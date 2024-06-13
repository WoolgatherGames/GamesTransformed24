using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini_Node : MonoBehaviour
{
    //Nodes

    [SerializeField] LayerMask mask;

    public int myIndex;

    public List<mini_destructable> CastRay()
    {
        //check all objects left + right 

        RaycastHit2D[] hitLeft = Physics2D.RaycastAll(transform.position, Vector2.left, Mathf.Infinity, mask);
        RaycastHit2D[] hitRight = Physics2D.RaycastAll(transform.position, Vector2.right, Mathf.Infinity, mask);

        List<GameObject> hitObjects = new List<GameObject>();
        //RaycastHit2D[] finalHit = null;
        List<RaycastHit2D> finalHit = new List<RaycastHit2D>();

        //Check both left and right directions and determine which side has a mark on it
        //note: could mby improve the efficiency if 
        bool skipRight = false;
        for (int left = 0; left < hitLeft.Length; left++)
        {
            if (hitLeft[left].collider.GetComponent<Mini_Node>() != null)
            {
                skipRight = true;
                //add all the objects to the left of this UP until the node was hit
                for (int priorToLeftNode = 0; priorToLeftNode < left; priorToLeftNode++)
                {
                    finalHit.Add(hitLeft[priorToLeftNode]);
                }
            }
        }
        if (!skipRight)
        {
            for (int right = 0; right < hitRight.Length; right++)
            {
                if (hitRight[right].collider.GetComponent<Mini_Node>() != null)
                {
                    for (int priorToRightNode = 0; priorToRightNode < right; priorToRightNode++)
                    {
                        finalHit.Add(hitRight[priorToRightNode]);
                    }
                }
            }
        }


        if (finalHit.Count > 0)
        {
            foreach (RaycastHit2D hit in finalHit)
            {
                hitObjects.Add(hit.collider.gameObject);
            }
        }


        List<mini_destructable> healthObjects = new List<mini_destructable>();

        if (hitObjects.Count > 0)
        {
            //check if they have a health bar, and deal damage to them if so 
            foreach (GameObject obj in hitObjects)
            {
                if (obj.GetComponent<mini_destructable>() != null)
                {
                    healthObjects.Add(obj.GetComponent<mini_destructable>());
                }
            }
        }

        return healthObjects;
    }
}
