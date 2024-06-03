
using GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Healing
{
    public class Patient : HoldData, IInteractable
    {
        enum PatientProblems
        {
            BrokenBone,
            Hemorrhage,
            OpenWound,
            Infection,
            Concussion
        }

        public void Interact()
        {
            HealingManager.Instance.OpenHealingMenu(this);
        }

        float maximumHealingProgress;//how much healing progress can currently be gained
        float currentHealingProgress;//how much healing progress has been gained
        float healingRate;//how fast this character heals (percentage per minute of game time)
        PatientProblems myProblem;

        void AdmitToWard()
        {
            //becomes a patient the player can heal
            healingRate = Random.Range(24f, 28f);

            maximumHealingProgress = 0;
            currentHealingProgress = 0;

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

        #region SaveLoad
        //note: these happen automatically on awake and destroy
        protected override DataChunk SaveData()
        {
            throw new System.NotImplementedException();
        }

        protected override void LoadData(DataChunk recievedData)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}