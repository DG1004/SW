using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StoreStd : MonoBehaviour
{
	RectTransform rect;
    Item[] items;

    void Awake()
	{
		rect = GetComponent<RectTransform>();
		// ������ �� �����
	}

	public void Show()
	{
		rect.localScale = Vector3.one;
		AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
		AudioManager.instance.EffectBgm(true);
	}

	public void Hide()
	{
		rect.localScale = Vector3.zero;
		AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
		AudioManager.instance.EffectBgm(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

	// ������ �ݴ� ��ư�� ������ �Լ�
	public void CloseShop()
	{
		Hide();
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
            if ((i >= 0 && i <= 3) || i == 8) // ���� �Ϲ� ���� �����
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
