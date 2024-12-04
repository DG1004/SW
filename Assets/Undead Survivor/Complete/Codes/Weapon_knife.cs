using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_knife : Weapon
{
    float timer;

    public override void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;

        if (timer > speed)
        {
            if (Input.GetKey(GameManager.instance.fireKey) )
                        {
                            timer = 0f;
                            Fire();
                           
                        }
        }
    }

    public override void Init(ItemData data)
    {
        // base.Init(data); Weapon 클래스의 Init를 호출(현재는 전체를 새로 작성중)

        // Basic Set
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;

        for (int index = 0; index < GameManager.instance.pool.bullet_prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.bullet_prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        speed = 0.5f * Character.WeaponRate;

        // Hand Set
        Hand hand = player.hands[(int)data.itemType];
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);

        Debug.Log("New Knife");
    }
    public override void LevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage;

        Debug.Log("New Knife");
    }

    public override void Fire()
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
        bullet.GetComponent<Bullet>().damage = damage;
        bullet.GetComponent<Bullet>().GetComponent<Rigidbody2D>().velocity = dir * 1.0f;
        //.Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
}
