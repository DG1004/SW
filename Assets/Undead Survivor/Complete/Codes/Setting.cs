using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Setting : MonoBehaviour
{
    public GameObject SettingPopup;

    public void ShowSetting()
    {
        if (SettingPopup.activeSelf == true) return;
        SettingPopup.SetActive(true);
        GameManager.instance.Stop();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void HideSetting()
    {
        if (SettingPopup.activeSelf == false) return;
        SettingPopup.SetActive(false);
        GameManager.instance.Resume();
        EventSystem.current.SetSelectedGameObject(null);
    }
}
