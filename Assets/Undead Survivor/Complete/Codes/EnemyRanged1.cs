using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;



namespace Goldmetal.UndeadSurvivor
{

    public class EnemyRanged1 : Enemy
    {
        public GameObject bullet;
        protected override void race_init()//오버라이딩 초기 스탯결정
        {
            this.race_index = 0;
            this.coe_attack = 2.5;
            this.coe_defence = 0.25;
            this.coe_health = 0.025;
            this.coe_speed = 0.25;
            bullet.SetActive(true);
            bullet.transform.position=transform.position;
            bullet.GetComponent<EnemyBullet>().Init(this,3,5,5);

            //SpawnData data = new SpawnData(0, 20, 0.01, 3, 2.2);
            //general_Init(data);
        }


    }
}