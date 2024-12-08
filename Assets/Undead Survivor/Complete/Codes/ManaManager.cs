using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    // �÷��̾��� ���� ���� ��
    public static double playerManas = 100.0;
    public static double maxManas = 100.0;

    public GameObject manaPrefab;

    private float lastManaUseTime;
    private bool isRegenerating = false;
    private float regenInterval = 1.0f;
    private double regenAmount = 5.0;

    private void Update()
    {
        // �׽�Ʈ��. CŰ�� ���� ���
        if (Input.GetKey(KeyCode.C))
        {
            Vector2 pos = GameManager.instance.player.transform.position;
            pos.y += 5.0f;
            DropManas(pos);
            lastManaUseTime = Time.time; // ���� ��� �ð� ����
            isRegenerating = false; // ���� ��� �� ȸ�� ����
        }

        if (playerManas < maxManas)
        {
            playerManas = System.Math.Clamp(playerManas, 0.0, maxManas); // Mathf ��� System.Math ���
        }

        // ������ ������ ���� �ð��� 3�� �̻��� �� ���� ȸ�� ����
        if (Time.time - lastManaUseTime >= 3.0f && playerManas < maxManas && !isRegenerating)
        {
            StartCoroutine(RegenerateMana());
        }
    }

    // ������ ����ϴ� �Լ�
    // ���Ͱ� ���� ��, �� �Լ��� ȣ���ؼ� ����Ͻø� �ſ�.  
    public void DropManas(Vector2 dropPosition)
    {
        // ���� �ν��Ͻ� ����
        GameObject mana = Instantiate(manaPrefab, dropPosition + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), Quaternion.identity);
    }

    // ���� ȸ�� �ڷ�ƾ
    private IEnumerator RegenerateMana()
    {
        isRegenerating = true;
        while (playerManas < maxManas)
        {
            playerManas += regenAmount;
            playerManas = System.Math.Clamp(playerManas, 0.0, maxManas); // System.Math.Clamp ���
            yield return new WaitForSeconds(regenInterval);

            // ���� ������ ���Ǿ����� ȸ�� �ߴ�
            if (Time.time - lastManaUseTime < 3.0f)
            {
                isRegenerating = false;
                yield break;
            }
        }
        isRegenerating = false;
    }
}
