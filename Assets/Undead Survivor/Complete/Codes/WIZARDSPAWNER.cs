using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WIZARDSPAWNER : MonoBehaviour
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
        if (!GameManager.instance.isLive)
            return;

        timer1 += Time.deltaTime;

        if (timer1 > 60f)
        {
            timer1 = 0;
            Spawn();

        }


    }
    void Spawn()
    {
        GameObject wizard = GameManager.instance.pool.Get_Enemy(14);
        wizard.transform.position = spawnPoint1[Random.Range(1, spawnPoint1.Length)].position;
        // WIZARDSHOP에 생성된 인스턴스 연결
        WIZARDSHOP shop = FindObjectOfType<WIZARDSHOP>();
        if (shop != null)
        {
            shop.wizard = wizard;
        }
    }
}