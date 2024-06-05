
using GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

namespace Healing
{
    public class Patient : HoldData, IInteractable
    {
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

        float maximumHealingProgress;   //how much healing progress can currently be gained (capped at a certain level above current healing progress)
        float currentHealingProgress;   //how much healing progress has been gained
        float healingRate;              //how fast this character heals (percentage per second of game time)
        float lastHealTimeStamp;        //the time that current healing progress was last updated (time.time)

        PatientProblems myProblem;

        void AdmitToWard()
        {
            Debug.Log("Ive been added to the ward");

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

        public void Discharge()
        {
            //discharge this patient. Delete their data from the healing manager
            Debug.Log("Ive been discharged");
            myProblem = PatientProblems.HealthRestored;//stops this data from being saved
            SaveLoadPersistentData.DestroyData(GetUniqueID());

            //Ideally, destroy the patient and spawn a new identical object (that isnt a patient) so it can walk off 
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
            //ONLY call this when this object is the currently active patient 

            float timeDifference = Time.time - lastHealTimeStamp;
            lastHealTimeStamp = Time.time;
            float progressMade = timeDifference * healingRate;
            currentHealingProgress = Mathf.Clamp(currentHealingProgress + progressMade, currentHealingProgress, maximumHealingProgress);

            HealingManager.Instance.UpdateHealthBars(currentHealingProgress, maximumHealingProgress);

            if (currentHealingProgress >= 100)
            {
                HealingManager.Instance.EnableDischargeButton();
            }
        }



        void Update()
        {
            ConsumeResources();
        }

        //float timeStampOfNextConsumptionCheck;
        float timeStampOfLastConsumptionCheck;
        float timeToNextConsumptionCheck;
        void ConsumeResources()
        {
            float timePerConsumption = 30f;

            if (timeStampOfLastConsumptionCheck + timeToNextConsumptionCheck < Time.time)
            {


                if (myProblem == PatientProblems.BrokenBone)
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
                }

                //if we reached this part of the code. resources where consumed succesfully so dont check back for another 30 seconds
                timeStampOfLastConsumptionCheck = Time.time;
                timeToNextConsumptionCheck = timePerConsumption;
            }
        }

        void ReduceHealingProgressFromNoResources()
        {


            float reductionRate = 1.25f;//multiplier of healing rate at which having no resources reduces a persons heal rate (1 = they stop healing but dont lose any progress)
            //need a different plan than this, as time stamp of next should be the same as time.time
            float reductionAmount = (Time.time - timeStampOfLastConsumptionCheck) * healingRate * reductionRate;//time passed 

            timeStampOfLastConsumptionCheck = Time.time;
            timeToNextConsumptionCheck = 1f;

            currentHealingProgress = Mathf.Clamp(currentHealingProgress - reductionAmount, 0f, maximumHealingProgress);
            maximumHealingProgress = Mathf.Clamp(maximumHealingProgress - reductionAmount, currentHealingProgress, currentHealingProgress + 40f);//if u change this 40f, you also need to change the 0.4 in the healing managers update health bar function
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