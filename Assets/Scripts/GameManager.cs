using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor.PackageManager;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum Direction { LEFT, TOP, RIGHT };
    public enum CauseOfFailure { DRONE,MISSED_TREE}
    private bool isGameActive;
    private bool isPaused;
    private int score;
    private int lives;
    private float enemySpeed;
    private float treeSpeed;
    private float enemySpawnRate;
    private float treeSpawnRate;
    private const float zBound = 7;
    private const float xBound = 11;
    private AudioSource soundEffects;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] powerUps;
    [SerializeField] private GameObject tree;
    [SerializeField] private GameObject titleText;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private AudioClip crash;
    [SerializeField] private AudioSource music;
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundEffectsVolumeSlider;
    [SerializeField] private MoveBackground backgroundScript;


    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        soundEffects = GetComponent<AudioSource>();
        backgroundScript = GameObject.FindGameObjectWithTag("Background").GetComponent<MoveBackground>();
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume",musicVolumeSlider.value);
            PlayerPrefs.Save();
        }
        music.volume = PlayerPrefs.GetFloat("musicVolume");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        if (!PlayerPrefs.HasKey("soundEffectsVolume"))
        {
            PlayerPrefs.SetFloat("soundEffectsVolume",soundEffectsVolumeSlider.value);
            PlayerPrefs.Save();
        }
        soundEffects.volume = PlayerPrefs.GetFloat("soundEffectsVolume");
        soundEffectsVolumeSlider.value = PlayerPrefs.GetFloat("soundEffectsVolume");
    }

    public void StartGame()
    {
        titleText.SetActive(false);
        PlayerPrefs.SetFloat("musicVolume",musicVolumeSlider.value);
        PlayerPrefs.SetFloat("soundEffectsVolume",soundEffectsVolumeSlider.value);
        PlayerPrefs.Save();
        enemySpawnRate = 2;
        treeSpawnRate = 5;
        enemySpeed = 4.0f;
        treeSpeed = 0.5f;
        lives = 3;
        livesText.text = $"Lives:{lives}";
        scoreText.text = $"Score:{score}";
        if (!PlayerPrefs.HasKey("highScore"))
        {
            PlayerPrefs.SetInt("highScore", 0);
        }
        highScoreText.text = $"High Score: {PlayerPrefs.GetInt("highScore")}"; 
        backgroundScript.SetSpeed(treeSpeed);
        isGameActive = true;
        Invoke(nameof(SpawnEnemy), enemySpawnRate);
        Invoke(nameof(SpawnTree), treeSpawnRate);
        InvokeRepeating(nameof(IncreaseSpeed), 25, 25);
        InvokeRepeating(nameof(IncreaseSpawnRate), 25, 25);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && isGameActive){
            isPaused = !isPaused;
            scoreText.gameObject.SetActive(!scoreText.gameObject.activeSelf);
            highScoreText.gameObject.SetActive(!highScoreText.gameObject.activeSelf);
            livesText.gameObject.SetActive(!livesText.gameObject.activeSelf);
            pauseText.gameObject.SetActive(!pauseText.gameObject.activeSelf);
            AudioListener.pause = !AudioListener.pause;
            Time.timeScale = isPaused ? 0 : 1;
        }

    }

    public void AdjustMusicVolumeFromSlider()
    {
        music.volume = musicVolumeSlider.value;
        // PlayerPrefs.SetFloat("musicVolume",musicVolumeSlider.value);
        // PlayerPrefs.Save();
    }

    public void AdjustSoundEffectsVolumeFromSlider()
    {
        soundEffects.volume = soundEffectsVolumeSlider.value;
        // PlayerPrefs.SetFloat("soundEffectsVolume",soundEffectsVolumeSlider.value);
        // PlayerPrefs.Save();
    }

    public void PlayTestEffect(){ AudioSource.PlayClipAtPoint(crash, Camera.main.transform.position,soundEffectsVolumeSlider.value);}

    public float GetSoundEffectsVolume(){return soundEffectsVolumeSlider.value;}
    public bool GetIsGameActive(){return isGameActive;}
    public bool GetIsGamePaused() { return isPaused; }
    public int GetLives() { return lives; }
    public float GetEnemySpeed(){return enemySpeed;}
    public float GetTreeSpeed() { return treeSpeed; }

    Direction ChooseDirection(){
        int randNum = Random.Range(1,4);
        return randNum switch
        {
            1 => Direction.LEFT,
            2 => Direction.TOP,
            3 => Direction.RIGHT,
            _ => Direction.TOP,
        };
    }

    void SpawnEnemy()
    {
        Direction enemySpawnDirection = ChooseDirection();
        int randNum = Random.Range(0, 3);
        GameObject enemyToSpawn = enemySpawnDirection switch
        {
            Direction.LEFT => Instantiate(enemies[randNum], GenerateSpawn(enemySpawnDirection, enemies[randNum].transform.position.y), Quaternion.Euler(0, 90, 0)),
            Direction.TOP => Instantiate(enemies[randNum], GenerateSpawn(enemySpawnDirection, enemies[randNum].transform.position.y), Quaternion.Euler(0, -180, 0)),
            _ => Instantiate(enemies[randNum], GenerateSpawn(enemySpawnDirection, enemies[randNum].transform.position.y), Quaternion.Euler(0, -90, 0)),
        };
        Invoke(nameof(SpawnEnemy), enemySpawnRate);
    }

    void SpawnTree(){
        Instantiate(tree,GenerateSpawn(Direction.TOP,tree.transform.position.y),tree.transform.rotation);
        Invoke(nameof(SpawnTree), treeSpawnRate);
    }

    Vector3 GenerateSpawn(Direction selectedDirection,float spawnedObjectYPos){
        float randZ;
        switch (selectedDirection)
        {
            case Direction.LEFT:
                randZ = Random.Range(-zBound, zBound);
                return new Vector3(-17.1f, spawnedObjectYPos, randZ);
            case Direction.TOP:
                float randX = Random.Range(-xBound, xBound);
                return new Vector3(randX, spawnedObjectYPos, 10);
            case Direction.RIGHT:
                randZ = Random.Range(-zBound, zBound);
                return new Vector3(17.1f, spawnedObjectYPos, randZ);
            default:
                return new Vector3(17.1f, spawnedObjectYPos, 3);
        }
    }

    public void MovementRestrictions(GameObject movingObject){
        if (movingObject.transform.position.x> xBound+7 ||
        movingObject.transform.position.x<-(xBound+7) ||
        movingObject.transform.position.z<-(zBound+7))
        {
            if(movingObject.CompareTag("Tree"))
            {
                if(!movingObject.GetComponent<Tree>().GetIsRebuilt())
                {
                    UpdateLives(-1,CauseOfFailure.MISSED_TREE);
                }
            }
            Destroy(movingObject);
        }
    }

    void IncreaseSpeed()
    {
        enemySpeed += 0.5f;
        treeSpeed += 0.5f;
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] activeTrees = GameObject.FindGameObjectsWithTag("Tree");
        foreach (GameObject enemy in activeEnemies) { enemy.GetComponent<Enemy>().SetSpeed(enemySpeed); }
        foreach (GameObject tree in activeTrees) { tree.GetComponent<Tree>().SetSpeed(treeSpeed); }
        backgroundScript.SetSpeed(treeSpeed);
        Direction spawnDirection = ChooseDirection();
        int randNum = Random.Range(0, 4);
        GameObject powerUp = Instantiate(powerUps[randNum], GenerateSpawn(spawnDirection, powerUps[randNum].transform.position.y), powerUps[randNum].transform.rotation);
        powerUp.GetComponent<PowerUps>().SetDirection(spawnDirection);
    }

    void IncreaseSpawnRate()
    {
        if (!(enemySpawnRate - 0.3f < 1))
        {
            enemySpawnRate -= 0.3f;
        }
        if (!(treeSpawnRate - 0.3f < 1))
        {
            treeSpawnRate -= 0.3f;
        }
        Direction powerUpSpawnDirection = ChooseDirection();
        int randNum = Random.Range(0, 4);
        GameObject powerUp = Instantiate(powerUps[randNum], GenerateSpawn(powerUpSpawnDirection, powerUps[randNum].transform.position.y), powerUps[randNum].transform.rotation);
        powerUp.GetComponent<PowerUps>().SetDirection(powerUpSpawnDirection);
        
    }

    public void UpdateScore(int numToAdd)
    {
        score += numToAdd;
        scoreText.text = $"Score:{score}";
        if (score > PlayerPrefs.GetInt("highScore"))
        {
            highScoreText.text = $"High Score: {score}"; 
        }
    }

    public void UpdateLives(int numToAdd,CauseOfFailure causeOfFailure = CauseOfFailure.DRONE){
        //Default value is never actually used, this is for when the player's lives are increased by a power up
        lives += numToAdd;
        livesText.text=$"Lives:{lives}";
        if (lives==0){
            GameOver(causeOfFailure);
        }
    }

    public void GameOver(CauseOfFailure causeOfFailure){
        isGameActive=false;
        CancelInvoke();
        scoreText.gameObject.SetActive(false);
        highScoreText.gameObject.SetActive(false);
        livesText.gameObject.SetActive(false);
        switch (causeOfFailure)
        {
            case CauseOfFailure.DRONE:
                gameOverText.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Game over, you were hit by a drone!";
                GameObject.FindGameObjectWithTag("Player").SetActive(false);
                break;
            case CauseOfFailure.MISSED_TREE:
                gameOverText.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Game over, you missed a tree!";
                break;
        }
        gameOverText.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = $"Your final score was {score}";
        if (score > PlayerPrefs.GetInt("highScore"))
        {
            gameOverText.transform.GetChild(2).gameObject.SetActive(true);
            PlayerPrefs.SetInt("highScore", score);
        }
        gameOverText.SetActive(true);
    }

    public void RestartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
