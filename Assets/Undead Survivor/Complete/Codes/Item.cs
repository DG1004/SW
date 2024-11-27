using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        Text textPrice; // 무기의 가격을 표시하는 텍스트

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
            textPrice.text = "가격:" + data.itemPrice;

            switch (data.itemType) {
                case ItemData.ItemType.Melee:
                case ItemData.ItemType.Range:
                case ItemData.ItemType.rare1:
                case ItemData.ItemType.rare2:
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
            if (CoinManager.playerCoins >= data.itemPrice) // 돈이 아이템 가격보다 많이 있어야 구매 가능
            {
                CoinManager.playerCoins -= data.itemPrice; // 아이템 가격 지불

                switch (data.itemType)
                {
                    case ItemData.ItemType.Melee:
                    case ItemData.ItemType.Range:
                    case ItemData.ItemType.rare1:
                    case ItemData.ItemType.rare2:
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

                if (level == data.damages.Length)
                {
                    GetComponent<Button>().interactable = false;
                }
            }
        }
    }
}
