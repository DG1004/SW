using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    // 플레이어의 현재 코인 수
    public static int playerCoins = 0;
    public GameObject coinPrefab;


    private void Update()
    {
        if(Input.GetKey(KeyCode.V))
        {
            Vector2 pos = GameManager.instance.player.transform.position;
            pos.y += 5.0f;
            DropCoins(pos);
        }
    }

    public void DropCoins(Vector2 dropPosition)
    {
        // 코인 인스턴스 생성
        GameObject coin = Instantiate(coinPrefab, dropPosition, Quaternion.identity);
    }
}
