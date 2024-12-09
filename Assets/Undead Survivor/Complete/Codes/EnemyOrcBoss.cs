using System.Collections;
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
            Pattern3
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
            attack = 0;
            defence = 10;
            speed = 5;
            maxhealth = 1000;
            health = maxhealth;

            if (patternCoroutine != null)
                StopCoroutine(patternCoroutine);
            patternCoroutine = StartCoroutine(PatternCycle());

            Debug.Log($"여기는 init {GameManager.instance.EnemyNum++}");
        }

        protected void FixedUpdate()
        {
            if (!GameManager.instance.isLive)
                return;
            if (!isLive)
                return;

            Vector2 dirVec = target.position - rigid.position;
            float distance = dirVec.magnitude;

            if (distance > 0.1f && !IsInHitAnimation())
            {
                Debug.Log("asdasdsadasd");
                Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
                rigid.MovePosition(rigid.position + nextVec);
                anim.SetFloat("Speed", speed);
            }
            else
            {
                anim.SetFloat("Speed", 0);
                rigid.velocity = Vector2.zero;

                if (IsInHitAnimation() && distance > attackDistance)
                {
                    CancelHitAnimation();
                }
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

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isLive)
                return;
            Debug.Log(collision.tag);

            if (collision.CompareTag("Bullet"))
            {
                float 피해량 = collision.GetComponent<Bullet>().damage - defence;
                StartCoroutine(KnockBack(피해량 / maxhealth));
                health -= 피해량;

                if (health > 0)
                {
                    anim.SetTrigger("Hit");
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
                }
                else
                {
                    HandleDeath();
                }
            }
            if (collision.CompareTag("Player"))
            {

                Pattern currentPattern = (Pattern)(currentPatternIndex % 3);
                if (currentPattern == Pattern.Pattern1)
                {
                    anim.SetTrigger("Hit1");
                }
                else if (currentPattern == Pattern.Pattern2)
                {
                    anim.SetTrigger("Hit2");
                }
                else if (currentPattern == Pattern.Pattern3)
                {
                    anim.SetFloat("Speed", 0); // Stand 상태
                }

                AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
                DealDamage();
            }
        }

        protected void HandleDeath()
        {
            Debug.Log($"여기는 OnTriggerEnter2D {GameManager.instance.EnemyNum--}");
            GameManager.instance.CoinManager.DropCoins(transform.position);
            GameManager.instance.ManaManager.DropManas(transform.position);
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.speed = 1;
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp();

            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }

        public void OnAttack(float damage)
        {
            if (!isLive) return;
            Debug.Log($"{damage}만큼의 데미지를 플레이어가 받음");
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
                    case Pattern.Pattern2:
                        yield return StartCoroutine(ExecutePattern2());
                        break;
                    case Pattern.Pattern3:
                        yield return StartCoroutine(ExecutePattern3());
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
                    
        private bool IsInHitAnimation()
        {
            return anim.GetCurrentAnimatorStateInfo(0).IsName("HitEnemy Orc1") || anim.GetCurrentAnimatorStateInfo(0).IsName("HitEnemy Orc2");
        }

        private void CancelHitAnimation()
        {
            anim.SetFloat("Speed", 0);
            anim.ResetTrigger("Hit1");
            anim.ResetTrigger("Hit2");
        }

        public void DealDamage()
        {
            if (!isLive) return;
            GameManager.instance.player.OnBeat(OnAttack,attack);
        }
    }
}