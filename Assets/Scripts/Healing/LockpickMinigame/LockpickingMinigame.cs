using Healing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LockpickingMinigame : MonoBehaviour
{

    [SerializeField] float[] numberYPositions;
    public float[] NumberYPositions { get { return numberYPositions; } }

    int numberInputValue;
    [SerializeField] Transform playerInput;
    [SerializeField] Transform lockpickKey;//bounce up and down
    bool lockpickKeyCharged;//set to true whenever the key hits a wall, and false when it bumps the player
    bool lockpickKeyMovingUpwards;

    int desiredNumber;

    [SerializeField] Vector2 yPosToAndFrom;
    void SetYPositions()
    {
        float low = yPosToAndFrom.x;
        float high = yPosToAndFrom.y;

        float gap = high - low;
        float distance = gap / 9;

        numberYPositions = new float[] { low, low + (distance * 1), low + (distance * 2), low + (distance * 3), low + (distance * 4), low + (distance * 5), low + (distance * 6), low + (distance * 7), low + (distance * 8), low + (distance * 9) };

        for (int i = 0; i < numberYPositions.Length; i++)
        {
            previewSprites[i].transform.localPosition = new Vector3(0f, numberYPositions[i], 0f);
            previewSprites[i].gameObject.SetActive(false);
            previewSprites[i].color = gradient.Evaluate(i / 10f);
        }
        
    }

    void SetNewDesiredNumber()
    {
        desiredNumber = Random.Range(0, 10);
    }

    float pauseMinigameTimer;

    private void Update()
    {
        if (pauseMinigameTimer > 0f)
        {
            pauseMinigameTimer -= Time.deltaTime;

            if (pauseMinigameTimer < 0f)
            {
                for (int i = 0; i < previewSprites.Length; i++)
                {
                    previewSprites[i].gameObject.SetActive(false);
                }
            }

            return;
        }

        UpdateLockpickKey();
        //CheckKeyAndPlayerPos();

        /*if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            numberInputValue++;
            if (numberInputValue >= 10) { numberInputValue = 0; }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            numberInputValue--;
            if (numberInputValue <= -1) { numberInputValue = 9; }
        }*/
        playerInput.position = new Vector3(playerInput.position.x, NumberYPositions[numberInputValue], 0f);
    }


    private void Start()
    {
        SetYPositions();
        SetNewDesiredNumber();
        lockpickPreviousPosIndex = 0;
        lockpickNextPosIndex = 1;
        numberInputValue = 5;
        lockpickKeyMovingUpwards = true;
        lockpickKeyCharged = true;
    }

    void OnDirectional(InputValue input)
    {
        Vector2 direction = input.Get<Vector2>();

        if (direction.y >= 0.8f)
        {
            numberInputValue++;
            if (numberInputValue >= 10) { numberInputValue = 0; }
        }
        if (direction.y <= -0.8f)
        {
            numberInputValue--;
            if (numberInputValue <= -1) { numberInputValue = 9; }
        }
    }

    float lerpVal;
    int lockpickPreviousPosIndex;
    int lockpickNextPosIndex;
    bool lockpickAtPosition;
    void UpdateLockpickKey()
    {
        lerpVal += 5f * Time.deltaTime;

        float yPos = Mathf.Lerp(numberYPositions[lockpickPreviousPosIndex], numberYPositions[lockpickNextPosIndex], Mathf.Clamp01(lerpVal));
        lockpickKey.position = new Vector3(lockpickKey.position.x, yPos, 0f);

        if (lerpVal >= 1)
        {
            lerpVal = 0f;
            if (lockpickKeyCharged && numberInputValue == lockpickNextPosIndex)
            {
                lockpickKeyCharged = false;
                AttemptLockpick();
            }
            lockpickPreviousPosIndex = lockpickNextPosIndex;
            if (lockpickKeyMovingUpwards)
            {
                lockpickNextPosIndex++;
                if (lockpickNextPosIndex > 9)
                {
                    lockpickKeyCharged = true;
                    lockpickKeyMovingUpwards = false;
                    lockpickNextPosIndex = 8;
                }
            }
            else
            {
                lockpickNextPosIndex--;
                if (lockpickNextPosIndex < 0)
                {
                    lockpickKeyCharged = true;
                    lockpickKeyMovingUpwards = true;
                    lockpickNextPosIndex = 1;
                }
            }
        }
    }

    [SerializeField] SpriteRenderer[] previewSprites;
    [SerializeField] Gradient gradient;
    void AttemptLockpick()
    {
        //display how close the player got
        int difference = Mathf.Abs(desiredNumber - numberInputValue);
        for (int i = 0; i < previewSprites.Length; i++)
        {
            if (i < previewSprites.Length - difference)
            {
                previewSprites[i].gameObject.SetActive(true);
                //previewSprites[i].color = gradient.Evaluate(i / 10);
                //previewSprites[i].transform.position = new Vector3(0f, numberYPositions[i], 0f);
            }
            else
            {
                previewSprites[i].gameObject.SetActive(false);
            }
        }

        if (numberInputValue == desiredNumber)
        {
            //win
            float healing = Random.Range(8f, 12f);
            HealingManager.Instance.MinigameHealPatient(healing);
            SetNewDesiredNumber();
            pauseMinigameTimer = 0.5f;
        }


    }




    private void OnDrawGizmos()
    {
        //preview the size of the minigame camera 
        Gizmos.DrawWireCube(new Vector3(0f, 0f, 0f), new Vector3(19f, 10f, 0f));
    }
}


/*void UpdateLockpickKey()
{
    float dir = lockpickKeyMovingUpwards? 1f : -1f;
    float speed = 2f;

    lockpickKey.transform.position += Vector3.up * dir * speed * Time.deltaTime;

    if (lockpickKey.transform.position.y > numberYPositions[numberYPositions.Length - 1])
    {
        lockpickKeyMovingUpwards = false;
        lockpickKeyCharged = true;
    }
    else if (lockpickKey.transform.position.y < numberYPositions[0])
    {
        lockpickKeyMovingUpwards = true;
        lockpickKeyCharged = true;
    }
}
    void CheckKeyAndPlayerPos()
{
    if (lockpickKeyCharged && playerInput.position.y - lockpickKey.position.y < 0.05f)
    {
        lockpickKeyCharged = false;
        Debug.Log("CHECK");
    }
}

 */