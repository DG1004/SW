using UnityEngine;
using System.Collections;
using Goldmetal.UndeadSurvivor;
using static UnityEngine.RuleTile.TilingRuleOutput;
using System.ComponentModel;

public class EnemyRush1 : Enemy
{
    [Header("Distances & Times")]
    public float approachDistance = 7f;          // �÷��̾�� �� �Ÿ� �̻� ������ ������ ����
    public float angleChangeThreshold = 2f;      // ���� ��ȭ ��� ���� (�� ����)
    public float requiredStableTime = 1f;        // ���� ��ȭ ���� �����ؾ� �ϴ� �ð� (��)
    public float rushDuration = 1f;              // ���� ���� �ð� (��)
    float rushSpeedMultiplier = 7f;       // ���� �� �ӵ� ����

    private Quaternion initialRotation;          // ���� �� ������ �ʱ� �����̼�
    private float stableTime = 0f;               // ���� ���� �ð� üũ��
    private float lastAngle;                     // ���� �����ӿ��� �ٶ󺸴� ����
    private bool isRushing = false;              // ���� ���� ������ ����

    protected void Start()
    {
        initialRotation = transform.rotation;    // �ʱ� �����̼� ����
        lastAngle = GetCurrentAngle();           // �ʱ� ���� ���
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
            // ���� �� �̵��� ���� �ڷ�ƾ���� ó���ϹǷ� ���⼭�� ���� ����
            return;
        }

        if (distanceToPlayer > approachDistance)
        {
            // �÷��̾�� �Ÿ� 7 �̻��� ��: �׳� �÷��̾ ���� õõ�� �̵�
            ReturnToNormalState();
            MoveTowardsPlayer();
        }
        else
        {
            // �÷��̾�� �Ÿ� 7 ������ ��: ���� �غ� ����
            PrepareToRush();
        }
    }

    /// <summary>
    /// �÷��̾ ���� �̵��ϴ� �⺻ ����
    /// </summary>
    void MoveTowardsPlayer()
    {
        Vector2 dirVec = (target.position - rigid.position).normalized;
        Vector2 nextPos = rigid.position + dirVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextPos);
        rigid.velocity = Vector2.zero;
    }

    /// <summary>
    /// ���� �غ� ����: �÷��̾ �ٶ󺸰�, ���� �������� üũ�� �� ���� ���� �� ���� ����
    /// </summary>
    void PrepareToRush()
    {
        // �ִϸ��̼� �����Ͽ� ��� ���� ǥ��
        anim.speed = 0f;
        rigid.velocity = Vector2.zero;

        // �÷��̾� �������� ȸ��
        Vector2 dirVec = (target.position - (Vector2)transform.position).normalized;
        float currentAngle = Mathf.Atan2(-dirVec.x, dirVec.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);

        // ���� ��ȭ�� ���
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, lastAngle));

        // ���� ��ȭ���� �Ӱ谪 ���϶�� ���� �ð� ����, �ƴϸ� �ʱ�ȭ
        if (angleDifference <= angleChangeThreshold)
        {
            stableTime += Time.fixedDeltaTime;
        }
        else
        {
            stableTime = 0f;
        }

        // ���� �ð��� requiredStableTime �̻��̸� ���� ����
        if (stableTime >= requiredStableTime)
        {
            StartCoroutine(RushRoutine(dirVec));
        }

        lastAngle = currentAngle;
    }

    /// <summary>
    /// ���� ���� �ڷ�ƾ
    /// </summary>
    IEnumerator RushRoutine(Vector2 direction)
    {
        isRushing = true;
        anim.speed = 1f;  // ���� �� �ִϸ��̼� ���

        // ���� ����
        rigid.velocity = direction.normalized * speed * rushSpeedMultiplier;

        yield return new WaitForSeconds(rushDuration);

        // ���� ����
        RushEnd();
    }

    /// <summary>
    /// ���� ���� �� ȣ��: �����̼� �ʱ�ȭ, �ӵ� ����
    /// </summary>
    void RushEnd()
    {
        isRushing = false;
        rigid.velocity = Vector2.zero;
        transform.rotation = initialRotation; // �ʱ� �����̼� ����
    }

    /// <summary>
    /// �Ϲ� ���·� ����: �ִϸ��̼� �ӵ� ����ȭ, ���� ���� �ð� �ʱ�ȭ ��
    /// </summary>
    void ReturnToNormalState()
    {
        anim.speed = 1f;
        stableTime = 0f;
        isRushing = false;
        transform.rotation = initialRotation;
    }

    /// <summary>
    /// ���� ��ü�� �ٶ󺸴� ������ ��ȯ
    /// </summary>
    /// <returns>���� �ٶ󺸴� ����(�� ����)</returns>
    float GetCurrentAngle()
    {
        Vector2 forward = transform.right;
        return Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
    }

    protected override void race_init()//�������̵� �ʱ� ���� ���� (���� ���� ����)
    {
        this.race_index = 5;
        this.coe_attack = 0.01;
        this.coe_defence = 0.5;
        this.coe_race = 0.005;
        this.coe_speed = 0.25;
        this.maxhealth = 70;
    }
}
