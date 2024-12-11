using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreStd : MonoBehaviour
{
	RectTransform rect;
    Item[] items;

    public Item[] ItemGroup;

    void Awake()
	{
		rect = GetComponent<RectTransform>();
		// 시작할 때 숨기기
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
	}

	// 상점을 닫는 버튼에 연결할 함수
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
        // 1. 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        // 2. 그 중에서 랜덤 3개 아이템 활성화
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

            // 3. 만렙 아이템의 경우는 소비아이템으로 대체
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
        for (int i = 0; i < ItemGroup.Length; i++) 
        {
            if (GameManager.instance.player.usingWeaponIdx[0] != ItemGroup[i].data.itemId &&
                GameManager.instance.player.usingWeaponIdx[1] != ItemGroup[i].data.itemId) 
            {
                ItemGroup[i].level = 0;
                ItemGroup[i].GetComponent<Button>().interactable = true;
            }
        }
    }
}
