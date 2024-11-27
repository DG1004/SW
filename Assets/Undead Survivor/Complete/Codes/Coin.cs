using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // 코인 1개의 값어치
    public int value;
    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어와 충돌 시 코인 수집
        if (other.CompareTag("Player"))
        {
            // 코인 매니저에 코인 추가
            CoinManager.playerCoins += value;

            // 코인 오브젝트 제거
            Destroy(gameObject);
        }
    }
}