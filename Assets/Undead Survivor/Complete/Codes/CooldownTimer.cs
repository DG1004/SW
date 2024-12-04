namespace Goldmetal.UndeadSurvivor
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    public class CooldownTimer : MonoBehaviour
    {
        public Text cooldownText; 
        public Image cooldownImage;  
        public float cooldownTime = 5f;  // ��ٿ� �ð� ���� (�� ����)
        private bool isCooldown = false;
        private bool isPaused = false;

        private void Start()
        {
            cooldownText.gameObject.SetActive(false);  // ó������ �ؽ�Ʈ�� ������ �ʰ� ����
            cooldownImage.gameObject.SetActive(false);  // ó������ �̹����� ������ �ʰ� ����
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isCooldown)
            {
                cooldownText.gameObject.SetActive(true);  // �����̽��� ������ �ؽ�Ʈ�� ���̰� ����
                cooldownImage.gameObject.SetActive(true);  // �����̽��� ������ �̹����� ���̰� ����
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
                    yield return new WaitForSeconds(1f);  // 1�� ���
                    currentTime--;
               
            }

            isCooldown = false;
            cooldownText.gameObject.SetActive(false);  // ó������ �ؽ�Ʈ�� ������ �ʰ� ����
            cooldownImage.gameObject.SetActive(false);  // ó������ �̹����� ������ �ʰ� ����
        }
    }
}