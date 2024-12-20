using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Goldmetal.UndeadSurvivor
{
    public class WIZARDSHOP : MonoBehaviour
    {
        RectTransform rect;
        public GameObject player;
        public GameObject wizard;

        public Button ghostSkillButton;
        public Button healSkillButton;
        public Button enhenceSkillButton;

        public Image ghostSkillCover;
        public Image healSkillCover;
        public Image enhenceSkillCover;

        private bool ghostSkillPurchased = false;
        private bool healSkillPurchased = false;
        private bool enhenceSkillPurchased = false;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
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
            EventSystem.current.SetSelectedGameObject(null);

        }

        public void CloseShop()
        {
            Hide();
        }

        public void PurchaseGhostSkill()
        {
            if (!ghostSkillPurchased && CoinManager.playerCoins >= 50000)
            {
                CoinManager.playerCoins -= 50000;
                ghostSkillPurchased = true;
                ghostSkillCover.gameObject.SetActive(false);
                ghostSkillButton.interactable = false;

                // Ghost ��ų Ȱ��ȭ ó��
                var playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.PurchaseGhostSkill(); // ���� ���� ������Ʈ
                    playerScript.canGhost = true;
                }
            }
        }

        public void PurchaseHealSkill()
        {
            if (!healSkillPurchased && CoinManager.playerCoins >= 50000)
            {
                CoinManager.playerCoins -= 50000;
                healSkillPurchased = true;
                healSkillCover.gameObject.SetActive(false);
                healSkillButton.interactable = false;

                // Heal ��ų Ȱ��ȭ ó��
                var playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.PurchaseHealSkill(); // ���� ���� ������Ʈ
                    playerScript.canHeal = true;
                }
            }
        }

        public void PurchaseEnhenceSkill()
        {
            if (!enhenceSkillPurchased && CoinManager.playerCoins >= 50000)
            {
                CoinManager.playerCoins -= 50000;
                enhenceSkillPurchased = true;
                enhenceSkillCover.gameObject.SetActive(false);
                enhenceSkillButton.interactable = false;

                // Enhence ��ų Ȱ��ȭ ó��
                var playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.PurchaseEnhenceSkill(); // ���� ���� ������Ʈ
                    playerScript.canEnhence = true;
                }
            }
        }
    }
}