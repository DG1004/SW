using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using UnityEngine;



namespace Goldmetal.UndeadSurvivor
{

    public class EnemyRanged1 : Enemy
    {
        float time = 0;
        public float fireRate = 10f;        // �߻� ���� (�� ����)
        private float nextFireTime = 0f;   // ���� �߻� �ð�
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
                rigid.velocity = Vector2.zero;
            }
            else if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate; // ���� �߻� �ð� ������Ʈ
            }
        }
        void Shoot()
        {
            Debug.Log("���� �Ѿ� ����");
            GameObject bullet = GameManager.instance.pool.Get_Enemy(3);
            bullet.transform.position = transform.position;
            bullet.GetComponent<EnemyBullet>().Init(this, 9, 7, 1);
        }
        protected override void race_init()//�������̵� �ʱ� ���Ȱ���
        {
            this.race_index = 0;
            this.coe_attack = 2.5;
            this.coe_defence = 0.25;
            this.coe_health = 0.025;
            this.coe_speed = 0.25;
            

            //SpawnData data = new SpawnData(0, 20, 0.01, 3, 2.2);
            //general_Init(data);
        }


    }
}