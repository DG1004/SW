using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class Coin : MonoBehaviour
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
}*/
public class Coin : MonoBehaviour
{
    // 코인 1개의 값어치
    public int value;
    private bool isLive;
    private bool isFollow;
    private Transform target;
    float lifetime = 15f;
    private float speed = 5f; // 코인 이동 속도

    void OnEnable()
    {
        if (GameManager.instance?.player == null)
        {
            Debug.LogError("GameManager에 플레이어 참조가 없습니다.");
            return;
        }
        target = GameManager.instance.player.transform;
        isLive = true;
        isFollow = false;
        CancelInvoke("OnDead");
        Invoke("OnDead", lifetime);
    }

    void Update()
    {
        if (!isLive || !isFollow)
            return;

        // 플레이어 방향으로 이동
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    // 코인 수집 메서드 (부모 오브젝트의 Collider에서 호출)
    public void Collect()
    {
        if (!isLive)
            return;

        CoinManager.playerCoins += value;
        OnDead();
    }
    void OnDead()
    {
        isLive = false;
        gameObject.SetActive(false); // 비활성화
        Destroy(gameObject, 0.1f);   // 딜레이 후 오브젝트 제거
    }
    // 코인 따라가기 시작 메서드 (자식 오브젝트의 스크립트에서 호출)
    public void StartFollowing()
    {
        isFollow = true;
    }

    // 부모 오브젝트의 Collider 이벤트
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Coin);
        }
    }
}