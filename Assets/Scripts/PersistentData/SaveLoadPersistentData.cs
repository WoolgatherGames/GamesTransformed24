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

        public static void DestroyData(string dataID)
        {
            VerifyData();
            if (savedData.ContainsKey(dataID))
            {
                savedData.Remove(dataID);
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
    public class PatientData : DataChunk
    {
        //save location, healing progress and time
        private float currentHealingProgress;
        private float maxHealingProgress;
        private float healingRate;
        private float lastHealTimeStamp;
        private float nextConsumptionTimeStamp;
        private Healing.Patient.PatientProblems medicalProblem;

        public float CurrentHealingProgress { get { return currentHealingProgress; } }
        public float MaxHealingProgress { get { return maxHealingProgress; } }
        public float HealingRate { get {  return healingRate; } }
        public float LastHealTimeStamp { get {  return lastHealTimeStamp; } }
        public float NextConsumptionTimeStamp { get { return  nextConsumptionTimeStamp; } }
        public Healing.Patient.PatientProblems MyProblem { get { return medicalProblem; } }

        public PatientData(float currentHealingProgress, float maxHealingProgress, float healingRate, float lastHealTimeStamp, float newConsumptionTimeStamp, Healing.Patient.PatientProblems medicalProblem)
        {
            this.currentHealingProgress = currentHealingProgress;
            this.maxHealingProgress = maxHealingProgress;
            this.healingRate = healingRate;
            this.lastHealTimeStamp = lastHealTimeStamp;
            this.nextConsumptionTimeStamp = newConsumptionTimeStamp;
            this.medicalProblem = medicalProblem;
        }
    }

    public class HospitalInventoryData : DataChunk
    {
        int flowerCount;
        public int FlowerCount { get { return flowerCount; } }

        int treeSapCount;
        public int TreeSapCount { get { return treeSapCount; } }

        int feathersCount;
        public int FeathersCount { get { return feathersCount; } }

        int mushroomCount;
        public int MushroomCount { get { return mushroomCount; } }

        int conchShellCount;
        public int ConchShellCount { get { return conchShellCount; } }

        public HospitalInventoryData(int flowerCount, int treeSapCount, int feathersCount, int mushroomCount, int conchShellCount)
        {
            this.flowerCount = flowerCount;
            this.treeSapCount = treeSapCount;
            this.feathersCount = feathersCount;
            this.mushroomCount = mushroomCount;
            this.conchShellCount = conchShellCount;
        }
    }

    public class FlowerPotData : DataChunk
    {
        float growthStartTime;
        bool isPlanted;

        public float GrowthStart { get { return growthStartTime; } }
        public bool IsPlanted { get { return isPlanted; } }

        public FlowerPotData(float growthStartTime, bool isPlanted)
        { 
            this.growthStartTime = growthStartTime; this.isPlanted = isPlanted;
        }
    }

    public class ForestResourceManagerData : DataChunk
    {
        int numberOfFlowers;
        public int NumberOfFlowers { get { return numberOfFlowers; } }
        int numberOfMushrooms;
        public int NumberOfMushrooms { get { return numberOfMushrooms; } }
        int numberOfFeathers;
        public int NumberOfFeathers { get { return numberOfFeathers; } }

        float spawnPlantResourcesTimeStamp;
        public float SpawnPlantResourcesTimeStamp { get { return spawnPlantResourcesTimeStamp; } }
        float spawnAirTimeStamp;
        public float SpawnAirTimeStamp { get { return spawnAirTimeStamp; } }


        public ForestResourceManagerData(int numberOfFlowers, int numberOfMushrooms, int numberOfFeathers, float spawnPlantResourcesTimeStamp, float spawnAirTimeStamp)
        {
            this.numberOfFlowers = numberOfFlowers;
            this.numberOfMushrooms = numberOfMushrooms;
            this.numberOfFeathers = numberOfFeathers;
            this.spawnPlantResourcesTimeStamp = spawnPlantResourcesTimeStamp;
            this.spawnAirTimeStamp = spawnAirTimeStamp;
        }
    }
}

