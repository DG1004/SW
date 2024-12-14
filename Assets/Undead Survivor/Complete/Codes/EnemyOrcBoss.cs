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

            Debug.Log($"여기는 init {GameManager.instance.EnemyNum++}");
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
                    anim.SetFloat("Speed", 0); // Stand 상태
                }
                else if (currentPattern == Pattern.Pattern2)
                {
                    anim.SetFloat("Speed", 0); // Stand 상태
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
                float 피해량 = collision.GetComponent<Bullet>().damage - defence;
                StartCoroutine(KnockBack(피해량 / maxhealth));
                health -= 피해량;

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
            // 로그 출력
            Debug.Log("몬스터 총알 생성");

            // 풀에서 총알 객체 가져오기 (풀 관리 방식에 따라 다를 수 있음)
            GameObject bullet = GameManager.instance.pool.Get_Enemy(4);

            // 기본 위치 설정 (몬스터의 현재 위치)
            Vector3 basePosition = target.position;

            // 고정된 거리 설정 (총알이 생성될 반경)
            float fixedDistance = 10f;

            // 무작위 각도 생성 (0도 ~ 360도)
            float randomAngle = Random.Range(0f, 360f);

            // 각도를 라디안으로 변환
            float angleRad = randomAngle * Mathf.Deg2Rad;

            // X와 Y 오프셋 계산
            float offsetX = Mathf.Cos(angleRad) * fixedDistance;
            float offsetY = Mathf.Sin(angleRad) * fixedDistance;

            // 새로운 위치 계산 (2D이므로 Z는 0으로 설정)
            Vector3 spawnPosition = basePosition + new Vector3(offsetX, offsetY, 0);

            // 총알의 위치 설정
            bullet.transform.position = spawnPosition;

            // 총알 초기화 (필요한 파라미터로 초기화)
            bullet.GetComponent<EnemyBullet>().Init(OnAttack, 6, 7, 3,false);


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
            GameManager.instance.GameVictroy();

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