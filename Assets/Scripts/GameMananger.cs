using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameMananger : MonoBehaviour
{
    [SerializeField] private MeshRenderer floorMeshRenderer;
    public static GameMananger Instance;
    [HideInInspector]public bool gameStarted = false;
    [HideInInspector]public bool gameEnded = false;
    [Header("Speed Settings")]
    public float startingSpeed = 0.5f;
    public float speedIncreasePerSecond = 0.1f;
    private float currentSpeed;
    [Header("UI")] public TextMeshProUGUI scoreText;
    private int highScore = 0;
    private float currentScore = 0;
    public float currentScoreIncreaseSpeed = 2f;
    public GameObject gameEndScreen;
    public InputField nameInputField;

    [Header("Obstacle Spawn")]
    public float minTimeDelayBetweenEachObstacle = 1f;
    public float maxTimeDelayBetweenEachObstacle = 1f;
    public float obstacleSpeedMultiplier = 3f;

    public GameObject[] allGroundObstacles;
    public GameObject[] allFlyingObstacles;
    [Space]
    public Transform groundObstaclesSpawnPoint;
    public Transform flyingObstaclesSpawnPoint;
    private float timeUntilNextObstacle= 1f;

    private List<GameObject> allCurrentObstacles = new List<GameObject>();

    [Header("SFX")]
    [SerializeField]
    public AudioSource audioSource;
    public AudioClip pointsSFX;




    private void Awake()
    {
        Instance = this;
        currentSpeed = startingSpeed;
        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
        }
        UpdateScoreUI();

    }
    private void Update()
    {
        if (gameStarted && !gameEnded)
        {
            timeUntilNextObstacle -= Time.deltaTime*currentSpeed;

            if(timeUntilNextObstacle <= 0)
            {
                timeUntilNextObstacle = Random.Range(minTimeDelayBetweenEachObstacle,maxTimeDelayBetweenEachObstacle);
                
                //Spawn new obstacle
                if(currentScore >= 50)
                {
                    //Randomly spawn ground or air obstacle
                    if (Random.value> 0.8f)
                    {
                        //Air obstacle
                       GameObject newObstacle = Instantiate(allFlyingObstacles[Random.Range(0, allFlyingObstacles.Length)],
                            flyingObstaclesSpawnPoint.position, Quaternion.identity);
                        allCurrentObstacles.Add(newObstacle);
                    }
                    else
                    {
                        GameObject newObstacle = Instantiate(allGroundObstacles[Random.Range(0, allGroundObstacles.Length)], 
                            groundObstaclesSpawnPoint.position, Quaternion.identity);
                        allCurrentObstacles.Add(newObstacle);
                    }
                }
                else
                {
                    //Randolmy spawn ground obstacle
                    GameObject newObstacle = Instantiate(allGroundObstacles[Random.Range(0, allGroundObstacles.Length)],
                            groundObstaclesSpawnPoint.position, Quaternion.identity);
                    allCurrentObstacles.Add(newObstacle);
                }
            }

            foreach(var obstacle in allCurrentObstacles)
            {
                obstacle.transform.Translate(new Vector3(-currentSpeed * Time.deltaTime*obstacleSpeedMultiplier, 0, 0));
            }


            currentSpeed += Time.deltaTime * speedIncreasePerSecond;
            floorMeshRenderer.material.mainTextureOffset += new Vector2(currentSpeed * Time.deltaTime, 0);


            int lastCuttentScore = Mathf.RoundToInt(currentScore);

            currentScore += currentSpeed * Time.deltaTime*currentScoreIncreaseSpeed;

            if(Mathf.RoundToInt(currentScore) > lastCuttentScore && Mathf.RoundToInt(currentScore) % 50 == 0)
            {
                audioSource.clip = pointsSFX;
                audioSource.Play();
            }

            if (Mathf.RoundToInt(currentScore) > highScore)
            {
                highScore = Mathf.RoundToInt(currentScore);

                PlayerPrefs.SetInt("HighScore", highScore);
            }
        }
        UpdateScoreUI();
    }
    private void UpdateScoreUI()
    {
        scoreText.SetText($"HI {highScore:D5}  {Mathf.RoundToInt(currentScore):D5}");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void ShowGameEndScreen()
    {
        gameEndScreen.SetActive(true);
    }
    public void SubmitScore()
    {
        nameInputField.image.color = Color.green;
        string playerNameToLeaderboard;
        if (nameInputField.text == "")
        {
            playerNameToLeaderboard = "unknown";
        }
        else
        {
            playerNameToLeaderboard = nameInputField.text;
        }
        int scoreToLeaderboard = Mathf.RoundToInt(currentScore);
        HighScoreTable.AddHighscoreEntry(scoreToLeaderboard,playerNameToLeaderboard);
    }
}
