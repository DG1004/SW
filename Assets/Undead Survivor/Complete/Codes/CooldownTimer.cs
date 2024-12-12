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

        [Header("Enhence Skill UI")]
        public Text enhenceCooldownText;
        public Image enhenceCooldownImage;
        public float enhenceCooldownTime = 30f;
        private bool isEnhenceCooldown = false;

        [Header("Heal Skill UI")]
        public Text healCooldownText;
        public Image healCooldownImage;
        public float healCooldownTime = 60f;
        private bool isHealCooldown = false;

        private void Start()
        {
            // ó������ ���� �Ⱥ��̰�
            SetUIVisible(dashCooldownText, dashCooldownImage, false);
            SetUIVisible(ghostCooldownText, ghostCooldownImage, false);
            SetUIVisible(enhenceCooldownText, enhenceCooldownImage, false);
            SetUIVisible(healCooldownText, healCooldownImage, false);
        }

        private void Update()
        {
            // GameManager.instance.isLive�� false�� ��Ÿ�� ������ �������� ����
            if (!GameManager.instance.isLive)
                return;

            // Dash ��ų (Space)
            if (Input.GetKeyDown(GameManager.instance.dashKey) && !isDashCooldown)
            {
                StartCoroutine(StartCooldown(dashCooldownTime, dashCooldownText, dashCooldownImage,
                    (val) => isDashCooldown = val));
            }

            // Ghost ��ų (Q)
            if (Input.GetKeyDown(GameManager.instance.ghostKey) && !isGhostCooldown)
            {
                StartCoroutine(StartCooldown(ghostCooldownTime, ghostCooldownText, ghostCooldownImage,
                    (val) => isGhostCooldown = val));
            }

            // Enhence ��ų (W)
            if (Input.GetKeyDown(GameManager.instance.enhenceKey) && !isEnhenceCooldown)
            {
                StartCoroutine(StartCooldown(enhenceCooldownTime, enhenceCooldownText, enhenceCooldownImage,
                    (val) => isEnhenceCooldown = val));
            }

            // Heal ��ų (E)
            if (Input.GetKeyDown(GameManager.instance.healKey) && !isHealCooldown)
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
                // GameManager.instance.isLive�� false�� Ÿ�̸Ӹ� ����
                if (!GameManager.instance.isLive)
                {
                    yield return null;
                    continue;
                }

                uiText.text = currentTime.ToString("0");
                yield return new WaitForSeconds(1f);  // 1�� ���
                currentTime--;
            }

            setIsCooldown(false);
            SetUIVisible(uiText, uiImage, false);
        }

        private void SetUIVisible(Text text, Image image, bool visible)
        {
            if (text != null) text.gameObject.SetActive(visible);
            if (image != null) image.gameObject.SetActive(visible);
        }
    }
}
