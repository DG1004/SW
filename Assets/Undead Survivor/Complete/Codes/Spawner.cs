using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goldmetal.UndeadSurvivor
{
    public class Spawner : MonoBehaviour
    {
        public Transform[] spawnPoint;
        public SpawnData[] spawnData;
        public int initialEnemyCount = 5; // Set the number of initial enemies

        void Awake()
        {
            spawnPoint = GetComponentsInChildren<Transform>();
            SpawnInitialEnemies();
        }

        void SpawnInitialEnemies()
        {
            for (int i = 0; i < initialEnemyCount; i++)
            {
                Spawn();
            }
        }

        void Spawn()
        {
            GameObject enemy = GameManager.instance.pool.Get(0);
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;

            // Use random spawnData for initial enemies
            //int randomIndex = Random.Range(0, spawnData.Length);
            enemy.GetComponent<Enemy>().Init(new SpawnData());
        }
    }
    [System.Serializable]
    public class SpawnData
    {
        public SpawnData(int spriteType=0,int health=100,float speed=5)
        {
            this.spriteType = spriteType;
            this.health = health;
            this.speed = speed;
        }
        public int spriteType;
        public int health;
        public float speed;
    }

}
