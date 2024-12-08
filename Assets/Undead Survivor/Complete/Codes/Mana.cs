using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class Coin : MonoBehaviour
{
    // ���� 1���� ����ġ
    public int value;
    void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾�� �浹 �� ���� ����
        if (other.CompareTag("Player"))
        {
            // ���� �Ŵ����� ���� �߰�
            CoinManager.playerCoins += value;

            // ���� ������Ʈ ����
            Destroy(gameObject);
        }
    }
}*/
public class Mana : MonoBehaviour
{
    // ���� 1���� ����ġ
    public double value;
    private bool isLive;
    private bool isFollow;
    private Transform target;

    private float speed = 5f; // ���� �̵� �ӵ�

    void OnEnable()
    {
        if (GameManager.instance?.player == null)
        {
            Debug.LogError("GameManager�� �÷��̾� ������ �����ϴ�.");
            return;
        }
        target = GameManager.instance.player.transform;
        isLive = true;
        isFollow = false;
    }

    void Update()
    {
        if (!isLive || !isFollow)
            return;

        // �÷��̾� �������� �̵�
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    // ���� ���� �޼��� (�θ� ������Ʈ�� Collider���� ȣ��)
    public void Collect()
    {
        if (!isLive)
            return;
        if (ManaManager.playerManas < 100)
            ManaManager.playerManas += value;
        isLive = false;
        gameObject.SetActive(false); // ��Ȱ��ȭ
        Destroy(gameObject, 0.1f);   // ������ �� ������Ʈ ����
    }

    // ���� ���󰡱� ���� �޼��� (�ڽ� ������Ʈ�� ��ũ��Ʈ���� ȣ��)
    public void StartFollowing()
    {
        isFollow = true;
    }

    // �θ� ������Ʈ�� Collider �̺�Ʈ
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }
}