using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Goldmetal.UndeadSurvivor 네임스페이스 내에 모든 코드를 포함합니다.
namespace Goldmetal.UndeadSurvivor
{
    // Spawner 클래스는 몬스터(적)를 생성하고 관리하는 역할을 합니다.
    // MonoBehaviour를 상속받아 유니티의 컴포넌트로 사용됩니다.


    public class Spawner : MonoBehaviour
    {
        public Transform[] spawnPoint;
        public SpawnData[] spawnData;
        public int initialEnemyCount = 1; // Set the number of initial enemies
        public int minEnemyCount = 30;
        private float timer = 0f;
        private float lastSpawnTime = 0f; // 마지막 추가 스폰 시간
        private const float spawnCooldown = 10f; // 추가 스폰 쿨다운 시간 (초)


        void Awake()
        {
            spawnPoint = GetComponentsInChildren<Transform>();

        }



        void Update()
        {
            if (!GameManager.instance.isLive)
                return;

            timer += Time.deltaTime;

            // minEnemyCount 이하로 내려가면 추가 스폰 수행, 쿨다운 체크
            if (GameManager.instance.isLive && GameManager.instance.EnemyNum < minEnemyCount && timer > 10f)
            {
                if (Time.time - lastSpawnTime >= spawnCooldown) // 마지막 스폰 이후 시간이 지났는지 확인
                {
                    lastSpawnTime = Time.time; // 마지막 스폰 시간 갱신
                    StartCoroutine(SpawnInitialEnemies());
                }
            }
        }
        void Start()
        {
            // 코루틴 시작
            StartCoroutine(SpawnInitialEnemies());
        }
        IEnumerator SpawnInitialEnemies()
        {

            for (int i = 0; i < initialEnemyCount + 3; i++)
            {
                Spawn(1); // 적 스폰

                // spawnInterval 만큼 대기
                yield return new WaitForSeconds(0.1f);
            }
            for (int i = 0; i < 5; i++)
            {
                Spawn(2); // 적 스폰

                // spawnInterval 만큼 대기
                yield return new WaitForSeconds(0.1f);
            }
            for (int i = 0; i < initialEnemyCount; i++)
            {
                Spawn(3); // 적 스폰

                // spawnInterval 만큼 대기
                yield return new WaitForSeconds(0.1f);
            }
            for (int i = 0; i < initialEnemyCount; i++)
            {
                Spawn(5); // 적 스폰

                // spawnInterval 만큼 대기
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
                    이거  Enemy_parent로 해도된는지 볼것
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
            // 계수를 가지고 적절한 스탯을 만들어내는 함수 spawner에서 처음 몬스터 만들때 필요함
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
        

        // 랜덤 변화를 위한 메서드 (±10%)
        private double RandomVariation()
        {
            return (UnityEngine.Random.value * 0.3) - 0.15; // -0.1부터 0.1 사이의 값
        }

        

        public double stats_attack;
        public double stats_defence;
        public double stats_speed;
        public double stats_health;

        
    }
    /*public SpawnData()//무시할것
    {
        //돌연변이를 만들어내는 함수
        돌연변이를 만들어 formula_cst이거 초기화 어떻게 할건지 결정

        data
            // 1. 기존 데이터의 계수를 복사합니다.
            coe_attack = data.coe_attack;   // a
        coe_defence = data.coe_defence; // b
        coe_speed = data.coe_speed;     // d
        coe_health = data.coe_health;   // c
        formula_cst = data.formula_cst;
        // 3. 공식의 A와 B를 계산합니다.
        double a = coe_attack;
        double b = coe_defence;
        double c = coe_health;
        double d = coe_speed;
        int count = 0;
        do
        {
            if (count++ > 0)
            {
                Debug.Log("!!!!!!!!!!!!!!!!!!!번식이슈!!!!!!!!!!!!!!!!");
            }

            // 2. 스탯에 ±10%의 랜덤 변화를 가합니다.
            stats_attack = data.stats_attack * (1 + RandomVariation());
            stats_defence = data.stats_defence * (1 + RandomVariation());
            stats_speed = data.stats_speed * (1 + RandomVariation());

            double A = a * stats_attack + b * stats_defence + c;
            double B = d * stats_speed / c;

            // 4. 판별식 D를 계산하여 크기를 구합니다.
            double D = formula_cst * formula_cst - 4 * A * B;

            if (D < 0)
            {
                // 판별식이 음수인 경우, 현실적인 해가 없으므로 예외 처리합니다.
                Debug.LogError("판별식이 음수입니다. 스탯 값을 확인하세요.");
                stats_health = 1; // 기본값 설정 또는 다른 예외 처리 방법 적용
            }
            else
            {
                double sqrtD = Mathf.Sqrt((float)D);
                double size1 = (formula_cst + sqrtD) / (2 * A);
                double size2 = (formula_cst - sqrtD) / (2 * A);

                // 양수인 크기 중 하나를 선택합니다.
                stats_health = Mathf.Max((float)size1, (float)size2);
            }
        } while (stats_health <= 0 || stats_attack <= 0 || stats_defence <= 0 || stats_speed <= 0);
        // 5. race_index 등 필요한 다른 값들을 복사합니다.
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
        // **Public Variables (인스펙터에서 설정 가능)**

        // 몬스터를 스폰할 위치들을 저장하는 배열입니다.
        // 이 배열은 인스펙터에서 설정하거나 코드에서 초기화됩니다.
        public Transform[] spawnPoint;

        // 스폰에 대한 데이터를 저장하는 배열입니다.
        // 각 레벨별로 다른 몬스터의 속성 및 스폰 시간을 설정할 수 있습니다.
        public SpawnData[] spawnData;

        // 각 레벨의 지속 시간을 저장합니다.
        // 게임의 전체 시간(maxGameTime)을 스폰 데이터의 길이로 나누어 계산됩니다.
        public float levelTime;

        // **Private Variables (내부적으로 사용)**

        // 현재 레벨을 나타내는 변수입니다.
        // 게임 진행 시간에 따라 변경됩니다.
        int level;

        // 스폰 타이머로, 시간 경과를 측정합니다.
        float timer;

        // Awake 함수는 스크립트가 처음 로드될 때 호출됩니다.
        void Awake()
        {
            // 현재 오브젝트의 자식들 중 Transform 컴포넌트를 가진 모든 요소를 가져와서 spawnPoint 배열에 저장합니다.
            // 이를 통해 스폰 위치들을 자동으로 수집할 수 있습니다.
            spawnPoint = GetComponentsInChildren<Transform>();

            // 전체 게임 시간을 스폰 데이터의 길이로 나누어 각 레벨의 지속 시간을 계산합니다.
            // 예를 들어, 게임 시간이 100초이고 스폰 데이터가 5개라면 레벨당 20초가 됩니다.
            levelTime = GameManager.instance.maxGameTime / spawnData.Length;
        }

        // Update 함수는 매 프레임마다 호출되며, 몬스터 스폰 및 레벨 관리를 담당합니다.
        void Update()
        {
            // 게임이 진행 중이지 않으면 아래 로직을 실행하지 않습니다.
            if (!GameManager.instance.isLive)
                return;

            // **타이머 증가**
            // 매 프레임의 경과 시간을 timer에 누적합니다.
            timer += Time.deltaTime;

            // **현재 레벨 계산**
            // 게임 진행 시간을 레벨당 시간으로 나누어 현재 레벨을 계산합니다.
            // 예를 들어, 게임이 45초 진행되었고 레벨당 시간이 20초라면 현재 레벨은 2가 됩니다.
            // Mathf.FloorToInt를 사용하여 소수점을 버리고 정수로 만듭니다.
            // Mathf.Min을 사용하여 레벨이 배열 범위를 넘지 않도록 합니다.
            level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

            // **몬스터 스폰 조건 확인**
            // timer가 현재 레벨의 spawnTime보다 크면 몬스터를 스폰합니다.
            // spawnData[level].spawnTime은 현재 레벨에서의 몬스터 스폰 간격을 의미합니다.
            if (timer > spawnData[level].spawnTime)
            {
                // 타이머를 0으로 초기화하여 다음 스폰 간격을 재측정합니다.
                timer = 0;

                // Spawn 함수를 호출하여 몬스터를 생성합니다.
                Spawn();
            }
        }

        // 몬스터를 생성하는 함수입니다.
        void Spawn()
        {
            // **몬스터 생성**
            // GameManager의 PoolManager를 통해 몬스터 오브젝트를 가져옵니다.
            // PoolManager는 오브젝트 풀링 기법을 사용하여 몬스터를 효율적으로 관리합니다.
            // Get(0)은 인덱스 0번 몬스터를 가져온다는 의미입니다.
            GameObject enemy = GameManager.instance.pool.Get(0);

            // **스폰 위치 설정**
            // 몬스터를 랜덤한 스폰 포인트에 배치합니다.
            // Random.Range(1, spawnPoint.Length)를 사용하여 인덱스 1부터 마지막까지 중에서 랜덤하게 선택합니다.
            // 인덱스 0은 부모 자신이므로 제외합니다.
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;

            // **몬스터 초기화**
            // 생성된 몬스터의 Enemy 스크립트를 가져와서 Init 함수를 호출하여 초기화합니다.
            // spawnData[level]을 전달하여 현재 레벨에 맞는 데이터로 초기화합니다.
            enemy.GetComponent<Enemy>().Init(spawnData[level]);
        }
    }

    // SpawnData 클래스는 몬스터의 스폰 정보와 속성을 저장합니다.
    // [System.Serializable]을 사용하여 인스펙터에서 설정할 수 있도록 합니다.
    [System.Serializable]
    public class SpawnData
    {
        // 몬스터 스폰 간격 시간을 나타냅니다.
        // 이 값이 작을수록 몬스터가 더 자주 스폰됩니다.
        public float spawnTime;

        // 몬스터의 스프라이트 타입을 지정합니다.
        // 몬스터의 외형을 결정하는 데 사용됩니다.
        public int spriteType;

        // 몬스터의 체력을 나타냅니다.
        // 이 값이 높을수록 몬스터가 더 많은 피해를 견딜 수 있습니다.
        public int health;

        // 몬스터의 이동 속도를 나타냅니다.
        // 이 값이 높을수록 몬스터가 더 빠르게 이동합니다.
        public float speed;
    }
*/


/*

// Goldmetal.UndeadSurvivor 네임스페이스 내에 모든 코드를 포함합니다.
namespace Goldmetal.UndeadSurvivor
{
    // Spawner 클래스는 몬스터(적)를 생성하고 관리하는 역할을 합니다.
    // MonoBehaviour를 상속받아 유니티의 컴포넌트로 사용됩니다.
    public class Spawner : MonoBehaviour
    {
        // **Public Variables (인스펙터에서 설정 가능)**

        // 몬스터를 스폰할 위치들을 저장하는 배열입니다.
        // 이 배열은 인스펙터에서 설정하거나 코드에서 초기화됩니다.
        public Transform[] spawnPoint;

        // 스폰에 대한 데이터를 저장하는 배열입니다.
        // 각 레벨별로 다른 몬스터의 속성 및 스폰 시간을 설정할 수 있습니다.
        public SpawnData[] spawnData;

        // 각 레벨의 지속 시간을 저장합니다.
        // 게임의 전체 시간(maxGameTime)을 스폰 데이터의 길이로 나누어 계산됩니다.
        public float levelTime;

        // **Private Variables (내부적으로 사용)**

        // 현재 레벨을 나타내는 변수입니다.
        // 게임 진행 시간에 따라 변경됩니다.
        int level;

        // 스폰 타이머로, 시간 경과를 측정합니다.
        float timer;

        // Awake 함수는 스크립트가 처음 로드될 때 호출됩니다.
        void Awake()
        {
            // 현재 오브젝트의 자식들 중 Transform 컴포넌트를 가진 모든 요소를 가져와서 spawnPoint 배열에 저장합니다.
            // 이를 통해 스폰 위치들을 자동으로 수집할 수 있습니다.
            spawnPoint = GetComponentsInChildren<Transform>();

            // 전체 게임 시간을 스폰 데이터의 길이로 나누어 각 레벨의 지속 시간을 계산합니다.
            // 예를 들어, 게임 시간이 100초이고 스폰 데이터가 5개라면 레벨당 20초가 됩니다.
            levelTime = GameManager.instance.maxGameTime / spawnData.Length;
        }

        // Update 함수는 매 프레임마다 호출되며, 몬스터 스폰 및 레벨 관리를 담당합니다.
        void Update()
        {
            // 게임이 진행 중이지 않으면 아래 로직을 실행하지 않습니다.
            if (!GameManager.instance.isLive)
                return;

            // **타이머 증가**
            // 매 프레임의 경과 시간을 timer에 누적합니다.
            timer += Time.deltaTime;

            // **현재 레벨 계산**
            // 게임 진행 시간을 레벨당 시간으로 나누어 현재 레벨을 계산합니다.
            // 예를 들어, 게임이 45초 진행되었고 레벨당 시간이 20초라면 현재 레벨은 2가 됩니다.
            // Mathf.FloorToInt를 사용하여 소수점을 버리고 정수로 만듭니다.
            // Mathf.Min을 사용하여 레벨이 배열 범위를 넘지 않도록 합니다.
            level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

            // **몬스터 스폰 조건 확인**
            // timer가 현재 레벨의 spawnTime보다 크면 몬스터를 스폰합니다.
            // spawnData[level].spawnTime은 현재 레벨에서의 몬스터 스폰 간격을 의미합니다.
            if (timer > spawnData[level].spawnTime)
            {
                // 타이머를 0으로 초기화하여 다음 스폰 간격을 재측정합니다.
                timer = 0;

                // Spawn 함수를 호출하여 몬스터를 생성합니다.
                Spawn();
            }
        }

        // 몬스터를 생성하는 함수입니다.
        void Spawn()
        {
            // **몬스터 생성**
            // GameManager의 PoolManager를 통해 몬스터 오브젝트를 가져옵니다.
            // PoolManager는 오브젝트 풀링 기법을 사용하여 몬스터를 효율적으로 관리합니다.
            // Get(0)은 인덱스 0번 몬스터를 가져온다는 의미입니다.
            GameObject enemy = GameManager.instance.pool.Get(0);

            // **스폰 위치 설정**
            // 몬스터를 랜덤한 스폰 포인트에 배치합니다.
            // Random.Range(1, spawnPoint.Length)를 사용하여 인덱스 1부터 마지막까지 중에서 랜덤하게 선택합니다.
            // 인덱스 0은 부모 자신이므로 제외합니다.
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;

            // **몬스터 초기화**
            // 생성된 몬스터의 Enemy 스크립트를 가져와서 Init 함수를 호출하여 초기화합니다.
            // spawnData[level]을 전달하여 현재 레벨에 맞는 데이터로 초기화합니다.
            enemy.GetComponent<Enemy>().Init(spawnData[level]);
        }
    }

    // SpawnData 클래스는 몬스터의 스폰 정보와 속성을 저장합니다.
    // [System.Serializable]을 사용하여 인스펙터에서 설정할 수 있도록 합니다.
    [System.Serializable]
    public class SpawnData
    {
        // 몬스터 스폰 간격 시간을 나타냅니다.
        // 이 값이 작을수록 몬스터가 더 자주 스폰됩니다.
        public float spawnTime;

        // 몬스터의 스프라이트 타입을 지정합니다.
        // 몬스터의 외형을 결정하는 데 사용됩니다.
        public int spriteType;

        // 몬스터의 체력을 나타냅니다.
        // 이 값이 높을수록 몬스터가 더 많은 피해를 견딜 수 있습니다.
        public int health;

        // 몬스터의 이동 속도를 나타냅니다.
        // 이 값이 높을수록 몬스터가 더 빠르게 이동합니다.
        public float speed;
    }
}

*/
