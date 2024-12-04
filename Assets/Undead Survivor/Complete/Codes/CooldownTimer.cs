using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Goldmetal.UndeadSurvivor
{
    public class CooldownTimer : MonoBehaviour
    {
        public Text cooldownText;
        public Image cooldownImage;
        public float cooldownTime = 5f;  // 쿨다운 시간 설정 (초 단위)
        private bool isCooldown = false;

        private void Start()
        {
            cooldownText.gameObject.SetActive(false);  // 처음에는 텍스트를 보이지 않게 설정
            cooldownImage.gameObject.SetActive(false);  // 처음에는 이미지를 보이지 않게 설정
        }

        private void Update()
        {
            // GameManager.instance.isLive가 false면 쿨타임 로직을 실행하지 않음
            if (!GameManager.instance.isLive)
                return;

            if (Input.GetKeyDown(KeyCode.Space) && !isCooldown)
            {
                cooldownText.gameObject.SetActive(true);  // 스페이스를 누르면 텍스트를 보이게 설정
                cooldownImage.gameObject.SetActive(true);  // 스페이스를 누르면 이미지를 보이게 설정
                StartCoroutine(StartCooldown());
            }
        }

        private IEnumerator StartCooldown()
        {
            isCooldown = true;
            float currentTime = cooldownTime;

            while (currentTime > 0)
            {
                // GameManager.instance.isLive가 false면 타이머를 멈춤
                if (!GameManager.instance.isLive)
                {
                    yield return null;
                    continue;
                }

                cooldownText.text = currentTime.ToString("0");
                yield return new WaitForSeconds(1f);  // 1초 대기
                currentTime--;
            }

            isCooldown = false;
            cooldownText.gameObject.SetActive(false);  // 텍스트 숨김
            cooldownImage.gameObject.SetActive(false);  // 이미지 숨김
        }
    }
}
