using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProgressBar
{
    //Note: you need to give the object the prefab called ProgressBarPrompt, and call display progress bar on ReturnPositionForPromptIndicator()

    //Add the below code:
    /*[SerializeField] ProgressBarPrompt progressBar;
    public Vector3 ReturnPositionForPromptIndicator()
    {
        progressBar.DisplayProgressBar(this);
    }*/

    public float GetTimerFillAmount();
}
