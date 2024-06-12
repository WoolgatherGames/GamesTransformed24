using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuggyMinigameCheckpoint : MonoBehaviour
{

    BuggyMinigameManager manager;
    int myNumber;
    public void SetMinigameManager(BuggyMinigameManager m, int number)
    {
        manager = m;
        myNumber = number;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //oaef theres a bug, where if the player respawns and exits this trigger at the same time, then this will still get triggered. perhaps i could check if the player fell off the buggy within the last couple frames
        //performing on exit so the player has to clear the checkpoint
        if (collision.GetComponent<BoneBuggy_MinigameVer>() != null)
        {
            manager.HitCheckpoint(myNumber);
        }
    }
}
