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
            SpawnData data = new SpawnData(1, 35, 0.05, 6, 1.0);
            Init(data);
        }
    }
}

