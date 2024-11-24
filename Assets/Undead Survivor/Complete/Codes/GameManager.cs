using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Goldmetal.UndeadSurvivor 네임스페이스 안에 모든 코드를 포함시킵니다.
namespace Goldmetal.UndeadSurvivor
{
    // GameManager 클래스는 게임의 전반적인 흐름과 상태를 관리합니다.
    // MonoBehaviour를 상속받아 유니티의 컴포넌트로 사용됩니다.
    public class GameManager : MonoBehaviour
    {
        // 싱글톤 패턴을 사용하여 어디서든 GameManager.instance로 접근할 수 있게 합니다.
        public static GameManager instance;


        // [Header]는 인스펙터에서 변수들을 그룹화하여 보기 좋게 만듭니다.
        [Header("# Game Control")]
        // 게임이 진행 중인지 여부를 나타내는 변수입니다.
        public bool isLive;
        // 게임이 진행된 시간을 저장하는 변수입니다.
        public float gameTime;
        // 게임의 최대 시간을 설정합니다 (여기서는 20초로 설정).
        public float maxGameTime = 2 * 10f;

        [Header("# Player Info")]
        // 플레이어의 ID를 저장합니다 (캐릭터 선택 등에 사용될 수 있음).
        public int playerId;
        // 플레이어의 현재 체력을 나타냅니다.
        public float health;
        // 플레이어의 최대 체력을 설정합니다.
        public float maxHealth = 100;
        // 플레이어의 현재 레벨을 나타냅니다.
        public int level;
        // 플레이어가 처치한 적의 수를 기록합니다.
        public int kill;
        // 현재 누적된 경험치를 저장합니다.
        public int exp;
        // 레벨업에 필요한 다음 경험치 양을 저장한 배열입니다.
        public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

        [Header("# Game Object")]
        // 오브젝트 풀링을 관리하는 매니저입니다 (적, 아이템 등을 효율적으로 관리).
        public PoolManager pool;
        // 플레이어 캐릭터를 제어하는 스크립트입니다.
        public Player player;
        // 레벨업 UI를 관리하는 스크립트입니다.
        public LevelUp uiLevelUp;
        // 게임 결과 화면을 관리하는 스크립트입니다.
        public Result uiResult;
        // 조이스틱 UI의 Transform입니다 (모바일 조작 등을 위해).
        public Transform uiJoy;
        // 게임 승리 시 남은 적들을 제거하기 위한 오브젝트입니다.
        public GameObject enemyCleaner;
        public TMSHOP tmShop;  // Inspector에서 반드시 할당해야 함
        public Store store;

        // Awake는 스크립트가 처음 로드될 때 호출되는 함수입니다.
        void Awake()
        {
            // 싱글톤 인스턴스를 현재 오브젝트로 설정합니다.
            instance = this;
            // 애플리케이션의 최대 프레임 레이트를 60으로 설정합니다.
            Application.targetFrameRate = 60;

           
        }

        // 게임을 시작할 때 호출되는 함수입니다.
        public void GameStart(int id)
        {
            // 플레이어의 ID를 설정합니다.
            playerId = id;
            // 플레이어의 체력을 최대 체력으로 초기화합니다.
            health = maxHealth;

            // 플레이어 오브젝트를 활성화하여 게임에 등장시킵니다.
            player.gameObject.SetActive(true);
            // 상점 오브젝트를 활성화하여 게임에 등장시킵니다.
            store.gameObject.SetActive(true); 
            // 레벨업 UI에서 플레이어 ID에 따른 선택을 설정합니다.
            uiLevelUp.Select(playerId % 2);
            // 게임을 재개합니다 (일시정지 상태에서 풀기 등).
            Resume();

            // 배경음악을 재생하고, 선택 효과음을 재생합니다.
            AudioManager.instance.PlayBgm(true);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        }

        // 플레이어가 사망하거나 게임 오버 조건이 충족되었을 때 호출됩니다.
        public void GameOver()
        {
            // GameOverRoutine 코루틴을 시작하여 게임 오버 처리를 합니다.
            StartCoroutine(GameOverRoutine());
        }

        // 게임 오버 연출을 위한 코루틴 함수입니다.
        IEnumerator GameOverRoutine()
        {
            // 게임 진행 상태를 false로 설정하여 게임 로직을 멈춥니다.
            isLive = false;

            // 0.5초 동안 대기합니다 (연출을 위한 딜레이).
            yield return new WaitForSeconds(0.5f);

            // 결과 UI를 활성화하고 패배 화면을 표시합니다.
            uiResult.gameObject.SetActive(true);
            uiResult.Lose();
            // 게임을 일시정지합니다.
            Stop();

            // 배경음악을 중지하고, 패배 효과음을 재생합니다.
            AudioManager.instance.PlayBgm(false);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
        }

        // 게임 승리 조건이 충족되었을 때 호출됩니다.
        public void GameVictroy()
        {
            // GameVictroyRoutine 코루틴을 시작하여 승리 처리를 합니다.
            StartCoroutine(GameVictroyRoutine());
        }

        // 게임 승리 연출을 위한 코루틴 함수입니다.
        IEnumerator GameVictroyRoutine()
        {
            // 게임 진행 상태를 false로 설정하여 게임 로직을 멈춥니다.
            isLive = false;
            // 남아있는 적들을 제거하기 위해 enemyCleaner를 활성화합니다.
            enemyCleaner.SetActive(true);

            // 0.5초 동안 대기합니다 (연출을 위한 딜레이).
            yield return new WaitForSeconds(0.5f);

            // 결과 UI를 활성화하고 승리 화면을 표시합니다.
            uiResult.gameObject.SetActive(true);
            uiResult.Win();
            // 게임을 일시정지합니다.
            Stop();

            // 배경음악을 중지하고, 승리 효과음을 재생합니다.
            AudioManager.instance.PlayBgm(false);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
        }

        // 게임을 다시 시작할 때 호출되는 함수입니다.
        public void GameRetry()
        {
            // 현재 씬(씬 번호 0)을 다시 로드하여 게임을 초기화합니다.
            SceneManager.LoadScene(0);
        }

        // 게임을 종료할 때 호출되는 함수입니다.
        public void GameQuit()
        {
            // 애플리케이션을 종료합니다 (에디터에서는 동작하지 않음).
            Application.Quit();
        }

        // 매 프레임마다 호출되는 업데이트 함수입니다.
        void Update()
        {
            // 게임이 진행 중일 때 아래 로직을 실행합니다.
            if (isLive)
            {

                // 게임 시간을 증가시킵니다.
                gameTime += Time.deltaTime;

                // 게임 시간이 최대 시간을 초과하면 승리 처리를 합니다.
                if (gameTime > maxGameTime)
                {
                    // 게임 시간을 최대 시간으로 고정합니다.
                    gameTime = maxGameTime;
                    // 게임 승리 함수를 호출합니다.
                    GameVictroy();
                }
            }
            // 플레이어가 상점에 있을 때 아래 로직을 실행합니다.
            if (player.isStore == 1)
            {
                player.inputVec.x = Input.GetAxisRaw("Horizontal");
                player.inputVec.y = Input.GetAxisRaw("Vertical");
                Vector2 nextVec = player.inputVec.normalized * player.speed * Time.fixedDeltaTime;
                player.rigid.MovePosition(player.rigid.position + nextVec);
            }
        }

        // 경험치를 획득할 때 호출되는 함수입니다.
        public void GetExp()
        {
            // 게임이 진행 중이 아니면 아래 로직을 실행하지 않습니다.
            if (!isLive)
                return;

            // 경험치를 1 증가시킵니다.
            exp++;

            // 현재 레벨에서 필요한 경험치에 도달했는지 확인합니다.
            // Mathf.Min을 사용하여 배열 범위를 넘지 않도록 합니다.
            if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)])
            {
                // 레벨을 1 증가시킵니다.
                level++;
                // 경험치를 0으로 초기화합니다.
                exp = 0;
                // 레벨업 UI를 표시합니다.
                uiLevelUp.Show();
            }
        }

        // 게임을 일시정지할 때 호출되는 함수입니다.
        public void Stop()
        {
            // 게임 진행 상태를 false로 설정합니다.
            isLive = false;
            // 게임의 시간 흐름을 멈춥니다.
            Time.timeScale = 0;
            // 조이스틱 UI를 숨깁니다 (크기를 0으로 설정).
            uiJoy.localScale = Vector3.zero;
        }

        // 게임을 재개할 때 호출되는 함수입니다.
        public void Resume()
        {
            // 게임 진행 상태를 true로 설정합니다.
            isLive = true;
            // 게임의 시간 흐름을 정상으로 돌립니다.
            Time.timeScale = 1;
            // 조이스틱 UI를 표시합니다 (크기를 원래대로 설정).
            uiJoy.localScale = Vector3.one;
        }
        public void ShowShop()
        {
            Stop();  // Stop the game
            tmShop.Show();  // Show the travelling merchant shop
        }
    }
}
