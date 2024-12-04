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

        private bool canDash = true; // �뽬 ���� ���θ� ��Ÿ���� ����
        private float dashCooldown = 5f; // �뽬 ��Ÿ��(20��)
        private float cooldownTimer = 0f; // ��Ÿ���� �����ϴ� Ÿ�̸�
        public Vector2 ���������÷��̾�ӵ�;
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

            // �÷��̾� �̵� ó��
            Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
            ���������÷��̾�ӵ� = inputVec.normalized * speed;
            rigid.MovePosition(rigid.position + nextVec);

            // �뽬 ó��
            if (Input.GetKey(GameManager.instance.dashKey) && canDash)
            {
                isdash = true;
                canDash = false; // �뽬 ��Ȱ��ȭ
                cooldownTimer = dashCooldown; // ��Ÿ�� �ʱ�ȭ
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

            // ��Ÿ�� ó��
            if (!canDash)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0)
                {
                    canDash = true; // �뽬 ��Ȱ��ȭ
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
			// �÷��̾ ���� �Ա��� �浹���� ��
			if (collision.gameObject.CompareTag("StoreEntrance"))
			{
                GameManager.instance.health = GameManager.instance.maxHealth;
                Debug.Log("ü���� ��� ȸ���Ǿ����ϴ�!");
                Debug.Log("��������");
				// �÷��̾ ������ �ִٴ� ���� ǥ���մϴ�.
				isStore = 1;
				// ���� ���� ���¸� false�� �����մϴ�.
				GameManager.instance.isLive = false;
				// �÷��̾��� ���� ��ġ�� ����մϴ�.
				PrePos.transform.position = transform.position;
				// �÷��̾��� ��ġ�� �������� �����̵��մϴ�.
				transform.position = new Vector3(store_inside.transform.position.x, store_inside.transform.position.y, store_inside.transform.position.z);
				// ī�޶� �ٲߴϴ�.
				vcam1.GetComponent<CinemachineVirtualCamera>().Priority = 11;
				vcam2.GetComponent<CinemachineVirtualCamera>().Follow = PrePos.transform;

				store.gameObject.SetActive(false);
			}
            // �÷��̾ ���� �ⱸ�� �浹���� ��
            else if (collision.gameObject.CompareTag("StoreExit"))
            {
                isStore = 0;
                transform.position = new Vector3(PrePos.transform.position.x, PrePos.transform.position.y, PrePos.transform.position.z);

                // �������� ���� �� ī�޶� cut������� �ٷ� ��ȯ���� �ʴ� �� ���� �ʿ�
                vcam1.GetComponent<CinemachineVirtualCamera>().Priority = 9;
                vcam2.GetComponent<CinemachineVirtualCamera>().Follow = transform;

                //wait();
                GameManager.instance.isLive = true;

                store.changePosition();
                store.gameObject.SetActive(true);
				// ����Ż��� ���� ��ġ ������ �ڵ�
                GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
				foreach(GameObject enemy in enemys)
				{
					if (enemy.activeSelf)
					{
                        enemy.transform.position = transform.position + (Vector3)(Random.insideUnitCircle.normalized * 20f);
                    }
                }
            }
            // �÷��̾ �⺻������ �浹���� ��
            else if (collision.gameObject.CompareTag("StoreStd"))
			{
                GameManager.instance.ShowShop(1);
            }
            // �÷��̾ �������̿� �浹���� ��
            else if (collision.gameObject.CompareTag("StoreSmith"))
			{

			}
			// �÷��̾ ���������ο� �浹���� ��
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
