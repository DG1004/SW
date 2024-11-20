using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
	public GameObject store_inside;
	public GameObject store_background;
	public GameObject ground;
    public GameObject player;

   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameObject.CompareTag("Player"))
            return;

        /*
        gameObject.SetActive(false);
        ground.SetActive(false);
        store_inside.SetActive(true);
        store_background.SetActive(true);
        */
    }
}
