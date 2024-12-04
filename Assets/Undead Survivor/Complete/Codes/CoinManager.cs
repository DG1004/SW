using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    // �÷��̾��� ���� ���� ��
    public static int playerCoins = 3000;
    public GameObject coinPrefab;

    private void Update()
    {
        // �׽�Ʈ��. vŰ�� ���� ���
        if(Input.GetKey(KeyCode.V))
        {
            Vector2 pos = GameManager.instance.player.transform.position;
            pos.y += 5.0f;
            DropCoins(pos);
        }
    }

    // ������ ����ϴ� �Լ�
    // ���Ͱ� ���� ��, �� �Լ��� ȣ���ؼ� ����Ͻø� �ſ�.
    public void DropCoins(Vector2 dropPosition)
    {
        // ���� �ν��Ͻ� ����
        GameObject coin = Instantiate(coinPrefab, dropPosition, Quaternion.identity);
    }
}
