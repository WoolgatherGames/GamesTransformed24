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
            AddPlantResourcesToMapPool();
            SpawnAllPlantResources();

            CheckAirplanes();
        }


        private void Update()
        {
            //AddPlantResourcesToMapPool();
            CheckAirplanes();
        }


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

        [SerializeField, Tooltip("A list of 2D squares that enemies can spawn within. There is an even chance of spawning at any position within the total size of the quads. Overlapping quads will increase the chance of an enemy spawning in that position")]
        SpawnQuad[] spawnBoxes;
        float totalQuadsVolume;

        private void CalculateQuadVolume()
        {
            totalQuadsVolume = 0f;
            foreach (SpawnQuad box in spawnBoxes)
            {
                totalQuadsVolume += box.quadVolume;
            }
        }

        Vector3 CalculateSpawnPosition()
        {
            float boxToSpawnInByVolume = Random.Range(0f, totalQuadsVolume);
            //Debug.Log(boxToSpawnInByVolume);
            float countedBoxVolume = 0f;
            int quadToSpawnIn = 0;
            for (int i = 0; i < spawnBoxes.Length; i++)
            {
                countedBoxVolume += spawnBoxes[i].quadVolume;
                if (countedBoxVolume >= boxToSpawnInByVolume)
                {
                    quadToSpawnIn = i;
                    break;
                }
            }

            float xSpawnPos = Random.Range(spawnBoxes[quadToSpawnIn].leftPosition, spawnBoxes[quadToSpawnIn].rightPosition);
            float ySpawnPos = Random.Range(spawnBoxes[quadToSpawnIn].bottomPosition, spawnBoxes[quadToSpawnIn].topPosition);
            float zSpawnPos = 0f;

            return new Vector3(xSpawnPos, ySpawnPos, zSpawnPos);
        }

        //mushrooms and flowers
        int flowersInForest;
        int mushroomInForest;
        int feathersInForest;
        float lastSpawnResourcesTimeStamp;

        void AddPlantResourcesToMapPool()
        {
            float timeBetweenPlantResources = 10f;
            if (lastSpawnResourcesTimeStamp + timeBetweenPlantResources > Time.time)
            {
                Debug.Log(lastSpawnResourcesTimeStamp + timeBetweenPlantResources + ":" + Time.time);
                return;
            }

            //dont spawn objects whilst the forest is open, the player should go back to the hospital before they spawn
            //this function adds a random number of flowers, feathers and mushrooms to the pool every X seconds, where X is whatever update is set too (currently 10)

            int multiplier = Mathf.RoundToInt((Time.time - lastSpawnResourcesTimeStamp) / timeBetweenPlantResources);

            lastSpawnResourcesTimeStamp = Time.time;
            IncreaseNumberOfFlowers(Random.Range(2, 4) * multiplier);
            IncreaseNumberOfMushrooms(Random.Range(2, 9) * multiplier);//how many spawn every 10 seconds
        }

        [SerializeField] GameObject flowerPrefab;
        [SerializeField] GameObject mushroomPrefab;

        void SpawnAllPlantResources()
        {
            //flowers
            for (int i = 0; i < flowersInForest; i++)
            {
                Instantiate(flowerPrefab, CalculateSpawnPosition(), Quaternion.identity);
            }
            for (int i = 0; i < mushroomInForest; i++)
            {
                Instantiate(mushroomPrefab, CalculateSpawnPosition(), Quaternion.identity);
            }
        }

        void IncreaseNumberOfFlowers(int numberSpawned)
        {
            flowersInForest += numberSpawned;

            int maximumFlowersAllowed = 100;
            if (flowersInForest > maximumFlowersAllowed)
            {
                flowersInForest = maximumFlowersAllowed;
            }
        }
        void IncreaseNumberOfMushrooms(int numberSpawned)
        {
            mushroomInForest += numberSpawned;

            int maximumAllowed = 100;
            if (mushroomInForest > maximumAllowed)
            {
                mushroomInForest = maximumAllowed;
            }
        }


        //air support
        [SerializeField] GameObject featherPrefab;

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

            int numberSpawned = Random.Range(30, 50);
            numberSpawned = IncreaseNumberOfFeathers(numberSpawned);

            for (int i = 0; i < numberSpawned; i++)
            {
                Instantiate(featherPrefab, CalculateSpawnPosition(), Quaternion.identity);
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

        protected override DataChunk SaveData()
        {
            return new ForestResourceManagerData(flowersInForest, mushroomInForest, feathersInForest, lastSpawnResourcesTimeStamp, airArrivalMostRecentTimeStamp);
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
        }




        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            foreach (SpawnQuad box in spawnBoxes)
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