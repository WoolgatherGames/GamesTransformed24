using GameData;
using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Healing
{
    public class HospitalInventory : HoldData
    {
        private static HospitalInventory instance;
        public static HospitalInventory Instance { get { return instance; } }

        protected override void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            base.Awake();
        }


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

        public void DepositAllResources()
        {
            flowerCount += PlayerInventory.FlowerCount;
            treeSapCount += PlayerInventory.TreeSapCount;
            feathersCount += PlayerInventory.FeathersCount;
            mushroomCount += PlayerInventory.MushroomCount;
            conchShellCount += PlayerInventory.ConchShellCount;

            PlayerInventory.EmptyResources();

            Debug.Log("flowers: " + flowerCount + ", Tree Sap: " + treeSapCount + ", Feathers: " + feathersCount + ", Mushrooms: " + mushroomCount + ", Conch Shell: " + conchShellCount);
        }

        public bool PatientConsumeResource(ResourceTypes[] consumeResources, int[] consumeCount)
        {
            ///check if the resources CAN be consumed
            ///returns true if there are enough resources to be consumed. Returns false if there arn't

            for (int i = 0; i < consumeResources.Length; i++)
            {
                int consumeThisMany = consumeCount[i];

                switch (consumeResources[i])
                {
                    case ResourceTypes.flower:
                        if (flowerCount - consumeThisMany <= 0)
                        {
                            return false;
                        }
                        break;
                    case ResourceTypes.treeSap:
                        if (treeSapCount - consumeThisMany <= 0)
                        {
                            return false;
                        }
                        break;
                    case ResourceTypes.feathers:
                        if (feathersCount - consumeThisMany <= 0)
                        {
                            return false;
                        }
                        break;
                    case ResourceTypes.mushroom:
                        if (mushroomCount - consumeThisMany <= 0)
                        {
                            return false;
                        }
                        break;
                    case ResourceTypes.conchShell:
                        if (conchShellCount - consumeThisMany <= 0)
                        {
                            return false;
                        }
                        break;
                }
            }

            //now remove those resources
            for (int i = 0; i < consumeResources.Length; i++)
            {
                int consumeThisMany = consumeCount[i];

                switch (consumeResources[i])
                {
                    case ResourceTypes.flower:
                        flowerCount -= consumeThisMany; break;
                    case ResourceTypes.treeSap:
                        treeSapCount -= consumeThisMany; break;
                    case ResourceTypes.feathers:
                        feathersCount -= consumeThisMany; break;
                    case ResourceTypes.mushroom:
                        mushroomCount -= consumeThisMany; break;
                    case ResourceTypes.conchShell:
                        conchShellCount -= consumeThisMany; break;
                }
            }

            return true;
        }

        protected override DataChunk SaveData()
        {
            return new HospitalInventoryData(FlowerCount, TreeSapCount, FeathersCount, MushroomCount, ConchShellCount);
        }

        protected override void LoadData(DataChunk recievedData)
        {
            HospitalInventoryData data = recievedData as HospitalInventoryData;
            if (data != null)
            {
                flowerCount = data.FlowerCount;
                treeSapCount = data.TreeSapCount;
                feathersCount = data.FeathersCount;
                mushroomCount = data.MushroomCount;
                conchShellCount = data.ConchShellCount;
            }
            else
            {
                flowerCount = 0;
                treeSapCount = 0;
                feathersCount = 0;
                mushroomCount = 0;
                conchShellCount = 0;
            }

        }
    }
}