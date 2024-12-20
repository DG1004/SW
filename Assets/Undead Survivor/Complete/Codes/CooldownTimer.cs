using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Goldmetal.UndeadSurvivor
{
    public class CooldownTimer : MonoBehaviour
    {
        [Header("Dash Skill UI")]
        public Text dashCooldownText;
        public Image dashCooldownImage;
        public float dashCooldownTime = 3f;
        private bool isDashCooldown = false;

        [Header("Ghost Skill UI")]
        public Text ghostCooldownText;
        public Image ghostCooldownImage;
        public float ghostCooldownTime = 30f;
        private bool isGhostCooldown = false;
        private bool isGhostSkillPurchased = false;

        [Header("Enhence Skill UI")]
        public Text enhenceCooldownText;
        public Image enhenceCooldownImage;
        public float enhenceCooldownTime = 30f;
        private bool isEnhenceCooldown = false;
        private bool isEnhenceSkillPurchased = false;

        [Header("Heal Skill UI")]
        public Text healCooldownText;
        public Image healCooldownImage;
        public float healCooldownTime = 60f;
        private bool isHealCooldown = false;
        private bool isHealSkillPurchased = false;

        private void Start()
        {
            // 처음에는 전부 안보이게
            SetUIVisible(dashCooldownText, dashCooldownImage, false);
            SetUIVisible(ghostCooldownText, ghostCooldownImage, false);
            SetUIVisible(enhenceCooldownText, enhenceCooldownImage, false);
            SetUIVisible(healCooldownText, healCooldownImage, false);
        }

        private void Update()
        {
            // GameManager.instance.isLive가 false면 쿨타임 로직을 실행하지 않음
            if (!GameManager.instance.isLive)
                return;

            // Dash 스킬 (Space)
            if (Input.GetKeyDown(GameManager.instance.dashKey) && !isDashCooldown)
            {
                StartCoroutine(StartCooldown(dashCooldownTime, dashCooldownText, dashCooldownImage,
                    (val) => isDashCooldown = val));
            }

            // Ghost 스킬 (Q)
            if (Input.GetKeyDown(GameManager.instance.ghostKey) && !isGhostCooldown && isGhostSkillPurchased)
            {
                StartCoroutine(StartCooldown(ghostCooldownTime, ghostCooldownText, ghostCooldownImage,
                    (val) => isGhostCooldown = val));
            }

            // Enhence 스킬 (W)
            if (Input.GetKeyDown(GameManager.instance.enhenceKey) && !isEnhenceCooldown && isEnhenceSkillPurchased)
            {
                StartCoroutine(StartCooldown(enhenceCooldownTime, enhenceCooldownText, enhenceCooldownImage,
                    (val) => isEnhenceCooldown = val));
            }

            // Heal 스킬 (E)
            if (Input.GetKeyDown(GameManager.instance.healKey) && !isHealCooldown && isHealSkillPurchased)
            {
                StartCoroutine(StartCooldown(healCooldownTime, healCooldownText, healCooldownImage,
                    (val) => isHealCooldown = val));
            }
        }

        private IEnumerator StartCooldown(float cooldown, Text uiText, Image uiImage, System.Action<bool> setIsCooldown)
        {
            setIsCooldown(true);
            SetUIVisible(uiText, uiImage, true);
            float currentTime = cooldown;

            while (currentTime > 0)
            {
                // GameManager.instance.isLive가 false면 타이머를 멈춤
                if (!GameManager.instance.isLive)
                {
                    yield return null;
                    continue;
                }

                uiText.text = currentTime.ToString("0");
                yield return new WaitForSeconds(1f);  // 1초 대기
                currentTime--;
            }

            uiText.text = ""; // 텍스트 초기화
            SetUIVisible(uiText, uiImage, false); // UI 숨김
            setIsCooldown(false);
        }

        private void SetUIVisible(Text text, Image image, bool visible)
        {
            if (text != null) text.gameObject.SetActive(visible);
            if (image != null) image.gameObject.SetActive(visible);
        }

        // 스킬 구매 상태 업데이트 메서드
        public void PurchaseGhostSkill()
        {
            isGhostSkillPurchased = true;
        }

        public void PurchaseEnhenceSkill()
        {
            isEnhenceSkillPurchased = true;
        }

        public void PurchaseHealSkill()
        {
            isHealSkillPurchased = true;
        }
    }
}
