using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goldmetal.UndeadSurvivor
{
    public class Weapon : MonoBehaviour
    {
        public int id;
        public int prefabId;
        public float damage;
        public int count;
        public float speed;

        float timer;
        protected Player player;

        void Awake()
        {
            player = GameManager.instance.player;
        }

        public virtual void Update()
        {
            if (!GameManager.instance.isLive)
                return;

            switch (id) {
                case 0:
                    transform.Rotate(Vector3.back * speed * Time.deltaTime);
                    break;
                default:
                    timer += Time.deltaTime;

                    if (timer > speed) {
                        if (Input.GetKey(GameManager.instance.fireKey))
                        {
                            timer = 0f;
                            Fire();
                        }
                    }
                    break;
            }

            // .. Test Code..
            if (Input.GetButtonDown("Jump")) {
                LevelUp(10, 1);
            }
        }

        public virtual void LevelUp(float damage, int count)
        {
            this.damage = damage * Character.Damage;
            this.count += count;

            if (id == 0)
                Batch();

            player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
        }

        public virtual void Init(ItemData data)
        {
            // Basic Set
            name = "Weapon " + data.itemId;
            transform.parent = player.transform;
            transform.localPosition = Vector3.zero;

            // Property Set
            id = data.itemId;
            damage = data.baseDamage * Character.Damage;
            count = data.baseCount + Character.Count;

            for (int index = 0; index < GameManager.instance.pool.bullet_prefabs.Length; index++) {
                if (data.projectile == GameManager.instance.pool.bullet_prefabs[index]) {
                    prefabId = index;
                    break;
                }
            }

            switch (id) {
                case 0:
                    speed = 150 * Character.WeaponSpeed;
                    Batch();
                    break;
                default:
                    speed = 0.5f * Character.WeaponRate;
                    break;
            }

            // Hand Set
            Hand hand = player.hands[(int)data.itemType];
            hand.spriter.sprite = data.hand;
            hand.gameObject.SetActive(true);

            player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
        }

        void Batch()
        {
            for (int index = 0; index < count; index++) {
                Transform bullet;

                if (index < transform.childCount) {
                    bullet = transform.GetChild(index);
                }
                else {
                    bullet = GameManager.instance.pool.Get_Bullet(prefabId).transform;
                    bullet.parent = transform;
                }

                bullet.localPosition = Vector3.zero;
                bullet.localRotation = Quaternion.identity;

                Vector3 rotVec = Vector3.forward * 360 * index / count;
                bullet.Rotate(rotVec);
                bullet.Translate(bullet.up * 1.5f, Space.World);
                bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -100 is Infinity Per.
            }
        }

        public virtual void Fire()
        {
            if (!player.scanner.nearestTarget)
            {
                Debug.Log("noTarget");
                return;
            }

            Vector3 targetPos = player.scanner.nearestTarget.position;
            Vector3 dir = targetPos - transform.position;
            dir = dir.normalized;

            Transform bullet = GameManager.instance.pool.Get_Bullet(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            bullet.GetComponent<Bullet>().Init(damage, count, dir);
            ManaManager.playerManas -= 15;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }

        public virtual void FireRifle()
        {
            if (!player.scanner.nearestTarget)
            {
                Debug.Log("noTarget");
                return;
            }

            Vector3 targetPos = player.scanner.nearestTarget.position;
            Vector3 dir = targetPos - transform.position;
            dir = dir.normalized;

            Transform bullet = GameManager.instance.pool.Get_Bullet(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            bullet.GetComponent<Bullet>().Init(damage, count, dir);
            ManaManager.playerManas -= 1;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
        public virtual void FireRifle2()
        {
            if (!player.scanner.nearestTarget)
            {
                Debug.Log("noTarget");
                return;
            }

            Vector3 targetPos = player.scanner.nearestTarget.position;
            Vector3 dir = targetPos - transform.position;
            dir = dir.normalized;

            Transform bullet = GameManager.instance.pool.Get_Bullet(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            bullet.GetComponent<Bullet>().Init(damage, count, dir);
            ManaManager.playerManas -= 0.3;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }

        public virtual void FireMeteor()
        {
            if (!player.scanner.nearestTarget)
            {
                Debug.Log("noTarget");
                return;
            }

            Vector3 targetPos = player.scanner.nearestTarget.position;
            Vector3 dir = targetPos - transform.position;
            dir = dir.normalized;

            Transform bullet = GameManager.instance.pool.Get_Bullet(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            bullet.GetComponent<Bullet>().Init(damage, count, dir);
            ManaManager.playerManas -= 100;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
        public void SwapWeapon(int curId)
        {
            GameManager.instance.player.hands[GameManager.instance.player.usingWeaponIdx[GameManager.instance.player.curWeapon]].gameObject.SetActive(false);
            // ���� ������� Hand�� ��Ȱ��ȭ
            Transform weaponTrs = GameManager.instance.player.transform.Find("Weapon " + curId);
            if (weaponTrs != null)
            {
                weaponTrs.gameObject.SetActive(false); // ���� ������� ���⸦ ��Ȱ��ȭ
            }
            else
            {
                Debug.LogWarning("Weapon ������Ʈ�� ã�� �� �����ϴ�.");
            }

            // ���� ��ü�� ���⸦ Ȱ��ȭ
            int nextWeaponIdx = GameManager.instance.player.curWeapon == 0 ? 1 : 0; // ��ü�� ���� �ε���
            int nextWeaponId = GameManager.instance.player.usingWeaponIdx[nextWeaponIdx];

            GameManager.instance.player.hands[nextWeaponId].gameObject.SetActive(true);
            Transform nextWeaponTrs = GameManager.instance.player.transform.Find("Weapon " + nextWeaponId);

            if (nextWeaponTrs != null)
            {
                nextWeaponTrs.gameObject.SetActive(true); // �� ���� Ȱ��ȭ
                Debug.Log($"Weapon {nextWeaponId} Ȱ��ȭ");
            }
            else
            {
                Debug.LogWarning("�� Weapon ������Ʈ�� ã�� �� �����ϴ�.");
            }

            // ���� ���� �ε��� ������Ʈ
            GameManager.instance.player.curWeapon = nextWeaponIdx;
        }
        public void RemoveWeapon(int id)
        {
            GameManager.instance.player.hands[id].gameObject.SetActive(false);
            Transform weaponTrs = GameManager.instance.player.transform.Find("Weapon " + id);
            if (weaponTrs != null)
            {
                Weapon weaponScript = weaponTrs.GetComponent<Weapon>();
                if (weaponScript != null)
                {
                    Destroy(weaponTrs.gameObject);
                    Debug.Log("���� ���� �ı�");
                }
                else
                {
                    Debug.LogWarning("Weapon ��ũ��Ʈ�� ã�� �� �����ϴ�.");
                }
            }
            else
            {
                Debug.LogWarning("Weapon ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
    }
}
