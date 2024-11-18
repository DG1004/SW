using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class travellingMerchant : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed;
    public Rigidbody2D target;

    bool isTouched;

    Rigidbody2D rigid;

    SpriteRenderer spriter;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();

    }
    private void FixedUpdate()
    {
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position - nextVec);
        rigid.velocity = Vector2.zero;

    }
}
