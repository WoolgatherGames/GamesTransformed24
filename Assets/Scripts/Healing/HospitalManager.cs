using Healing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HospitalManager : MonoBehaviour
{
    //manages ur patients 

    [System.Serializable]
    class Bed
    {
        public Transform bed;
        public bool hasAPatient;
    }

    [SerializeField] Bed[] beds;

    [SerializeField] GameObject patientPrefab;

    private void Start()
    {
        //delete this: 
        Application.targetFrameRate = 30;

        for (int i = 0; i < beds.Length; i++)
        {
            if (!beds[i].hasAPatient)
            {
                Instantiate(patientPrefab, beds[i].bed);
            }
        }
    }
}
