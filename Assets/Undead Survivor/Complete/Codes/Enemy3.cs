using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;



namespace Goldmetal.UndeadSurvivor
{
    
    public class Enemy3 : Enemy
    {



        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            coll = GetComponent<Collider2D>();
            anim = GetComponent<Animator>();
            spriter = GetComponent<SpriteRenderer>();
            wait = new WaitForFixedUpdate();
            Init(new SpawnData(1));
        }


        void FixedUpdate()
        {

            target = GameManager.instance.player.GetComponent<Rigidbody2D>();
            if (!GameManager.instance.isLive)
                return;

            if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
                return;

            Vector2 dirVec = target.position - rigid.position;
            Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
            rigid.velocity = Vector2.zero;
        }

        void LateUpdate()
        {
            if (!GameManager.instance.isLive)
                return;

            if (!isLive)
                return;

            spriter.flipX = target.position.x < rigid.position.x;
        }

        void OnEnable()
        {
            target = GameManager.instance.player.GetComponent<Rigidbody2D>();
            isLive = true;
            coll.enabled = true;
            rigid.simulated = true;
            spriter.sortingOrder = 2;
            anim.SetBool("Dead", false);
            //health = maxHealth;
        }

        public void Init(SpawnData data)
        {
            float k = 1;//Mathf.Log(GameManager.instance.gameTime);
            //anim.runtimeAnimatorController = animCon[data.spriteType];
            energy = 0;
            spawnData = data;
            attack = (float)(k * data.stats_attack * data.stats_health);
            defence = (float)(k * data.stats_defence * data.stats_health);
            health = (float)(k * data.stats_health);
            speed = (float)(data.stats_speed / data.stats_health);
            Debug.Log($"���ݴ� --> {attack}");
            Debug.Log($"���� --> {defence}");
            Debug.Log($"ü�´� --> {health}");
            Debug.Log($"�ӵ��� --> {speed}");
            transform.localScale = new Vector3(defence / 3, health / 10 / 3, 1);
            rigid.mass = defence * health * 0.1f;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Bullet") || !isLive)
                return;

            health -= collision.GetComponent<Bullet>().damage - defence;
            StartCoroutine(KnockBack());

            if (health > 0)
            {
                anim.SetTrigger("Hit");
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
            }
            else
            {
                isLive = false;
                coll.enabled = false;
                rigid.simulated = false;
                spriter.sortingOrder = 1;
                anim.SetBool("Dead", true);
                GameManager.instance.kill++;
                GameManager.instance.GetExp();

                if (GameManager.instance.isLive)
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
            }
        }
        public void OnAttack(float damage)
        {
            if (!isLive) return;
            energy += 2 * damage;
            while (energy > health)
            {
                Reproduce();
                energy -= health;
            }

        }
        void Reproduce()
        {
            Debug.Log("����!!!!!!!!!!");
            // Ǯ���� ���ο� �� ������Ʈ�� �����ɴϴ�.
            GameObject enemy = GameManager.instance.pool.Get(spawnData.race_index);

            // ���� ��ġ�� �θ� ��ó�� �����մϴ�.
            enemy.transform.position = transform.position + (Vector3)(Random.insideUnitCircle.normalized * 10f);

            // �ణ�� ���������� ���� ���ο� spawnData�� �����մϴ�.
            SpawnData newSpawnData = new SpawnData(spawnData);
            // ���� �ʱ�ȭ�մϴ�.
            enemy.GetComponent<Enemy>().Init(newSpawnData);
        }
        IEnumerator KnockBack()
        {
            yield return wait; // ���� �ϳ��� ���� ������ ������
            Vector3 playerPos = GameManager.instance.player.transform.position;
            Vector3 dirVec = transform.position - playerPos;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }

        void Dead()
        {
            gameObject.SetActive(false);
        }
    }
}
