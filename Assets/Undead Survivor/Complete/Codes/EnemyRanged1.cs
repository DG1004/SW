using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using UnityEngine;



namespace Goldmetal.UndeadSurvivor
{

    public class EnemyRanged1 : Enemy
    {
        //float time = 0; 안쓰인다는 오류 있음
        float fireRate = 5f;        // 발사 간격 (초 단위)
        private float nextFireTime = 0f;   // 다음 발사 시간
        protected override void FixedUpdate()
        {
            if (!GameManager.instance.isLive)
                return;
            if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
                return;
            float distanceToPlayer = Vector2.Distance(target.position, rigid.position);
            if (distanceToPlayer > 10f)
            {
                Vector2 dirVec = target.position - rigid.position;
                Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
                rigid.MovePosition(rigid.position + nextVec);
            }
            else if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate; // 다음 발사 시간 업데이트
            }
            rigid.velocity = Vector2.zero;
        }
        void Shoot()
        {
            GameObject bullet = GameManager.instance.pool.Get_Enemy(4);///
            bullet.transform.position = transform.position;
            bullet.GetComponent<EnemyBullet>().Init(OnAttack, 10, 7, attack,true);
        }
        protected override void race_init()//오버라이딩 초기 스탯결정
        {
            this.race_index = 3;////
            this.coe_attack = 0.025;
            this.coe_defence = 0.5;
            this.coe_race = 0.0075;
            this.coe_speed = 1;
            this.maxhealth = 35 ; 

            //SpawnData data = new SpawnData(0, 20, 0.01, 3, 2.2);
            //general_Init(data);
        }


    }
}