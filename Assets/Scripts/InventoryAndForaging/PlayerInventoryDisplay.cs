using Inventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventoryDisplay : MonoBehaviour
{
    //i want to subscribe to player inventory and update display whenever the inventory gains items

    [SerializeField] TMP_Text flowerCounter;
    [SerializeField] TMP_Text mushroomCounter;
    [SerializeField] TMP_Text featherCounter;
    [SerializeField] TMP_Text treeSapCounter;
    [SerializeField] TMP_Text conchShellCounter;

    private void OnEnable()
    {
        //subscribe
        PlayerInventory.OnResourcesChanged += UpdateResources;
        UpdateResources();//catch any updates that might have occured whilst this was disabled (such as when gathering sap)
    }

    private void OnDisable()
    {
        //unsubscribe
        PlayerInventory.OnResourcesChanged -= UpdateResources;
    }

    void UpdateResources()
    {
        flowerCounter.text = PlayerInventory.FlowerCount.ToString();
        mushroomCounter.text = PlayerInventory.MushroomCount.ToString();
        featherCounter.text = PlayerInventory.FeathersCount.ToString();
        treeSapCounter.text = PlayerInventory.TreeSapCount.ToString();
        conchShellCounter.text = PlayerInventory.ConchShellCount.ToString();
    }
}
