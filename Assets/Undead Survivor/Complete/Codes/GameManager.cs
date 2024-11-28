using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

// Goldmetal.UndeadSurvivor 네임스페이스 안에 모든 코드를 포함시킵니다.
namespace Goldmetal.UndeadSurvivor
{
    // GameManager 클래스는 게임의 전반적인 흐름과 상태를 관리합니다.
    // MonoBehaviour를 상속받아 유니티의 컴포넌트로 사용됩니다.
    public class GameManager : MonoBehaviour
    {
        // 싱글톤 패턴을 사용하여 어디서든 GameManager.instance로 접근할 수 있게 합니다.
        public static GameManager instance;
        public int EnemyNum;

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
        // 플레이어가 처치한 적의 수를 기록합니다.
        public int kill;
        // 플레이어가 획득한 코인의 수를 기록합니다.
        public int Coin;
        // 삭제 예정
        public int level;
        public int exp;
        public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };
        [Header("# Game Object")]
        // 코인을 관리하는 매니저임
        public CoinManager CoinManager;
        // 오브젝트 풀링을 관리하는 매니저입니다 (적, 아이템 등을 효율적으로 관리).
        public PoolManager pool;
        // 플레이어 캐릭터를 제어하는 스크립트입니다.
        public Player player;
        // 레벨업 UI를 관리하는 스크립트입니다.
        public LevelUp uiLevelUp;
        //  상점 UI 관리
        public StoreStd uiStore;
        // 게임 결과 화면을 관리하는 스크립트입니다.
        public Result uiResult;
        // 게임 승리 시 남은 적들을 제거하기 위한 오브젝트입니다.
        public GameObject enemyCleaner;
        public StoreStd storeStd;
        public TMSHOP tmShop;  // Inspector에서 반드시 할당해야 함
        public StoreEntrance store;
        public Arrow arrow;

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
            EnemyNum = 0;
            // 플레이어의 ID를 설정합니다.
            playerId = id;
            // 플레이어의 체력을 최대 체력으로 초기화합니다.
            health = maxHealth;

            // 플레이어 오브젝트를 활성화하여 게임에 등장시킵니다.
            player.gameObject.SetActive(true);
            // 상점 오브젝트를 활성화하여 게임에 등장시키고, 위치를 지정합니다.
            store.gameObject.SetActive(true);
            store.changePosition();
            // 화살표 오브젝트를 활성화하여 게임에 등장시킵니다.
            arrow.gameObject.SetActive(true);
            // 상점 UI에서 플레이어 ID에 따른 선택을 설정합니다.
            //uiStore.Select(playerId % 2);
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
            // esc 키를 눌러 게임을 멈추거나 재개한다.
            // 상점에서 esc키를 누를 경우 오류가 생기니 조심
            // isLive가 true일 때 몬스터가 계속해서 생성되는 것 고치기
            if (Input.GetKeyDown(KeyCode.H))
            {
                SceneController scene = GetComponent<SceneController>();
                scene.LoadScene("StoreScene");
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                SceneController scene = GetComponent<SceneController>();
                scene.LoadScene("CompleteScene");
            }
            if (Input.GetKeyDown(KeyCode.Escape) && isLive) {
                Stop();
                return;
                }
            if (Input.GetKeyDown(KeyCode.Escape) && !isLive)
            {
                Resume();
            }
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

        // 삭제 예정
        public void GetExp()
        {
            if (!isLive)
                return;

            exp++;

            if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)])
            {
                level++;
                exp = 0;
            }
        }


        // 게임을 일시정지할 때 호출되는 함수입니다.
        public void Stop()
        {
            // 게임 진행 상태를 false로 설정합니다.
            isLive = false;
            // 게임의 시간 흐름을 멈춥니다.
            Time.timeScale = 0;
        }

        // 게임을 재개할 때 호출되는 함수입니다.
        public void Resume()
        {
            // 게임 진행 상태를 true로 설정합니다.
            isLive = true;
            // 게임의 시간 흐름을 정상으로 돌립니다.
            Time.timeScale = 1;
        }
        public void ShowShop(int id)
        {
            if (id == 1) // 기본상점 UI
            {
                // 상점에 들어가 있는 동안은 isLive = false 이기 때문에 Stop함수를 호출할 필요가 없음
                storeStd.Show();
            }
            else if (id == 2) // 보따리상점 UI
            {
                Stop();  // Stop the game
                tmShop.Show();  // Show the travelling merchant shop
            }
        }
    }
}