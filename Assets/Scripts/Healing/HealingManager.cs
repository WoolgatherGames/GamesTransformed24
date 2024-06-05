using Movement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Healing
{
    public class HealingManager : MonoBehaviour
    {
        private static HealingManager instance;
        public static HealingManager Instance { get { return instance; } }
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            HideHealingUI();
        }

        Patient currentPatient;
        public Patient CurrentPatient { get { return currentPatient; } }

        float preventDoubleClick;

        public void OpenHealingMenu(Patient patient)
        {
            if (preventDoubleClick > 0f) { return; }
            preventDoubleClick = 0.5f;

            patientCanBeDischarged = false;

            PlayerController.Instance.DisableCharacterController();
            //playerInput.Input.Directional.performed += OnDirectionChange;
            //playerInput.Input.Directional.canceled += OnDirectionChange;

            currentPatient = patient;
            healingMinigameActive = true;

            ShowHealingUI();

            //testing 
            StartDirectionalMinigame();
        }

        void CloseHealingMenu()
        {
            if (preventDoubleClick > 0f) { return; }
            preventDoubleClick = 0.5f;

            HideHealingUI();

            foreach (Transform child in transform)
            {
                //destroy minigame
                Destroy(child.gameObject);
            }

            PlayerController.Instance.EnableCharacterController();
            //playerInput.Input.Directional.performed -= OnDirectionChange;
            //playerInput.Input.Directional.canceled -= OnDirectionChange;

            healingMinigameActive = false;

            currentPatient = null;
        }

        bool healingMinigameActive = false;

        void OnInteract()
        {

            if (healingMinigameActive)
            {
                if (patientCanBeDischarged)
                {
                    DischargePatient();
                }
                else
                {
                    CloseHealingMenu();
                }
            }
        }

        public void MinigameHealPatient(float healingPercentage)
        {
            currentPatient?.AddMaxHealingProgress(healingPercentage);
        }


        [SerializeField] GameObject inputMinigame;

        float timeSinceUpdatedPatientHealing;

        private void Update()
        {
            if (preventDoubleClick >= 0f)
            {
                preventDoubleClick -= Time.deltaTime;
            }

            if (currentPatient != null)
            {
                timeSinceUpdatedPatientHealing += Time.deltaTime;
                if (timeSinceUpdatedPatientHealing > 1f)
                {
                    currentPatient.UpdateHealingProgress();
                }
            }
        }

        void DestroyMinigame()
        {

        }

        void StartDirectionalMinigame()
        {
            Instantiate(inputMinigame, transform);
        }

        void DischargePatient()
        {
            currentPatient?.Discharge();
            CloseHealingMenu();
        }

        #region UI

        [SerializeField] GameObject healingUI;
        void ShowHealingUI()
        {
            healingUI.SetActive(true);
            dischargeButton.SetActive(false);
        }
        void HideHealingUI()
        {
            healingUI.SetActive(false);
        }

        [SerializeField] Image currentHealthProgressBar;
        [SerializeField] Image maximumHealthProgressBar;
        [SerializeField] Image maximumHealthAllowed;
        [SerializeField] TMP_Text progressText;

        public void UpdateHealthBars(float currentHealth, float maxHealth)
        {
            progressText.text = Mathf.RoundToInt(currentHealth).ToString() + "%";
            currentHealthProgressBar.fillAmount = currentHealth / 100;
            maximumHealthProgressBar.fillAmount = maxHealth / 100;
            maximumHealthAllowed.fillAmount = (currentHealth / 100) + 0.4f;
        }

        [SerializeField] GameObject dischargeButton;
        [SerializeField] GameObject closeButton;
        bool patientCanBeDischarged;
        public void EnableDischargeButton()
        {
            closeButton.SetActive(false);
            dischargeButton.SetActive(true);
            patientCanBeDischarged = true;
        }

        #endregion
    }

}




/*oaef. move this or use events.
   [SerializeField] JoystickArt joystick;
   void OnDirectionChange(InputAction.CallbackContext context)
   {
       int direction = Vector2ToNumpadNotation(context.ReadValue<Vector2>());

       //send a signal to the art asset
       joystick.UpdateJoystickSprite(direction);

       if (direction != 5 && desiredInput != null)
       {
           //ignore neutral input (5)

           //this returns true if the input has been completed
           bool completeInput = desiredInput.CheckInput(direction);

           if (completeInput)
           {
               //testing
               desiredInput = null;
               CompleteInput();
           }
       }
   }



   void CompleteInput()
   {
       Debug.Log("completed");
       DecideMinigameRequiredInput();
   }

   class DirectionalInput
   {
       public float inputTime;//time of the previous input
       public int requiredIndex;
       public int allowedIndex;
       public List<int> requiredNotations;
       public List<int> allowedNotations;

       public DirectionalInput(List<int> allowedNumbers, List<int> requiredNumbers)
       {
           allowedNotations = allowedNumbers;
           requiredNotations = requiredNumbers;
           inputTime = Time.time;
           ResetIndex();
       }

       public bool CheckInput(int playerInputNotation)
       {
           //returns true IF the player has input the directional input entierly
           bool returnVal = false;

           float allowedTime = 2f;
           if (Time.time + allowedTime < inputTime)
           {
               //the player took too long to input (input time is the time of the previous input)
               ResetIndex();
           }

           inputTime = Time.time;

           int playerRequiredInput = playerInputNotation;
           int playerAllowedInput = playerInputNotation;

           if (requiredNotations[requiredIndex] == 10 && (playerInputNotation == 1 || playerInputNotation == 2 || playerInputNotation == 3))
           {
               //a number 10 denotes that ANY downwards direction is acceptable (for semi-circles)
               playerRequiredInput = 10;
           }
           if (allowedNotations[allowedIndex] == 10 && (playerInputNotation == 1 || playerInputNotation == 2 || playerInputNotation == 3))
           {
               playerAllowedInput = 10;
           }

           if (playerRequiredInput == requiredNotations[requiredIndex])
           {
               returnVal = HitRequiredInput();
           }
           else if (playerAllowedInput == allowedNotations[allowedIndex])
           {
               allowedIndex++;
           }
           else
           {
               ResetIndex();
               if (playerRequiredInput == requiredNotations[0])
               {
                   returnVal = HitRequiredInput();
               }
           }

           return returnVal;
       }

       bool HitRequiredInput()
       {
           //returns true if this is the final required index
           requiredIndex++;
           allowedIndex++;

           if (requiredIndex == requiredNotations.Count)
           {
               //
               ResetIndex();
               return true;
           }
           else { return false; }
       }

       void ResetIndex()
       {

           requiredIndex = 0;
           allowedIndex = 0;
           //requiredIndex = requiredNotations.Count - 1;
           //allowedIndex = requiredNotations.Count - 1;
       }
   }

   DirectionalInput desiredInput;

   void DecideMinigameRequiredInput()
   {
       //decide what input is required of the player (note that neutral inputs (5s) get ignored so DONT use them in any directional inputs

       ///notation codes:
       ///7 8 9
       ///4 5 6
       ///1 2 3
       ///10 = 1, 2 OR 3
       ///--- --- --- --- --- --- ---
       ///*numbers in [] dont need to be input but can be 
       ///**all codes should be considered to have a [5] at the end
       ///***all codes should have a "double" variant if performed twice in a row
       ///quarter circle forward:   2 [3] 6
       ///quarter cirlce backwards: 2 [1] 4
       ///over circle forwards:     8 [9] 6
       ///over circle backwards:    8 [7] 4
       ///zig zag forward:          6 [3] 2 3
       ///zig zag backwards:        4 [1] 2 1
       ///semi circle forwards:     6 [3] 2 [1] 4 [5] 6   OR   6 3 [2] 1 [4] [5] 6
       ///semi circle backwards     4 [1] 2 [3] 6 [5] 4   OR   4 1 [2] 3 [6] [5] 4
       ///
       int chosenDirectionalInput = Random.Range(0, 16);
       if (0 <= chosenDirectionalInput && chosenDirectionalInput <= 2)
       {
           //quarter circle forward
           Debug.Log("quarter circle forward");
           desiredInput = new DirectionalInput(new List<int> { 2, 3, 6 }, new List<int> { 2, 6 });
       }
       else if (3 <= chosenDirectionalInput && chosenDirectionalInput <= 5)
       {
           //quarter circle backwards
           Debug.Log("quarter circle backwards");
           desiredInput = new DirectionalInput(new List<int> { 2, 1, 4 }, new List<int> { 2, 4 });
       }
       else if (6 <= chosenDirectionalInput && chosenDirectionalInput <= 7)
       {
           //over circle forwards
           Debug.Log("over circle forwards");
           desiredInput = new DirectionalInput(new List<int> { 8, 9, 6 }, new List<int> { 8, 6 });
       }
       else if (8 <= chosenDirectionalInput && chosenDirectionalInput <= 9)
       {
           //over circle backwards
           Debug.Log("over circle backwards");
           desiredInput = new DirectionalInput(new List<int> { 8, 7, 4 }, new List<int> { 8, 4 });
       }
       else if (10 <= chosenDirectionalInput && chosenDirectionalInput <= 11)
       {
           //zig zag forwards
           Debug.Log("zig zag forwards");
           desiredInput = new DirectionalInput(new List<int> { 6, 3, 2, 3 }, new List<int> { 6, 2, 3 });
       }
       else if (12 <= chosenDirectionalInput && chosenDirectionalInput <= 13)
       {
           //zig zag backwards
           Debug.Log("zig zag backwards");
           desiredInput = new DirectionalInput(new List<int> { 4, 1, 2, 1 }, new List<int> { 4, 2, 1 });
       }
       else if (14 == chosenDirectionalInput)
       {
           //semi circle forwards
           Debug.Log("semi circle forwards");
           desiredInput = new DirectionalInput(new List<int> { 6, 10, 10, 10, 4, 5, 6 }, new List<int> { 6, 10, 4, 6 });
       }
       else if (15 == chosenDirectionalInput)
       {
           //semi circle backwards
           Debug.Log("semi circle backwards");
           desiredInput = new DirectionalInput(new List<int> { 4, 10, 10, 10, 6, 5, 4 }, new List<int> { 4, 10, 6, 4 });
       }
       else
       {
           //this part of the code shouldnt be accessible
           Debug.LogError("That shouldnt have happened. the random number was: " + chosenDirectionalInput);
           DecideMinigameRequiredInput(); return;
       }

   }


   int Vector2ToNumpadNotation(Vector2 direction)
   {
       ///7 8 9
       ///4 5 6
       ///1 2 3
       //if i ever plan to do this kind of system again, just use the vectors lol, the notations make it hard to say "any downwards direction is fine" (im gonna use codenumbers instead to denote "any downwards direction"

       direction = direction.normalized;
       float reqDist = 0.5f;//required distance for the input to count (note: im normalizing the vector just to make sure it comes in below 1 (im worried about the posibility of some gamepads being really weird???)

       if (direction.x < -reqDist)
       {
           //the direction is pointing left
           if (direction.y > reqDist)
           {
               //left + up
               return 7;
           }
           else if (direction.y < -reqDist)
           {
               //left down
               return 1;
           }
           else
           {
               return 4;
           }
       }
       else if (direction.x > reqDist)
       {
           if (direction.y > reqDist)
           {
               //right up
               return 9;
           }
           else if (direction.y < -reqDist)
           {
               //right down
               return 3;
           }
           else
           {
               //right
               return 6;
           }
       }
       else if (direction.y > reqDist)
       {
           //just up
           return 8;
       }
       else if (direction.y < -reqDist)
       {
           //just down
           return 2;
       }
       else
       {
           //neutral
           return 5;
       }
   }*/
//---------------
/*
class DirectionalInputCheck
{
    public int inputCount;//how many times this has been input recently (note: im not using this anymore, but it seems simpler to leave it in)
    public int requiredIndex;//how many minimum directions have been input correctly
    public int allowedIndex;//how many allowed directions have been input correctly
    public List<int> requiredNumbers;//what notation does the player need to input
    public List<int> allowedNumbers;//what notation can the player input (0 means nothing is allowed)

    public DirectionalInputCheck(List<int> _allowedNumbers, List<int> _requiredNumbers)
    {
        inputCount = 0;
        requiredIndex = 0;
        allowedIndex = 0;
        requiredNumbers = _requiredNumbers;
        allowedNumbers = _allowedNumbers;
    }
}

DirectionalInputCheck mostRecentDirectionalInput;

void CalculateDirectionalInputMovement()
{
    //oaef this code doesnt get the most recent input, but rather one input from the last few seconds based on priority order (semi circles first) 

    ///figure out if the player has performed a quarter circle forward, backwards ect. 
    if (recentInputDirections == null)
        return;
    mostRecentDirectionalInput = null;
    ///notation codes:
    ///*numbers in [] dont need to be input but can be 
    ///**all codes should be considered to have a [5] at the end
    ///***all codes should have a "double" variant if performed twice in a row
    ///quarter circle forward:   2 [3] 6
    ///quarter cirlce backwards: 2 [1] 4
    ///over circle forwards:     8 [9] 6
    ///over circle backwards:    8 [7] 4
    ///zig zag forward:          6 [3] 2 3
    ///zig zag backwards:        4 [1] 2 1
    ///semi circle forwards:     6 [3] 2 [1] 4 [5] 6   OR   6 3 [2] 1 [4] [5] 6
    ///semi circle backwards     4 [1] 2 [3] 6 [5] 4   OR   4 1 [2] 3 [6] [5] 4

    //track how many times each code is found
    //DirectionalInputCheck replaceMe = new DirectionalInputCheck(new List<int> {all numbers}, new List<int> {required numbers});
    DirectionalInputCheck quarterCircleForwards = new DirectionalInputCheck(new List<int> { 2, 3, 6 }, new List<int> { 2, 6 });
    DirectionalInputCheck quarterCircleBackwards = new DirectionalInputCheck(new List<int> { 2, 1, 4 }, new List<int> { 2, 4 });
    DirectionalInputCheck overCircleForwards = new DirectionalInputCheck(new List<int> { 8, 9, 6 }, new List<int> { 8, 6 });
    DirectionalInputCheck overCircleBackwards = new DirectionalInputCheck(new List<int> { 8, 7, 4 }, new List<int> { 8, 4 });
    DirectionalInputCheck zigZagForwards = new DirectionalInputCheck(new List<int> { 6, 3, 2, 3 }, new List<int> { 6, 2, 3 });
    DirectionalInputCheck zigZagBackwards = new DirectionalInputCheck(new List<int> { 4, 1, 2, 1 }, new List<int> { 4, 2, 1 });
    DirectionalInputCheck semiCircleForwardsVerOne = new DirectionalInputCheck(new List<int> { 6, 3, 2, 1, 4, 5, 6 }, new List<int> { 6, 2, 4, 6 });
    DirectionalInputCheck semiCircleForwardsVerTwo = new DirectionalInputCheck(new List<int> { 6, 3, 2, 1, 4, 5, 6 }, new List<int> { 6, 3, 1, 6 });
    DirectionalInputCheck semiCircleBackwardsVerOne = new DirectionalInputCheck(new List<int> { 4, 1, 2, 3, 6, 5, 4 }, new List<int> { 4, 2, 6, 4 });
    DirectionalInputCheck semiCircleBackwardsVerTwo = new DirectionalInputCheck(new List<int> { 4, 1, 2, 3, 6, 5, 4 }, new List<int> { 4, 1, 3, 4 });

    for (int i = 0; i < recentInputDirections.Count; i++)
    {
        int checkInput = recentInputDirections[i];
        if (checkInput == 5) { continue; }//ignore neutral input (5s) as i dont want to cancel if the player returns to neutral after each input direction

        CheckInput(quarterCircleForwards, checkInput);
        CheckInput(quarterCircleBackwards, checkInput);
        CheckInput(overCircleForwards, checkInput);
        CheckInput(overCircleBackwards, checkInput);
        CheckInput(zigZagForwards, checkInput);
        CheckInput(zigZagBackwards, checkInput);
        CheckInput(semiCircleForwardsVerOne, checkInput);
        CheckInput(semiCircleForwardsVerTwo, checkInput);
        CheckInput(semiCircleBackwardsVerOne, checkInput);
        CheckInput(semiCircleBackwardsVerTwo, checkInput);
    }


    //now check what input the player has done
    if (mostRecentDirectionalInput == null)
    {
        Debug.Log("nothing");
    }
    else if (mostRecentDirectionalInput == semiCircleForwardsVerOne || mostRecentDirectionalInput == semiCircleForwardsVerTwo)
    {
        Debug.Log("Semi circle forward");
    }
    else if (mostRecentDirectionalInput == semiCircleBackwardsVerOne || mostRecentDirectionalInput == semiCircleBackwardsVerTwo)
    {
        Debug.Log("semi circle backwards");
    }
    else if (mostRecentDirectionalInput == zigZagForwards)
    {
        Debug.Log("zig zag forward");
    }
    else if (mostRecentDirectionalInput == zigZagBackwards)
    {
        Debug.Log("zig zag backwards");
    }
    else if (mostRecentDirectionalInput == quarterCircleForwards)
    {
        Debug.Log("quarter circle forward");
    }
    else if (mostRecentDirectionalInput == quarterCircleBackwards)
    {
        Debug.Log("quarter circle backwards");
    }
    else if (mostRecentDirectionalInput == overCircleForwards)
    {
        Debug.Log("over circle forwards");
    }
    else if (mostRecentDirectionalInput == overCircleBackwards)
    {
        Debug.Log("over circle backwards");
    }
    else
    {
        Debug.Log("nothing");
    }
}

void CheckInput(DirectionalInputCheck inputMotion, int playerInput)
{
    //returns true if the input motion has been completed, else it returns false
    if (playerInput == inputMotion.requiredNumbers[inputMotion.requiredIndex])
    {
        inputMotion.requiredIndex++;
        inputMotion.allowedIndex++;

        if (inputMotion.requiredIndex >= inputMotion.requiredNumbers.Count)
        {
            //this has been fully input
            mostRecentDirectionalInput = inputMotion;

            inputMotion.inputCount++;
            ResetDirectionalInputCheckIndexes(inputMotion);
        }
    }
    else if (playerInput == inputMotion.allowedNumbers[inputMotion.allowedIndex])
    {
        inputMotion.allowedIndex++;
    }
    else
    {
        //the player failed to put in an allowed number
        ResetDirectionalInputCheckIndexes(inputMotion);
    }
}

void ResetDirectionalInputCheckIndexes(DirectionalInputCheck indexHolder)
{
    indexHolder.requiredIndex = 0;
    indexHolder.allowedIndex = 0;
}*/