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
        // ������ �� �����
       
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

}
