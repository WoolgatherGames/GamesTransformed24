using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public static class SaveLoadPersistentData
    {

        static Dictionary<string, DataChunk> savedData;
        //i could make this more efficient and filter between scenes??

        public static DataChunk LoadData(string id)
        {
            VerifyData();
            savedData.TryGetValue(id, out DataChunk chunk);
            return chunk;
        }
        public static void SaveData(string id, DataChunk newData)
        {
            VerifyData();
            if (savedData.ContainsKey(id))
            {
                savedData[id] = newData;
            }
            else
            {
                savedData.Add(id, newData);
            }
        }

        static void VerifyData()
        {
            if (savedData == null)
            {
                savedData = new Dictionary<string, DataChunk>();
            }
        }


        #region LegacyScriptableObjectSystem
        //static PersistentData data;
        /*public static void SaveData(string objectID, DataChunk dataToSave)
        {
            boop += 1f;

            VerifyData();
            data.SaveData(objectID, dataToSave);
        }

        public static DataChunk LoadData(string objectID)
        {
            Debug.Log(boop);

            VerifyData();
            return data.LoadData(objectID);
        }

        static void VerifyData()
        {
            //set data if it doesnt exist
            if (data == null)
            {
                data = Resources.Load<PersistentData>("PersistentData");
            }
        }*/
        #endregion
    }


    public class DataChunk
    {

    }
    public class LocationInformation : DataChunk
    {
        Vector2 myPosition;
        public Vector2 MyPosition { get { return myPosition; } }

        public LocationInformation(Vector2 position)
        {
            myPosition = position;
        }
    }
    public class TimeStamp : DataChunk
    {
        private float time;
        public float Time { get { return time; } }

        public TimeStamp(float _time)
        {
            time = _time;
        }
    }
    public class Patient : DataChunk
    {
        //save location, healing progress and time
        private float healingProgress;
    }
}

