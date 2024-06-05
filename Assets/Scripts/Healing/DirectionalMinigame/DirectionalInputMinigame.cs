using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Healing
{
    public class DirectionalInputMinigame : MonoBehaviour
    {
        [SerializeField] JoystickArt joystick;
        [SerializeField] DirectionalInputDisplay[] inputPrompts;

        DirectionalInput desiredInput;
        List<InputDirections> futureDirectionalInputs;

        enum InputDirections
        {
            quarterForward,
            quarterBackward,
            overForward,
            overBackward,
            zigForward,
            zigBackward,
            semiForward,
            semiBackward
        }

        private void Start()
        {
            futureDirectionalInputs = new List<InputDirections>();
            QueueInput();
            QueueInput();
            UpdateDesiredInput();
        }

        void OnDirectional(InputValue input)
        {
            int direction = Vector2ToNumpadNotation(input.Get<Vector2>());

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
            float progressMadePerInput = Random.Range(2f, 4f);
            HealingManager.Instance.MinigameHealPatient(progressMadePerInput);
            futureDirectionalInputs.RemoveAt(0);
            QueueInput();
            UpdateDesiredInput();
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


        void UpdateDesiredInput()
        {
            switch (futureDirectionalInputs[0])
            {
                case (InputDirections.quarterForward):
                    desiredInput = new DirectionalInput(new List<int> { 2, 3, 6 }, new List<int> { 2, 6 });
                    break;
                case (InputDirections.quarterBackward):
                    desiredInput = new DirectionalInput(new List<int> { 2, 1, 4 }, new List<int> { 2, 4 });
                    break;
                case (InputDirections.overForward):
                    desiredInput = new DirectionalInput(new List<int> { 8, 9, 6 }, new List<int> { 8, 6 });
                    break;
                case (InputDirections.overBackward):
                    desiredInput = new DirectionalInput(new List<int> { 8, 7, 4 }, new List<int> { 8, 4 });
                    break;
                case (InputDirections.zigForward):
                    desiredInput = new DirectionalInput(new List<int> { 6, 3, 2, 3 }, new List<int> { 6, 2, 3 });
                    break;
                case (InputDirections.zigBackward):
                    desiredInput = new DirectionalInput(new List<int> { 4, 1, 2, 1 }, new List<int> { 4, 2, 1 });
                    break;
                case (InputDirections.semiForward):
                    desiredInput = new DirectionalInput(new List<int> { 6, 10, 10, 10, 4, 5, 6 }, new List<int> { 6, 10, 4, 6 });
                    break;
                case (InputDirections.semiBackward):
                    desiredInput = new DirectionalInput(new List<int> { 4, 10, 10, 10, 6, 5, 4 }, new List<int> { 4, 10, 6, 4 });
                    break;
            }

            UpdateVisualIndicators();
        }

        void QueueInput()
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
                futureDirectionalInputs.Add(InputDirections.quarterForward);
            }
            else if (3 <= chosenDirectionalInput && chosenDirectionalInput <= 5)
            {
                //quarter circle backwards
                futureDirectionalInputs.Add(InputDirections.quarterBackward);
            }
            else if (6 <= chosenDirectionalInput && chosenDirectionalInput <= 7)
            {
                //over circle forwards
                futureDirectionalInputs.Add(InputDirections.overForward);
            }
            else if (8 <= chosenDirectionalInput && chosenDirectionalInput <= 9)
            {
                //over circle backwards
                futureDirectionalInputs.Add(InputDirections.overBackward);
            }
            else if (10 <= chosenDirectionalInput && chosenDirectionalInput <= 11)
            {
                //zig zag forwards
                futureDirectionalInputs.Add(InputDirections.zigForward);
            }
            else if (12 <= chosenDirectionalInput && chosenDirectionalInput <= 13)
            {
                //zig zag backwards
                futureDirectionalInputs.Add(InputDirections.zigBackward);

            }
            else if (14 == chosenDirectionalInput)
            {
                //semi circle forwards
                futureDirectionalInputs.Add(InputDirections.semiForward);

            }
            else if (15 == chosenDirectionalInput)
            {
                //semi circle backwards
                futureDirectionalInputs.Add(InputDirections.semiBackward);

            }
            else
            {
                //this part of the code shouldnt be accessible
                Debug.LogError("That shouldnt have happened. the random number was: " + chosenDirectionalInput);
                futureDirectionalInputs.Add(InputDirections.quarterForward);
                return;
            }

        }

        void UpdateVisualIndicators()
        {
            for (int i = 0; i < futureDirectionalInputs.Count; i++)
            {
                if (inputPrompts.Length > i)
                {
                    switch (futureDirectionalInputs[i])
                    {
                        case InputDirections.quarterForward:
                            inputPrompts[i].UpdateSprite(0); break;
                        case InputDirections.quarterBackward:
                            inputPrompts[i].UpdateSprite(1); break;
                        case InputDirections.overForward:
                            inputPrompts[i].UpdateSprite(2); break;
                        case InputDirections.overBackward:
                            inputPrompts[i].UpdateSprite(3); break;
                        case InputDirections.zigForward:
                            inputPrompts[i].UpdateSprite(4); break;
                        case InputDirections.zigBackward:
                            inputPrompts[i].UpdateSprite(5); break;
                        case InputDirections.semiForward:
                            inputPrompts[i].UpdateSprite(6); break;
                        case InputDirections.semiBackward:
                            inputPrompts[i].UpdateSprite(7); break;
                    }
                }
                else
                {
                    break;
                }
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
        }
    }
}