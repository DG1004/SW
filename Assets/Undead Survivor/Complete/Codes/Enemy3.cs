using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;



namespace Goldmetal.UndeadSurvivor
{

    public class Enemy3 : Enemy_Parent
    {
        public void Init()//�������̵� �ʱ� ���Ȱ���
        {
            SpawnData data = new SpawnData(0, 35, 0.05, 6, 0.8);
            Init(data);
        }
    }
}

