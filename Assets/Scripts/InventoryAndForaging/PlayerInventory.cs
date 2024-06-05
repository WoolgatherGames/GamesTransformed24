using System.Collections;
using System.Collections.Generic;
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
        }

        public static void EmptyResources()
        {
            flowerCount = 0;
            treeSapCount = 0;
            feathersCount = 0;
            mushroomCount = 0;
            conchShellCount = 0;
        }
    }

}
