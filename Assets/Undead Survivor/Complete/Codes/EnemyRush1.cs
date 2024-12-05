using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRush1 : Enemy
{
    //float time = 0; �Ⱦ��δٴ� ���� ����
    public float fireRate = 10f;        // �߻� ���� (�� ����)
    private float nextFireTime = 0f;   // ���� �߻� �ð�
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
            dirVec = EnemyBullet.CalculateAimDirection(transform.position, target.position, GameManager.instance.player.���������÷��̾�ӵ�, multiplyRate * speed);
        }
    }

    void RushEnd()
    {
        coll.isTrigger = false;
        anim.speed = 1;
    }
  
    protected override void race_init()//�������̵� �ʱ� ���Ȱ���
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
