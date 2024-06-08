using Movement;
using UnityEngine;
using UnityEngine.UI;


public class ProgressBarPrompt : MonoBehaviour
{
    [SerializeField] GameObject progressBarParent;
    [SerializeField] Image progressBarFill;
    [Space(5)]
    [SerializeField] Color progressBarUncompleteColour;
    [SerializeField] Color progressBarCompletedColour;
    bool updateProgressBar;

    void Start()
    {
        HideProgressBar();
    }
    void Update()
    {
        if (updateProgressBar)
        {
            UpdateProgressBar();
        }
    }

    public void DisplayProgressBar(IProgressBar origin)
    {
        data = origin;
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
    private void OnDestroy()
    {
        PlayerController.OnInteractionTargetChange -= HideProgressBar;
    }
    public void HideProgressBar()
    {
        PlayerController.OnInteractionTargetChange -= HideProgressBar;

        if (progressBarParent == null) { return; }

        progressBarParent.SetActive(false);
        updateProgressBar = false;
    }

    IProgressBar data;
    void UpdateProgressBar()
    {
        if (data == null) { return; }

        float fillAmount = data.GetTimerFillAmount();

        progressBarFill.fillAmount = fillAmount;

        if (fillAmount < 1f)
        {
            progressBarFill.color = progressBarUncompleteColour;
        }
        else
        {
            progressBarFill.color = progressBarCompletedColour;
        }
    }
}

/*
class ProgressBarPrompt
{
    /// <summary>
    /// Copy this code into objects you want to have a progress bar displayed when the player goes near them to interact
    /// You also need to DisplayProgressBar to the ReturnPositionForPromptIndicator() function
    /// I should have maybe made this a component to attatch to objects
    /// 
    /// -Logan
    /// </summary>

    //remember to add DisplayProgressBar to whatever version of this function the script uses
    public Vector3 ReturnPositionForPromptIndicator()
    {
        DisplayProgressBar();
        return Vector3.zero;//change
    }

    //Copy all the below code 
    [SerializeField] GameObject progressBarParent;
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

    }
}
*/