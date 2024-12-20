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
    public Button weapon1Button;   // ù ��° ���� ��ư
    public Button weapon2Button;   // �� ��° ���� ��ư

    public Sprite defaultIcon;     // �⺻ ������ (���� ������ ���� �� ǥ��)
    public string defaultName = "Empty"; // �⺻ �̸�

    public bool isPopup = false;
    public void ShowPopup()
    {
        PopupWindow.SetActive(true);

        int weapon1Id = GameManager.instance.player.usingWeaponIdx[0];
        int weapon2Id = GameManager.instance.player.usingWeaponIdx[1];

        // ��ư ���� �ʱ�ȭ
        UpdateButtonInfo(weapon1Button, weapon1Id);
        UpdateButtonInfo(weapon2Button, weapon2Id);

        // ��ư Ŭ�� �̺�Ʈ ���
        weapon1Button.onClick.RemoveAllListeners(); // ���� ������ ����
        weapon1Button.onClick.AddListener(() => ChooseWeapon(weapon1Id)); // ù ��° ���� ����
        weapon2Button.onClick.RemoveAllListeners();
        weapon2Button.onClick.AddListener(() => ChooseWeapon(weapon2Id)); // �� ��° ���� ����
    }

    public void HidePopup()
    {
        PopupWindow.SetActive(false);
        isPopup = false;
    }

    private void UpdateButtonInfo(Button button, int weaponId)
    {
        // ��ư�� �ڽ� ��� (�ؽ�Ʈ�� �̹���) ��������
        Text weaponText = button.GetComponentInChildren<Text>();
        Image weaponImage = button.transform.Find("Image").GetComponent<Image>();

        if (weaponId >= 0) // ��ȿ�� ���� ID�� ���
        {
            Transform weaponTrs = GameManager.instance.player.transform.Find("Weapon " + weaponId); // ���� ������ ���⸦ �������� �Լ�
            ItemData itemData = GameManager.instance.ItemGroup[weaponId - 9].data; // ���� ������ �������� �Լ�

            weaponText.text = itemData.itemName;          // ���� �̸� ������Ʈ
            weaponImage.sprite = itemData.itemIcon; 
            // ���� �̹����� �����´�. (ItemGroup���� 1~8�� ����(���� ����)�� ��� -9�� �Ѵ�.)
        }
        else // ���Ⱑ ���� ��� �⺻ �� ����
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