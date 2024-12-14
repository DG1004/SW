using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
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
        public Vector2 예측샷용플레이어속도;

        // ghost 스킬
        private bool isGhosting = false;
        public float ghostDuration = 5f;
        private float ghostTime = 0f;
        public bool canGhost = true;
        private float ghostCooldown = 30f;
        private float ghostCooldownTimer = 0f;
        private float ghostSpeedMultiplier = 1.5f;

        // enhence 스킬
        private bool isEnhencing = false;
        public float enhenceDuration = 5f;
        private float enhenceTime = 0f;
        public bool canEnhence = true;
        private float enhenceCooldown = 30f;
        private float enhenceCooldownTimer = 0f;
        public float attackMultiplier = 1f; // 기본 공격력 배율: 1f, enhence일 때 1.5f

        // heal 스킬
        public bool canHeal = true;
        private float healCooldown = 60f;
        private float healCooldownTimer = 0f;

        SpriteRenderer spriter;
        Animator anim;

        private bool isGhostSkillPurchased = false;
        private bool isEnhenceSkillPurchased = false;
        private bool isHealSkillPurchased = false;

        private CooldownTimer cooldownTimerScript;

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

            cooldownTimerScript = GetComponent<CooldownTimer>();
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

            // 무기 교체
            if (Input.GetKeyDown(GameManager.instance.weaponChangeKey))
            {
                if (curWeapon == -1 || usingWeaponIdx[0] == -1 || usingWeaponIdx[1] == -1) return;
                else GameManager.instance.SwapWeapon(usingWeaponIdx[curWeapon]);
            }

            // 대쉬 스킬 사용
            if (Input.GetKey(GameManager.instance.dashKey) && canDash)
            {
                isdash = true;
                canDash = false;
                cooldownTimer = dashCooldown;
            }

            // 고스트 스킬 사용 (Q키)
            if (Input.GetKeyDown(GameManager.instance.ghostKey) && canGhost && isGhostSkillPurchased)
            {
                isGhosting = true;
                canGhost = false;
                ghostCooldownTimer = ghostCooldown;
                ghostTime = ghostDuration; // ghost 지속시간 초기화
            }

            // 엔헨스 스킬 사용 (W키)
            if (Input.GetKeyDown(GameManager.instance.enhenceKey) && canEnhence && isEnhenceSkillPurchased)
            {
                isEnhencing = true;
                canEnhence = false;
                enhenceCooldownTimer = enhenceCooldown;
                enhenceTime = enhenceDuration; // enhence 지속시간 초기화
                attackMultiplier = 1.5f; // 공격력 1.5배 증가
            }

            // 힐 스킬 사용 (E키)
            if (Input.GetKeyDown(GameManager.instance.healKey) && canHeal && isHealSkillPurchased)
            {
                // 체력을 최대치로 회복
                GameManager.instance.health = GameManager.instance.maxHealth;
                canHeal = false;
                healCooldownTimer = healCooldown;
            }
        }

        public void PurchaseGhostSkill()
        {
            isGhostSkillPurchased = true;
            Debug.Log("!");

            if (cooldownTimerScript != null)
            {
                cooldownTimerScript.PurchaseGhostSkill();
            }
        }

        public void PurchaseEnhenceSkill()
        {
            isEnhenceSkillPurchased = true;
            if (cooldownTimerScript != null)
            {
                cooldownTimerScript.PurchaseEnhenceSkill();
            }
        }

        public void PurchaseHealSkill()
        {
            isHealSkillPurchased = true;
            if (cooldownTimerScript != null)
            {
                cooldownTimerScript.PurchaseHealSkill();
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

            float currentSpeed = speed;

            if (dashTime > 0)
            {
                currentSpeed = dashSpeed;
            }
            else
            {
                currentSpeed = defaultSpeed;
            }

            if (isGhosting && ghostTime > 0)
            {
                currentSpeed *= ghostSpeedMultiplier;
            }

            Vector2 nextVec = inputVec.normalized * currentSpeed * Time.fixedDeltaTime;
            예측샷용플레이어속도 = inputVec.normalized * currentSpeed;
            rigid.MovePosition(rigid.position + nextVec);

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

            if (!canDash)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0)
                {
                    canDash = true;
                }
            }

            isdash = false;

            if (isGhosting)
            {
                ghostTime -= Time.deltaTime;
                if (ghostTime <= 0)
                {
                    isGhosting = false;
                }
            }

            if (!canGhost)
            {
                ghostCooldownTimer -= Time.deltaTime;
                if (ghostCooldownTimer <= 0)
                {
                    canGhost = true;
                }
            }

            if (isEnhencing)
            {
                enhenceTime -= Time.deltaTime;
                if (enhenceTime <= 0)
                {
                    isEnhencing = false;
                    attackMultiplier = 1f;
                }
            }

            if (!canEnhence)
            {
                enhenceCooldownTimer -= Time.deltaTime;
                if (enhenceCooldownTimer <= 0)
                {
                    canEnhence = true;
                }
            }

            if (!canHeal)
            {
                healCooldownTimer -= Time.deltaTime;
                if (healCooldownTimer <= 0)
                {
                    canHeal = true;
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
            if (collision.gameObject.CompareTag("StoreEntrance"))
            {
                GameManager.instance.health = GameManager.instance.maxHealth;
                ManaManager.playerManas = ManaManager.maxManas;
                Debug.Log("체력이 모두 회복되었습니다!");
                Debug.Log("상점진입");

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

                GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemys)
                {
                    if (enemy.activeSelf)
                    {
                        enemy.transform.position = transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle.normalized * 15f);
                    }
                }
                for(int i = 0; i < 7; i++)
                {
                    GetComponentInChildren<Spawner>().SpawnInitialEnemies();
                }
            }
            else if (collision.gameObject.CompareTag("StoreStd"))
            {
                GameManager.instance.ShowShop(1);
            }
            else if (collision.gameObject.CompareTag("StoreSmith"))
            {
                // 대장장이 로직 구현 예정
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
