using GameData;
using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPot : HoldData, IInteractable
{
    float growthStartTime;
    bool hasAFlowerPlanted;

    enum GrowthState
    {
        emptyPot,
        baby,
        growing,
        readyToHarvest
    }

    float checkTimer;
    void Start()
    {
        UpdateSprite();
    }

    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer > 1f)
        {
            checkTimer = 0f;
            UpdateSprite();
        }
    }


    [SerializeField] SpriteRenderer flowerRenderer;
    [SerializeField] Sprite baby;
    [SerializeField] Sprite growing;
    [SerializeField] Sprite ready;
    void UpdateSprite()
    {
        GrowthState growthState = DetermineGrowthState();

        switch (growthState)
        {
            case (GrowthState.emptyPot):
                flowerRenderer.sprite = null; break;
            case (GrowthState.baby):
                flowerRenderer.sprite = baby; break;
            case (GrowthState.growing):
                flowerRenderer.sprite = growing; break;
            case (GrowthState.readyToHarvest):
                flowerRenderer.sprite = ready; break;
        }
    }

    public void Interact()
    {
        GrowthState growthState = DetermineGrowthState();

        if (growthState == GrowthState.emptyPot)
        {
            //plant flower
            Plant();
        }
        else if (growthState == GrowthState.readyToHarvest)
        {
            //harvest flower
            Harvest();
        }

        UpdateSprite();
    }

    void Plant()
    {
        growthStartTime = Time.time;
        hasAFlowerPlanted = true;
    }

    void Harvest()
    {
        hasAFlowerPlanted = false;

        //BALANCE
        int flowersCollected = Random.Range(1, 4);
        PlayerInventory.CollectResource(ResourceTypes.flower, flowersCollected);
    }


    GrowthState DetermineGrowthState()
    {
        if (!hasAFlowerPlanted)
        {
            return GrowthState.emptyPot;
        }

        float growthTime = Time.time - growthStartTime;

        if (growthTime <= -0.5f)
        {
            //flower hasnt been planted
            return GrowthState.emptyPot;
        }
        else if (growthTime <= 30f)
        {
            //flower has just been planted. its small
            return GrowthState.baby;
        }
        else if (growthTime <= 180f)
        {
            //flower is still growing. its bigger
            return GrowthState.growing;
        }
        else
        {
            //flower has finished growing
            return GrowthState.readyToHarvest;
        }
    }

    protected override void LoadData(DataChunk recievedData)
    {
        FlowerPotData data = recievedData as FlowerPotData;
        if (data != null)
        {
            growthStartTime = data.GrowthStart;
            hasAFlowerPlanted = data.IsPlanted;
        }
        else
        {
            growthStartTime = 0f;
            hasAFlowerPlanted = false;
        }
    }

    protected override DataChunk SaveData()
    {
        return new FlowerPotData(growthStartTime, hasAFlowerPlanted);
    }


}
