using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace Goldmetal.UndeadSurvivor
{
    public class Player : MonoBehaviour
    {
        public Vector2 inputVec;
        public float speed;
        public int isStore;
        Vector3 prePos;
        public Scanner scanner;
        public Hand[] hands;
        public RuntimeAnimatorController[] animCon;
        public Rigidbody2D rigid;

        public GameObject store_inside;
        public GameObject vcam;
        public GameObject Pool;

        SpriteRenderer spriter;
        Animator anim;

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            spriter = GetComponent<SpriteRenderer>();
            anim = GetComponent<Animator>();
            scanner = GetComponent<Scanner>();
            hands = GetComponentsInChildren<Hand>(true);
        }

        void OnEnable()
        {
            speed *= Character.Speed;
            anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
        }

        void Update()
        {
            if (!GameManager.instance.isLive)
                return;

            inputVec.x = Input.GetAxisRaw("Horizontal");
            inputVec.y = Input.GetAxisRaw("Vertical");
        }

        void FixedUpdate()
        {
            if (!GameManager.instance.isLive)
                return;

            Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
        }

        void LateUpdate()
        {
            if (!GameManager.instance.isLive)
                return;

            anim.SetFloat("Speed", inputVec.magnitude);


            if (inputVec.x != 0) {
                spriter.flipX = inputVec.x < 0;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            // 플레이어가 상점 입구에 충돌했을 때
            if (collision.gameObject.CompareTag("StoreEntrance"))
            {
                // 플레이어가 상점에 있다는 것을 표시합니다.
                isStore = 1;
                // 게임 진행 상태를 false로 설정합니다.
                GameManager.instance.isLive = false;
                // 플레이어의 기존 위치를 기억합니다.
                prePos = transform.position;
                // 플레이어의 위치를 상점으로 순간이동합니다.
                transform.position = new Vector3(store_inside.transform.position.x, store_inside.transform.position.y, store_inside.transform.position.z);
                // 카메라를 바꿉니다.
                vcam.GetComponent<CinemachineVirtualCamera>().Priority = 11;
            }
            // 플레이어가 상점 출구에 충돌했을 때
            if (collision.gameObject.CompareTag("StoreExit"))
            {
                isStore = 0;
                transform.position = new Vector3(prePos.x + 1, prePos.y + 1, prePos.z + 1);
                //wait();
                GameManager.instance.isLive = true;

                // 상점에서 나갈 때 카메라가 cut방식으로 바로 전환되지 않는 것 수정 필요
                vcam.GetComponent<CinemachineVirtualCamera>().Priority = 9;
            }
            // 플레이어가 기본상점에 충돌했을 때
            if (collision.gameObject.CompareTag("StoreStd"))
            {

            }
            // 플레이어가 대장장이에 충돌했을 때
            if (collision.gameObject.CompareTag("StoreSmith"))
            {

            }
        }
        void OnCollisionStay2D(Collision2D collision)
        {
            if (!GameManager.instance.isLive)
                return;
            
            if (collision.gameObject.CompareTag("travellingMerchant"))
            {
                GameManager.instance.ShowShop();
            }
            else if (collision.gameObject.CompareTag("Enemy"))
            {
                GameManager.instance.health -= Time.deltaTime * 10;

                if (GameManager.instance.health < 0)
                {
                    for (int index = 2; index < transform.childCount; index++)
                    {
                        transform.GetChild(index).gameObject.SetActive(false);
                    }

                    anim.SetTrigger("Dead");
                    GameManager.instance.GameOver();
                }
            }
        }

        void OnMove(InputValue value)
        {
            inputVec = value.Get<Vector2>();
        }

        IEnumerator wait()
        {
            yield return new WaitForSecondsRealtime(10);
        }
    }
}
