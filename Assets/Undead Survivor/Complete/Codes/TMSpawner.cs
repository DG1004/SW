using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMSpawner : MonoBehaviour
{
    public Transform[] spawnPoint1;

    float timer1;

    private void Awake()
    {
        spawnPoint1 = GetComponentsInChildren<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        timer1 += Time.deltaTime;

        if (timer1 > 10f) {
            timer1 = 0;
            Spawn();
           
        }

       
    }
    void Spawn()
    {
        GameObject travellingMerchant = GameManager.instance.pool.Get(3);
        travellingMerchant.transform.position = spawnPoint1[Random.Range(1,spawnPoint1.Length)].position;
    }


}
