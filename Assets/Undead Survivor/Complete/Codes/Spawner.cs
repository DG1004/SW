using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Goldmetal.UndeadSurvivor ���ӽ����̽� ���� ��� �ڵ带 �����մϴ�.
namespace Goldmetal.UndeadSurvivor
{
    // Spawner Ŭ������ ����(��)�� �����ϰ� �����ϴ� ������ �մϴ�.
    // MonoBehaviour�� ��ӹ޾� ����Ƽ�� ������Ʈ�� ���˴ϴ�.
    public class Spawner : MonoBehaviour
    {
        // **Public Variables (�ν����Ϳ��� ���� ����)**

        // ���͸� ������ ��ġ���� �����ϴ� �迭�Դϴ�.
        // �� �迭�� �ν����Ϳ��� �����ϰų� �ڵ忡�� �ʱ�ȭ�˴ϴ�.
        public Transform[] spawnPoint;

        // ������ ���� �����͸� �����ϴ� �迭�Դϴ�.
        // �� �������� �ٸ� ������ �Ӽ� �� ���� �ð��� ������ �� �ֽ��ϴ�.
        public SpawnData[] spawnData;

        // �� ������ ���� �ð��� �����մϴ�.
        // ������ ��ü �ð�(maxGameTime)�� ���� �������� ���̷� ������ ���˴ϴ�.
        public float levelTime;

        // **Private Variables (���������� ���)**

        // ���� ������ ��Ÿ���� �����Դϴ�.
        // ���� ���� �ð��� ���� ����˴ϴ�.
        int level;

        // ���� Ÿ�̸ӷ�, �ð� ����� �����մϴ�.
        float timer;

        // Awake �Լ��� ��ũ��Ʈ�� ó�� �ε�� �� ȣ��˴ϴ�.
        void Awake()
        {
            // ���� ������Ʈ�� �ڽĵ� �� Transform ������Ʈ�� ���� ��� ��Ҹ� �����ͼ� spawnPoint �迭�� �����մϴ�.
            // �̸� ���� ���� ��ġ���� �ڵ����� ������ �� �ֽ��ϴ�.
            spawnPoint = GetComponentsInChildren<Transform>();

            // ��ü ���� �ð��� ���� �������� ���̷� ������ �� ������ ���� �ð��� ����մϴ�.
            // ���� ���, ���� �ð��� 100���̰� ���� �����Ͱ� 5����� ������ 20�ʰ� �˴ϴ�.
            levelTime = GameManager.instance.maxGameTime / spawnData.Length;
        }

        // Update �Լ��� �� �����Ӹ��� ȣ��Ǹ�, ���� ���� �� ���� ������ ����մϴ�.
        void Update()
        {
            // ������ ���� ������ ������ �Ʒ� ������ �������� �ʽ��ϴ�.
            if (!GameManager.instance.isLive)
                return;

            // **Ÿ�̸� ����**
            // �� �������� ��� �ð��� timer�� �����մϴ�.
            timer += Time.deltaTime;

            // **���� ���� ���**
            // ���� ���� �ð��� ������ �ð����� ������ ���� ������ ����մϴ�.
            // ���� ���, ������ 45�� ����Ǿ��� ������ �ð��� 20�ʶ�� ���� ������ 2�� �˴ϴ�.
            // Mathf.FloorToInt�� ����Ͽ� �Ҽ����� ������ ������ ����ϴ�.
            // Mathf.Min�� ����Ͽ� ������ �迭 ������ ���� �ʵ��� �մϴ�.
            level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

            // **���� ���� ���� Ȯ��**
            // timer�� ���� ������ spawnTime���� ũ�� ���͸� �����մϴ�.
            // spawnData[level].spawnTime�� ���� ���������� ���� ���� ������ �ǹ��մϴ�.
            if (timer > spawnData[level].spawnTime)
            {
                // Ÿ�̸Ӹ� 0���� �ʱ�ȭ�Ͽ� ���� ���� ������ �������մϴ�.
                timer = 0;

                // Spawn �Լ��� ȣ���Ͽ� ���͸� �����մϴ�.
                Spawn();
            }
        }

        // ���͸� �����ϴ� �Լ��Դϴ�.
        void Spawn()
        {
            // **���� ����**
            // GameManager�� PoolManager�� ���� ���� ������Ʈ�� �����ɴϴ�.
            // PoolManager�� ������Ʈ Ǯ�� ����� ����Ͽ� ���͸� ȿ�������� �����մϴ�.
            // Get(0)�� �ε��� 0�� ���͸� �����´ٴ� �ǹ��Դϴ�.
            GameObject enemy = GameManager.instance.pool.Get(0);

            // **���� ��ġ ����**
            // ���͸� ������ ���� ����Ʈ�� ��ġ�մϴ�.
            // Random.Range(1, spawnPoint.Length)�� ����Ͽ� �ε��� 1���� ���������� �߿��� �����ϰ� �����մϴ�.
            // �ε��� 0�� �θ� �ڽ��̹Ƿ� �����մϴ�.
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;

            // **���� �ʱ�ȭ**
            // ������ ������ Enemy ��ũ��Ʈ�� �����ͼ� Init �Լ��� ȣ���Ͽ� �ʱ�ȭ�մϴ�.
            // spawnData[level]�� �����Ͽ� ���� ������ �´� �����ͷ� �ʱ�ȭ�մϴ�.
            enemy.GetComponent<Enemy>().Init(spawnData[level]);
        }
    }

    // SpawnData Ŭ������ ������ ���� ������ �Ӽ��� �����մϴ�.
    // [System.Serializable]�� ����Ͽ� �ν����Ϳ��� ������ �� �ֵ��� �մϴ�.
    [System.Serializable]
    public class SpawnData
    {
        // ���� ���� ���� �ð��� ��Ÿ���ϴ�.
        // �� ���� �������� ���Ͱ� �� ���� �����˴ϴ�.
        public float spawnTime;

        // ������ ��������Ʈ Ÿ���� �����մϴ�.
        // ������ ������ �����ϴ� �� ���˴ϴ�.
        public int spriteType;

        // ������ ü���� ��Ÿ���ϴ�.
        // �� ���� �������� ���Ͱ� �� ���� ���ظ� �ߵ� �� �ֽ��ϴ�.
        public int health;

        // ������ �̵� �ӵ��� ��Ÿ���ϴ�.
        // �� ���� �������� ���Ͱ� �� ������ �̵��մϴ�.
        public float speed;
    }
}
