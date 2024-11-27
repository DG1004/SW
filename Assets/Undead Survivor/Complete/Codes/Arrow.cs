using Goldmetal.UndeadSurvivor;
using UnityEngine;

public class Arrow : MonoBehaviour
{
	public Transform target; // ȭ��ǥ�� ����ų ��� (��ü)
	public RectTransform arrow; // ȭ��ǥ UI �̹���
    public RectTransform objectImage; // ��ü�� �̹��� UI
    public Camera mainCamera; // ���� ī�޶�
	public Transform player; // �÷��̾� (������)


	void Update()
	{
		// ������ �������� ���� �����Ѵ�
		if (!GameManager.instance.isLive || target == null || arrow == null || mainCamera == null || player == null)
			return;

		// ��ü�� �÷��̾��� ȭ�� ��ǥ
		Vector3 targetScreenPosition = mainCamera.WorldToScreenPoint(target.position);
		Vector3 playerScreenPosition = mainCamera.WorldToScreenPoint(player.position);

		// ��ü�� ȭ�� �ȿ� �ִ��� üũ
		// ��ü�� ����� �� ȭ��ǥ�� ������ �־ +-30���� ��ü�� ���̱� ���� ȭ��ǥ�� ����
		bool isOffScreen = targetScreenPosition.z > 0 &&
						   (targetScreenPosition.x < -30 || targetScreenPosition.x > Screen.width+30 ||
							targetScreenPosition.y < -30 || targetScreenPosition.y > Screen.height+30);

		if (isOffScreen)
		{
			// ��ü�� ȭ�� �ۿ� ������ ȭ��ǥ Ȱ��ȭ
			GetComponent<RectTransform>().localScale = Vector3.one;

            // ��ü ���� ��� (��ü -> �÷��̾�)
            Vector3 direction = (targetScreenPosition - playerScreenPosition).normalized;

			// ȭ�� �����ڸ��� ��ġ ���
			Vector3 edgePosition = playerScreenPosition + direction * Mathf.Min(Screen.width, Screen.height);
			edgePosition = ClampToScreenEdge(edgePosition);

            // ȭ��ǥ ��ġ�� ȸ�� ����
            arrow.position = edgePosition;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			arrow.rotation = Quaternion.Euler(0, 0, angle);

            // ȸ�� �ʱ�ȭ
            objectImage.rotation = Quaternion.identity;
        }
        else
		{
            // ��ü�� ȭ�� �ȿ� ������ ȭ��ǥ ��Ȱ��ȭ
            //rect = GetComponent<RectTransform>();
            //rect.localScale = Vector3.zero;
			GetComponent<RectTransform>().localScale = Vector3.zero;
            //gameObject.SetActive(false);
		}
	}

	// ȭ�� �����ڸ��� ��ġ�� �����ϴ� �Լ�
	private Vector3 ClampToScreenEdge(Vector3 position)
	{
		Vector3 clampedPosition = position;

		if (position.x < 0)
			clampedPosition.x = 0;
		if (position.x > Screen.width)
			clampedPosition.x = Screen.width;
		if (position.y < 0)
			clampedPosition.y = 0;
		if (position.y > Screen.height)
			clampedPosition.y = Screen.height;

		// ȭ�� ��迡 �ణ�� ���� ���� �߰� (ȭ��ǥ�� �߸��� �ʵ���)
		clampedPosition.x = Mathf.Clamp(clampedPosition.x, 50, Screen.width - 50);
		clampedPosition.y = Mathf.Clamp(clampedPosition.y, 50, Screen.height - 50);

		return clampedPosition;
	}
}