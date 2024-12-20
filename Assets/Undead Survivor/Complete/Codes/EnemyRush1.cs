using UnityEngine;
using System.Collections;
using Goldmetal.UndeadSurvivor;
using static UnityEngine.RuleTile.TilingRuleOutput;
using System.ComponentModel;

public class EnemyRush1 : Enemy
{
    [Header("Distances & Times")]
    public float approachDistance = 7f;          // 플레이어와 이 거리 이상 떨어져 있으면 접근
    public float angleChangeThreshold = 2f;      // 각도 변화 허용 범위 (도 단위)
    public float requiredStableTime = 1f;        // 각도 변화 없이 유지해야 하는 시간 (초)
    public float rushDuration = 1f;              // 돌진 지속 시간 (초)
    float rushSpeedMultiplier = 7f;       // 돌진 시 속도 배율

    private Quaternion initialRotation;          // 시작 시 몬스터의 초기 로테이션
    private float stableTime = 0f;               // 각도 안정 시간 체크용
    private float lastAngle;                     // 이전 프레임에서 바라보던 각도
    private bool isRushing = false;              // 현재 돌진 중인지 여부

    protected void Start()
    {
        initialRotation = transform.rotation;    // 초기 로테이션 저장
        lastAngle = GetCurrentAngle();           // 초기 각도 기록
    }

    protected override void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        float distanceToPlayer = Vector2.Distance(target.position, rigid.position);

        if (isRushing)
        {
            // 돌진 중 이동은 별도 코루틴에서 처리하므로 여기서는 로직 없음
            return;
        }

        if (distanceToPlayer > approachDistance)
        {
            // 플레이어와 거리 7 이상일 때: 그냥 플레이어를 향해 천천히 이동
            ReturnToNormalState();
            MoveTowardsPlayer();
        }
        else
        {
            // 플레이어와 거리 7 이하일 때: 돌진 준비 상태
            PrepareToRush();
        }
    }

    /// <summary>
    /// 플레이어를 향해 이동하는 기본 로직
    /// </summary>
    void MoveTowardsPlayer()
    {
        Vector2 dirVec = (target.position - rigid.position).normalized;
        Vector2 nextPos = rigid.position + dirVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextPos);
        rigid.velocity = Vector2.zero;
    }

    /// <summary>
    /// 돌진 준비 상태: 플레이어를 바라보고, 각도 안정성을 체크한 뒤 조건 충족 시 돌진 시작
    /// </summary>
    void PrepareToRush()
    {
        // 애니메이션 정지하여 대기 상태 표현
        anim.speed = 0f;
        rigid.velocity = Vector2.zero;

        // 플레이어 방향으로 회전
        Vector2 dirVec = (target.position - (Vector2)transform.position).normalized;
        float currentAngle = Mathf.Atan2(-dirVec.x, dirVec.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);

        // 각도 변화량 계산
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, lastAngle));

        // 각도 변화량이 임계값 이하라면 안정 시간 증가, 아니면 초기화
        if (angleDifference <= angleChangeThreshold)
        {
            stableTime += Time.fixedDeltaTime;
        }
        else
        {
            stableTime = 0f;
        }

        // 안정 시간이 requiredStableTime 이상이면 돌진 시작
        if (stableTime >= requiredStableTime)
        {
            StartCoroutine(RushRoutine(dirVec));
        }

        lastAngle = currentAngle;
    }

    /// <summary>
    /// 돌진 실행 코루틴
    /// </summary>
    IEnumerator RushRoutine(Vector2 direction)
    {
        isRushing = true;
        anim.speed = 1f;  // 돌진 중 애니메이션 재생

        // 돌진 시작
        rigid.velocity = direction.normalized * speed * rushSpeedMultiplier;

        yield return new WaitForSeconds(rushDuration);

        // 돌진 종료
        RushEnd();
    }

    /// <summary>
    /// 돌진 종료 시 호출: 로테이션 초기화, 속도 리셋
    /// </summary>
    void RushEnd()
    {
        isRushing = false;
        rigid.velocity = Vector2.zero;
        transform.rotation = initialRotation; // 초기 로테이션 복구
    }

    /// <summary>
    /// 일반 상태로 복귀: 애니메이션 속도 정상화, 각도 안정 시간 초기화 등
    /// </summary>
    void ReturnToNormalState()
    {
        anim.speed = 1f;
        stableTime = 0f;
        isRushing = false;
        transform.rotation = initialRotation;
    }

    /// <summary>
    /// 현재 객체가 바라보는 각도를 반환
    /// </summary>
    /// <returns>현재 바라보는 각도(도 단위)</returns>
    float GetCurrentAngle()
    {
        Vector2 forward = transform.right;
        return Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
    }

    protected override void race_init()//오버라이딩 초기 스탯 결정 (기존 로직 유지)
    {
        this.race_index = 5;
        this.coe_attack = 0.01;
        this.coe_defence = 0.5;
        this.coe_race = 0.005;
        this.coe_speed = 0.25;
        this.maxhealth = 70;
    }
}
