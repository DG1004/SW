using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;



namespace Goldmetal.UndeadSurvivor
{

    public class Enemy_Parent : MonoBehaviour
    {
        protected SpawnData spawnData;

        protected float energy;

        public float attack;
        public float defence;
        public float speed;
        public float health;
        //public float maxHealth;
        public RuntimeAnimatorController[] animCon;
        public Rigidbody2D target;

        protected bool isLive;

        protected Rigidbody2D rigid;
        protected Collider2D coll;
        protected Animator anim;
        protected SpriteRenderer spriter;
        protected WaitForFixedUpdate wait;


        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            coll = GetComponent<Collider2D>();
            anim = GetComponent<Animator>();
            spriter = GetComponent<SpriteRenderer>();
            wait = new WaitForFixedUpdate();
        }

        void FixedUpdate()
        {
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
            float k = 1;//Mathf.Log(Mathf.Log(GameManager.instance.gameTime+2.71f)+1.71f);
            //anim.runtimeAnimatorController = animCon[data];
            energy = 0;
            spawnData = data;
            attack = (float)(k * data.stats_attack * data.stats_health);
            defence = (float)(k * data.stats_defence * data.stats_health);
            health = (float)(k * data.stats_health);
            speed = (float)(data.stats_speed / data.stats_health);
            /*Debug.Log($"공격는 --> {attack}");
            Debug.Log($"방어는 --> {defence}");
            Debug.Log($"체력는 --> {health}");
            Debug.Log($"속도는 --> {speed}");*/
            transform.localScale = new Vector3(defence / 3, health / 10 / 3, 1);
            rigid.mass = defence * health * 0.1f;
            InvokeRepeating("energr_updater", 0f, 5f);
            Debug.Log($"여기는 init {GameManager.instance.EnemyNum++}");
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
                Debug.Log($"여기는 OnTriggerEnter2D {GameManager.instance.EnemyNum--}");
                GameManager.instance.CoinManager.DropCoins(transform.position);
                CancelInvoke("energr_updater");
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
        public void energr_updater()
        {
            if (!isLive||!GameManager.instance.isLive) return;
            energy += health * (30 / GameManager.instance.EnemyNum);//몬스터 수에 따라 유동적으로 조정하기 위해서
            OnAttack(0);
        }
        public void OnAttack(float damage)
        {
            if (!isLive) return;
            energy += 10 * damage;
            while (energy > health)
            {
                Reproduce();
                energy -= health;
            }

        }

        void Reproduce()
        {
            // 풀에서 새로운 적 오브젝트를 가져옵니다.
            GameObject enemy = GameManager.instance.pool.Get(spawnData.race_index);

            // 적의 위치를 부모 근처로 설정합니다.
            enemy.transform.position = transform.position;// + (Vector3)(Random.insideUnitCircle.normalized * 1f);

            // 약간의 무작위성을 가진 새로운 spawnData를 생성합니다.
            SpawnData newSpawnData = new SpawnData(spawnData);
            // 적을 초기화합니다.
            enemy.GetComponent<Enemy_Parent>().Init(newSpawnData);
        }
        IEnumerator KnockBack()
        {
            yield return wait; // 다음 하나의 물리 프레임 딜레이
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


namespace prev
{
    /*public class Enemy : MonoBehaviour
    {
        public float speed;
        public float health;
        public float maxHealth;
        public float energy; // 에너지 필드 추가
        public SpawnData spawnData; // 자신의 spawnData 저장
        public RuntimeAnimatorController[] animCon;
        public Rigidbody2D target;

        // 크기 조정을 위한 변수
        public float baseScale = 1.0f; // 기본 스케일 값
        public float scaleMultiplier = 0.05f; // health 당 스케일 증가량
        public float minScale = 0.5f; // 최소 스케일
        public float maxScale = 2.0f; // 최대 스케일

        bool isLive;

        Rigidbody2D rigid;
        Collider2D coll;
        Animator anim;
        SpriteRenderer spriter;
        WaitForFixedUpdate wait;

        // 공격 타이머 변수 추가
        private float attackTimer = 0f;
        public float attackCooldown = 1f; // 공격 간격을 1초로 설정

        // 콜라이더 크기 조정을 위한 변수
        CircleCollider2D circleCollider;
        float originalColliderRadius;

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            coll = GetComponent<Collider2D>();
            anim = GetComponent<Animator>();
            spriter = GetComponent<SpriteRenderer>();
            wait = new WaitForFixedUpdate();

            // CircleCollider2D가 있는지 확인하고, 원래 반지름 저장
            circleCollider = GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                originalColliderRadius = circleCollider.radius;
            }
            else
            {
                Debug.LogWarning("Enemy 오브젝트에 CircleCollider2D가 없습니다. Collider 크기 조정을 위해 추가해주세요.");
            }
        }

        void FixedUpdate()
        {
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
            health = maxHealth;
            energy = 0; // 에너지 초기화
            attackTimer = 0f; // 공격 타이머 초기화
        }

        public void Init(SpawnData data)
        {
            spawnData = data; // 자신의 spawnData 저장
            anim.runtimeAnimatorController = animCon[data.spriteType];
            speed = data.speed;
            maxHealth = data.health;
            health = data.health;

            // health에 비례하여 크기 조정
            float scaleValue = baseScale + (data.health * scaleMultiplier);
            scaleValue = Mathf.Clamp(scaleValue, minScale, maxScale);
            transform.localScale = new Vector3(scaleValue, scaleValue, 1);

            // 콜라이더 크기 조정 (CircleCollider2D인 경우)
            if (circleCollider != null)
            {
                circleCollider.radius = originalColliderRadius * scaleValue;
            }
        }
        *//**//*
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Bullet") || !isLive)
                return;

            health -= collision.GetComponent<Bullet>().damage;
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
        *//*void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isLive)
                return;
            Debug.Log($"히히히---{collision.collider.tag}");
            if (collision.collider.CompareTag("Bullet"))
            {
                health -= collision.collider.GetComponent<Bullet>().damage;
                StartCoroutine(KnockBack());

                if (health > 0)
                {
                    anim.SetTrigger("Hit");
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
                }
                else
                {
                    Die();
                }
            }
        }*//*
        public void OnDeal(float damage)
        {

        }

        void OnCollisionStay2D(Collision2D collision)
        {
            if (!isLive)
                return;

            if (collision.collider.CompareTag("Player"))
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackCooldown)
                {
                    attackTimer = 0f; // 공격 타이머 초기화

                    // 플레이어에게 데미지를 줍니다.
                    energy += 25;

                    // 플레이어에게 데미지
                    Player player = collision.collider.GetComponent<Player>();
                    if (player != null)
                    {
                        player.OnDamaged(0.01f); // 필요에 따라 데미지 값 조정
                    }

                    // 에너지가 100 이상이면 번식
                    if (energy >= this.health / 4.0f)
                    {
                        Reproduce();
                        energy -= this.health / 4.0f; // 번식 후 에너지 초기화
                    }
                }
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                attackTimer = 0f; // 플레이어가 범위를 벗어나면 공격 타이머 초기화
            }
        }

        IEnumerator KnockBack()
        {
            yield return wait; // 다음 물리 프레임까지 대기
            Vector3 playerPos = GameManager.instance.player.transform.position;
            Vector3 dirVec = transform.position - playerPos;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }

        void Reproduce()
        {
            Debug.Log("번식!!!!!!!!!!");
            // 풀에서 새로운 적 오브젝트를 가져옵니다.
            GameObject enemy = GameManager.instance.pool.Get(0);

            // 적의 위치를 부모 근처로 설정합니다.
            enemy.transform.position = transform.position + (Vector3)(Random.insideUnitCircle.normalized * 10f);

            // 약간의 무작위성을 가진 새로운 spawnData를 생성합니다.
            SpawnData newSpawnData = new SpawnData();
            newSpawnData.spriteType = spawnData.spriteType; // 부모의 spriteType을 상속
            newSpawnData.health = Mathf.Max(1, spawnData.health + Random.Range(-20, 20)); // 체력을 약간 변형
            newSpawnData.speed = Mathf.Max(0.1f, spawnData.speed + Random.Range(-0.5f, 0.6f)); // 속도를 약간 변형

            // 적을 초기화합니다.
            enemy.GetComponent<Enemy>().Init(newSpawnData);
        }

        void Die()
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

        void Dead()
        {
            gameObject.SetActive(false);
        }
    }*/
}
