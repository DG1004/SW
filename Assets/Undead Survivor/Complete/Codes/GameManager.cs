using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

// Goldmetal.UndeadSurvivor ���ӽ����̽� �ȿ� ��� �ڵ带 ���Խ�ŵ�ϴ�.
namespace Goldmetal.UndeadSurvivor
{
    // GameManager Ŭ������ ������ �������� �帧�� ���¸� �����մϴ�.
    // MonoBehaviour�� ��ӹ޾� ����Ƽ�� ������Ʈ�� ���˴ϴ�.
    public class GameManager : MonoBehaviour
    {
        // �̱��� ������ ����Ͽ� ��𼭵� GameManager.instance�� ������ �� �ְ� �մϴ�.
        public static GameManager instance;
        public int EnemyNum;

        // [Header]�� �ν����Ϳ��� �������� �׷�ȭ�Ͽ� ���� ���� ����ϴ�.
        [Header("# Game Control")]
        // ������ ���� ������ ���θ� ��Ÿ���� �����Դϴ�.
        public bool isLive;
        // ������ ����� �ð��� �����ϴ� �����Դϴ�.
        public float gameTime;
        // ������ �ִ� �ð��� �����մϴ� (���⼭�� 20�ʷ� ����).
        public float maxGameTime = 2 * 10f;

        [Header("# Player Info")]
        // �÷��̾��� ID�� �����մϴ� (ĳ���� ���� � ���� �� ����).
        public int playerId;
        // �÷��̾��� ���� ü���� ��Ÿ���ϴ�.
        public float health;
        // �÷��̾��� �ִ� ü���� �����մϴ�.
        public float maxHealth = 100;
        // �÷��̾ óġ�� ���� ���� ����մϴ�.
        public int kill;
        // �÷��̾ ȹ���� ������ ���� ����մϴ�.
        public int Coin;
        // ���� ����
        public int level;
        public int exp;
        public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };
        [Header("# Game Object")]
        // ������ �����ϴ� �Ŵ�����
        public CoinManager CoinManager;
        // ������Ʈ Ǯ���� �����ϴ� �Ŵ����Դϴ� (��, ������ ���� ȿ�������� ����).
        public PoolManager pool;
        // �÷��̾� ĳ���͸� �����ϴ� ��ũ��Ʈ�Դϴ�.
        public Player player;
        // ������ UI�� �����ϴ� ��ũ��Ʈ�Դϴ�.
        public LevelUp uiLevelUp;
        //  ���� UI ����
        public StoreStd uiStore;
        // ���� ��� ȭ���� �����ϴ� ��ũ��Ʈ�Դϴ�.
        public Result uiResult;
        // ���� �¸� �� ���� ������ �����ϱ� ���� ������Ʈ�Դϴ�.
        public GameObject enemyCleaner;
        public StoreStd storeStd;
        public TMSHOP tmShop;  // Inspector���� �ݵ�� �Ҵ��ؾ� ��
        public StoreEntrance store;
        public Arrow arrow;

        // Awake�� ��ũ��Ʈ�� ó�� �ε�� �� ȣ��Ǵ� �Լ��Դϴ�.
        void Awake()
        {
            // �̱��� �ν��Ͻ��� ���� ������Ʈ�� �����մϴ�.
            instance = this;
            // ���ø����̼��� �ִ� ������ ����Ʈ�� 60���� �����մϴ�.
            Application.targetFrameRate = 60;
        }

        // ������ ������ �� ȣ��Ǵ� �Լ��Դϴ�.
        public void GameStart(int id)
        {
            EnemyNum = 0;
            // �÷��̾��� ID�� �����մϴ�.
            playerId = id;
            // �÷��̾��� ü���� �ִ� ü������ �ʱ�ȭ�մϴ�.
            health = maxHealth;

            // �÷��̾� ������Ʈ�� Ȱ��ȭ�Ͽ� ���ӿ� �����ŵ�ϴ�.
            player.gameObject.SetActive(true);
            // ���� ������Ʈ�� Ȱ��ȭ�Ͽ� ���ӿ� �����Ű��, ��ġ�� �����մϴ�.
            store.gameObject.SetActive(true);
            store.changePosition();
            // ȭ��ǥ ������Ʈ�� Ȱ��ȭ�Ͽ� ���ӿ� �����ŵ�ϴ�.
            arrow.gameObject.SetActive(true);
            // ���� UI���� �÷��̾� ID�� ���� ������ �����մϴ�.
            //uiStore.Select(playerId % 2);
            // ������ �簳�մϴ� (�Ͻ����� ���¿��� Ǯ�� ��).
            Resume();

            // ��������� ����ϰ�, ���� ȿ������ ����մϴ�.
            AudioManager.instance.PlayBgm(true);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        }

        // �÷��̾ ����ϰų� ���� ���� ������ �����Ǿ��� �� ȣ��˴ϴ�.
        public void GameOver()
        {
            // GameOverRoutine �ڷ�ƾ�� �����Ͽ� ���� ���� ó���� �մϴ�.
            StartCoroutine(GameOverRoutine());
        }

        // ���� ���� ������ ���� �ڷ�ƾ �Լ��Դϴ�.
        IEnumerator GameOverRoutine()
        {
            // ���� ���� ���¸� false�� �����Ͽ� ���� ������ ����ϴ�.
            isLive = false;

            // 0.5�� ���� ����մϴ� (������ ���� ������).
            yield return new WaitForSeconds(0.5f);

            // ��� UI�� Ȱ��ȭ�ϰ� �й� ȭ���� ǥ���մϴ�.
            uiResult.gameObject.SetActive(true);
            uiResult.Lose();
            // ������ �Ͻ������մϴ�.
            Stop();

            // ��������� �����ϰ�, �й� ȿ������ ����մϴ�.
            AudioManager.instance.PlayBgm(false);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
        }

        // ���� �¸� ������ �����Ǿ��� �� ȣ��˴ϴ�.
        public void GameVictroy()
        {
            // GameVictroyRoutine �ڷ�ƾ�� �����Ͽ� �¸� ó���� �մϴ�.
            StartCoroutine(GameVictroyRoutine());
        }

        // ���� �¸� ������ ���� �ڷ�ƾ �Լ��Դϴ�.
        IEnumerator GameVictroyRoutine()
        {
            // ���� ���� ���¸� false�� �����Ͽ� ���� ������ ����ϴ�.
            isLive = false;
            // �����ִ� ������ �����ϱ� ���� enemyCleaner�� Ȱ��ȭ�մϴ�.
            enemyCleaner.SetActive(true);

            // 0.5�� ���� ����մϴ� (������ ���� ������).
            yield return new WaitForSeconds(0.5f);

            // ��� UI�� Ȱ��ȭ�ϰ� �¸� ȭ���� ǥ���մϴ�.
            uiResult.gameObject.SetActive(true);
            uiResult.Win();
            // ������ �Ͻ������մϴ�.
            Stop();

            // ��������� �����ϰ�, �¸� ȿ������ ����մϴ�.
            AudioManager.instance.PlayBgm(false);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
        }

        // ������ �ٽ� ������ �� ȣ��Ǵ� �Լ��Դϴ�.
        public void GameRetry()
        {
            // ���� ��(�� ��ȣ 0)�� �ٽ� �ε��Ͽ� ������ �ʱ�ȭ�մϴ�.
            SceneManager.LoadScene(0);
        }

        // ������ ������ �� ȣ��Ǵ� �Լ��Դϴ�.
        public void GameQuit()
        {
            // ���ø����̼��� �����մϴ� (�����Ϳ����� �������� ����).
            Application.Quit();
        }

        // �� �����Ӹ��� ȣ��Ǵ� ������Ʈ �Լ��Դϴ�.
        void Update()
        {
            // esc Ű�� ���� ������ ���߰ų� �簳�Ѵ�.
            // �������� escŰ�� ���� ��� ������ ����� ����
            // isLive�� true�� �� ���Ͱ� ����ؼ� �����Ǵ� �� ��ġ��
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
            // ������ ���� ���� �� �Ʒ� ������ �����մϴ�.
            if (isLive)
            {

                // ���� �ð��� ������ŵ�ϴ�.
                gameTime += Time.deltaTime;

                // ���� �ð��� �ִ� �ð��� �ʰ��ϸ� �¸� ó���� �մϴ�.
                if (gameTime > maxGameTime)
                {
                    // ���� �ð��� �ִ� �ð����� �����մϴ�.
                    gameTime = maxGameTime;
                    // ���� �¸� �Լ��� ȣ���մϴ�.
                    GameVictroy();
                }
            }
            // �÷��̾ ������ ���� �� �Ʒ� ������ �����մϴ�.
            if (player.isStore == 1)
            {
                player.inputVec.x = Input.GetAxisRaw("Horizontal");
                player.inputVec.y = Input.GetAxisRaw("Vertical");
                Vector2 nextVec = player.inputVec.normalized * player.speed * Time.fixedDeltaTime;
                player.rigid.MovePosition(player.rigid.position + nextVec);
            }
        }

        // ���� ����
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


        // ������ �Ͻ������� �� ȣ��Ǵ� �Լ��Դϴ�.
        public void Stop()
        {
            // ���� ���� ���¸� false�� �����մϴ�.
            isLive = false;
            // ������ �ð� �帧�� ����ϴ�.
            Time.timeScale = 0;
        }

        // ������ �簳�� �� ȣ��Ǵ� �Լ��Դϴ�.
        public void Resume()
        {
            // ���� ���� ���¸� true�� �����մϴ�.
            isLive = true;
            // ������ �ð� �帧�� �������� �����ϴ�.
            Time.timeScale = 1;
        }
        public void ShowShop(int id)
        {
            if (id == 1) // �⺻���� UI
            {
                // ������ �� �ִ� ������ isLive = false �̱� ������ Stop�Լ��� ȣ���� �ʿ䰡 ����
                storeStd.Show();
            }
            else if (id == 2) // ���������� UI
            {
                Stop();  // Stop the game
                tmShop.Show();  // Show the travelling merchant shop
            }
        }
    }
}