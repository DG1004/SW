namespace Goldmetal.UndeadSurvivor
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    public class CooldownTimer : MonoBehaviour
    {
        public Text cooldownText; 
        public Image cooldownImage;  
        public float cooldownTime = 5f;  // 쿨다운 시간 설정 (초 단위)
        private bool isCooldown = false;
        private bool isPaused = false;

        private void Start()
        {
            cooldownText.gameObject.SetActive(false);  // 처음에는 텍스트를 보이지 않게 설정
            cooldownImage.gameObject.SetActive(false);  // 처음에는 이미지를 보이지 않게 설정
        }

        private void Update()
        {
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
               
                    cooldownText.text = currentTime.ToString("0");
                    yield return new WaitForSeconds(1f);  // 1초 대기
                    currentTime--;
               
            }

            isCooldown = false;
            cooldownText.gameObject.SetActive(false);  // 처음에는 텍스트를 보이지 않게 설정
            cooldownImage.gameObject.SetActive(false);  // 처음에는 이미지를 보이지 않게 설정
        }
    }
}