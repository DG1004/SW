using Goldmetal.UndeadSurvivor;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMSHOP : MonoBehaviour
{
    RectTransform rect;

    public GameObject player;
    public GameObject travellingMerchant;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        // 시작할 때 숨기기
       
    }

    public void Show()
    {
       
       rect.localScale = Vector3.one;
        GameManager.instance.Stop();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true);

    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);
    }

    // 상점을 닫는 버튼에 연결할 함수
    public void CloseShop()
    {
        if (travellingMerchant != null)
        {
            Destroy(travellingMerchant);
        }
        Hide();
        GameManager.instance.Resume(); // 게임 재개
    }

}
