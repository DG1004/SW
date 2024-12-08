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
        public Transform[] spawnPoint;
        public SpawnData[] spawnData;
        public int initialEnemyCount = 1; // Set the number of initial enemies
        public int minEnemyCount = 30;
        private float timer = 0f;
        private float lastSpawnTime = 0f; // ������ �߰� ���� �ð�
        private const float spawnCooldown = 10f; // �߰� ���� ��ٿ� �ð� (��)


        void Awake()
        {
            spawnPoint = GetComponentsInChildren<Transform>();

        }



        void Update()
        {
            if (!GameManager.instance.isLive)
                return;

            timer += Time.deltaTime;

            // minEnemyCount ���Ϸ� �������� �߰� ���� ����, ��ٿ� üũ
            if (GameManager.instance.isLive && GameManager.instance.EnemyNum < minEnemyCount && timer > 10f)
            {
                if (Time.time - lastSpawnTime >= spawnCooldown) // ������ ���� ���� �ð��� �������� Ȯ��
                {
                    lastSpawnTime = Time.time; // ������ ���� �ð� ����
                    StartCoroutine(SpawnInitialEnemies());
                }
            }
        }
        void Start()
        {
            // �ڷ�ƾ ����
            StartCoroutine(SpawnInitialEnemies());
        }
        IEnumerator SpawnInitialEnemies()
        {

            for (int i = 0; i < initialEnemyCount + 3; i++)
            {
                Spawn(1); // �� ����

                // spawnInterval ��ŭ ���
                yield return new WaitForSeconds(0.1f);
            }
            for (int i = 0; i < 5; i++)
            {
                Spawn(2); // �� ����

                // spawnInterval ��ŭ ���
                yield return new WaitForSeconds(0.1f);
            }
            for (int i = 0; i < initialEnemyCount; i++)
            {
                Spawn(3); // �� ����

                // spawnInterval ��ŭ ���
                yield return new WaitForSeconds(0.1f);
            }
            for (int i = 0; i < initialEnemyCount; i++)
            {
                Spawn(5); // �� ����

                // spawnInterval ��ŭ ���
                yield return new WaitForSeconds(0.1f);
            }
        }

        void Spawn(int race_index)
        {
            Debug.Log(race_index);
            GameObject enemy = GameManager.instance.pool.Get_Enemy(race_index);
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
            // Use random spawnData for initial enemies
            // int randomIndex = Random.Range(0, spawnData.Length);
            enemy.GetComponent<Enemy>().Init();
           /* switch (race_index)
            {
                case 0:
                    enemy.GetComponent<Enemy>().Init();//////!!!!!!!!!!!!!
                    �̰�  Enemy_parent�� �ص��ȴ��� ����
                    break;
                case 1:
                    enemy.GetComponent<Enemy3>().Init();
                    break;
            }*/
        }
    }


    public class SpawnData
    {
        public SpawnData(double coe_attack,double coe_defence, double coe_speed, double coe_health)
        {
            // ����� ������ ������ ������ ������ �Լ� spawner���� ó�� ���� ���鶧 �ʿ���
            this.stats_attack = 0.25/coe_attack;
            this.stats_defence = 0.25 / coe_defence;
            this.stats_speed = 0.25 / coe_speed;
            this.stats_health = 0.25 / coe_health;

        }
 
        public SpawnData(SpawnData data, double coe_attack, double coe_defence, double coe_speed, double coe_health)
        {
            do
            {
                stats_attack = data.stats_attack * (1 + RandomVariation());
                stats_defence = data.stats_defence * (1 + RandomVariation());
                stats_speed = data.stats_speed * (1 + RandomVariation());
                stats_health = (1 - coe_attack * stats_attack - coe_defence * stats_defence - coe_speed * stats_speed) / coe_health;
            } while (stats_health <= 15 || stats_attack <= 0 || stats_defence <= 0 || stats_speed <= 0);

        }
        

        // ���� ��ȭ�� ���� �޼��� (��10%)
        private double RandomVariation()
        {
            return (UnityEngine.Random.value * 0.3) - 0.15; // -0.1���� 0.1 ������ ��
        }

        

        public double stats_attack;
        public double stats_defence;
        public double stats_speed;
        public double stats_health;

        
    }
    /*public SpawnData()//�����Ұ�
    {
        //�������̸� ������ �Լ�
        �������̸� ����� formula_cst�̰� �ʱ�ȭ ��� �Ұ��� ����

        data
            // 1. ���� �������� ����� �����մϴ�.
            coe_attack = data.coe_attack;   // a
        coe_defence = data.coe_defence; // b
        coe_speed = data.coe_speed;     // d
        coe_health = data.coe_health;   // c
        formula_cst = data.formula_cst;
        // 3. ������ A�� B�� ����մϴ�.
        double a = coe_attack;
        double b = coe_defence;
        double c = coe_health;
        double d = coe_speed;
        int count = 0;
        do
        {
            if (count++ > 0)
            {
                Debug.Log("!!!!!!!!!!!!!!!!!!!�����̽�!!!!!!!!!!!!!!!!");
            }

            // 2. ���ȿ� ��10%�� ���� ��ȭ�� ���մϴ�.
            stats_attack = data.stats_attack * (1 + RandomVariation());
            stats_defence = data.stats_defence * (1 + RandomVariation());
            stats_speed = data.stats_speed * (1 + RandomVariation());

            double A = a * stats_attack + b * stats_defence + c;
            double B = d * stats_speed / c;

            // 4. �Ǻ��� D�� ����Ͽ� ũ�⸦ ���մϴ�.
            double D = formula_cst * formula_cst - 4 * A * B;

            if (D < 0)
            {
                // �Ǻ����� ������ ���, �������� �ذ� �����Ƿ� ���� ó���մϴ�.
                Debug.LogError("�Ǻ����� �����Դϴ�. ���� ���� Ȯ���ϼ���.");
                stats_health = 1; // �⺻�� ���� �Ǵ� �ٸ� ���� ó�� ��� ����
            }
            else
            {
                double sqrtD = Mathf.Sqrt((float)D);
                double size1 = (formula_cst + sqrtD) / (2 * A);
                double size2 = (formula_cst - sqrtD) / (2 * A);

                // ����� ũ�� �� �ϳ��� �����մϴ�.
                stats_health = Mathf.Max((float)size1, (float)size2);
            }
        } while (stats_health <= 0 || stats_attack <= 0 || stats_defence <= 0 || stats_speed <= 0);
        // 5. race_index �� �ʿ��� �ٸ� ������ �����մϴ�.
        this.race_index = data.race_index;
    }
*/

    /*
        [System.Serializable]
        public class SpawnData
        {
            public SpawnData(int spriteType = 0, int health = 100, float speed = 5)
            {
                this.spriteType = spriteType;
                this.health = health;
                this.speed = speed;
            }
            public int spriteType;
            public int health;
            public float speed;
        }
    */
}

/*    public class Spawner : MonoBehaviour
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
*/


/*

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

*/
