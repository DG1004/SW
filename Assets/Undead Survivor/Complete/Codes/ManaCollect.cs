using Goldmetal.UndeadSurvivor;
using UnityEngine;


public class ManaCollect : MonoBehaviour
{
    private Mana mana;

    void Awake()
    {
        mana = GetComponentInParent<Mana>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mana.StartFollowing();
        }
    }
}