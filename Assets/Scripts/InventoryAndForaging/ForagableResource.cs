using Inventory;
using Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Foraging
{
    public class ForagableResource : MonoBehaviour
    {
        //resource that can be picked up from the ground

        //this should be a prefab and on loading the forest scene, these should be scattered around

        private enum WhichAmI
        {
            flower,
            feather,
            mushroom
        }

        [SerializeField] WhichAmI resourceType;

        /*public void Interact()
        {
            Pickup();
        }*/

        /*void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController>() != null)
            {
                Pickup();
            }
        }*/

        public void Pickup()
        {
            switch (resourceType)
            {
                case WhichAmI.flower:
                    PlayerInventory.CollectResource(ResourceTypes.flower, 1); break;
                case WhichAmI.feather:
                    PlayerInventory.CollectResource(ResourceTypes.feathers, 1); break;
                case WhichAmI.mushroom:
                    PlayerInventory.CollectResource(ResourceTypes.mushroom, 1); break;
            }


            //note i need to also remove this object from the scatter resources list
            Destroy(this.gameObject);
        }
    }
}