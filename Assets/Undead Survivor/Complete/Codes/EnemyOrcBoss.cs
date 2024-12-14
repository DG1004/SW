using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Goldmetal.UndeadSurvivor
{
    public class EnemyOrcBoss : MonoBehaviour
    {
        public float attack;
        float defence;
        protected float speed;
        float health;
        protected float maxhealth;
        protected bool isLive;

        public RuntimeAnimatorController[] animCon;
        public Rigidbody2D target;

        protected Rigidbody2D rigid;
        protected Collider2D coll;
        protected Animator anim;
        protected SpriteRenderer spriter;
        protected WaitForFixedUpdate wait;

        private enum Pattern
        {
            Pattern1,
            Pattern2,
            Pattern3,
            Pattern4,

        }

        private int currentPatternIndex = 0;
        private Coroutine patternCoroutine;

        public float hitDamage = 10f;
        public float attackDistance = 5f;

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            coll = GetComponent<Collider2D>();
            anim = GetComponent<Animator>();
            spriter = GetComponent<SpriteRenderer>();
            wait = new WaitForFixedUpdate();
        }

        void OnEnable()
        {
            target = GameManager.instance.player.GetComponent<Rigidbody2D>();
            isLive = true;
            coll.enabled = true;
            rigid.simulated = true;
            spriter.sortingOrder = 2;
            anim.SetBool("Dead", false);

            Init();
        }

        protected void Init()
        {
            attack = 15;
            defence = 10;
            speed = 5;
            maxhealth = 3500;
            health = maxhealth;

            if (patternCoroutine != null)
                StopCoroutine(patternCoroutine);
            patternCoroutine = StartCoroutine(PatternCycle());

            Debug.Log($"����� init {GameManager.instance.EnemyNum++}");
        }

        float StopTime=0;
        protected void FixedUpdate()
        {
            if (!GameManager.instance.isLive)
                return;
            if (!isLive)
                return;

            Vector2 dirVec = target.position - rigid.position;
            float distance = dirVec.magnitude;
            if (currentPatternIndex % 4 == 2&&Time.time>StopTime)
            {
                Shoot();
                StopTime = Time.time + 0.05f;
            }
            else if (currentPatternIndex % 4 == 1 && Time.time > StopTime)
            {
                GameManager.instance.EnemyNum = 10000;
                GameManager.instance.player.GetComponentInChildren<Spawner>().SpawnInitialEnemies();
                StopTime = Time.time + 3f;
            }
            else if (distance > 0.1f && !IsInHitAnimation())
            {
                if (Time.time > StopTime)
                {
                    speed = distance + 3f;
                    Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
                    rigid.MovePosition(rigid.position + nextVec);
                    anim.SetFloat("Speed", speed);
                }

            }
            else
            {
                anim.SetFloat("Speed", 0);
                rigid.velocity = Vector2.zero;
                StopTime = Time.time+1.5f;

            }
        }

        void LateUpdate()
        {
            if (!GameManager.instance.isLive)
                return;
            if (!isLive)
                return;
            spriter.flipX = target.position.x < rigid.position.x;
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!isLive)
                return;
            if (IsInHitAnimation())
            {
                CancelHitAnimation();
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!isLive)
                return;
            if (collision.CompareTag("Player") && !IsInHitAnimation())
            {

                Pattern currentPattern = (Pattern)(currentPatternIndex % 4);
                if (currentPattern == Pattern.Pattern1)
                {
                    anim.SetTrigger("Hit1");
                }
                else if (currentPattern == Pattern.Pattern4)
                {
                    anim.SetTrigger("Hit2");
                }
                else if (currentPattern == Pattern.Pattern3)
                {
                    anim.SetFloat("Speed", 0); // Stand ����
                }
                else if (currentPattern == Pattern.Pattern2)
                {
                    anim.SetFloat("Speed", 0); // Stand ����
                }

                AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
                //  DealDamage();
            }
        }
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isLive)
                return;
            Debug.Log(collision.tag);

            if (collision.CompareTag("Bullet"))
            {
                float ���ط� = collision.GetComponent<Bullet>().damage - defence;
                StartCoroutine(KnockBack(���ط� / maxhealth));
                health -= ���ط�;

                if (health > 0)
                {
                    anim.SetTrigger("OnHit");
                    GameManager.instance.ManaManager.DropManas(transform.position);
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
                }
                else
                {
                    HandleDeath();
                }
            }

        }
        float fixedDistance = 10f;
        void Shoot()
        {
            // �α� ���
            Debug.Log("���� �Ѿ� ����");

            // Ǯ���� �Ѿ� ��ü �������� (Ǯ ���� ��Ŀ� ���� �ٸ� �� ����)
            GameObject bullet = GameManager.instance.pool.Get_Enemy(4);

            // �⺻ ��ġ ���� (������ ���� ��ġ)
            Vector3 basePosition = target.position;

            // ������ �Ÿ� ���� (�Ѿ��� ������ �ݰ�)
            float fixedDistance = 10f;

            // ������ ���� ���� (0�� ~ 360��)
            float randomAngle = Random.Range(0f, 360f);

            // ������ �������� ��ȯ
            float angleRad = randomAngle * Mathf.Deg2Rad;

            // X�� Y ������ ���
            float offsetX = Mathf.Cos(angleRad) * fixedDistance;
            float offsetY = Mathf.Sin(angleRad) * fixedDistance;

            // ���ο� ��ġ ��� (2D�̹Ƿ� Z�� 0���� ����)
            Vector3 spawnPosition = basePosition + new Vector3(offsetX, offsetY, 0);

            // �Ѿ��� ��ġ ����
            bullet.transform.position = spawnPosition;

            // �Ѿ� �ʱ�ȭ (�ʿ��� �Ķ���ͷ� �ʱ�ȭ)
            bullet.GetComponent<EnemyBullet>().Init(OnAttack, 6, 7, 3,false);


        }
        protected void HandleDeath()
        {
            Debug.Log($"����� OnTriggerEnter2D {GameManager.instance.EnemyNum--}");
            GameManager.instance.CoinManager.DropCoins(transform.position);
            GameManager.instance.ManaManager.DropManas(transform.position);
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.speed = 1;
            anim.SetBool("Dead", true);
            GameManager.instance.GameVictroy();

            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }

        public void OnAttack(float damage)
        {
            if (!isLive) return;
            Debug.Log($"{damage}��ŭ�� �������� �÷��̾ ����");
        }

        IEnumerator KnockBack(float damageRate)
        {
            yield return wait;
            Vector3 playerPos = GameManager.instance.player.transform.position;
            Vector3 dirVec = transform.position - playerPos;
            rigid.AddForce(dirVec.normalized * damageRate * 6, ForceMode2D.Impulse);
        }

        void Dead()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator PatternCycle()
        {
            while (isLive)
            {
                Pattern currentPattern = (Pattern)(currentPatternIndex % 3);
                switch (currentPattern)
                {
                    case Pattern.Pattern1:
                        yield return StartCoroutine(ExecutePattern1());
                        break;
                    case Pattern.Pattern4:
                        yield return StartCoroutine(ExecutePattern2());
                        break;
                    case Pattern.Pattern3:
                        yield return StartCoroutine(ExecutePattern3());
                        break;
                    case Pattern.Pattern2:
                        yield return StartCoroutine(ExecutePattern4());
                        break;
                }
                currentPatternIndex++;
                yield return new WaitForSeconds(1f);
            }
        }

        private IEnumerator ExecutePattern1()
        {
            anim.SetFloat("Speed", 0);
            yield return new WaitForSeconds(1f);

            anim.SetFloat("Speed", speed);
            yield return new WaitForSeconds(1f);

            yield return new WaitUntil(() => Vector2.Distance(target.position, rigid.position) > attackDistance);

            anim.SetFloat("Speed", 0);
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator ExecutePattern2()
        {
            anim.SetFloat("Speed", 0);
            yield return new WaitForSeconds(1f);

            anim.SetFloat("Speed", speed);
            yield return new WaitForSeconds(1f);

            yield return new WaitUntil(() => Vector2.Distance(target.position, rigid.position) > attackDistance);

            anim.SetFloat("Speed", 0);
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator ExecutePattern3()
        {
            anim.SetFloat("Speed", 0);
            yield return new WaitForSeconds(1f);

            anim.SetFloat("Speed", speed);
            yield return new WaitForSeconds(1f);

            yield return new WaitUntil(() => Vector2.Distance(target.position, rigid.position) > attackDistance);

            anim.SetFloat("Speed", 0);
            yield return new WaitForSeconds(10f);
        }
        private IEnumerator ExecutePattern4()
        {
            anim.SetFloat("Speed", 0);
            yield return new WaitForSeconds(1f);

            anim.SetFloat("Speed", speed);
            yield return new WaitForSeconds(1f);

            yield return new WaitUntil(() => Vector2.Distance(target.position, rigid.position) > attackDistance);

            anim.SetFloat("Speed", 0);
            yield return new WaitForSeconds(10f);
        }

        private bool IsInHitAnimation()
        {
            return anim.GetCurrentAnimatorStateInfo(0).IsName("HitEnemy Orc1") || anim.GetCurrentAnimatorStateInfo(0).IsName("HitEnemy Orc2");
        }

        private void CancelHitAnimation()
        {
            anim.SetFloat("Speed", 0);
            anim.ResetTrigger("Hit1");
            anim.ResetTrigger("Hit2");
            anim.SetTrigger("FinishPattern");
        }

        public void DealDamage()
        {
            if (!isLive) return;
            GameManager.instance.player.OnBeat(OnAttack, attack);
        }
    }
}