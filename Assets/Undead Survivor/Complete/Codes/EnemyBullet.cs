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
                // �Ǽ��� �ƴ� ��, Ÿ���� ���� �� ���� ���
                // �ܼ��� ���� �������� �߻�
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
                // ������ ������ ���, Ÿ���� ���� �� ���� ��Ȳ
                return p.normalized;
            }

            Vector2 aimPoint = targetPosition + targetVelocity * t;
            Vector2 aimDirection = aimPoint - cannonPosition;

            return aimDirection.normalized;
        }
        [Header("Bullet Settings")]
        public float speed = 10f;          // �Ѿ� �ӵ�
        public float damage = 10;            // �Ѿ��� ������ ���ط�
        public float lifetime = 5f;        // �Ѿ� ���� �ð�
        public bool isLive = false;        // �Ѿ��� Ȱ�� ����;
        Action<float> action;
        bool ����������;
        private Rigidbody2D rb;

        /// <summary>
        /// �Ѿ��� Ȱ��ȭ�� �� ȣ��˴ϴ�.
        /// </summary>
        private void OnEnable()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// �Ѿ��� �ʱ�ȭ�մϴ�. �ܺο��� ȣ���Ͽ� �ӵ�, ���� �ð�, ������ ���� ������ �� �ֽ��ϴ�.
        /// </summary>
        /// <param name="master">�Ѿ��� �߻��� ���� ���� (���� ������ ����)</param>
        /// <param name="speed">�Ѿ��� �ӵ�</param>
        /// <param name="lifetime">�Ѿ��� ���� �ð�</param>
        /// <param name="damage">�Ѿ��� ������ ���ط�</param>
        public void Init(Action<float> action, float speed, float lifetime, float damage,bool ����������)
        {

            rb = GetComponent<Rigidbody2D>();
            this.action = action;
            this.speed = speed;
            this.lifetime = lifetime;
            this.damage = damage;
            isLive = true;
            this.���������� = ����������;
            // �÷��̾��� ���� ��ġ�� ���� �Ѿ��� ���� ����
            var target = GameManager.instance.player;
            Vector2 direction;
            if (����������)
            {
                direction=CalculateAimDirection(transform.position, target.transform.position, target.���������÷��̾�ӵ�, speed); //(GameManager.instance.player.transform.position - transform.position).normalized;

            }
            else
            {
                direction=(target.transform.position-transform.position).normalized;
            }
            rb.velocity = direction * speed;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            // ���� �ð��� ���� �� �Ѿ��� �ı�
            CancelInvoke("OnDead");
            Invoke("OnDead", lifetime);
        }

        /// <summary>
        /// �Ѿ��� �浹���� �� ȣ��˴ϴ�. �������� �����ϰ� �Ѿ��� �ı��մϴ�.
        /// </summary>
        /// <param name="damageAmount">������ ��������</param>
        
        /// <summary>
        /// �浹 �� ȣ��˴ϴ�. �÷��̾� �Ǵ� ���� �浹�ϸ� ������ ó���� �մϴ�.
        /// </summary>
        /// <param name="collision">�浹�� �ݶ��̴�</param>
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
        /// �Ѿ��� �ı��ϴ� �޼����Դϴ�.
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
