using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Goldmetal.UndeadSurvivor
{
    public class Item : MonoBehaviour
    {
        public ItemData data;
        public int level;
        public Weapon weapon;
        public Gear gear;

        Image icon;
        Text textLevel;
        Text textName;
        Text textDesc;
        Text textPrice; // ������ ������ ǥ���ϴ� �ؽ�Ʈ

        void Awake()
        {
            icon = GetComponentsInChildren<Image>()[1];
            icon.sprite = data.itemIcon;

            Text[] texts = GetComponentsInChildren<Text>();
            textLevel = texts[0];
            textName = texts[1];
            textDesc = texts[2];
            textPrice = texts[3];
            textName.text = data.itemName;
        }

        void OnEnable()
        {
            textLevel.text = "Lv." + (level + 1);
            textPrice.text = "����:" + data.itemPrice;

            switch (data.itemType) {
                case ItemData.ItemType.Melee:
                case ItemData.ItemType.Range:
                case ItemData.ItemType.rare1:
                case ItemData.ItemType.rare2:
                case ItemData.ItemType.rare3:
                case ItemData.ItemType.rare4:
                    textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                    break;
                case ItemData.ItemType.Glove:
                case ItemData.ItemType.Shoe:
                    textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                    break;
                default:
                    textDesc.text = string.Format(data.itemDesc);
                    break;
            }
        }

        public void OnClick()
        {
            if (CoinManager.playerCoins >= data.itemPrice) // ���� ������ ���ݺ��� ���� �־�� ���� ����
            {
                StartCoroutine(onClickCoroutine());
            }
        }

        private IEnumerator onClickCoroutine()
        {
            CoinManager.playerCoins -= data.itemPrice; // ������ ���� ����

            switch (data.itemType)
            {
                case ItemData.ItemType.Melee:
                case ItemData.ItemType.Range:
                case ItemData.ItemType.rare1:
                case ItemData.ItemType.rare2:
                case ItemData.ItemType.rare3:
                case ItemData.ItemType.rare4:
                    if (level == 0)
                    {
                        GameObject newWeapon = new GameObject();
                        weapon = newWeapon.AddComponent<Weapon>();
                        weapon.Init(data);
                    }
                    else
                    {
                        float nextDamage = data.baseDamage;
                        int nextCount = 0;

                        nextDamage += data.baseDamage * data.damages[level];
                        nextCount += data.counts[level];

                        weapon.LevelUp(nextDamage, nextCount);
                    }

                    level++;
                    break;
                case ItemData.ItemType.newGun:
                    if (level == 0)
                    {
                        GameObject newWeapon = new GameObject();
                        weapon = newWeapon.AddComponent<Weapon_Long>();
                        weapon.Init(data);
                    }
                    else
                    {
                        float nextDamage = data.baseDamage;
                        int nextCount = 0;

                        nextDamage += data.baseDamage * data.damages[level];
                        nextCount += data.counts[level];

                        weapon.LevelUp(nextDamage, nextCount);
                    }

                    level++;
                    break;
                case ItemData.ItemType.newKnife:
                    if (level == 0)
                    {
                        GameObject newWeapon = new GameObject();
                        weapon = newWeapon.AddComponent<Weapon_knife>();
                        weapon.Init(data);
                    }
                    else
                    {
                        float nextDamage = data.baseDamage;
                        int nextCount = 0;

                        nextDamage += data.baseDamage * data.damages[level];

                        weapon.LevelUp(nextDamage, nextCount);
                    }

                    level++;
                    break;
                case ItemData.ItemType.arrow:
                    if (level == 0)
                    {
                        GameObject newWeapon = new GameObject();
                        weapon = newWeapon.AddComponent<Weapon_Long_slow>();
                        weapon.Init(data);
                    }
                    else
                    {
                        float nextDamage = data.baseDamage;
                        int nextCount = 0;

                        nextDamage += data.baseDamage * data.damages[level];

                        weapon.LevelUp(nextDamage, nextCount);
                    }

                    level++;
                    break;
                case ItemData.ItemType.rifle:
                    if (level == 0)
                    {
                        GameObject newWeapon = new GameObject();
                        weapon = newWeapon.AddComponent<Weapon_Long_fast>();
                        weapon.Init(data);
                    }
                    else
                    {
                        float nextDamage = data.baseDamage;
                        int nextCount = 0;

                        nextDamage += data.baseDamage * data.damages[level];

                        weapon.LevelUp(nextDamage, nextCount);
                    }

                    level++;
                    break;
                case ItemData.ItemType.meteor:
                    if (level == 0)
                    {
                        GameObject newWeapon = new GameObject();
                        weapon = newWeapon.AddComponent<Weapon_Rare1>();
                        weapon.Init(data);
                    }
                    else
                    {
                        float nextDamage = data.baseDamage;
                        int nextCount = 0;

                        nextDamage += data.baseDamage * data.damages[level];

                        weapon.LevelUp(nextDamage, nextCount);
                    }

                    level++;
                    break;
                case ItemData.ItemType.red:
                    if (level == 0)
                    {
                        GameObject newWeapon = new GameObject();
                        weapon = newWeapon.AddComponent<Weapon_knife>();
                        weapon.Init(data);
                    }
                    else
                    {
                        float nextDamage = data.baseDamage;
                        int nextCount = 0;

                        nextDamage += data.baseDamage * data.damages[level];

                        weapon.LevelUp(nextDamage, nextCount);
                    }

                    level++;
                    break;
                case ItemData.ItemType.gun2:
                    if (level == 0)
                    {
                        GameObject newWeapon = new GameObject();
                        weapon = newWeapon.AddComponent<Weapon_Long_fast2>();
                        weapon.Init(data);
                    }
                    else
                    {
                        float nextDamage = data.baseDamage;
                        int nextCount = 0;

                        nextDamage += data.baseDamage * data.damages[level];

                        weapon.LevelUp(nextDamage, nextCount);
                    }

                    level++;
                    break;
                case ItemData.ItemType.arrow2:
                    if (level == 0)
                    {
                        GameObject newWeapon = new GameObject();
                        weapon = newWeapon.AddComponent<Weapon_Long_slow2>();
                        weapon.Init(data);
                    }
                    else
                    {
                        float nextDamage = data.baseDamage;
                        int nextCount = 0;

                        nextDamage += data.baseDamage * data.damages[level];

                        weapon.LevelUp(nextDamage, nextCount);
                    }

                    level++;
                    break;
                case ItemData.ItemType.circle:
                    if (level == 0)
                    {
                        GameObject newWeapon = new GameObject();
                        weapon = newWeapon.AddComponent<Weapon_Circle>();
                        weapon.Init(data);
                    }
                    else
                    {
                        float nextDamage = data.baseDamage;
                        int nextCount = 0;

                        nextDamage += data.baseDamage * data.damages[level];

                        weapon.LevelUp(nextDamage, nextCount);
                    }

                    level++;
                    break;
                case ItemData.ItemType.Glove:
                case ItemData.ItemType.Shoe:
                    if (level == 0)
                    {
                        GameObject newGear = new GameObject();
                        gear = newGear.AddComponent<Gear>();
                        gear.Init(data);
                    }
                    else
                    {
                        float nextRate = data.damages[level];
                        gear.LevelUp(nextRate);
                    }

                    level++;
                    break;
                case ItemData.ItemType.Heal:
                    GameManager.instance.health = GameManager.instance.maxHealth;
                    break;
            }

            if (GameManager.instance.player.curWeapon == -1) // ���� ���Ⱑ ������.
            {
                GameManager.instance.player.curWeapon = 0;
                GameManager.instance.player.usingWeaponIdx[0] = data.itemId;
            }
            else if ((GameManager.instance.player.usingWeaponIdx[1] == -1) && (GameManager.instance.player.usingWeaponIdx[0] != data.itemId)) // ���� ���Ⱑ 1���̰� ���� ���� �ƴ� ��
            {
                GameManager.instance.player.usingWeaponIdx[1] = data.itemId;
                GameManager.instance.SwapWeapon(GameManager.instance.player.usingWeaponIdx[GameManager.instance.player.curWeapon]); // ���� ������ id
            }
            else // ���Ⱑ 2�� �ִ�.
            {
                // ���� ����ִ� ����� �� �ϳ��� ����
                if (GameManager.instance.player.usingWeaponIdx[0] == data.itemId || GameManager.instance.player.usingWeaponIdx[1] == data.itemId)
                {
                    if (GameManager.instance.player.usingWeaponIdx[GameManager.instance.player.curWeapon] != data.itemId)
                        GameManager.instance.SwapWeapon(GameManager.instance.player.usingWeaponIdx[GameManager.instance.player.curWeapon]);
                }
                else // ���ο� ���� ����, ���� ���� �� �ϳ��� ������.
                {
                    GameManager.instance.weaponPopup.isPopup = true;
                    GameManager.instance.weaponPopup.ShowPopup(); // ���� ���� ����â�� ����. â�� ������ �� ��ư Ŭ�� �̺�Ʈ����

                    while(GameManager.instance.weaponPopup.isPopup == true) // ��ư�� Ŭ���ϰ� �˾�â�� ������ ������ ���
                    {
                        if (GameManager.instance.isLive == true) 
                        {
                            GameManager.instance.Stop();
                        }
                        yield return null;
                    }

                    Debug.Log("���� ���� ����");
                    GameManager.instance.Resume();

                    GameManager.instance.player.usingWeaponIdx[GameManager.instance.player.curWeapon] = data.itemId;
                }
            }

            if (level == data.damages.Length)
            {
                GetComponent<Button>().interactable = false;
            }

            GameManager.instance.storeStd.ResetLevel();
        }
    }
}
