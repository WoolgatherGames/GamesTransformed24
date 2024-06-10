using GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Foraging
{
    public class ForestResourceManager : HoldData
    {
        //is responsible for spawning and scattering all of the forests resources

        //i need an intimidary script that can make the sound of the horn blowing no matter the scene 


        //responsible for: 
        //holding the number of flowers and feathers currently in the forest and randomly scattering them on load in designated spawn areas (use the random spawning code from my mobile game)
        //spawning new flowers on a regular basis (dont need to spawn them immediatly. can spawn them on start, but make sure to up the count)
        //deciding when the owl air force will fly over and drop feathers (spawn these immediatly) 
        //PUT THE BOAT IN A DIFFERENT SCRIPT!!!!


        private static ForestResourceManager instance;
        public static ForestResourceManager Instance { get { return instance; } }

        protected override void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

            base.Awake();
        }

        private void Start()
        {
            CalculateQuadVolume();

            //Spawn objects here
            AddForageableResourcesToMapPool();

            CheckAirplanes();
            SpawnAllResourcesOnLoad();
        }


        private void Update()
        {
            //AddPlantResourcesToMapPool();
            CheckAirplanes();
        }








        #region SpawnAreas

        [System.Serializable]
        class SpawnQuad
        {
            public float leftPosition;
            public float rightPosition;
            public float topPosition;
            public float bottomPosition;

            public float quadVolume
            {
                get
                {
                    float x = Mathf.Abs(rightPosition - leftPosition);
                    float y = Mathf.Abs(topPosition - bottomPosition);
                    return x * y;
                }
            }
        }


        [SerializeField] GameObject mushroomPrefab;
        [SerializeField, Tooltip("A list of 2D squares that enemies can spawn within. There is an even chance of spawning at any position within the total size of the quads. Overlapping quads will increase the chance of an enemy spawning in that position")]
        SpawnQuad[] mushroomSpawnBoxes;
        float mushroomQuadsVolume;

        [Space(5)]
        [SerializeField] GameObject flowerPrefab;
        [SerializeField] SpawnQuad[] flowerSpawnBoxes;
        float flowerQuadsVolume;

        [Space(5)]
        [SerializeField] GameObject featherPrefab;
        [SerializeField] SpawnQuad[] featherSpawnBoxes;
        float featherQuadsVolume;

        [Space(5)]
        [SerializeField] GameObject conchShellPrefab;
        [SerializeField] SpawnQuad[] conchShellBoxes;
        float shellQuadsVolume;

        private void CalculateQuadVolume()
        {
            mushroomQuadsVolume = 0f;
            foreach (SpawnQuad box in mushroomSpawnBoxes)
            {
                mushroomQuadsVolume += box.quadVolume;
            }

            flowerQuadsVolume = 0f;
            foreach (SpawnQuad box in flowerSpawnBoxes)
            {
                flowerQuadsVolume += box.quadVolume;
            }

            featherQuadsVolume = 0f;
            foreach (SpawnQuad box in featherSpawnBoxes)
            {
                featherQuadsVolume += box.quadVolume;
            }

            shellQuadsVolume = 0f;
            foreach (SpawnQuad box in conchShellBoxes)
            {
                shellQuadsVolume += box.quadVolume;
            }
        }

        Vector3 CalculateMushroomSpawnPosition()
        {
            float boxToSpawnInByVolume = Random.Range(0f, mushroomQuadsVolume);
            //Debug.Log(boxToSpawnInByVolume);
            float countedBoxVolume = 0f;
            int quadToSpawnIn = 0;
            for (int i = 0; i < mushroomSpawnBoxes.Length; i++)
            {
                countedBoxVolume += mushroomSpawnBoxes[i].quadVolume;
                if (countedBoxVolume >= boxToSpawnInByVolume)
                {
                    quadToSpawnIn = i;
                    break;
                }
            }

            float xSpawnPos = Random.Range(mushroomSpawnBoxes[quadToSpawnIn].leftPosition, mushroomSpawnBoxes[quadToSpawnIn].rightPosition);
            float ySpawnPos = Random.Range(mushroomSpawnBoxes[quadToSpawnIn].bottomPosition, mushroomSpawnBoxes[quadToSpawnIn].topPosition);
            float zSpawnPos = 0f;

            return new Vector3(xSpawnPos, ySpawnPos, zSpawnPos);
        }

        Vector3 CalculateFlowerSpawnPosition()
        {
            float boxToSpawnInByVolume = Random.Range(0f, flowerQuadsVolume);
            //Debug.Log(boxToSpawnInByVolume);
            float countedBoxVolume = 0f;
            int quadToSpawnIn = 0;
            for (int i = 0; i < flowerSpawnBoxes.Length; i++)
            {
                countedBoxVolume += flowerSpawnBoxes[i].quadVolume;
                if (countedBoxVolume >= boxToSpawnInByVolume)
                {
                    quadToSpawnIn = i;
                    break;
                }
            }

            float xSpawnPos = Random.Range(flowerSpawnBoxes[quadToSpawnIn].leftPosition, flowerSpawnBoxes[quadToSpawnIn].rightPosition);
            float ySpawnPos = Random.Range(flowerSpawnBoxes[quadToSpawnIn].bottomPosition, flowerSpawnBoxes[quadToSpawnIn].topPosition);
            float zSpawnPos = 0f;

            return new Vector3(xSpawnPos, ySpawnPos, zSpawnPos);
        }

        Vector3 CalculateFeatherSpawnPosition()
        {
            float boxToSpawnInByVolume = Random.Range(0f, featherQuadsVolume);
            //Debug.Log(boxToSpawnInByVolume);
            float countedBoxVolume = 0f;
            int quadToSpawnIn = 0;
            for (int i = 0; i < featherSpawnBoxes.Length; i++)
            {
                countedBoxVolume += featherSpawnBoxes[i].quadVolume;
                if (countedBoxVolume >= boxToSpawnInByVolume)
                {
                    quadToSpawnIn = i;
                    break;
                }
            }

            float xSpawnPos = Random.Range(featherSpawnBoxes[quadToSpawnIn].leftPosition, featherSpawnBoxes[quadToSpawnIn].rightPosition);
            float ySpawnPos = Random.Range(featherSpawnBoxes[quadToSpawnIn].bottomPosition, featherSpawnBoxes[quadToSpawnIn].topPosition);
            float zSpawnPos = 0f;

            return new Vector3(xSpawnPos, ySpawnPos, zSpawnPos);
        }

        Vector3 CalculateShellSpawnPosition()
        {
            float boxToSpawnInByVolume = Random.Range(0f, shellQuadsVolume);
            //Debug.Log(boxToSpawnInByVolume);
            float countedBoxVolume = 0f;
            int quadToSpawnIn = 0;
            for (int i = 0; i < conchShellBoxes.Length; i++)
            {
                countedBoxVolume += conchShellBoxes[i].quadVolume;
                if (countedBoxVolume >= boxToSpawnInByVolume)
                {
                    quadToSpawnIn = i;
                    break;
                }
            }

            float xSpawnPos = Random.Range(conchShellBoxes[quadToSpawnIn].leftPosition, conchShellBoxes[quadToSpawnIn].rightPosition);
            float ySpawnPos = Random.Range(conchShellBoxes[quadToSpawnIn].bottomPosition, conchShellBoxes[quadToSpawnIn].topPosition);
            float zSpawnPos = 0f;

            return new Vector3(xSpawnPos, ySpawnPos, zSpawnPos);
        }

        #endregion

        void SpawnAllResourcesOnLoad()
        {
            //flowers
            for (int i = 0; i < flowersInForest; i++)
            {
                Instantiate(flowerPrefab, CalculateFlowerSpawnPosition(), Quaternion.identity);
            }
            for (int i = 0; i < mushroomInForest; i++)
            {
                Instantiate(mushroomPrefab, CalculateMushroomSpawnPosition(), Quaternion.identity);
            }
            for (int i = 0; i < feathersInForest; i++)
            {
                Instantiate(featherPrefab, CalculateFeatherSpawnPosition(), Quaternion.identity);
            }
            for (int i = 0; i < conchShellsInForest; i++)
            {
                Instantiate(conchShellPrefab, CalculateShellSpawnPosition(), Quaternion.identity);
            }
        }

        //mushrooms and flowers
        int flowersInForest;
        int mushroomInForest;
        int feathersInForest;
        float lastSpawnResourcesTimeStamp;

        void AddForageableResourcesToMapPool()
        {
            //adds flowers + mushrooms + conch shells 
            //feathers are handled seperately. 

            float timeBetweenForageableResources = 10f;
            if (lastSpawnResourcesTimeStamp + timeBetweenForageableResources > Time.time)
            {
                return;
            }

            //dont spawn objects whilst the forest is open, the player should go back to the hospital before they spawn
            //this function adds a random number of flowers, feathers and mushrooms to the pool every X seconds, where X is whatever update is set too (currently 10)

            int multiplier = Mathf.RoundToInt((Time.time - lastSpawnResourcesTimeStamp) / timeBetweenForageableResources);

            lastSpawnResourcesTimeStamp = Time.time;

            //BALANCE
            IncreaseNumberOfFlowers(Random.Range(2, 11) * multiplier);  
            IncreaseNumberOfMushrooms(Random.Range(2, 17) * multiplier);//how many spawn every 10 seconds
            IncreaseNumberOfShells(Random.Range(1, 5) * multiplier);
        }


        void IncreaseNumberOfFlowers(int numberSpawned)
        {
            flowersInForest += numberSpawned;

            int maximumFlowersAllowed = 150;
            if (flowersInForest > maximumFlowersAllowed)
            {
                flowersInForest = maximumFlowersAllowed;
            }
        }
        public void RemoveFlower()
        {
            flowersInForest--;
        }
        void IncreaseNumberOfMushrooms(int numberSpawned)
        {
            mushroomInForest += numberSpawned;

            int maximumAllowed = 150;
            if (mushroomInForest > maximumAllowed)
            {
                mushroomInForest = maximumAllowed;
            }
        }
        public void RemoveMushroom()
        {
            mushroomInForest--;
        }

        int conchShellsInForest;
        void IncreaseNumberOfShells(int add)
        {
            conchShellsInForest += add;
            int maximumAllowed = 35;
            if (conchShellsInForest > maximumAllowed)
            {
                conchShellsInForest = maximumAllowed;
            }
        }
        public void RemoveConchShell()
        {
            conchShellsInForest--;
        }


        //air support


        float airArrivalMostRecentTimeStamp;
        bool featherDropAnimationActive;

        void CheckAirplanes()
        {
            float timeBetweenAirplaneDrops = 80f;
            float durationOfAirdropAnimation = 5f;

            if (airArrivalMostRecentTimeStamp + timeBetweenAirplaneDrops < Time.time)
            {
                //an airplane drop should have happened already. Spawn feathers immediatly

                airArrivalMostRecentTimeStamp += timeBetweenAirplaneDrops;
                SpawnFeathers();
            }
            else if (airArrivalMostRecentTimeStamp + timeBetweenAirplaneDrops < Time.time + durationOfAirdropAnimation)
            {
                if (!featherDropAnimationActive)
                {
                    //spawn feather drop animation
                    airArrivalMostRecentTimeStamp = Time.time + durationOfAirdropAnimation;

                    //oaef
                    //have a seperate prefab for the planes that spawns the feather at the end of their animation
                    SpawnFeathers();
                }
            }

        }

        void SpawnFeathers()
        {
            //oaef stagger the feather spawn over a few seconds instead of blasting it all at once
            Debug.Log("im spawning");

            int numberSpawned = Random.Range(25, 40);
            numberSpawned = IncreaseNumberOfFeathers(numberSpawned);

            for (int i = 0; i < numberSpawned; i++)
            {
                Instantiate(featherPrefab, CalculateMushroomSpawnPosition(), Quaternion.identity);
            }
        }

        

        int IncreaseNumberOfFeathers(int numberSpawned)
        {
            //returns the number of feathers that where allowed to spawn

            int maximumFeathersAllowed = 100;
            numberSpawned = Mathf.Clamp(numberSpawned, 0, maximumFeathersAllowed - feathersInForest);

            feathersInForest += numberSpawned;

            return numberSpawned;
        }
        public void RemoveFeather()
        {
            feathersInForest--;
        }





        protected override DataChunk SaveData()
        {
            return new ForestResourceManagerData(flowersInForest, mushroomInForest, feathersInForest, conchShellsInForest, lastSpawnResourcesTimeStamp, airArrivalMostRecentTimeStamp);
        }

        protected override void LoadData(DataChunk recievedData)
        {
            ForestResourceManagerData data = recievedData as ForestResourceManagerData;
            if (data != null)
            {
                flowersInForest = data.NumberOfFlowers;
                mushroomInForest = data.NumberOfMushrooms;
                feathersInForest = data.NumberOfFeathers;

                lastSpawnResourcesTimeStamp = data.SpawnPlantResourcesTimeStamp;
                airArrivalMostRecentTimeStamp = data.SpawnAirTimeStamp;
            }
            else
            {
                //starting values 
                flowersInForest = 20;
                mushroomInForest = 20;
                feathersInForest = 20;
                conchShellsInForest = 10;

                lastSpawnResourcesTimeStamp = 0f;
                airArrivalMostRecentTimeStamp = 0f;
            }
        }




        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            foreach (SpawnQuad box in mushroomSpawnBoxes)
            {
                Vector3 topLeftPos = new Vector3(box.leftPosition, box.topPosition, 0f);
                Vector3 topRightPos = new Vector3(box.rightPosition, box.topPosition, 0f);
                Vector3 bottomRightPos = new Vector3(box.rightPosition, box.bottomPosition, 0f);
                Vector3 bottomLeftPos = new Vector3(box.leftPosition, box.bottomPosition, 0f);

                Gizmos.DrawLine(topLeftPos, topRightPos);
                Gizmos.DrawLine(topRightPos, bottomRightPos);
                Gizmos.DrawLine(bottomRightPos, bottomLeftPos);
                Gizmos.DrawLine(bottomLeftPos, topLeftPos);
            }

            Gizmos.color = Color.yellow;

            foreach (SpawnQuad box in flowerSpawnBoxes)
            {
                Vector3 topLeftPos = new Vector3(box.leftPosition, box.topPosition, 0f);
                Vector3 topRightPos = new Vector3(box.rightPosition, box.topPosition, 0f);
                Vector3 bottomRightPos = new Vector3(box.rightPosition, box.bottomPosition, 0f);
                Vector3 bottomLeftPos = new Vector3(box.leftPosition, box.bottomPosition, 0f);

                Gizmos.DrawLine(topLeftPos, topRightPos);
                Gizmos.DrawLine(topRightPos, bottomRightPos);
                Gizmos.DrawLine(bottomRightPos, bottomLeftPos);
                Gizmos.DrawLine(bottomLeftPos, topLeftPos);
            }

            Gizmos.color = Color.cyan;

            foreach (SpawnQuad box in featherSpawnBoxes)
            {
                Vector3 topLeftPos = new Vector3(box.leftPosition, box.topPosition, 0f);
                Vector3 topRightPos = new Vector3(box.rightPosition, box.topPosition, 0f);
                Vector3 bottomRightPos = new Vector3(box.rightPosition, box.bottomPosition, 0f);
                Vector3 bottomLeftPos = new Vector3(box.leftPosition, box.bottomPosition, 0f);

                Gizmos.DrawLine(topLeftPos, topRightPos);
                Gizmos.DrawLine(topRightPos, bottomRightPos);
                Gizmos.DrawLine(bottomRightPos, bottomLeftPos);
                Gizmos.DrawLine(bottomLeftPos, topLeftPos);
            }

            Gizmos.color = Color.blue;

            foreach (SpawnQuad box in conchShellBoxes)
            {
                Vector3 topLeftPos = new Vector3(box.leftPosition, box.topPosition, 0f);
                Vector3 topRightPos = new Vector3(box.rightPosition, box.topPosition, 0f);
                Vector3 bottomRightPos = new Vector3(box.rightPosition, box.bottomPosition, 0f);
                Vector3 bottomLeftPos = new Vector3(box.leftPosition, box.bottomPosition, 0f);

                Gizmos.DrawLine(topLeftPos, topRightPos);
                Gizmos.DrawLine(topRightPos, bottomRightPos);
                Gizmos.DrawLine(bottomRightPos, bottomLeftPos);
                Gizmos.DrawLine(bottomLeftPos, topLeftPos);
            }
        }
    }
}