using Goldmetal.UndeadSurvivor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Store : MonoBehaviour
{
	public Player player;

	float time = 0;
	//
	private void Update()
	{
		time += Time.deltaTime;
		if (time > 9f)
		{
			time = 0;
			changePosition();
		}
	}

	void changePosition()
	{
		// x^2 + y^2 = distance^2
		float distance = player.GetComponent<Player>().speed * Time.fixedDeltaTime * 900;
		// x^2 < distance^2   ,   x < distance
		float pos_x = Random.Range(0, distance);
		// y^2 = distance^2 - x^2   ,   y = root(distance^2 - x^2)
		float pos_y = Mathf.Sqrt(Mathf.Pow(distance, 2) - Mathf.Pow(pos_x, 2));

		int maxX = 350;     //���� 350, ���� 250
		int maxY = 250;
		/*
		�⺻Ȯ��: 50%
		���� ����������� ���� �ݴ����� ������ �� Ȯ���� ������(������ Ȱ��)

		 */
		double dir_x = 0, dir_y = 0;
		dir_x = Mathf.Pow(player.transform.position.x, 2) / Mathf.Pow(maxX, 2);
		dir_y = Mathf.Pow(player.transform.position.y, 2) / Mathf.Pow(maxY, 2);


        
		Debug.Log(distance);
	}
}
