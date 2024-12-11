using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goldmetal.UndeadSurvivor
{
    public class Bullet : MonoBehaviour
    {
        public float damage;
        public int per;
        public float speed = 15f; // ±âº»°ª 15

        Rigidbody2D rigid;

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            transform.localScale = Vector3.one;
        }

        public void Init(float damage, int per, Vector3 dir)
        {
            this.damage = damage;
            this.per = per;

            if (per >= 0) {
                rigid.velocity = dir * speed;
            }
        }

        public void InitCircle(float damage, int per, Vector3 dir)
        {
            this.damage = damage;
            this.per = per;

        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Enemy") || per == -100)
                return;
                
            per--;

            if (per < 0) {
                rigid.velocity = Vector2.zero;
                gameObject.SetActive(false);
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Area") || per == -100)
                return;

            gameObject.SetActive(false);
        }
    }
}