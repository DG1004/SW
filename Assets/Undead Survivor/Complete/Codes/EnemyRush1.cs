using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRush1 : Enemy
{
    //float time = 0; 안쓰인다는 오류 있음
    public float fireRate = 10f;        // 발사 간격 (초 단위)
    private float nextFireTime = 0f;   // 다음 발사 시간
    private float multiplyRate=7f;
    private Vector2 dirVec;
    private float time;
    protected override void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        float distanceToPlayer = Vector2.Distance(target.position, rigid.position);
        if (distanceToPlayer > 7f)
        {
            RushEnd();
            dirVec = target.position - rigid.position;
            Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
            rigid.velocity = Vector2.zero;
            nextFireTime = Time.time + 3f;
        }
        else if (Time.time > nextFireTime)
        {
            coll.isTrigger = true;
            /*Vector2 nextVec = *Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);*/
            rigid.velocity = multiplyRate * dirVec.normalized * speed;
        }
        else
        {
            anim.speed = 0;
            rigid.velocity = Vector2.zero;
            dirVec = EnemyBullet.CalculateAimDirection(transform.position, target.position, GameManager.instance.player.예측샷용플레이어속도, multiplyRate * speed);
        }
    }

    void RushEnd()
    {
        coll.isTrigger = false;
        anim.speed = 1;
    }
  
    protected override void race_init()//오버라이딩 초기 스탯결정
    {
        this.race_index = 5;////
        this.coe_attack = 0.025;
        this.coe_defence = 0.125;
        this.coe_health = 0.005;
        this.coe_speed = 0.25;


        //SpawnData data = new SpawnData(0, 20, 0.01, 3, 2.2);
        //general_Init(data);
    }
}
