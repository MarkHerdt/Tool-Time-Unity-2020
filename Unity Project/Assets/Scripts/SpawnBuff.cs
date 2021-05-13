using UnityEngine;

namespace ToolTime
{
    public class SpawnBuff : MonoBehaviour
    {
        Transform[] spawnPoints;

        public GameObject[] buffPrefabs;
        GameObject buff;

        float spawnTimer;
        float spawnDelay = 10; // Minimum time between each buff spawn

        int spawnChance = 100; // Chance for a buff to spawn

        bool levelFinished;

        private void Awake()
        {
            spawnPoints = GetComponentsInChildren<Transform>();
        }

        private void Start()
        {
            // Subsribe to event system
            EventSystemController.self.onGlobalTimerEnd += RoundHasEnded;

            levelFinished = false;
        }

        void RoundHasEnded()
        {
            levelFinished = true;
        }

        private void Update()
        {
            if (!levelFinished)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= spawnDelay) // Calls "SpawnBuffObject" every set amount of time
                {
                    SpawnBuffObject();
                    spawnTimer = 0;
                }
            }
        }

        /// <summary>
        /// Spawns a random buff
        /// </summary>
        void SpawnBuffObject()
        {
            int randomChance = Random.Range(1, 100);
            int randomBuff = Random.Range(0, buffPrefabs.Length - 1);
            int randomSpawn = Random.Range(0, spawnPoints.Length);
            if (randomChance <= spawnChance)
            {
                buff = Instantiate(buffPrefabs[randomBuff], spawnPoints[randomSpawn].transform.position, Quaternion.identity);
                Destroy(buff, 10);
            }
        }
    }
}
