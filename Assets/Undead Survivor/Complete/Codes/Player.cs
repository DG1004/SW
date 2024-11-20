using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Goldmetal.UndeadSurvivor
{
	public class Player : MonoBehaviour
	{
		public Vector2 inputVec;
		public float speed;
		public Scanner scanner;
		public Hand[] hands;
		public RuntimeAnimatorController[] animCon;

		public GameObject store_inside;
		public GameObject store_background;
		public GameObject ground;
		public GameObject vcam;
		public GameObject Pool;

		Rigidbody2D rigid;
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
			if (collision.gameObject.CompareTag("Store"))
			{
				Pool.gameObject.SetActive(false);
				transform.position = new Vector3(store_inside.transform.position.x, store_inside.transform.position.y, store_inside.transform.position.z);
				vcam.GetComponent<CinemachineVirtualCamera>().Priority = 11;
			}
			//gameObject.SetActive(false);
			//ground.SetActive(false);
			//store_inside.SetActive(true);
			//store_background.SetActive(true);
		}
		
		void OnCollisionStay2D(Collision2D collision)
		{
			if (!GameManager.instance.isLive)
				return;
			if (!collision.gameObject.CompareTag("Enemy"))
				return;

			// 만약 몬스터가 아니면 return
			// >> 상점의 구조물이나 상점의 벽면에 충돌했을 때 대미지를 입지 않기 위함

			GameManager.instance.health -= Time.deltaTime * 10;

			if (GameManager.instance.health < 0) {
				for (int index = 2; index < transform.childCount; index++) {
					transform.GetChild(index).gameObject.SetActive(false);
				}

				anim.SetTrigger("Dead");
				GameManager.instance.GameOver();
			}
		}

		void OnMove(InputValue value)
		{
			inputVec = value.Get<Vector2>();
		}
	}
}
