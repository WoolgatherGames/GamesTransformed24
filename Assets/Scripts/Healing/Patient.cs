
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
        float startingHealth { get { return 20f; } }
        public static float allowedMaximumHealth { get { return 30f; } }
        float[] requiredResourceThresholds { get { return new float[] { 0f, 50f, 80f }; } }
        float giveResourceThresholdDifference { get { return 10f; } }
        int resourceThresholdsMet;


        float currentResourceThresold 
        { 
            get 
            { 
                if (resourceThresholdsMet >= requiredResourceThresholds.Length) 
                { 
                    return 999f; 
                } 
                else 
                { 
                    return requiredResourceThresholds[resourceThresholdsMet]; 
                } 
            } 
        }


        float healingReductionRate { get { return 2f; } }//multiplier for how fast healing regresses once a patient is out of resource. (0f = they wont regress at all but they wont heal either)


        PatientProblemDialogue resourceProblem;
        PatientProblems medicalProblem;
        public PatientProblems MedicalProblem { get { return medicalProblem; } }


        //float percentageGapBetweenRequiringResources { get { return allowedMaximumHealth; } }
        //float percentageGapBetweenResourceMenu { get { return (allowedMaximumHealth - 10f); } }



        //float requiredResoucesTimeBetween { get { if (healingRate <= 0f) { return 0f; } else { return allowedMaximumHealth / healingRate; } } }
        //float canGiveResourcesTimeBetween { get { if (healingRate <= 0f) { return 0f; } else { return (allowedMaximumHealth - 10f) / healingRate; } } }




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



        void AdmitToWard()
        {
            //becomes a patient the player can heal

            //healing rate = randomly decide how much % progres should be made per 60 seconds
            healingRate = Random.Range(8f, 11f) * 0.0166f;//0.0166 is equivilent to 1/60 because healing rate is based on PER minute (60 seconds)

            currentHealingProgress = startingHealth;
            maximumHealingProgress = currentHealingProgress;
            lastHealTimeStamp = Time.time;

            int problemNumber = Random.Range(0, 5);
            switch (problemNumber)
            {
                case 0:
                    medicalProblem = PatientProblems.BrokenBone; break;
                case 1:
                    medicalProblem = PatientProblems.Hemorrhage; break;
                case 2:
                    medicalProblem = PatientProblems.OpenWound; break;
                case 3:
                    medicalProblem = PatientProblems.Infection; break;
                case 4:
                    medicalProblem = PatientProblems.Concussion; break;
            }
        }

        void ChooseResourceProblem()
        {
            patientNeedsResources = true;
            resourceProblem = HealingManager.Instance.ReturnRandomResourceProblem();
        }

        public delegate void Discharged();
        public event Discharged OnDischarged;

        public void Discharge()
        {
            //discharge this patient. Delete their data from the healing manager
            Debug.Log("Ive been discharged");
            medicalProblem = PatientProblems.HealthRestored;//stops this data from being saved
            SaveLoadPersistentData.DestroyData(GetUniqueID());

            //Ideally, destroy the patient and spawn a new identical object (that isnt a patient) so it can walk off 
            OnDischarged?.Invoke();

            Destroy(this.gameObject);
        }

        public void AddMaxHealingProgress(float progressGained)
        {
            //ONLY call this whilst this is the active patient

            //add healing progress but cap it to (allowedMaximumHealth) above the current progress
            maximumHealingProgress = Mathf.Clamp(maximumHealingProgress + progressGained, currentHealingProgress, currentHealingProgress + allowedMaximumHealth);
            if (maximumHealingProgress > 100f)
            {
                maximumHealingProgress = 100.1f;
            }

            HealingManager.Instance.UpdateHealthBars(currentHealingProgress, maximumHealingProgress);
        }

        public void UpdateHealingProgress()
        {
            bool doIHeal = AreResourcesRequired();

            float timeDifference = Time.time - lastHealTimeStamp;
            lastHealTimeStamp = Time.time;
            float progressMade = timeDifference * healingRate;

            if (AreResourcesRequired())
            {
                progressMade = -progressMade * healingReductionRate;
            }
            //we also need to check if progress made would put us ABOVE the cap
            else if (currentHealingProgress + progressMade > currentResourceThresold)
            {
                //player should heal upto the threshold, then for whats left over, begin regressing
                float leftoverProgress = currentResourceThresold - currentHealingProgress + progressMade;
                progressMade = -leftoverProgress * healingReductionRate;

                //this patient now needs resources
                ChooseResourceProblem();
            }
            
            currentHealingProgress = Mathf.Clamp(currentHealingProgress + progressMade, currentHealingProgress, maximumHealingProgress);

            HealingManager.Instance.UpdateHealthBars(currentHealingProgress, maximumHealingProgress, this);

            if (currentHealingProgress >= 100)
            {
                HealingManager.Instance.EnableDischargeButton(this);
            }
        }


        HospitalRoom room;

        float timeStampOfLastConsumptionCheck;
        float timeToNextConsumptionCheck;
        float timeStampOfNextConsumptionCheck;


        bool patientNeedsResources;
        public bool CheckIfResourceMenuShouldOpen()
        {
            float threshhold = currentResourceThresold - giveResourceThresholdDifference;

            if (currentHealingProgress > threshhold || AreResourcesRequired())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        bool AreResourcesRequired()
        {
            if (patientNeedsResources)
            {
                return true;
            }
            else if (currentHealingProgress > currentResourceThresold)
            {
                //if the code ever comes through here i reckon somethings gone wrong 
                ChooseResourceProblem();
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public void ConsumeResources(ResourceTypes resource)
        {
            //call this when the patient is given resources

            //OAEF


            //if all resources have been given
            //oaef check if the correct resource(s) where given
            timeStampOfLastConsumptionCheck = Time.time;
            patientNeedsResources = false;
            resourceThresholdsMet++;

            //oaef tell the healing manager to change to the minigame
        }
        public ResourceTypes[] ReturnRequiredResources()
        {
            ResourceTypes requiredResourceOne = ResourceTypes.none;
            ResourceTypes requiredResourceTwo = ResourceTypes.none;
            switch (medicalProblem)
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

        /*void ReduceHealingProgressFromNoResources(float timePassed)
        {
            //float reductionRate = 0.85f;//multiplier of healing rate at which having no resources reduces a persons heal rate (1 = they stop healing but dont lose any progress)
            //need a different plan than this, as time stamp of next should be the same as time.time
            //float reductionAmount = (Time.time - timeStampOfLastConsumptionCheck) * healingRate;//time passed 
            //timeToNextConsumptionCheck = 1f;

            timePassed = timePassed * healingRate;

            currentHealingProgress = Mathf.Clamp(currentHealingProgress - (timePassed * healingReductionRate), 0f, maximumHealingProgress);
            maximumHealingProgress = Mathf.Clamp(maximumHealingProgress - (timePassed * outOfResourcesMaximumHealingReductionRate), currentHealingProgress, currentHealingProgress + allowedMaximumHealth);
            if (maximumHealingProgress > 100f)
            {
                maximumHealingProgress = 100.1f;
            }

            //I only need to do this if THIS is the active patient
            if (HealingManager.Instance.CurrentPatient == this)
            {
                HealingManager.Instance.UpdateHealthBars(currentHealingProgress, maximumHealingProgress);
            }
        }*/



        [SerializeField] GameObject progressBarParent;
        [SerializeField] Image progressBarFill;
        void DisplayProgressBar()
        {
            //performed when this object is interactable
            PlayerController.OnInteractionTargetChange += HideProgressBar;

            progressBarParent.SetActive(true);

        }
        void OnDisable()
        {
            PlayerController.OnInteractionTargetChange -= HideProgressBar;
        }
        void HideProgressBar()
        {
            PlayerController.OnInteractionTargetChange -= HideProgressBar;

            progressBarParent.SetActive(false);

        }


        #region SaveLoad
        //note: these happen automatically on awake and destroy
        protected override DataChunk SaveData()
        {
            if (medicalProblem == PatientProblems.HealthRestored) { return null; }//once their health has been restored, theyre a patient anymore and i dont want their data being saved
            return new PatientData(currentHealingProgress, maximumHealingProgress, healingRate, lastHealTimeStamp, medicalProblem, resourceProblem, patientNeedsResources, resourceThresholdsMet);
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
                //timeStampOfLastConsumptionCheck = data.NextConsumptionTimeStamp;
                medicalProblem = data.MedicalProblem;
                resourceProblem = data.ResourceProblem;
                patientNeedsResources = data.PatientNeedsResources;
                resourceThresholdsMet = data.ResourceThresholdsMet;
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