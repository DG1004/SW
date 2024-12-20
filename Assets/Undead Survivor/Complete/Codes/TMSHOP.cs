using Goldmetal.UndeadSurvivor;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using UnityEngine.EventSystems;

namespace Goldmetal.UndeadSurvivor
{
    public class TMSHOP : MonoBehaviour
    {
        RectTransform rect;
        Item[] items;

        public GameObject player;
        public GameObject travellingMerchant;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
            // ������ �� �����
        }

        public void Show()
        {

            rect.localScale = Vector3.one;
            GameManager.instance.Stop();
            AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
            AudioManager.instance.EffectBgm(true);

            if (travellingMerchant != null)
            {
                Destroy(travellingMerchant); // ��ü ����
                travellingMerchant = null;  // ���� �ʱ�ȭ
            }
            else
            {
            }
        }

        public void Hide()
        {
            rect.localScale = Vector3.zero;
            GameManager.instance.Resume();
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
            AudioManager.instance.EffectBgm(false);
            EventSystem.current.SetSelectedGameObject(null);

        }

        // ������ �ݴ� ��ư�� ������ �Լ�
        public void CloseShop()
        {
            if (travellingMerchant != null)
            {
                Destroy(travellingMerchant);
            }
            Hide();
            GameManager.instance.Resume(); // ���� �簳
        }
        public void Select(int index)
        {
            items[index].OnClick();
        }

        void Next()
        {
            // 1. ��� ������ ��Ȱ��ȭ
            foreach (Item item in items)
            {
                item.gameObject.SetActive(false);
            }

            // 2. �� �߿��� ���� 3�� ������ Ȱ��ȭ
            int[] ran = new int[3];
            while (true)
            {
                ran[0] = Random.Range(0, items.Length);
                ran[1] = Random.Range(0, items.Length);
                ran[2] = Random.Range(0, items.Length);

                if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
                    break;
            }

            for (int index = 0; index < ran.Length; index++)
            {
                Item ranItem = items[ran[index]];

                // 3. ���� �������� ���� �Һ���������� ��ü
                if (ranItem.level == ranItem.data.damages.Length)
                {
                    items[4].gameObject.SetActive(true);
                }
                else
                {
                    ranItem.gameObject.SetActive(true);
                }
            }
        }
        public void ResetLevel()
        {
            for (int i = 0; i < GameManager.instance.ItemGroup.Length; i++)
            {
                if (i >= 4 && i <= 7) // ���� ������ ���� �����
                {
                    if (GameManager.instance.player.usingWeaponIdx[0] != GameManager.instance.ItemGroup[i].data.itemId &&
                        GameManager.instance.player.usingWeaponIdx[1] != GameManager.instance.ItemGroup[i].data.itemId)
                    {
                        GameManager.instance.ItemGroup[i].level = 0;
                        GameManager.instance.ItemGroup[i].GetComponent<Button>().interactable = true;
                    }
                }
            }
        }
    }
}