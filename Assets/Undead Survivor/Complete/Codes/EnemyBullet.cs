using Goldmetal.UndeadSurvivor;
using System;
using UnityEngine;


namespace Goldmetal.UndeadSurvivor
{
    public class EnemyBullet : MonoBehaviour
    {
        public static Vector2 CalculateAimDirection(Vector2 cannonPosition, Vector2 targetPosition, Vector2 targetVelocity, float bulletSpeed)
        {
            Vector2 p = targetPosition - cannonPosition;
            float a = Vector2.Dot(targetVelocity, targetVelocity) - bulletSpeed * bulletSpeed;
            float b = 2 * Vector2.Dot(p, targetVelocity);
            float c = Vector2.Dot(p, p);

            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
            {
                // 실수가 아닌 해, 타겟을 맞출 수 없는 경우
                // 단순히 현재 방향으로 발사
                return p.normalized;
            }

            float sqrtDiscriminant = Mathf.Sqrt(discriminant);

            float t1 = (-b + sqrtDiscriminant) / (2 * a);
            float t2 = (-b - sqrtDiscriminant) / (2 * a);

            float t = Mathf.Min(t1, t2);
            if (t < 0)
            {
                t = Mathf.Max(t1, t2);
            }

            if (t < 0)
            {
                // 여전히 음수인 경우, 타겟을 맞출 수 없는 상황
                return p.normalized;
            }

            Vector2 aimPoint = targetPosition + targetVelocity * t;
            Vector2 aimDirection = aimPoint - cannonPosition;

            return aimDirection.normalized;
        }
        [Header("Bullet Settings")]
        public float speed = 10f;          // 총알 속도
        public float damage = 10;            // 총알이 입히는 피해량
        public float lifetime = 5f;        // 총알 생존 시간
        public bool isLive = false;        // 총알의 활성 상태;
        Action<float> action;
        bool 예측샷여부;
        private Rigidbody2D rb;

        /// <summary>
        /// 총알이 활성화될 때 호출됩니다.
        /// </summary>
        private void OnEnable()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// 총알을 초기화합니다. 외부에서 호출하여 속도, 생존 시간, 데미지 등을 설정할 수 있습니다.
        /// </summary>
        /// <param name="master">총알을 발사한 적의 정보 (현재 사용되지 않음)</param>
        /// <param name="speed">총알의 속도</param>
        /// <param name="lifetime">총알의 생존 시간</param>
        /// <param name="damage">총알이 입히는 피해량</param>
        public void Init(Action<float> action, float speed, float lifetime, float damage,bool 예측샷여부)
        {

            rb = GetComponent<Rigidbody2D>();
            this.action = action;
            this.speed = speed;
            this.lifetime = lifetime;
            this.damage = damage;
            isLive = true;
            this.예측샷여부 = 예측샷여부;
            // 플레이어의 현재 위치를 향해 총알의 방향 설정
            var target = GameManager.instance.player;
            Vector2 direction;
            if (예측샷여부)
            {
                direction=CalculateAimDirection(transform.position, target.transform.position, target.예측샷용플레이어속도, speed); //(GameManager.instance.player.transform.position - transform.position).normalized;

            }
            else
            {
                direction=(target.transform.position-transform.position).normalized;
            }
            rb.velocity = direction * speed;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            // 일정 시간이 지난 후 총알을 파괴
            CancelInvoke("OnDead");
            Invoke("OnDead", lifetime);
        }

        /// <summary>
        /// 총알이 충돌했을 때 호출됩니다. 데미지를 적용하고 총알을 파괴합니다.
        /// </summary>
        /// <param name="damageAmount">적용할 데미지량</param>
        
        /// <summary>
        /// 충돌 시 호출됩니다. 플레이어 또는 벽과 충돌하면 적절한 처리를 합니다.
        /// </summary>
        /// <param name="collision">충돌한 콜라이더</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                GameManager.instance.player.OnBeat(action, damage);
                OnDead();
            }
            if (collision.CompareTag("Bullet"))
            {
                OnDead();
            }

        }

        /// <summary>
        /// 총알을 파괴하는 메서드입니다.
        /// </summary>
        private void OnDead()
        {
            if (isLive)
            {
                isLive = false;
                gameObject.SetActive(false);
            }
        }
    }

}
