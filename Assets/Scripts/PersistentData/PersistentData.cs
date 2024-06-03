using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public class PersistentData : ScriptableObject
    {
        //im scrapping this class. turns out a static class should save data anyway
        Dictionary<string, DataChunk> savedData;

        private void OnEnable()
        {
            savedData = new Dictionary<string, DataChunk>();
        }

        public DataChunk LoadData(string id)
        {
            savedData.TryGetValue(id, out DataChunk chunk);
            return chunk;
        }
        public void SaveData(string id, DataChunk newData)
        {
            if (savedData.ContainsKey(id))
            {
                savedData[id] = newData;
            }
            else
            {
                savedData.Add(id, newData);
            }
        }
    }




}

