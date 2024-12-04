using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    // �÷��̾��� ���� ���� ��
    public static int playerManas = 200000;
    public GameObject manaPrefab;

    private void Update()
    {
        // �׽�Ʈ��. vŰ�� ���� ���
        if (Input.GetKey(KeyCode.V))
        {
            Vector2 pos = GameManager.instance.player.transform.position;
            pos.y += 5.0f;
            DropManas(pos);
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
