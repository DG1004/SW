using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    // �÷��̾��� ���� ���� ��
    public static int playerManas = 100;
    public static int maxManas = 100;

    public GameObject manaPrefab;

    private void Update()
    {
        // �׽�Ʈ��. vŰ�� ���� ���
        if (Input.GetKey(KeyCode.C))
        {
            Vector2 pos = GameManager.instance.player.transform.position;
            pos.y += 5.0f;
            DropManas(pos);
        }
        if (playerManas < maxManas)
        {
            playerManas = Mathf.Clamp(playerManas, 0, maxManas);
        }
    }

    // ������ ����ϴ� �Լ�
    // ���Ͱ� ���� ��, �� �Լ��� ȣ���ؼ� ����Ͻø� �ſ�.
    public void DropManas(Vector2 dropPosition)
    {
        // ���� �ν��Ͻ� ����
        GameObject mana = Instantiate(manaPrefab, dropPosition, Quaternion.identity);
    }
}
