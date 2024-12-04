using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;



namespace Goldmetal.UndeadSurvivor
{

    public class Enemy : MonoBehaviour
    {
        SpawnData spawnData;

        float energy;//���� ������

        //������ �������� Ư���� �����ϴ� ������
        protected double coe_attack;
        protected double coe_defence;
        protected double coe_speed;
        protected double coe_health;
        protected int race_index;

        //������ �������ȵ�
        public float attack;
        float defence;
        float speed;
        float health;

        bool isLive;

        //public float maxHealth;
        public RuntimeAnimatorController[] animCon;
        public Rigidbody2D target;
        
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
                Debug.Log($"����� OnTriggerEnter2D {GameManager.instance.EnemyNum--}");
                GameManager.instance.CoinManager.DropCoins(transform.position);
                GameManager.instance.ManaManager.DropManas(transform.position);
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
        protected void Init(SpawnData data)
        {
            race_init();
            float k = 1;//Mathf.Log(Mathf.Log(GameManager.instance.gameTime+2.71f)+1.71f);
            //anim.runtimeAnimatorController = animCon[data];
            energy = 0;
            spawnData = data;
            attack = (float)(k * data.stats_attack);
            defence = (float)(k * data.stats_defence);
            health = (float)(k * data.stats_health);
            speed = (float)(data.stats_speed);
            /* attack = (float)(k * data.stats_attack * data.stats_health);
             defence = (float)(k * data.stats_defence * data.stats_health);
             health = (float)(k * data.stats_health);
             speed = (float)(data.stats_speed / data.stats_health);*/
            /*Debug.Log($"���ݴ� --> {attack}");
            Debug.Log($"���� --> {defence}");
            Debug.Log($"ü�´� --> {health}");
            Debug.Log($"�ӵ��� --> {speed}");*/
            transform.localScale = new Vector3(defence, health / 10 , 1);
            rigid.mass = defence * health * 0.1f;
            InvokeRepeating("energr_updater", Random.Range(1f,5f), 5f);
            Debug.Log($"����� init {GameManager.instance.EnemyNum++}");
        }
        public void Init()
        {
            race_init();
            Init(new SpawnData(coe_attack, coe_defence, coe_speed, coe_health));
        }
        protected virtual void race_init() { }

       
        private bool _lock=false;
        void energr_updater() 
        {
            if (!isLive||!GameManager.instance.isLive) return;
            energy += speed *100/ (GameManager.instance.EnemyNum);//���� ���� ���� ���������� �����ϱ� ���ؼ�
            if(!_lock) TryReproduce();
        }
        public void OnAttack(float damage)
        {
            if (!isLive) return;
            energy += 5 * damage;
            _lock = true;
            TryReproduce();
            _lock = false;
        }
        void TryReproduce()
        {
            while (energy > health)
            {
                Vector2 dirVec = target.position - rigid.position;
                var newPos = (Vector3)(target.position + dirVec.normalized * 20f);
                Reproduce(newPos);
                energy -= health;
            }
        }
        SpawnData MakeMutation()
        {
            return new SpawnData(spawnData, coe_attack, coe_defence, coe_speed, coe_health);
        }
        void Reproduce(Vector3 newPos)
        {
            // Ǯ���� ���ο� �� ������Ʈ�� �����ɴϴ�.
            GameObject enemy_object = GameManager.instance.pool.Get_Enemy(race_index);
            // ���� ��ġ�� �θ� ��ó�� �����մϴ�.
            enemy_object.transform.position = newPos;

            // ���� �ʱ�ȭ�մϴ�.
            var enemy = enemy_object.GetComponent<Enemy>();
            enemy.Init(MakeMutation());
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


namespace prev
{
    /*public class Enemy : MonoBehaviour
    {
        public float speed;
        public float health;
        public float maxHealth;
        public float energy; // ������ �ʵ� �߰�
        public SpawnData spawnData; // �ڽ��� spawnData ����
        public RuntimeAnimatorController[] animCon;
        public Rigidbody2D target;

        // ũ�� ������ ���� ����
        public float baseScale = 1.0f; // �⺻ ������ ��
        public float scaleMultiplier = 0.05f; // health �� ������ ������
        public float minScale = 0.5f; // �ּ� ������
        public float maxScale = 2.0f; // �ִ� ������

        bool isLive;

        Rigidbody2D rigid;
        Collider2D coll;
        Animator anim;
        SpriteRenderer spriter;
        WaitForFixedUpdate wait;

        // ���� Ÿ�̸� ���� �߰�
        private float attackTimer = 0f;
        public float attackCooldown = 1f; // ���� ������ 1�ʷ� ����

        // �ݶ��̴� ũ�� ������ ���� ����
        CircleCollider2D circleCollider;
        float originalColliderRadius;

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            coll = GetComponent<Collider2D>();
            anim = GetComponent<Animator>();
            spriter = GetComponent<SpriteRenderer>();
            wait = new WaitForFixedUpdate();

            // CircleCollider2D�� �ִ��� Ȯ���ϰ�, ���� ������ ����
            circleCollider = GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                originalColliderRadius = circleCollider.radius;
            }
            else
            {
                Debug.LogWarning("Enemy ������Ʈ�� CircleCollider2D�� �����ϴ�. Collider ũ�� ������ ���� �߰����ּ���.");
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
            energy = 0; // ������ �ʱ�ȭ
            attackTimer = 0f; // ���� Ÿ�̸� �ʱ�ȭ
        }

        public void Init(SpawnData data)
        {
            spawnData = data; // �ڽ��� spawnData ����
            anim.runtimeAnimatorController = animCon[data.spriteType];
            speed = data.speed;
            maxHealth = data.health;
            health = data.health;

            // health�� ����Ͽ� ũ�� ����
            float scaleValue = baseScale + (data.health * scaleMultiplier);
            scaleValue = Mathf.Clamp(scaleValue, minScale, maxScale);
            transform.localScale = new Vector3(scaleValue, scaleValue, 1);

            // �ݶ��̴� ũ�� ���� (CircleCollider2D�� ���)
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
            Debug.Log($"������---{collision.collider.tag}");
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
                    attackTimer = 0f; // ���� Ÿ�̸� �ʱ�ȭ

                    // �÷��̾�� �������� �ݴϴ�.
                    energy += 25;

                    // �÷��̾�� ������
                    Player player = collision.collider.GetComponent<Player>();
                    if (player != null)
                    {
                        player.OnDamaged(0.01f); // �ʿ信 ���� ������ �� ����
                    }

                    // �������� 100 �̻��̸� ����
                    if (energy >= this.health / 4.0f)
                    {
                        Reproduce();
                        energy -= this.health / 4.0f; // ���� �� ������ �ʱ�ȭ
                    }
                }
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                attackTimer = 0f; // �÷��̾ ������ ����� ���� Ÿ�̸� �ʱ�ȭ
            }
        }

        IEnumerator KnockBack()
        {
            yield return wait; // ���� ���� �����ӱ��� ���
            Vector3 playerPos = GameManager.instance.player.transform.position;
            Vector3 dirVec = transform.position - playerPos;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }

        void Reproduce()
        {
            Debug.Log("����!!!!!!!!!!");
            // Ǯ���� ���ο� �� ������Ʈ�� �����ɴϴ�.
            GameObject enemy = GameManager.instance.pool.Get(0);

            // ���� ��ġ�� �θ� ��ó�� �����մϴ�.
            enemy.transform.position = transform.position + (Vector3)(Random.insideUnitCircle.normalized * 10f);

            // �ణ�� ���������� ���� ���ο� spawnData�� �����մϴ�.
            SpawnData newSpawnData = new SpawnData();
            newSpawnData.spriteType = spawnData.spriteType; // �θ��� spriteType�� ���
            newSpawnData.health = Mathf.Max(1, spawnData.health + Random.Range(-20, 20)); // ü���� �ణ ����
            newSpawnData.speed = Mathf.Max(0.1f, spawnData.speed + Random.Range(-0.5f, 0.6f)); // �ӵ��� �ణ ����

            // ���� �ʱ�ȭ�մϴ�.
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
