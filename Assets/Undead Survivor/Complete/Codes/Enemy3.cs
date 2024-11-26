using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;



namespace Goldmetal.UndeadSurvivor
{

    public class Enemy3 : Enemy_Parent
    {
        public void Init()//오버라이딩 초기 스탯결정
        {
            SpawnData data = new SpawnData(0, 35, 0.05, 6, 0.8);
            Init(data);
        }
    }
}

