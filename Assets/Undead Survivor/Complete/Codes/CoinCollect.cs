using Goldmetal.UndeadSurvivor;
using UnityEngine;


public class CoinCollect : MonoBehaviour
{
    private Coin coin;

    void Awake()
    {
        coin = GetComponentInParent<Coin>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            coin.StartFollowing();
        }
    }
}