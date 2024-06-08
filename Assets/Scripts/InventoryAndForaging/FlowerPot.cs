using GameData;
using Inventory;
using Movement;
using System.Collections;
using UnityEngine;


public class FlowerPot : HoldData, IInteractable, IProgressBar
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
    [SerializeField] private Transform interactionPromptLocation;
    [SerializeField] ProgressBarPrompt progressBar;
    public Vector3 ReturnPositionForPromptIndicator()
    {
        progressBar.DisplayProgressBar(this);
        return interactionPromptLocation.position;
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

    /*[SerializeField] GameObject progressBarParent;
    [SerializeField] Image progressBarFill;
    [SerializeField] Color progressBarUncompleteColour;
    [SerializeField] Color progressBarCompletedColour;
    bool updateProgressBar;
    void DisplayProgressBar()
    {
        //performed when this object is interactable
        PlayerController.OnInteractionTargetChange += HideProgressBar;

        //progressBarParent.SetActive(true);
        updateProgressBar = true;
        UpdateProgressBar();
    }
    void HideProgressBar()
    {
        PlayerController.OnInteractionTargetChange -= HideProgressBar;

        //progressBarParent.SetActive(false);
        updateProgressBar = false;
    }
    private void OnDisable()
    {
        PlayerController.OnInteractionTargetChange -= HideProgressBar;
    }
    void UpdateProgressBar()
    {
        //show progress towards flower

        if (!hasAFlowerPlanted)
        {
            HideProgressBar();
        }

        float growthTime = Time.time - growthStartTime;
        float fillAmount = growthTime / totalFlowerGrowthTime;
        progressBarFill.fillAmount = fillAmount;

        if (fillAmount < 1f)
        {
            progressBarFill.color = progressBarUncompleteColour;
        }
        else
        {
            progressBarFill.color = progressBarCompletedColour;
        }
    }*/

    public float GetTimerFillAmount()
    {
        if (!hasAFlowerPlanted)
        {
            return 0f;
        }

        float growthTime = Time.time - growthStartTime;
        float fillAmount = growthTime / totalFlowerGrowthTime;
        
        return fillAmount;
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
        int flowersCollected = Random.Range(2, 6);
        PlayerInventory.CollectResource(ResourceTypes.flower, flowersCollected);
    }

    //balance
    float totalFlowerGrowthTime { get { return 180f; } }

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
        else if (growthTime <= totalFlowerGrowthTime * 0.5f)
        {
            //flower has just been planted. its small
            return GrowthState.baby;
        }
        else if (growthTime <= totalFlowerGrowthTime)
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
