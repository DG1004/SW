using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using static UnityEditor.Progress;
using System;

namespace Goldmetal.UndeadSurvivor
{
    public class Player : MonoBehaviour
    {
        public Vector2 inputVec;
        public float speed;
        public int isStore;
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

        private bool canDash = true;
        private float dashCooldown = 3f;
        private float cooldownTimer = 0f;
        public Vector2 ���������÷��̾�ӵ�;

        // ghost ��ų
        private bool isGhosting = false;
        public float ghostDuration = 5f;
        private float ghostTime = 0f;
        private bool canGhost = true;
        private float ghostCooldown = 30f;
        private float ghostCooldownTimer = 0f;
        private float ghostSpeedMultiplier = 1.5f;

        // enhence ��ų
        private bool isEnhencing = false;
        public float enhenceDuration = 5f;
        private float enhenceTime = 0f;
        private bool canEnhence = true;
        private float enhenceCooldown = 30f;
        private float enhenceCooldownTimer = 0f;
        public float attackMultiplier = 1f; // �⺻ ���ݷ� ����: 1f, enhence�� �� 1.5f

        // heal ��ų
        private bool canHeal = true;
        private float healCooldown = 60f;
        private float healCooldownTimer = 0f;

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
            if (isStore == 1)
            {
                inputVec.x = Input.GetAxisRaw("Horizontal");
                inputVec.y = Input.GetAxisRaw("Vertical");
            }

            if (!GameManager.instance.isLive)
                return;

            inputVec.x = Input.GetAxisRaw("Horizontal");
            inputVec.y = Input.GetAxisRaw("Vertical");

            // ���� ��ü
            if (Input.GetKeyDown(GameManager.instance.weaponChangeKey))
            {
                if (curWeapon == -1 || usingWeaponIdx[0] == -1 || usingWeaponIdx[1] == -1) return;
                else GameManager.instance.SwapWeapon(usingWeaponIdx[curWeapon]);
            }

            // �뽬 ��ų ���
            if (Input.GetKey(GameManager.instance.dashKey) && canDash)
            {
                isdash = true;
                canDash = false;
                cooldownTimer = dashCooldown;
            }

            // ghost ��ų ��� (QŰ)
            if (Input.GetKeyDown(GameManager.instance.ghostKey) && canGhost)
            {
                isGhosting = true;
                canGhost = false;
                ghostCooldownTimer = ghostCooldown;
                ghostTime = ghostDuration; // ghost ���ӽð� �ʱ�ȭ
            }

            // enhence ��ų ��� (WŰ)
            if (Input.GetKeyDown(GameManager.instance.enhenceKey) && canEnhence)
            {
                isEnhencing = true;
                canEnhence = false;
                enhenceCooldownTimer = enhenceCooldown;
                enhenceTime = enhenceDuration; // enhence ���ӽð� �ʱ�ȭ
                attackMultiplier = 1.5f; // ���ݷ� 1.5�� ����
            }

            // heal ��ų ��� (EŰ)
            if (Input.GetKeyDown(GameManager.instance.healKey) && canHeal)
            {
                // ü���� �ִ�ġ�� ȸ��
                GameManager.instance.health = GameManager.instance.maxHealth;
                canHeal = false;
                healCooldownTimer = healCooldown;
            }
        }

        void FixedUpdate()
        {
            if (isStore == 1)
            {
                Vector2 nextVec_s = inputVec.normalized * speed * Time.fixedDeltaTime;
                rigid.MovePosition(rigid.position + nextVec_s);
            }

            if (!GameManager.instance.isLive)
                return;

            // �÷��̾� �̵� ó��
            // �뽬/��Ʈ ��ų �ݿ�: �⺻�ӵ� * �뽬�ӵ� or ��Ʈ�ӵ� ����
            float currentSpeed = speed;
            // �뽬 ����
            if (dashTime > 0)
            {
                currentSpeed = dashSpeed;
            }
            else
            {
                currentSpeed = defaultSpeed;
            }
            // ��Ʈ ����
            if (isGhosting && ghostTime > 0)
            {
                currentSpeed *= ghostSpeedMultiplier;
            }

            Vector2 nextVec = inputVec.normalized * currentSpeed * Time.fixedDeltaTime;
            ���������÷��̾�ӵ� = inputVec.normalized * currentSpeed;
            rigid.MovePosition(rigid.position + nextVec);


            // �뽬 �ð�/��Ÿ�� ó��
            if (dashTime <= 0)
            {
                if (isdash)
                {
                    dashTime = defaultTime;
                }
                else
                {
                    speed = defaultSpeed;
                }
            }
            else
            {
                dashTime -= Time.deltaTime;
            }

            // �뽬 ��Ÿ�� ó��
            if (!canDash)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0)
                {
                    canDash = true; // �뽬 ��Ȱ��ȭ
                }
            }

            isdash = false;


            // ghost ���ӽð�/��Ÿ�� ó��
            if (isGhosting)
            {
                ghostTime -= Time.deltaTime;
                if (ghostTime <= 0)
                {
                    // ��Ʈ ȿ�� ����
                    isGhosting = false;
                }
            }

            if (!canGhost)
            {
                ghostCooldownTimer -= Time.deltaTime;
                if (ghostCooldownTimer <= 0)
                {
                    canGhost = true; // ��Ʈ ��Ȱ��ȭ
                }
            }

            // enhence ���ӽð�/��Ÿ�� ó��
            if (isEnhencing)
            {
                enhenceTime -= Time.deltaTime;
                if (enhenceTime <= 0)
                {
                    // ��� ȿ�� ����
                    isEnhencing = false;
                    attackMultiplier = 1f; // ���� ���ݷ����� ����
                }
            }

            if (!canEnhence)
            {
                enhenceCooldownTimer -= Time.deltaTime;
                if (enhenceCooldownTimer <= 0)
                {
                    canEnhence = true; // ��� ��Ȱ��ȭ
                }
            }

            // heal ��Ÿ�� ó��
            if (!canHeal)
            {
                healCooldownTimer -= Time.deltaTime;
                if (healCooldownTimer <= 0)
                {
                    canHeal = true; // �� ��Ȱ��ȭ
                }
            }
        }


        void LateUpdate()
        {
            if (!GameManager.instance.isLive && isStore == 0)
                return;

            anim.SetFloat("Speed", inputVec.magnitude);

            if (inputVec.x != 0)
            {
                spriter.flipX = inputVec.x < 0;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            // ���� ���� ����
            if (collision.gameObject.CompareTag("StoreEntrance"))
            {
                GameManager.instance.health = GameManager.instance.maxHealth;
                ManaManager.playerManas = ManaManager.maxManas;
                Debug.Log("ü���� ��� ȸ���Ǿ����ϴ�!");
                Debug.Log("��������");

                isStore = 1;
                GameManager.instance.isLive = false;
                PrePos.transform.position = transform.position;
                transform.position = new Vector3(store_inside.transform.position.x, store_inside.transform.position.y, store_inside.transform.position.z);
                vcam1.GetComponent<CinemachineVirtualCamera>().Priority = 11;
                vcam2.GetComponent<CinemachineVirtualCamera>().Follow = PrePos.transform;
                store.gameObject.SetActive(false);
            }
            else if (collision.gameObject.CompareTag("StoreExit"))
            {
                isStore = 0;
                transform.position = new Vector3(PrePos.transform.position.x, PrePos.transform.position.y, PrePos.transform.position.z);
                vcam1.GetComponent<CinemachineVirtualCamera>().Priority = 9;
                vcam2.GetComponent<CinemachineVirtualCamera>().Follow = transform;

                GameManager.instance.isLive = true;
                store.changePosition();
                store.gameObject.SetActive(true);

                // ����Ż��� ���� ��ġ ������
                GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemys)
                {
                    if (enemy.activeSelf)
                    {
                        enemy.transform.position = transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle.normalized * 15f);
                    }
                }
                StartCoroutine(GetComponentInChildren<Spawner>().SpawnInitialEnemies());
            }
            else if (collision.gameObject.CompareTag("StoreStd"))
            {
                GameManager.instance.ShowShop(1);
            }
            else if (collision.gameObject.CompareTag("StoreSmith"))
            {
                // �������� ���� ���� ����
            }
            else if (collision.gameObject.CompareTag("travellingMerchant"))
            {
                GameManager.instance.ShowShop(2);
            }
            else if (collision.gameObject.CompareTag("wizard"))
            {
                GameManager.instance.ShowShop(3);
            }
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            if (!GameManager.instance.isLive)
                return;

            if (collision.gameObject.CompareTag("Enemy"))
            {
                var offender = collision.gameObject.GetComponent<Enemy>();
                float damage = offender.attack * 0.01f;
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
                    ManaManager.playerManas = 100;
                }
                else
                {
                    offender.OnAttack(damage);
                }
            }
        }

        public void OnBeat(Action<float> action, float damage)
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
                action.Invoke(damage);
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
