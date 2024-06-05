using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Foraging
{
    public class BoatTimer : MonoBehaviour
    {
        //keeps a count of when the boat should arrive 

        private static BoatTimer instance;
        public static BoatTimer Instance { get { return instance; } }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        float timer;

        private void Update()
        {
            timer += Time.deltaTime;
        }

        void CheckBoatStatus()
        {
            
        }
    }
}