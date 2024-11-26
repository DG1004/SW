using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreStd : MonoBehaviour
{
	RectTransform rect;

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
	}

	// ������ �ݴ� ��ư�� ������ �Լ�
	public void CloseShop()
	{
		Hide();
	}
}
