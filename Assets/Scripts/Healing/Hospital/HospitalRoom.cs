using GameData;
using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Healing
{
    public class HospitalRoom : HoldData
    {
        //make a trigger bound around the entire room 
        //each room stores its own inventory

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

        public void DepositResource(ResourceTypes resource)
        {
            if (PlayerInventory.RemoveResource(resource))
            {
                switch (resource)
                {
                    case ResourceTypes.flower:
                        flowerCount++; break;
                    case ResourceTypes.treeSap:
                        treeSapCount++; break;
                    case ResourceTypes.feathers:
                        feathersCount++; break;
                    case ResourceTypes.mushroom:
                        mushroomCount++; break;
                    case ResourceTypes.conchShell:
                        conchShellCount++; break;
                }
            }
        }

        public void RetrieveResource(ResourceTypes resource)
        {
            switch (resource)
            {
                case ResourceTypes.flower:
                    if (flowerCount > 0) { flowerCount--; PlayerInventory.CollectResource(resource, 1); }
                    return;
                case ResourceTypes.treeSap:
                    if (treeSapCount > 0) { treeSapCount--; PlayerInventory.CollectResource(resource, 1); }
                    return;
                case ResourceTypes.feathers:
                    if (feathersCount > 0) { feathersCount--; PlayerInventory.CollectResource(resource, 1); }
                    return;
                case ResourceTypes.mushroom:
                    if (mushroomCount > 0) { mushroomCount--; PlayerInventory.CollectResource(resource, 1); }
                    return;
                case ResourceTypes.conchShell:
                    if (conchShellCount > 0) { conchShellCount--; PlayerInventory.CollectResource(resource, 1); }
                    return;
            }
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
                        if (flowerCount - consumeThisMany < 0)
                        {
                            return false;
                        }
                        break;
                    case ResourceTypes.treeSap:
                        if (treeSapCount - consumeThisMany < 0)
                        {
                            return false;
                        }
                        break;
                    case ResourceTypes.feathers:
                        if (feathersCount - consumeThisMany < 0)
                        {
                            return false;
                        }
                        break;
                    case ResourceTypes.mushroom:
                        if (mushroomCount - consumeThisMany < 0)
                        {
                            return false;
                        }
                        break;
                    case ResourceTypes.conchShell:
                        if (conchShellCount - consumeThisMany < 0)
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
            return new HospitalInventoryData(flowerCount, treeSapCount, feathersCount, mushroomCount, conchShellCount);
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
                //BALANCE
                //how much of each resource each room starts with (there are 3 rooms)
                flowerCount = 10;
                treeSapCount = 10;
                feathersCount = 10;
                mushroomCount = 10;
                conchShellCount = 10;
            }

        }
    }
}