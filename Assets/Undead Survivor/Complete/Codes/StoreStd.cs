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
}
