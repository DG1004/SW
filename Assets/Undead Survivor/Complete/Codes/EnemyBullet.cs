using Goldmetal.UndeadSurvivor;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;          // 총알 속도
    public float damage = 10;            // 총알이 입히는 피해량
    public float lifetime = 5f;        // 총알 생존 시간
    public bool isLive = false;        // 총알의 활성 상태;
    Enemy master;

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
    public void Init(Enemy master, float speed, float lifetime, float damage)
    {
        rb = GetComponent<Rigidbody2D>();
        this.master = master;
        this.speed = speed;
        this.lifetime = lifetime;
        this.damage = damage;
        isLive = true;

        // 플레이어의 현재 위치를 향해 총알의 방향 설정
        Vector2 direction = (GameManager.instance.player.transform.position - transform.position).normalized;
        rb.velocity = direction * speed;

        // 일정 시간이 지난 후 총알을 파괴
        CancelInvoke("DestroyBullet");
        Invoke("DestroyBullet", lifetime);
    }

    /// <summary>
    /// 총알이 충돌했을 때 호출됩니다. 데미지를 적용하고 총알을 파괴합니다.
    /// </summary>
    /// <param name="damageAmount">적용할 데미지량</param>
    public void OnAttack(float damageAmount)
    {
        if (isLive)
        {
            master.OnAttack(damageAmount);
            DestroyBullet();
        }
    }

    /// <summary>
    /// 충돌 시 호출됩니다. 플레이어 또는 벽과 충돌하면 적절한 처리를 합니다.
    /// </summary>
    /// <param name="collision">충돌한 콜라이더</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.player.OnBeat(this,damage);
        }
        
    }

    /// <summary>
    /// 총알을 파괴하는 메서드입니다.
    /// </summary>
    private void DestroyBullet()
    {
        if (isLive)
        {
            isLive = false;
            Destroy(gameObject);
        }
    }
}
