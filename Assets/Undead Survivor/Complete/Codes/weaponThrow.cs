using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class weaponThrow : MonoBehaviour
{
    public GameObject PopupWindow;
    public Button weapon1Button;   // 첫 번째 무기 버튼
    public Button weapon2Button;   // 두 번째 무기 버튼

    public Sprite defaultIcon;     // 기본 아이콘 (무기 정보가 없을 때 표시)
    public string defaultName = "Empty"; // 기본 이름

    public bool isPopup = false;
    public void ShowPopup()
    {
        PopupWindow.SetActive(true);

        int weapon1Id = GameManager.instance.player.usingWeaponIdx[0];
        int weapon2Id = GameManager.instance.player.usingWeaponIdx[1];

        // 버튼 정보 초기화
        UpdateButtonInfo(weapon1Button, weapon1Id);
        UpdateButtonInfo(weapon2Button, weapon2Id);

        // 버튼 클릭 이벤트 등록
        weapon1Button.onClick.RemoveAllListeners(); // 기존 리스너 제거
        weapon1Button.onClick.AddListener(() => ChooseWeapon(weapon1Id)); // 첫 번째 무기 선택
        weapon2Button.onClick.RemoveAllListeners();
        weapon2Button.onClick.AddListener(() => ChooseWeapon(weapon2Id)); // 두 번째 무기 선택
    }

    public void HidePopup()
    {
        PopupWindow.SetActive(false);
        isPopup = false;
    }

    private void UpdateButtonInfo(Button button, int weaponId)
    {
        // 버튼의 자식 요소 (텍스트와 이미지) 가져오기
        Text weaponText = button.GetComponentInChildren<Text>();
        Image weaponImage = button.transform.Find("Image").GetComponent<Image>();

        if (weaponId >= 0) // 유효한 무기 ID인 경우
        {
            Transform weaponTrs = GameManager.instance.player.transform.Find("Weapon " + weaponId); // 현재 장착한 무기를 가져오는 함수
            ItemData itemData = GameManager.instance.ItemGroup[weaponId - 9].data; // 무기 정보를 가져오는 함수

            weaponText.text = itemData.itemName;          // 무기 이름 업데이트
            weaponImage.sprite = itemData.itemIcon; 
            // 무기 이미지를 가져온다. (ItemGroup에는 1~8번 무기(옛날 무기)가 없어서 -9를 한다.)
        }
        else // 무기가 없는 경우 기본 값 설정
        {
            weaponText.text = defaultName;
            weaponImage.sprite = defaultIcon;
        }
    }

    public void ChooseWeapon(int weaponIdx)
    {
        Debug.Log(weaponIdx);
        if (weaponIdx == GameManager.instance.player.usingWeaponIdx[0])
        {
            GameManager.instance.RemoveWeapon(GameManager.instance.player.usingWeaponIdx[0]);
            GameManager.instance.player.curWeapon = 0;
        }
        else
        {
            GameManager.instance.RemoveWeapon(GameManager.instance.player.usingWeaponIdx[1]);
            GameManager.instance.player.curWeapon = 1;
        }

        HidePopup();
    }
}