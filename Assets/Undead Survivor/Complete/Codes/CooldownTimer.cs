using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Goldmetal.UndeadSurvivor
{
    public class CooldownTimer : MonoBehaviour
    {
        public Text cooldownText;
        public Image cooldownImage;
        public float cooldownTime = 5f;  // ��ٿ� �ð� ���� (�� ����)
        private bool isCooldown = false;

        private void Start()
        {
            cooldownText.gameObject.SetActive(false);  // ó������ �ؽ�Ʈ�� ������ �ʰ� ����
            cooldownImage.gameObject.SetActive(false);  // ó������ �̹����� ������ �ʰ� ����
        }

        private void Update()
        {
            // GameManager.instance.isLive�� false�� ��Ÿ�� ������ �������� ����
            if (!GameManager.instance.isLive)
                return;

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
                // GameManager.instance.isLive�� false�� Ÿ�̸Ӹ� ����
                if (!GameManager.instance.isLive)
                {
                    yield return null;
                    continue;
                }

                cooldownText.text = currentTime.ToString("0");
                yield return new WaitForSeconds(1f);  // 1�� ���
                currentTime--;
            }

            isCooldown = false;
            cooldownText.gameObject.SetActive(false);  // �ؽ�Ʈ ����
            cooldownImage.gameObject.SetActive(false);  // �̹��� ����
        }
    }
}
