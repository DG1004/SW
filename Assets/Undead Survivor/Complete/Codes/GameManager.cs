using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Goldmetal.UndeadSurvivor ���ӽ����̽� �ȿ� ��� �ڵ带 ���Խ�ŵ�ϴ�.
namespace Goldmetal.UndeadSurvivor
{
    // GameManager Ŭ������ ������ �������� �帧�� ���¸� �����մϴ�.
    // MonoBehaviour�� ��ӹ޾� ����Ƽ�� ������Ʈ�� ���˴ϴ�.
    public class GameManager : MonoBehaviour
    {
        // �̱��� ������ ����Ͽ� ��𼭵� GameManager.instance�� ������ �� �ְ� �մϴ�.
        public static GameManager instance;


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
        // �÷��̾��� ���� ������ ��Ÿ���ϴ�.
        public int level;
        // �÷��̾ óġ�� ���� ���� ����մϴ�.
        public int kill;
        // ���� ������ ����ġ�� �����մϴ�.
        public int exp;
        // �������� �ʿ��� ���� ����ġ ���� ������ �迭�Դϴ�.
        public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

        [Header("# Game Object")]
        // ������Ʈ Ǯ���� �����ϴ� �Ŵ����Դϴ� (��, ������ ���� ȿ�������� ����).
        public PoolManager pool;
        // �÷��̾� ĳ���͸� �����ϴ� ��ũ��Ʈ�Դϴ�.
        public Player player;
        // ������ UI�� �����ϴ� ��ũ��Ʈ�Դϴ�.
        public LevelUp uiLevelUp;
        // ���� ��� ȭ���� �����ϴ� ��ũ��Ʈ�Դϴ�.
        public Result uiResult;
        // ���̽�ƽ UI�� Transform�Դϴ� (����� ���� ���� ����).
        public Transform uiJoy;
        // ���� �¸� �� ���� ������ �����ϱ� ���� ������Ʈ�Դϴ�.
        public GameObject enemyCleaner;
        public TMSHOP tmShop;  // Inspector���� �ݵ�� �Ҵ��ؾ� ��
        public Store store;

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
            // �÷��̾��� ID�� �����մϴ�.
            playerId = id;
            // �÷��̾��� ü���� �ִ� ü������ �ʱ�ȭ�մϴ�.
            health = maxHealth;

            // �÷��̾� ������Ʈ�� Ȱ��ȭ�Ͽ� ���ӿ� �����ŵ�ϴ�.
            player.gameObject.SetActive(true);
            // ���� ������Ʈ�� Ȱ��ȭ�Ͽ� ���ӿ� �����ŵ�ϴ�.
            store.gameObject.SetActive(true); 
            // ������ UI���� �÷��̾� ID�� ���� ������ �����մϴ�.
            uiLevelUp.Select(playerId % 2);
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

        // ����ġ�� ȹ���� �� ȣ��Ǵ� �Լ��Դϴ�.
        public void GetExp()
        {
            // ������ ���� ���� �ƴϸ� �Ʒ� ������ �������� �ʽ��ϴ�.
            if (!isLive)
                return;

            // ����ġ�� 1 ������ŵ�ϴ�.
            exp++;

            // ���� �������� �ʿ��� ����ġ�� �����ߴ��� Ȯ���մϴ�.
            // Mathf.Min�� ����Ͽ� �迭 ������ ���� �ʵ��� �մϴ�.
            if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)])
            {
                // ������ 1 ������ŵ�ϴ�.
                level++;
                // ����ġ�� 0���� �ʱ�ȭ�մϴ�.
                exp = 0;
                // ������ UI�� ǥ���մϴ�.
                uiLevelUp.Show();
            }
        }

        // ������ �Ͻ������� �� ȣ��Ǵ� �Լ��Դϴ�.
        public void Stop()
        {
            // ���� ���� ���¸� false�� �����մϴ�.
            isLive = false;
            // ������ �ð� �帧�� ����ϴ�.
            Time.timeScale = 0;
            // ���̽�ƽ UI�� ����ϴ� (ũ�⸦ 0���� ����).
            uiJoy.localScale = Vector3.zero;
        }

        // ������ �簳�� �� ȣ��Ǵ� �Լ��Դϴ�.
        public void Resume()
        {
            // ���� ���� ���¸� true�� �����մϴ�.
            isLive = true;
            // ������ �ð� �帧�� �������� �����ϴ�.
            Time.timeScale = 1;
            // ���̽�ƽ UI�� ǥ���մϴ� (ũ�⸦ ������� ����).
            uiJoy.localScale = Vector3.one;
        }
        public void ShowShop()
        {
            Stop();  // Stop the game
            tmShop.Show();  // Show the travelling merchant shop
        }
    }
}
