using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    // 플레이어의 현재 코인 수
    public static int playerCoins = 3000;
    public GameObject coinPrefab;

    private void Update()
    {
        // 테스트용. v키로 코인 드랍
        if(Input.GetKey(KeyCode.V))
        {
            Vector2 pos = GameManager.instance.player.transform.position;
            pos.y += 5.0f;
            DropCoins(pos);
        }
    }

    // 코인을 드랍하는 함수
    // 몬스터가 죽을 때, 이 함수를 호출해서 사용하시면 돼요.
    public void DropCoins(Vector2 dropPosition)
    {
        // 코인 인스턴스 생성
        GameObject coin = Instantiate(coinPrefab, dropPosition, Quaternion.identity);
    }
}
