using Healing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuggyMinigameManager : MonoBehaviour
{
    //each track should have the first checkpoint be at -8, -2 pointing upwards

    [System.Serializable]
    struct Track
    {
        public GameObject track;
        public BuggyMinigameCheckpoint[] checkpoints;
    }
    [SerializeField] Track[] tracks;

    BuggyMinigameCheckpoint[] currentCheckpoints;
    int nextCheckpoint;

    private void Start()
    {
        ChooseTrack();
    }

    void ChooseTrack()
    {
        int choice = Random.Range(0, tracks.Length);
        foreach (Track raceTrack in tracks)
        {
            raceTrack.track.SetActive(false);
        }

        //Testing
        //choice = 2;


        tracks[choice].track.SetActive(true);
        currentCheckpoints = tracks[choice].checkpoints;

        for (int i = 0; i < currentCheckpoints.Length; i++)
        {
            currentCheckpoints[i].SetMinigameManager(this, i);
        }
        nextCheckpoint = 1;
    }

    public void HitCheckpoint(int checkpointNumber)
    {
        /*if (checkpointNumber == 0)
        {
            ChooseTrack();
        }*/

        if (checkpointNumber == nextCheckpoint)
        {
            //oaef score on the healing manager
            if (HealingManager.Instance != null)
            {
                HealingManager.Instance.MinigameHealPatient(2.5f);
            }

            nextCheckpoint++;
            if (nextCheckpoint >= currentCheckpoints.Length)
            {
                nextCheckpoint = 0;
            }
        }

        Debug.Log(nextCheckpoint);
    }

    public Transform ReturnLastCheckpointTransform()
    {
        if (nextCheckpoint == 0)
        {
            return currentCheckpoints[currentCheckpoints.Length - 1].transform;
        }
        else
        {
            return currentCheckpoints[nextCheckpoint - 1].transform;
        }
    }

    private void OnDrawGizmos()
    {
        //preview the size of the minigame camera 
        Gizmos.DrawWireCube(new Vector3(0f, 0f, 0f), new Vector3(19f, 10f, 0f));
    }
}
