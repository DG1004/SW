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
