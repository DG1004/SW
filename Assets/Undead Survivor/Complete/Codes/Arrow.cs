using Goldmetal.UndeadSurvivor;
using UnityEngine;

public class Arrow : MonoBehaviour
{
	public Transform target; // 화살표가 가리킬 대상 (물체)
	public RectTransform arrow; // 화살표 UI 이미지
    public RectTransform objectImage; // 물체의 이미지 UI
    public Camera mainCamera; // 메인 카메라
	public Transform player; // 플레이어 (기준점)


	void Update()
	{
		// 게임이 진행중일 때만 실행한다
		if (!GameManager.instance.isLive || target == null || arrow == null || mainCamera == null || player == null)
			return;

		// 물체와 플레이어의 화면 좌표
		Vector3 targetScreenPosition = mainCamera.WorldToScreenPoint(target.position);
		Vector3 playerScreenPosition = mainCamera.WorldToScreenPoint(player.position);

		// 물체가 화면 안에 있는지 체크
		// 물체와 가까울 때 화살표에 오차가 있어서 +-30으로 물체가 보이기 전에 화살표를 없앰
		bool isOffScreen = targetScreenPosition.z > 0 &&
						   (targetScreenPosition.x < -30 || targetScreenPosition.x > Screen.width+30 ||
							targetScreenPosition.y < -30 || targetScreenPosition.y > Screen.height+30);

		if (isOffScreen)
		{
			// 물체가 화면 밖에 있으면 화살표 활성화
			GetComponent<RectTransform>().localScale = Vector3.one;

            // 물체 방향 계산 (물체 -> 플레이어)
            Vector3 direction = (targetScreenPosition - playerScreenPosition).normalized;

			// 화면 가장자리에 위치 계산
			Vector3 edgePosition = playerScreenPosition + direction * Mathf.Min(Screen.width, Screen.height);
			edgePosition = ClampToScreenEdge(edgePosition);

            // 화살표 위치와 회전 설정
            arrow.position = edgePosition;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			arrow.rotation = Quaternion.Euler(0, 0, angle);

            // 회전 초기화
            objectImage.rotation = Quaternion.identity;
        }
        else
		{
            // 물체가 화면 안에 있으면 화살표 비활성화
            //rect = GetComponent<RectTransform>();
            //rect.localScale = Vector3.zero;
			GetComponent<RectTransform>().localScale = Vector3.zero;
            //gameObject.SetActive(false);
		}
	}

	// 화면 가장자리로 위치를 제한하는 함수
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

		// 화면 경계에 약간의 여유 공간 추가 (화살표가 잘리지 않도록)
		clampedPosition.x = Mathf.Clamp(clampedPosition.x, 50, Screen.width - 50);
		clampedPosition.y = Mathf.Clamp(clampedPosition.y, 50, Screen.height - 50);

		return clampedPosition;
	}
}