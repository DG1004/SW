using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goldmetal.UndeadSurvivor
{
    public class Reposition : MonoBehaviour
    {
        Collider2D coll;

        void Awake()
        {
            coll = GetComponent<Collider2D>();
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Area"))
                return;

            Vector3 playerPos = GameManager.instance.player.transform.position;
            Vector3 myPos = transform.position;

            switch (transform.tag) {
                case "Ground":
                    float diffX = playerPos.x - myPos.x; // 플레이어 위치와 타일맵의 위치로 거리 구하기
                    float diffY = playerPos.y - myPos.y;
                    float dirX = diffX < 0 ? -1 : 1; // 플레이어의 이동 방향
                    float dirY = diffY < 0 ? -1 : 1;
                    diffX = Mathf.Abs(diffX); // 거리가 음수면 안되기 때문에 절대값
                    diffY = Mathf.Abs(diffY);

                    if (diffX > diffY) {
                        transform.Translate(Vector3.right * dirX * 80);
                    }
                    else if (diffX < diffY) {
                        transform.Translate(Vector3.up * dirY * 60);
                    }
                    break;
                case "Enemy":
                    if (coll.enabled) {
                        Vector3 dist = playerPos - myPos;
                        Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
                        transform.Translate(ran + dist * 2);
                    }
                    break;
            }
        }
    }
}