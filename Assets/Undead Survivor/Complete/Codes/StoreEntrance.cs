using Goldmetal.UndeadSurvivor;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class StoreEntrance : MonoBehaviour
{
	public Player player;

	float time = 0;
	//
	private void Update()
	{
		// ������ �������� ���� �ڵ带 ����
		if (GameManager.instance.isLive == false)
			return;

		// �����ð��� ������ ������ ��ġ�� �ٲ۴�.
		time += Time.deltaTime;
		if (time > 180f)
		{
			time = 0;
			changePosition();
		}
	}

	//�����Ա��� ��ġ�� �ٲٴ� �Լ�
    public void changePosition()
	{
		// ������ ��ġ
		// x^2 + y^2 = distance^2
		float distance = player.GetComponent<Player>().speed * Time.fixedDeltaTime * 2000;	// 2000�� �� �� 1��
		// x^2 < distance^2   ,   x < distance
		float pos_x = Random.Range(0, distance);
		// y^2 = distance^2 - x^2   ,   y = root(distance^2 - x^2)
		float pos_y = Mathf.Sqrt(Mathf.Pow(distance, 2) - Mathf.Pow(pos_x, 2));

		// ������ ����
		/*
		�⺻Ȯ��: 50%
		���� ����������� ���� �ݴ����� ������ �� Ȯ���� ������(������ Ȱ��)
		 */
		double weight_x = 0, weight_y = 0;
		int ran_x, ran_y;
        int maxX = 350;     //���� 350, ���� 250
        int maxY = 250;
        ran_x = Random.Range(0, 100);
        ran_y = Random.Range(0, 100);

		weight_x = MathF.Sign(player.transform.position.x) * Mathf.Pow(player.transform.position.x, 2) / Mathf.Pow(maxX, 2) * 100;
		weight_y = MathF.Sign(player.transform.position.y) * Mathf.Pow(player.transform.position.y, 2) / Mathf.Pow(maxY, 2) * 100;

		int dir_x=1;
		int dir_y=1;

        // ���� ��ġ�� ground ���� �ƴ� ��� ��ȣ�� �ʿ�

        if ((double)ran_x + weight_x >= 50 && (double)ran_y + weight_y >= 50)
		{
			// x�ݴ�, y�ݴ����
			//transform.position = new Vector3(player.transform.position.x - pos_x, player.transform.position.y - pos_y, 0);
			dir_x = -1;
			dir_y = -1;
            Debug.Log("������ �÷��̾� ���� ���ʾƷ� ����");
		}
		else if ((double)ran_x + weight_x < 50 && (double)ran_y + weight_y >= 50)
		{
			// x��, y�ݴ����
			//transform.position = new Vector3(player.transform.position.x + pos_x, player.transform.position.y - pos_y, 0);
			dir_x = 1;
			dir_y = -1;
            Debug.Log("������ �÷��̾� ���� �����ʾƷ� ����");
        }
        else if ((double)ran_x + weight_x >= 50 && (double)ran_y + weight_y < 50)
		{
			// x�ݴ�, y������
			//transform.position = new Vector3(player.transform.position.x - pos_x, player.transform.position.y + pos_y, 0);
			dir_x = -1;
			dir_y = 1;
            Debug.Log("������ �÷��̾� ���� ������ ����");

        }
        else if ((double)ran_x + weight_x < 50 && (double)ran_y + weight_y < 50)
		{
			// x��, y������
			//transform.position = new Vector3(player.transform.position.x + pos_x, player.transform.position.y + pos_y, 0);
			dir_x = 1;
			dir_y = 1;
            Debug.Log("������ �÷��̾� ���� �������� ����");
        }
        transform.position = new Vector3(player.transform.position.x + (dir_x * pos_x), player.transform.position.y + (dir_y * pos_y), 0);

		if (transform.position.x >= maxX || transform.position.x <= -maxX || transform.position.y >= maxY || transform.position.y <= -maxY)
		{
			changePosition();
		}
    }


}
