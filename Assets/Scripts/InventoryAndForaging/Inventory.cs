using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory
{

    public class Inventory : MonoBehaviour
    {
        class InventorySlot
        {
            ResourceTypes containedResource;
            public ResourceTypes ContainedResource { get { return containedResource; } }
            int resourceCount;
            public int ResourceCount { get { return resourceCount; } }

            public InventorySlot(ResourceTypes containedResource, int resourceCount)
            {
                this.containedResource = containedResource;
                this.resourceCount = resourceCount;
            }

            public void ChangeContainedResourceType(ResourceTypes newResourceType)
            {
                RemoveAllResources();
                containedResource = newResourceType;
            }

            public bool AddResource()
            {
                //returns false if you can't use this stack to store resources

                int maximumAllowedResourceInAStack = 5;
                if (resourceCount == maximumAllowedResourceInAStack)
                {
                    return false;
                }
                else
                {
                    resourceCount++;
                    return true;
                }


            }
            public void RemoveResource()
            {
                resourceCount--;
            }
            public void RemoveAllResources()
            {
                resourceCount = 0;
            }
        }

        List<InventorySlot> inventorySpace;

        public void CollectResource(ResourceTypes typeOfResource)
        {
            for (int i = 0;  i < inventorySpace.Count; i++)
            {
                if (inventorySpace[i].ContainedResource == typeOfResource)
                {
                    if (inventorySpace[i].AddResource())
                    {
                        return;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (inventorySpace[i].ContainedResource == ResourceTypes.none)
                {
                    inventorySpace[i].ChangeContainedResourceType(typeOfResource);
                    inventorySpace[i].AddResource();
                }
            }
        }

        void DrawResources()
        {
            //draws resources to the UI
        }

        /*int flowerCount;

        int treeSapCount;

        int feathersCount;

        int mushroomCount;

        int conchShellCount;*/
    }

    public enum ResourceTypes
    {
        none,
        flower,
        treeSap,
        feathers,
        mushroom,
        conchShell
    }

}