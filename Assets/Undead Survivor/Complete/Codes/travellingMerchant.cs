using Goldmetal.UndeadSurvivor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class travellingMerchant : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed;
    public Rigidbody2D target;

    bool isTouched = true;

    Rigidbody2D rigid;

    SpriteRenderer spriter;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();

    }
    private void FixedUpdate()
    {

        if (!isTouched)
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position - nextVec);
        rigid.velocity = Vector2.zero;

    }
    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // 이 객체를 파괴하여 사라지게 합니다.
        }
    }

    private void LateUpdate()
    {
        spriter.flipX = target.position.x > rigid.position.x;
    }
}
