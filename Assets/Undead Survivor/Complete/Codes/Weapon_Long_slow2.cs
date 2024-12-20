using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Weapon_Long_slow2 : Weapon
{
    float timer;
    public override void Update()
    {
        if (!GameManager.instance.isLive)
            return;
        /*
        if (Input.GetKey(KeyCode.Z))
        {
            Fire();
        }
        */
        timer += Time.deltaTime;

        if (timer > speed)
        {
            if (Input.GetKey(GameManager.instance.fireKey) && ManaManager.playerManas >= 30)
            {
                timer = 0f;
                FireArrow2();
                GameManager.instance.health += 20;

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
        count = data.baseCount + Character.Count;

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

        Debug.Log("NewArrow");
    }
    public override void LevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        Debug.Log("New Arrow");
    }
}
