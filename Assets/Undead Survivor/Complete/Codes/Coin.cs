using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
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
}