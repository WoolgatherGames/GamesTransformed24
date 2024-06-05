using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;
using Movement;
using Inventory;

public class Tree : HoldData, IInteractable
{
    public void Interact()
    {
        if (timeTillReady <= 0f && timeTillMined < 0f)
        {
            PlayerController.Instance.DisableCharacterController();
            timeTillMined = 1.5f;//how long it takes to mine the tree
        }
        else
        {
            Debug.Log("This tree has no sap right now, come back later");
        }

    }

    float timeTillReady;

    float timeTillMined;//whilst above 0, means that this tree is being mined

    void Start()
    {
        timeTillMined = -1f;
    }

    void Update()
    {
        if (timeTillReady > 0f)
        {
            timeTillReady -= Time.deltaTime;
        }

        if (timeTillMined > 0f)
        {
            timeTillMined -= Time.deltaTime;
            if (timeTillMined <= 0f)
            {
                //collect resource from this free
                CollectResources();
            }
        }
    }

    void CollectResources()
    {
        timeTillMined = -1f;//ensure this is reset

        //BALANCE
        timeTillReady = 60f;//duration between trees
        int sapCollected = Random.Range(4, 8);//remember that the 2nd number is exclusive

        PlayerInventory.CollectResource(ResourceTypes.treeSap, sapCollected);

        PlayerController.Instance.EnableCharacterController();
    }

    protected override void LoadData(DataChunk recievedData)
    {
        TimeStamp data = recievedData as TimeStamp;
        if (data != null)//if data is null, this is probably the first time its ever been logged ...OR a horrible error has occured and two objects with the same ID exist
        {
            timeTillReady = data.Time;
        }
    }

    protected override DataChunk SaveData()
    {
        return new TimeStamp(timeTillReady);
    }
}
