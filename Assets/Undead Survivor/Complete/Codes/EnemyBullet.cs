using Goldmetal.UndeadSurvivor;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;          // �Ѿ� �ӵ�
    public float damage = 10;            // �Ѿ��� ������ ���ط�
    public float lifetime = 5f;        // �Ѿ� ���� �ð�
    public bool isLive = false;        // �Ѿ��� Ȱ�� ����;
    Enemy master;

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
    public void Init(Enemy master, float speed, float lifetime, float damage)
    {
        rb = GetComponent<Rigidbody2D>();
        this.master = master;
        this.speed = speed;
        this.lifetime = lifetime;
        this.damage = damage;
        isLive = true;

        // �÷��̾��� ���� ��ġ�� ���� �Ѿ��� ���� ����
        Vector2 direction = (GameManager.instance.player.transform.position - transform.position).normalized;
        rb.velocity = direction * speed;

        // ���� �ð��� ���� �� �Ѿ��� �ı�
        CancelInvoke("DestroyBullet");
        Invoke("DestroyBullet", lifetime);
    }

    /// <summary>
    /// �Ѿ��� �浹���� �� ȣ��˴ϴ�. �������� �����ϰ� �Ѿ��� �ı��մϴ�.
    /// </summary>
    /// <param name="damageAmount">������ ��������</param>
    public void OnAttack(float damageAmount)
    {
        if (isLive)
        {
            master.OnAttack(damageAmount);
            DestroyBullet();
        }
    }

    /// <summary>
    /// �浹 �� ȣ��˴ϴ�. �÷��̾� �Ǵ� ���� �浹�ϸ� ������ ó���� �մϴ�.
    /// </summary>
    /// <param name="collision">�浹�� �ݶ��̴�</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.player.OnBeat(this,damage);
        }
        
    }

    /// <summary>
    /// �Ѿ��� �ı��ϴ� �޼����Դϴ�.
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
