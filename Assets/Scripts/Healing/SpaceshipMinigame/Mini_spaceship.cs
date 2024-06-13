using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Mini_spaceship : MonoBehaviour
{
    [SerializeField] Transform nodeSpawnPos;
    [SerializeField] GameObject node;//a collider plus the node object code
    [SerializeField] float nodeRadius = 0.5f;//make sure this matches the radius of the collider on the node objects prefab
    Vector2 lastNodePosition;

    [SerializeField]
    LineRenderer lineRenderer;

    bool disableNodeSpawn;

    [SerializeField] Vector2 widthBounds;
    [SerializeField] Vector2 heightBounds;


    private void Update()
    {
        if (transform.localPosition.x < widthBounds.x)
        {
            transform.localPosition = new Vector3(widthBounds.y - 0.1f, transform.position.y, 0f);
            ResetNodes();
        }
        else if (transform.localPosition.x > widthBounds.y)
        {
            transform.localPosition = new Vector3(widthBounds.x + 0.1f, transform.position.y, 0f);
            ResetNodes();
        }
        if (transform.localPosition.y < heightBounds.x)
        {
            transform.localPosition = new Vector3(transform.position.x, heightBounds.y - 0.1f, 0f) ;
            ResetNodes();
        }
        else if (transform.localPosition.y > heightBounds.y)
        {
            transform.localPosition = new Vector3(transform.position.x, heightBounds.x + 0.1f, 0f);
            ResetNodes();
        }

        if (!disableNodeSpawn)
        {
            if (nodeList.Count == 0)
            {
                CreateNode(nodeSpawnPos.position);
            }
            else
            {
                float distance = Vector2.Distance(lastNodePosition, nodeSpawnPos.position);
                int loopsThisFrame = 0;
                while (distance > nodeRadius)
                {
                    CreateNode(nodeSpawnPos.position);

                    distance = Vector2.Distance(lastNodePosition, nodeSpawnPos.position);

                    //incase of emergency
                    loopsThisFrame++;
                    if (loopsThisFrame > 50)//if the player can move more than this many radius' of their node per frame. then you need to raise this number
                        break;
                }
            }
        }

        if (destroyNodesEmergencyBreak > 0f)
        {
            destroyNodesEmergencyBreak -= Time.deltaTime;
        }
    }
    List<Mini_Node> nodeList = new List<Mini_Node>();
    List<Vector2> nodePositions = new List<Vector2>();
    void CreateNode(Vector2 placementPoint)
    {
        Mini_Node newNode = Instantiate(node, placementPoint, Quaternion.identity).GetComponent<Mini_Node>();
        newNode.myIndex = nodeList.Count;
        nodeList.Add(newNode);

        nodePositions.Add(placementPoint);
        lastNodePosition = placementPoint;

        CheckNodeConnections(placementPoint);

        UpdateLineRenderer();
    }

    void CheckNodeConnections(Vector2 placementPoint)
    {
        //check if any connection has been made with a previous mode (forming a circle)
        for (int i = 0; i < nodePositions.Count - 1; i++)
        {
            //dont count the last node made or the one we just made
            if (Vector2.Distance(placementPoint, nodePositions[i]) < nodeRadius)
            {
                //A connection has been found. From the found node, to the placed node. Trigger all of their raycasts, then deal damage to the found targets

                //Delete all nodes BEFORE this one
                if (i > 0)
                {
                    for (int delete = i - 1; delete >= 0; delete--)
                    {
                        nodePositions.RemoveAt(delete);
                        Destroy(nodeList[delete]);
                        nodeList.RemoveAt(delete);
                    }
                }

                StartCoroutine(TriggerNodes());
                /*List<Health> objectsToTakeDamage = new List<Health>();//removed this code and put it in a coroutine
                for (int x = 0; x < nodeList.Count; x++)
                {
                    objectsToTakeDamage.AddRange(nodeList[x].GetComponent<Node>().CastRay());
                }



                objectsToTakeDamage = objectsToTakeDamage.Distinct().ToList();//remove duplicates from the list to ensure the same object doesnt take damage multiple times
                foreach (Health target in objectsToTakeDamage)
                {
                    //deal damage to them based on the size of the circle
                    target.TakeDamage(CalculateDamage(nodePositions.Count));
                }

                ResetNodes();*/
                break;
            }
        }
    }
    IEnumerator TriggerNodes()
    {
        disableNodeSpawn = true;
        yield return new WaitForSeconds(0.1f);
        List<mini_destructable> objectsToTakeDamage = new List<mini_destructable>();
        for (int x = 0; x < nodeList.Count; x++)
        {
            objectsToTakeDamage.AddRange(nodeList[x].CastRay());
        }
        yield return new WaitForEndOfFrame();//throwing this in here for performance reasons to just, ensure that too much code doesnt happen in one frame

        objectsToTakeDamage = objectsToTakeDamage.Distinct().ToList();//remove duplicates from the list to ensure the same object doesnt take damage multiple times
        foreach (mini_destructable target in objectsToTakeDamage)
        {
            target.Die();
        }

        yield return new WaitForSeconds(0.2f);
        ResetNodes();
        disableNodeSpawn = false;
    }

    [SerializeField] float baseDamage = 20f;
    [SerializeField] float damagePerNode = 0.2f;


    public void ResetNodes()
    {
        nodePositions = new List<Vector2>();
        foreach (Mini_Node node in nodeList)
        {
            Destroy(node.gameObject);
        }
        nodeList = new List<Mini_Node>();

        UpdateLineRenderer();
    }


    float destroyNodesEmergencyBreak;
    public void DestroyNodesFromIndex(int index)
    {
        if (destroyNodesEmergencyBreak > 0f) { return; } else { destroyNodesEmergencyBreak = 0.1f; }//testing
        if (index >= nodeList.Count)
        {
            return;
        }

        for (int i = 0; i < index; i++)
        {
            Destroy(nodeList[i].gameObject);
        }

        nodeList.RemoveRange(0, index + 1);
        nodePositions.RemoveRange(0, index + 1);

        if (nodeList.Count > 0)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].myIndex = i;
            }
        }
        UpdateLineRenderer();
    }

    void UpdateLineRenderer()
    {
        Vector3[] linePositions = new Vector3[nodePositions.Count];
        for (int i = 0; i < linePositions.Length; i++)
        {
            linePositions[i] = new Vector3(nodePositions[i].x, nodePositions[i].y, 0f);
        }

        lineRenderer.positionCount = linePositions.Length;
        lineRenderer.SetPositions(linePositions);
    }

    Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    Vector2 movementInput;
    void OnDirectional(InputValue input)
    {
        movementInput = input.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    float accelerationTimer;
    void Movement()
    {
        //add a check to see if player WANTS to go max speed or not
        float movementSpeed = 17f;

        float accelerationPower = 0.75f;
        float turnSpeed = -15f;
        float maxAcceleration = 2f;
        float minAcceleration = -0.5f;

        float forwardInput = movementInput.y;
        float turnInput = movementInput.x;

        if (forwardInput > 0f)
        {
            float mod = accelerationTimer > 0f ? 1f : 3f;
            accelerationTimer += forwardInput * 0.02f * accelerationPower * mod;//note: 0.02f is basically delta time inside fixed update
        }
        else if (forwardInput < 0f)
        {
            float mod = accelerationTimer < 0f ? 1f : 3f;
            accelerationTimer += forwardInput * 0.02f * accelerationPower * mod;
        }
        else
        {
            //player is not inputting anything. move towards zero
            //accelerationTimer *= 0.85f;
            float mod = accelerationTimer > 0f ? -1.5f : 1.5f;
            accelerationTimer += mod * accelerationPower * 0.02f;
        }
        accelerationTimer = Mathf.Clamp(accelerationTimer, minAcceleration, maxAcceleration);
        float totalSpeed = accelerationTimer * movementSpeed;

        //test this:
        //increase turn speed IF the car isnt turning forward (drift?)
        if (forwardInput < 0f)
            turnInput *= 2f;
        else if (forwardInput == 0f)
            turnInput *= 1.25f;

        //if travelling at high speed, reduce turn speed
        //float turnDampner = Mathf.Lerp(1f, 0.75f, accelerationTimer / maxAcceleration);
        //turnInput *= turnDampner;

        rb.AddTorque(turnInput * turnSpeed);
        rb.AddForce(transform.up * totalSpeed);
    }
}
