using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    // 플레이어의 현재 코인 수
    public static int playerManas = 100;
    public static int maxManas = 100;

    public GameObject manaPrefab;

    private float lastManaUseTime;
    private bool isRegenerating = false;
    private float regenInterval = 1.0f;
    private int regenAmount = 5;

    private void Update()
    {
        // 테스트용. C키로 마나 드랍
        if (Input.GetKey(KeyCode.C))
        {
            Vector2 pos = GameManager.instance.player.transform.position;
            pos.y += 5.0f;
            DropManas(pos);
            lastManaUseTime = Time.time; // 마나 사용 시간 갱신
            isRegenerating = false; // 마나 사용 시 회복 중지
        }

        if (playerManas < maxManas)
        {
            playerManas = Mathf.Clamp(playerManas, 0, maxManas);
        }

        // 마나가 사용되지 않은 시간이 3초 이상일 때 마나 회복 시작
        if (Time.time - lastManaUseTime >= 3.0f && playerManas < maxManas && !isRegenerating)
        {
            StartCoroutine(RegenerateMana());
        }
    }

    // 마나를 드랍하는 함수
    // 몬스터가 죽을 때, 이 함수를 호출해서 사용하시면 돼요.
    public void DropManas(Vector2 dropPosition)
    {
        // 마나 인스턴스 생성
        GameObject mana = Instantiate(manaPrefab, dropPosition, Quaternion.identity);
    }

    // 마나 회복 코루틴
    private IEnumerator RegenerateMana()
    {
        isRegenerating = true;
        while (playerManas < maxManas)
        {
            playerManas += regenAmount;
            playerManas = Mathf.Clamp(playerManas, 0, maxManas);
            yield return new WaitForSeconds(regenInterval);

            // 만약 마나가 사용되었으면 회복 중단
            if (Time.time - lastManaUseTime < 3.0f)
            {
                isRegenerating = false;
                yield break;
            }
        }
        isRegenerating = false;
    }
}
