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
		Vector3 prePos;
		public Scanner scanner;
		public Hand[] hands;
		public RuntimeAnimatorController[] animCon;
		public Rigidbody2D rigid;
		public GameObject store_inside;
		public GameObject vcam;
		public StoreEntrance store;

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
			// �÷��̾ ���� �Ա��� �浹���� ��
			if (collision.gameObject.CompareTag("StoreEntrance"))
			{
				// �÷��̾ ������ �ִٴ� ���� ǥ���մϴ�.
				isStore = 1;
				// ���� ���� ���¸� false�� �����մϴ�.
				GameManager.instance.isLive = false;
				// �÷��̾��� ���� ��ġ�� ����մϴ�.
				prePos = transform.position;
				// �÷��̾��� ��ġ�� �������� �����̵��մϴ�.
				transform.position = new Vector3(store_inside.transform.position.x, store_inside.transform.position.y, store_inside.transform.position.z);
				// ī�޶� �ٲߴϴ�.
				vcam.GetComponent<CinemachineVirtualCamera>().Priority = 11;
				store.gameObject.SetActive(false);
			}
            // �÷��̾ ���� �ⱸ�� �浹���� ��
            if (collision.gameObject.CompareTag("StoreExit"))
            {
                isStore = 0;
                transform.position = new Vector3(prePos.x, prePos.y, prePos.z);
                //wait();
                GameManager.instance.isLive = true;

                store.changePosition();
                store.gameObject.SetActive(true);
                GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
				foreach(GameObject enemy in enemys)
				{
					if (enemy.activeSelf)
					{
                        enemy.transform.position = transform.position + (Vector3)(Random.insideUnitCircle.normalized * 20f);
                    }
                }
                // �������� ���� �� ī�޶� cut������� �ٷ� ��ȯ���� �ʴ� �� ���� �ʿ�
                vcam.GetComponent<CinemachineVirtualCamera>().Priority = 9;
            }
            // �÷��̾ �⺻������ �浹���� ��
            if (collision.gameObject.CompareTag("StoreStd"))
			{
                GameManager.instance.ShowShop(1);
            }
            // �÷��̾ �������̿� �浹���� ��
            if (collision.gameObject.CompareTag("StoreSmith"))
			{

			}
			// �÷��̾ ���������ο� �浹���� ��
            if (collision.gameObject.CompareTag("travellingMerchant"))
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
                var offender = collision.gameObject.GetComponent<Enemy_Parent>();
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
                }
                else
                {
                    offender.OnAttack(damage);
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
