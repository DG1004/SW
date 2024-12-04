using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using static UnityEditor.Progress;

namespace Goldmetal.UndeadSurvivor
{
	public class Player : MonoBehaviour
	{
		public Vector2 inputVec;
		public float speed;
		public int isStore;
		//Vector3 prePos;
		public Scanner scanner;
		public Hand[] hands;
		public RuntimeAnimatorController[] animCon;
		public Rigidbody2D rigid;
		public GameObject store_inside;
		public GameObject vcam1;
        public GameObject vcam2;
        public GameObject PrePos;
        public StoreEntrance store;

        public int[] usingWeaponIdx = new int[2];
        public int curWeapon;

		private bool isdash = false;
		public float dashSpeed;
		public float defaultTime;
		private float dashTime;
		private float defaultSpeed;

        private bool canDash = true; // 대쉬 가능 여부를 나타내는 변수
        private float dashCooldown = 5f; // 대쉬 쿨타임(20초)
        private float cooldownTimer = 0f; // 쿨타임을 추적하는 타이머
        public Vector2 예측샷용플레이어속도;
        SpriteRenderer spriter;
		Animator anim;

		void Awake()
		{
			rigid = GetComponent<Rigidbody2D>();
			spriter = GetComponent<SpriteRenderer>();
			anim = GetComponent<Animator>();
			scanner = GetComponent<Scanner>();
			hands = GetComponentsInChildren<Hand>(true);
			defaultSpeed = speed;
            usingWeaponIdx[0] = -1;
            usingWeaponIdx[1] = -1;
            curWeapon = -1;
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

            if(Input.GetKeyDown(GameManager.instance.weaponChangeKey))
            {
                if(curWeapon != -1 || usingWeaponIdx[0] != -1 || usingWeaponIdx[1]!=-1)
                {
                    GameManager.instance.SwapWeapon(usingWeaponIdx[curWeapon]);
                }
            }
		}

        void FixedUpdate()
        {
            if (!GameManager.instance.isLive)
                return;

            // 플레이어 이동 처리
            Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
            예측샷용플레이어속도 = inputVec.normalized * speed;
            rigid.MovePosition(rigid.position + nextVec);

            // 대쉬 처리
            if (Input.GetKey(GameManager.instance.dashKey) && canDash)
            {
                isdash = true;
                canDash = false; // 대쉬 비활성화
                cooldownTimer = dashCooldown; // 쿨타임 초기화
            }

            if (dashTime <= 0)
            {
                speed = defaultSpeed;
                if (isdash)
                {
                    dashTime = defaultTime;
                }
            }
            else
            {
                dashTime -= Time.deltaTime;
                speed = dashSpeed;
            }

            // 쿨타임 처리
            if (!canDash)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0)
                {
                    canDash = true; // 대쉬 재활성화
                }
            }

            isdash = false;
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
                GameManager.instance.health = GameManager.instance.maxHealth;
                Debug.Log("체력이 모두 회복되었습니다!");
                Debug.Log("상점진입");
				// 플레이어가 상점에 있다는 것을 표시합니다.
				isStore = 1;
				// 게임 진행 상태를 false로 설정합니다.
				GameManager.instance.isLive = false;
				// 플레이어의 기존 위치를 기억합니다.
				PrePos.transform.position = transform.position;
				// 플레이어의 위치를 상점으로 순간이동합니다.
				transform.position = new Vector3(store_inside.transform.position.x, store_inside.transform.position.y, store_inside.transform.position.z);
				// 카메라를 바꿉니다.
				vcam1.GetComponent<CinemachineVirtualCamera>().Priority = 11;
				vcam2.GetComponent<CinemachineVirtualCamera>().Follow = PrePos.transform;

				store.gameObject.SetActive(false);
			}
            // 플레이어가 상점 출구에 충돌했을 때
            else if (collision.gameObject.CompareTag("StoreExit"))
            {
                isStore = 0;
                transform.position = new Vector3(PrePos.transform.position.x, PrePos.transform.position.y, PrePos.transform.position.z);

                // 상점에서 나갈 때 카메라가 cut방식으로 바로 전환되지 않는 것 수정 필요
                vcam1.GetComponent<CinemachineVirtualCamera>().Priority = 9;
                vcam2.GetComponent<CinemachineVirtualCamera>().Follow = transform;

                //wait();
                GameManager.instance.isLive = true;

                store.changePosition();
                store.gameObject.SetActive(true);
				// 상점탈출시 몬스터 위치 재조정 코드
                GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
				foreach(GameObject enemy in enemys)
				{
					if (enemy.activeSelf)
					{
                        enemy.transform.position = transform.position + (Vector3)(Random.insideUnitCircle.normalized * 20f);
                    }
                }
            }
            // 플레이어가 기본상점에 충돌했을 때
            else if (collision.gameObject.CompareTag("StoreStd"))
			{
                GameManager.instance.ShowShop(1);
            }
            // 플레이어가 대장장이에 충돌했을 때
            else if (collision.gameObject.CompareTag("StoreSmith"))
			{

			}
			// 플레이어가 보따리상인에 충돌했을 때
            else if (collision.gameObject.CompareTag("travellingMerchant"))
            {
                GameManager.instance.ShowShop(2);
            }
        }
		void OnCollisionStay2D(Collision2D collision)
		{
			if (!GameManager.instance.isLive)
				return;

			if (collision.gameObject.CompareTag("Enemy"))
			{
                var offender = collision.gameObject.GetComponent<Enemy>();
                float damage = offender.attack * 1.0f;
                GameManager.instance.health -= damage;

                if (GameManager.instance.health < 0)
                {
                    for (int index = 2; index < transform.childCount; index++)
                    {
                        transform.GetChild(index).gameObject.SetActive(false);
                    }
                    anim.SetTrigger("Dead");
                    GameManager.instance.GameOver();
					CoinManager.playerCoins = 1500;
                }
                else
                {
                    offender.OnAttack(damage);
                }
            }
		}
		public void OnBeat(EnemyBullet offender,float damage)
		{
            GameManager.instance.health -= damage;
            if (GameManager.instance.health < 0)
            {
                for (int index = 2; index < transform.childCount; index++)
                {
                    transform.GetChild(index).gameObject.SetActive(false);
                }
                anim.SetTrigger("Dead");
                CoinManager.playerCoins = 1500;
                GameManager.instance.GameOver();
                
            }
            else
            {
                offender.OnAttack(damage);
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
