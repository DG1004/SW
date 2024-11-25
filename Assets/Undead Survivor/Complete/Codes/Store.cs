using Goldmetal.UndeadSurvivor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;


public class Store : MonoBehaviour
{
	public Player player;

	float time = 0;
	//
	private void Update()
	{
		// 게임이 실행중일 때만 코드를 실행
		if (GameManager.instance.isLive == false)
			return;

		// 일정시간이 지나면 상점의 위치를 바꾼다.
		time += Time.deltaTime;
		if (time > 180f)
		{
			time = 0;
			changePosition();
		}
	}


    public void changePosition()
	{
		// 상점의 위치
		// x^2 + y^2 = distance^2
		float distance = player.GetComponent<Player>().speed * Time.fixedDeltaTime * 200;
		// x^2 < distance^2   ,   x < distance
		float pos_x = Random.Range(0, distance);
		// y^2 = distance^2 - x^2   ,   y = root(distance^2 - x^2)
		float pos_y = Mathf.Sqrt(Mathf.Pow(distance, 2) - Mathf.Pow(pos_x, 2));

		// 상점의 방향
		/*
		기본확률: 50%
		벽과 가까워질수록 벽과 반대쪽의 방향이 될 확률이 높아짐(제곱을 활용)
		 */
		double weight_x = 0, weight_y = 0;
		int ran_x, ran_y;
        int maxX = 350;     //가로 350, 세로 250
        int maxY = 250;
        ran_x = Random.Range(0, 100);
        ran_y = Random.Range(0, 100);

		weight_x = MathF.Sign(player.transform.position.x) * Mathf.Pow(player.transform.position.x, 2) / Mathf.Pow(maxX, 2) * 100;
		weight_y = MathF.Sign(player.transform.position.y) * Mathf.Pow(player.transform.position.y, 2) / Mathf.Pow(maxY, 2) * 100;

		int dir_x=1;
		int dir_y=1;

        // 상점 위치가 ground 위가 아닐 경우 재호출 필요

        if ((double)ran_x + weight_x >= 50 && (double)ran_y + weight_y >= 50)
		{
			// x반대, y반대방향
			//transform.position = new Vector3(player.transform.position.x - pos_x, player.transform.position.y - pos_y, 0);
			dir_x = -1;
			dir_y = -1;
            Debug.Log("--");
		}
		else if ((double)ran_x + weight_x < 50 && (double)ran_y + weight_y >= 50)
		{
			// x정, y반대방향
			//transform.position = new Vector3(player.transform.position.x + pos_x, player.transform.position.y - pos_y, 0);
			dir_x = 1;
			dir_y = -1;
            Debug.Log("+-");
        }
        else if ((double)ran_x + weight_x >= 50 && (double)ran_y + weight_y < 50)
		{
			// x반대, y정방향
			//transform.position = new Vector3(player.transform.position.x - pos_x, player.transform.position.y + pos_y, 0);
			dir_x = -1;
			dir_y = 1;
            Debug.Log("-+");

        }
        else if ((double)ran_x + weight_x < 50 && (double)ran_y + weight_y < 50)
		{
			// x정, y정방향
			//transform.position = new Vector3(player.transform.position.x + pos_x, player.transform.position.y + pos_y, 0);
			dir_x = 1;
			dir_y = 1;
            Debug.Log("++");
        }
		transform.position = new Vector3(player.transform.position.x + (dir_x * pos_x), player.transform.position.y + (dir_y * pos_y), 0);

		if (transform.position.x >= maxX || transform.position.x <= -maxX || transform.position.y >= maxY || transform.position.y <= -maxY)
		{
			changePosition();
		}
    }


}
