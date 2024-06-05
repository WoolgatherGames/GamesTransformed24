using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;
using System;
using UnityEngine.SceneManagement;

public abstract class HoldData : MonoBehaviour
{

    protected virtual void Awake()
    {
        LoadData(SaveLoadPersistentData.LoadData(GetUniqueID()));

    }

    protected virtual void OnDestroy()
    {
        SaveLoadPersistentData.SaveData(GetUniqueID(), SaveData());
    }

    protected abstract DataChunk SaveData();
    protected abstract void LoadData(DataChunk recievedData);

    protected string GetUniqueID()
    {
        //i know two things about objects on load. their name, and their position. so i could use that to generate a unique ID. Scene name is also important as itd be hard to manage this across all scenes
        //note this will break if two objects with the same name and position exist and both need save data...itll break so bad
        //a better system would be to use guid but i have spent so many hours figuring this out and its just a game jam...im 95% confident nothing will break


        //To make this more robust add the line: this.GetType().ToString() 
        //note: that this returns the namespace.scriptName 


        string sceneName = SceneManager.GetActiveScene().name;
        string name = this.gameObject.name;
        string position = transform.position.ToString();

        return sceneName + name + position;
    }
}
