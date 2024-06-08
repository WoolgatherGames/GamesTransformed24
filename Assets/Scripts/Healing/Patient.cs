
using GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Inventory;
using Movement;

namespace Healing
{
    public class Patient : HoldData
    {
        //balance values
        float outOfResourcesCurrentHealingReductionRate { get { return 0.5f; } }
        float outOfResourcesMaximumHealingReductionRate { get { return 1.5f; } }
        

        public enum PatientProblems
        {
            BrokenBone,
            Hemorrhage,
            OpenWound,
            Infection,
            Concussion,
            HealthRestored//true once the patients problem has been restored and prevents their data from being saved to the system
        }

        public void Interact()
        {
            UpdateHealingProgress();
            HealingManager.Instance.OpenHealingMenu(this);
        }
        /*[SerializeField] private Transform interactionPromptLocation;
        public Vector3 ReturnPositionForPromptIndicator()
        {
            return interactionPromptLocation.position;
        }*/


        float maximumHealingProgress;   //how much healing progress can currently be gained (capped at a certain level above current healing progress)
        float currentHealingProgress;   //how much healing progress has been gained
        float healingRate;              //how fast this character heals (percentage per second of game time)
        float lastHealTimeStamp;        //the time that current healing progress was last updated (time.time)

        PatientProblems myProblem;
        public PatientProblems MyProblem { get { return myProblem; } }

        void AdmitToWard()
        {
            //becomes a patient the player can heal

            //healing rate = randomly decide how much % progres should be made per 60 seconds
            healingRate = Random.Range(8f, 11f) * 0.0166f;//0.0166 is equivilent to 1/60 because healing rate is based on PER minute (60 seconds)

            maximumHealingProgress = 0;
            currentHealingProgress = 0;
            lastHealTimeStamp = Time.time;

            int problemNumber = Random.Range(0, 5);
            switch (problemNumber)
            {
                case 0:
                    myProblem = PatientProblems.BrokenBone; break;
                case 1:
                    myProblem = PatientProblems.Hemorrhage; break;
                case 2:
                    myProblem = PatientProblems.OpenWound; break;
                case 3:
                    myProblem = PatientProblems.Infection; break;
                case 4:
                    myProblem = PatientProblems.Concussion; break;
            }
        }

        public delegate void Discharged();
        public event Discharged OnDischarged;

        public void Discharge()
        {
            //discharge this patient. Delete their data from the healing manager
            Debug.Log("Ive been discharged");
            myProblem = PatientProblems.HealthRestored;//stops this data from being saved
            SaveLoadPersistentData.DestroyData(GetUniqueID());

            //Ideally, destroy the patient and spawn a new identical object (that isnt a patient) so it can walk off 
            OnDischarged?.Invoke();

            Destroy(this.gameObject);
        }

        public void AddMaxHealingProgress(float progressGained)
        {
            //ONLY call this whilst this is the active patient

            //add healing progress but cap it to 40 above the current progress
            maximumHealingProgress = Mathf.Clamp(maximumHealingProgress + progressGained, currentHealingProgress, currentHealingProgress + 40f);//if u change this 40f, you also need to change the 0.4 in the healing managers update health bar function
            if (maximumHealingProgress > 100f)
            {
                maximumHealingProgress = 100.1f;
            }

            HealingManager.Instance.UpdateHealthBars(currentHealingProgress, maximumHealingProgress);
        }

        public void UpdateHealingProgress()
        {
            float timeDifference = Time.time - lastHealTimeStamp;
            lastHealTimeStamp = Time.time;
            float progressMade = timeDifference * healingRate;
            currentHealingProgress = Mathf.Clamp(currentHealingProgress + progressMade, currentHealingProgress, maximumHealingProgress);


            if (HealingManager.Instance.CurrentPatient == this)
            {
                HealingManager.Instance.UpdateHealthBars(currentHealingProgress, maximumHealingProgress);
            }

            if (currentHealingProgress >= 100)
            {
                HealingManager.Instance.EnableDischargeButton();
            }
        }


        void Update()
        {
            ConsumeResources();
        }

        HospitalRoom room;

        //float timeStampOfNextConsumptionCheck;
        float timeStampOfLastConsumptionCheck;
        float timeToNextConsumptionCheck;
        void ConsumeResources()
        {
            float timePerConsumption = 30f;

            if (timeStampOfLastConsumptionCheck + timeToNextConsumptionCheck < Time.time)
            {
                /*if (myProblem == PatientProblems.BrokenBone)
                {
                    //broken bone, consume feathers and flowers
                    ResourceTypes[] resources = { ResourceTypes.feathers, ResourceTypes.flower };
                    int[] consumedAmount = { 2, 1 };
                    if (HospitalInventory.Instance.PatientConsumeResource(resources, consumedAmount) == false)
                    {
                        //there wasnt enough resources. Stop being able to heal
                        ReduceHealingProgressFromNoResources();
                        return;
                    }
                }
                else if (myProblem == PatientProblems.Hemorrhage)
                {
                    //hemmorage. flower + tree sap
                    ResourceTypes[] resources = { ResourceTypes.flower, ResourceTypes.treeSap};
                    int[] consumedAmount = { 2, 1 };
                    if (HospitalInventory.Instance.PatientConsumeResource(resources, consumedAmount) == false)
                    {
                        //there wasnt enough resources. Stop being able to heal
                        ReduceHealingProgressFromNoResources();
                        return;
                    }
                }
                else if (myProblem == PatientProblems.OpenWound)
                {
                    //open wound. tree sap + feathers
                    ResourceTypes[] resources = { ResourceTypes.treeSap, ResourceTypes.feathers };
                    int[] consumedAmount = { 2, 1 };
                    if (HospitalInventory.Instance.PatientConsumeResource(resources, consumedAmount) == false)
                    {
                        //there wasnt enough resources. Stop being able to heal
                        ReduceHealingProgressFromNoResources();
                        return;
                    }
                }
                else if (myProblem == PatientProblems.Infection)
                {
                    //infection. mushroom + flower to cure
                    ResourceTypes[] resources = { ResourceTypes.mushroom, ResourceTypes.flower };
                    int[] consumedAmount = { 2, 1 };
                    if (HospitalInventory.Instance.PatientConsumeResource(resources, consumedAmount) == false)
                    {
                        //there wasnt enough resources. Stop being able to heal
                        ReduceHealingProgressFromNoResources();
                        return;
                    }
                }
                else if (myProblem == PatientProblems.Concussion)
                {
                    //concussion. conch shell and mushrooms
                    ResourceTypes[] resources = { ResourceTypes.conchShell, ResourceTypes.mushroom };
                    int[] consumedAmount = { 2, 1 };
                    if (HospitalInventory.Instance.PatientConsumeResource(resources, consumedAmount) == false)
                    {
                        //there wasnt enough resources. Stop being able to heal
                        ReduceHealingProgressFromNoResources();
                        return;
                    }
                }*/

                UpdateHealingProgress();//heal the patient first, so progress is removed fairly (rather than lowering the cap first)

                ResourceTypes[] resources = ReturnRequiredResources();
                int[] consumedAmount = { 3, 2 };

                
                if (room == null)
                {
                    room = GetComponentInParent<Bed>().Room;
                }
                //if (HospitalInventory.Instance.PatientConsumeResource(resources, consumedAmount) == false)
                if (room.PatientConsumeResource(resources, consumedAmount) == false)
                {
                    //there wasnt enough resources. Stop being able to heal
                    ReduceHealingProgressFromNoResources();
                    return;
                }

                //if we reached this part of the code. resources where consumed succesfully so dont check back for another 30 seconds
                timeStampOfLastConsumptionCheck = Time.time;
                timeToNextConsumptionCheck = timePerConsumption;
            }
        }
        public ResourceTypes[] ReturnRequiredResources()
        {
            ResourceTypes requiredResourceOne = ResourceTypes.none;
            ResourceTypes requiredResourceTwo = ResourceTypes.none;
            switch (myProblem)
            {
                case Patient.PatientProblems.BrokenBone:
                    requiredResourceOne = ResourceTypes.feathers; requiredResourceTwo = ResourceTypes.flower; break;
                case Patient.PatientProblems.Hemorrhage:
                    requiredResourceOne = ResourceTypes.flower; requiredResourceTwo = ResourceTypes.treeSap;break;
                case Patient.PatientProblems.OpenWound:
                    requiredResourceOne = ResourceTypes.treeSap; requiredResourceTwo = ResourceTypes.feathers;break;
                case Patient.PatientProblems.Infection:
                    requiredResourceOne = ResourceTypes.mushroom; requiredResourceTwo = ResourceTypes.flower; break;
                case Patient.PatientProblems.Concussion:
                    requiredResourceOne = ResourceTypes.conchShell; requiredResourceTwo = ResourceTypes.mushroom; break;
            }

            return new ResourceTypes[] { requiredResourceOne, requiredResourceTwo };
        }

        void ReduceHealingProgressFromNoResources()
        {
            //float reductionRate = 0.85f;//multiplier of healing rate at which having no resources reduces a persons heal rate (1 = they stop healing but dont lose any progress)
            //need a different plan than this, as time stamp of next should be the same as time.time
            float reductionAmount = (Time.time - timeStampOfLastConsumptionCheck) * healingRate;//time passed 

            timeStampOfLastConsumptionCheck = Time.time;
            timeToNextConsumptionCheck = 1f;

            currentHealingProgress = Mathf.Clamp(currentHealingProgress - (reductionAmount * outOfResourcesCurrentHealingReductionRate), 0f, maximumHealingProgress);
            maximumHealingProgress = Mathf.Clamp(maximumHealingProgress - (reductionAmount * outOfResourcesMaximumHealingReductionRate), currentHealingProgress, currentHealingProgress + 40f);//if u change this 40f, you also need to change the 0.4 in the healing managers update health bar function
            if (maximumHealingProgress > 100f)
            {
                maximumHealingProgress = 100.1f;
            }

            //I only need to do this if THIS is the active patient
            if (HealingManager.Instance.CurrentPatient == this)
            {
                HealingManager.Instance.UpdateHealthBars(currentHealingProgress, maximumHealingProgress);
            }


        }



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

        #region SaveLoad
        //note: these happen automatically on awake and destroy
        protected override DataChunk SaveData()
        {
            if (myProblem == PatientProblems.HealthRestored) { return null; }//once their health has been restored, theyre a patient anymore and i dont want their data being saved
            return new PatientData(currentHealingProgress, maximumHealingProgress, healingRate, lastHealTimeStamp, timeStampOfLastConsumptionCheck, myProblem);
        }

        protected override void LoadData(DataChunk recievedData)
        {
            PatientData data = recievedData as PatientData;
            if (data != null)//if data is null, this is probably the first time its ever been logged ...OR a horrible error has occured and two objects with the same ID exist
            {
                currentHealingProgress = data.CurrentHealingProgress;
                maximumHealingProgress = data.MaxHealingProgress;
                healingRate = data.HealingRate;
                lastHealTimeStamp = data.LastHealTimeStamp;
                timeStampOfLastConsumptionCheck = data.NextConsumptionTimeStamp;
                myProblem = data.MyProblem;
            }


            //if this patients data doesnt exist, admit them to the ward
            if (data == null)
            {
                AdmitToWard();
            }
        }

        #endregion
    }
}