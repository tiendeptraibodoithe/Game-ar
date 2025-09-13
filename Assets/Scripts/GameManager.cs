using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool startedGame = false;
    public bool stillAlive = true;
    public int points = 0;
    public GameObject pressText;
    public TextMesh scoreText;
    public BirdMovement birdMovementScript;
    public GameObject[] movableObjectParents;
    public EnemySpawnManager enemySpawnManager;


    [Header("Animation Settings")]
    public GameObject immovableObjects; // ImmovableObjects GameObject
    public Animator immovableObjectsAnimator; // Animator component của ImmovableObjects
    public string startAnimationName = "AnimationStartGame"; // Tên animation
    public float animationDuration = 1f; // Thời gian animation (giây)
    public GameObject birdObject; // Bird GameObject để tạm tắt

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Chạy animation khi game bắt đầu
        PlayStartAnimation();
    }

    private void Update()
    {
        if (!startedGame && Input.GetMouseButtonUp(0))
        {
            pressText.SetActive(false);
            startedGame = true;

            // Bắt đầu spawn enemy khi game chính thức start
            if (enemySpawnManager != null)
            {
                enemySpawnManager.BeginSpawning();
            }
        }
        if (startedGame)
        {
            scoreText.text = points + "";
        }
        if (Input.GetKeyUp("space") || Input.GetMouseButtonUp(0) && !stillAlive && startedGame)
        {
            ResetGame();
        }
    }

    private void PlayStartAnimation()
    {
        if (immovableObjectsAnimator != null)
        {
            immovableObjectsAnimator.Play(startAnimationName, -1, 0f);

            // Tạm tắt toàn bộ Bird object trong lúc animation
            StartCoroutine(DisableBirdDuringAnimation());
        }
    }

    private IEnumerator DisableBirdDuringAnimation()
    {
        // Tắt toàn bộ Bird GameObject
        if (birdObject != null)
        {
            birdObject.SetActive(false);
        }

        // Chờ animation hoàn thành
        yield return new WaitForSeconds(animationDuration);

        // Bật lại Bird GameObject
        if (birdObject != null)
        {
            birdObject.SetActive(true);
        }
    }

    public void AddPoints(int points)
    {
        this.points += points;
    }

    public void ResetGame()
    {
        points = 0;
        scoreText.text = "";
        startedGame = false;
        stillAlive = true;
        pressText.SetActive(true);
        foreach (GameObject gb in movableObjectParents)
        {
            foreach (Transform child in gb.transform)
            {
                Destroy(child.gameObject);
            }
            gb.GetComponent<ObjectsMoveManager>().SpawnCall();
        }

        if (enemySpawnManager != null)
        {
            enemySpawnManager.ResetEnemies(false);
        }

        birdMovementScript.ResetBird();

        Health playerHealth = birdObject.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
        }

        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.ResetExperience();
        }

        // Reset toàn bộ upgrade
        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.ResetUpgrades();
        }

        PlayStartAnimation();
    }
}