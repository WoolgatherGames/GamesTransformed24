using System;
using System.Collections;
using UnityEngine;


namespace Inventory {
    public static class PlayerInventory
    {
        static int flowerCount;
        public static int FlowerCount { get { return flowerCount; } }

        static int treeSapCount;
        public static int TreeSapCount { get { return treeSapCount; } }
        
        static int feathersCount;
        public static int FeathersCount { get { return feathersCount; } }

        static int mushroomCount;
        public static int MushroomCount { get { return mushroomCount; } }

        static int conchShellCount;
        public static int ConchShellCount { get { return conchShellCount; } }


        public delegate void ResourcesChanged();
        public static event ResourcesChanged OnResourcesChanged;

        public static void CollectResource(ResourceTypes resource, int numberCollected)
        {
            if (numberCollected < 0) { numberCollected = 0; }

            switch (resource)
            {
                case ResourceTypes.flower:
                    flowerCount += numberCollected; break;
                case ResourceTypes.treeSap:
                    treeSapCount += numberCollected; break;
                case ResourceTypes.feathers:
                    feathersCount += numberCollected; break;
                case ResourceTypes.mushroom:
                    mushroomCount += numberCollected; break;
                case ResourceTypes.conchShell:
                    conchShellCount += numberCollected; break;
            }

            OnResourcesChanged?.Invoke();
        }

        public static bool RemoveResource(ResourceTypes resource)
        {
            //returns false if an object couldnt be removed, true if one was

            switch (resource)
            {
                case ResourceTypes.flower:
                    if (flowerCount == 0) { return false; }
                    else { flowerCount--; OnResourcesChanged?.Invoke(); return true; }
                case ResourceTypes.treeSap:
                    if (treeSapCount == 0) { return false; }
                    else { treeSapCount--; OnResourcesChanged?.Invoke(); return true; }
                case ResourceTypes.feathers:
                    if (feathersCount == 0) { return false; }
                    else { feathersCount--; OnResourcesChanged?.Invoke(); return true; }
                case ResourceTypes.mushroom:
                    if (mushroomCount == 0) { return false; }
                    else { mushroomCount--; OnResourcesChanged?.Invoke(); return true; }
                case ResourceTypes.conchShell:
                    if (conchShellCount == 0) { return false; }
                    else { conchShellCount--; OnResourcesChanged?.Invoke(); return true; }
            }

            Debug.LogError("This shouldnt have happened");
            return false;
        }

        public static void EmptyResources()
        {
            flowerCount = 0;
            treeSapCount = 0;
            feathersCount = 0;
            mushroomCount = 0;
            conchShellCount = 0;

            OnResourcesChanged?.Invoke();
        }
    }

}
