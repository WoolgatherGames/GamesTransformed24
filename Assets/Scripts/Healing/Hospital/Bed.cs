using GameData;
using Healing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Healing
{
    public class Bed : HoldData, IInteractable
    {
        [SerializeField]
        HospitalRoom hospitalRoom;//maybe store an int and get the hospital manager to say which room is which rather than having this serialized
        public HospitalRoom Room { get { return hospitalRoom; } }

        [SerializeField] GameObject patientPrefab;
        [SerializeField] Transform patientPosition;
        bool hasAPatient;

        Patient patient;

        public void Interact()
        {
            if (hasAPatient)
            {
                if (patient == null)
                {
                    patient = GetComponentInChildren<Patient>();
                }

                patient.Interact();
            }
            else
            {
                hasAPatient = true;
                SpawnPatient();
            }
        }

        public void SpawnPatient()
        {
            Instantiate(patientPrefab, patientPosition);

            patient = GetComponentInChildren<Patient>();
            patient.OnDischarged += OnPatientDischarge;
        }

        [SerializeField] Transform InteractIndicatorPosition;
        public Vector3 ReturnPositionForPromptIndicator()
        {
            return InteractIndicatorPosition.position;
        }

        void OnPatientDischarge()
        {
            hasAPatient = false;
            patient = null;
        }

        protected override DataChunk SaveData()
        {
            Debug.Log("im savin");
            return new BedData(hasAPatient);
        }

        protected override void LoadData(DataChunk recievedData)
        {
            BedData data = recievedData as BedData;
            if (data != null)
            {
                hasAPatient = data.HasAPatient;
                if (hasAPatient)
                {
                    SpawnPatient();
                }
            }
            else
            {
                hasAPatient = false;
            }
        }

        protected override void OnDestroy()
        {
            if (patient != null) { patient.OnDischarged -= OnPatientDischarge; }
            base.OnDestroy();
        }
    }
}