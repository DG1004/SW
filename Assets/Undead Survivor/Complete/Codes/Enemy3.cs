using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;



namespace Goldmetal.UndeadSurvivor
{

    public class Enemy3 : Enemy
    {
        protected override void race_init()//오버라이딩 초기 스탯결정
        {
            this.race_index = 2;////
            this.coe_attack = 2.5;
            this.coe_defence = 0.125;
            this.coe_health = 0.0125;
            this.coe_speed = 0.25;
            //SpawnData data = new SpawnData(0, 20, 0.01, 3, 2.2);
            //general_Init(data);
        }
       /* public void Init()//오버라이딩 초기 스탯결정
        {
            SpawnData data = new SpawnData(1, 35, 0.05, 6, 1.0);
            Init(data);
        }*/
    }
}

