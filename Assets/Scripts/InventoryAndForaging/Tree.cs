using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameData;
using Movement;
using Inventory;

public class Tree : HoldData, IInteractable, IProgressBar
{
    //balance
    float DurationBetweenSapCollection { get { return 60f; } }

    public void Interact()
    {
        if (timeTillReady <= 0f && timeTillMined < 0f)
        {
            progressBar.HideProgressBar();
            PlayerController.Instance.DisableCharacterController();
            timeTillMined = 1.5f;//how long it takes to mine the tree
        }
        else
        {
            Debug.Log("This tree has no sap right now, come back later");
        }

    }

    [SerializeField] private Transform interactionPromptLocation;
    [SerializeField] ProgressBarPrompt progressBar;
    public Vector3 ReturnPositionForPromptIndicator()
    {
        progressBar.DisplayProgressBar(this);
        return interactionPromptLocation.position;
    }

    float timeTillReady;

    float timeTillMined;//whilst above 0, means that this tree is being mined

    void Start()
    {
        timeTillMined = -1f;
    }

    void Update()
    {
        if (timeTillReady > 0f)
        {
            timeTillReady -= Time.deltaTime;
        }

        if (timeTillMined > 0f)
        {
            timeTillMined -= Time.deltaTime;
            if (timeTillMined <= 0f)
            {
                //collect resource from this free
                CollectResources();
            }
        }
    }



    void CollectResources()
    {
        timeTillMined = -1f;//ensure this is reset

        //BALANCE
        timeTillReady = DurationBetweenSapCollection;
        int sapCollected = Random.Range(4, 8);//remember that the 2nd number is exclusive

        PlayerInventory.CollectResource(ResourceTypes.treeSap, sapCollected);

        PlayerController.Instance.EnableCharacterController();
    }



    protected override void LoadData(DataChunk recievedData)
    {
        TimeStamp data = recievedData as TimeStamp;
        if (data != null)//if data is null, this is probably the first time its ever been logged ...OR a horrible error has occured and two objects with the same ID exist
        {
            timeTillReady = data.Time;
        }
    }

    protected override DataChunk SaveData()
    {
        return new TimeStamp(timeTillReady);
    }

    public float GetTimerFillAmount()
    {
        return 1f - (timeTillReady / DurationBetweenSapCollection);
    }

    /*[SerializeField] GameObject progressBarParent;
    [SerializeField] Image progressBarFill;
    bool updateProgressBar;
    void DisplayProgressBar()
    {
        //performed when this object is interactable
        PlayerController.OnInteractionTargetChange += HideProgressBar;

        progressBarParent.SetActive(true);
        updateProgressBar = true;
        UpdateProgressBar();
    }
    void OnDisable()
    {
        PlayerController.OnInteractionTargetChange -= HideProgressBar;
    }
    void HideProgressBar()
    {
        PlayerController.OnInteractionTargetChange -= HideProgressBar;

        progressBarParent.SetActive(false);
        updateProgressBar = false;
    }

    void UpdateProgressBar()
    {

    }*/
}
