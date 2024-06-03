using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class Tree : HoldData, IInteractable
{
    public void Interact()
    {
        throw new System.NotImplementedException();
    }

    float timeTillReady;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        timeTillReady += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log(timeTillReady);
        }
    }
}
