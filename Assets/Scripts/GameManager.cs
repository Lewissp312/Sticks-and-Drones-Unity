using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    public const float zBound = 10;
    public const float xBound = 17;
    public static GameManager Instance;
    public enum Direction { LEFT, TOP, RIGHT };
    public enum CauseOfFailure { DRONE,LASER,MISSED_TREE}
    private bool isGameActive;
    private bool isPaused;
    private int score;
    private int lives;
    private float enemySpeed;
    private float treeSpeed;
    private float enemySpawnRate;
    private float treeSpawnRate;
    private float soundEffectsVolume;
    private MoveBackground backgroundScript;
    private Vector3 cameraPosition;
    private IObjectPool<Enemy> dronePool;
    private IObjectPool<Tree> treePool;
    private IObjectPool<SoundOrEffect> soundOrEffectPool;
    private InputAction _pauseAction;
    [SerializeField] private GameObject[] powerUps;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject guideMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private Button playButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button gameOverRestartButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioSource music;
    [SerializeField] private Slider musicVolumeMenuSlider;
    [SerializeField] private Slider musicVolumePauseSlider;
    [SerializeField] private Slider soundEffectsVolumeMenuSlider;
    [SerializeField] private Slider soundEffectsVolumePauseSlider;
    [SerializeField] private PlayerController playerController;


    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        _pauseAction = InputSystem.actions.FindAction("Pause");
        backgroundScript = GameObject.FindGameObjectWithTag("Background").GetComponent<MoveBackground>();
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", musicVolumeMenuSlider.value);
            PlayerPrefs.Save();
        }
        music.volume = PlayerPrefs.GetFloat("musicVolume");
        musicVolumeMenuSlider.value = music.volume;
        if (!PlayerPrefs.HasKey("musicTime"))
        {
            PlayerPrefs.SetFloat("musicTime", 0);
            PlayerPrefs.Save();
        }
        music.time = PlayerPrefs.GetFloat("musicTime");
        if (!PlayerPrefs.HasKey("soundEffectsVolume"))
        {
            PlayerPrefs.SetFloat("soundEffectsVolume", soundEffectsVolumeMenuSlider.value);
            PlayerPrefs.Save();
        } 
        soundEffectsVolume = PlayerPrefs.GetFloat("soundEffectsVolume");
        soundEffectsVolumeMenuSlider.value = soundEffectsVolume;
        dronePool = ObjectPooler.Instance.GetDronePool();
        treePool = ObjectPooler.Instance.GetTreePool();
        soundOrEffectPool = ObjectPooler.Instance.GetSoundOrEffectPool();
        cameraPosition = Camera.main.transform.position;
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        PlayerPrefs.SetFloat("musicVolume",musicVolumeMenuSlider.value);
        PlayerPrefs.SetFloat("soundEffectsVolume",soundEffectsVolumeMenuSlider.value);
        PlayerPrefs.Save();
        musicVolumePauseSlider.value = musicVolumeMenuSlider.value;
        soundEffectsVolumePauseSlider.value = soundEffectsVolumeMenuSlider.value;
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
        InvokeRepeating(nameof(IncreaseSpeed), 10, 25);
        InvokeRepeating(nameof(IncreaseSpawnRate), 25, 25);
    }

    void Update()
    {
        if (_pauseAction.WasPressedThisFrame() && isGameActive)
        {
            PauseMenu();
        }
    }

    public void PauseMenu()
    {
        isPaused = !isPaused;
        scoreText.gameObject.SetActive(!scoreText.gameObject.activeSelf);
        highScoreText.gameObject.SetActive(!highScoreText.gameObject.activeSelf);
        livesText.gameObject.SetActive(!livesText.gameObject.activeSelf);
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        Time.timeScale = isPaused ? 0 : 1;
        if (isPaused)
        {
            resumeButton.Select();
        }
        if (!isPaused)
        {
            PlayerPrefs.SetFloat("musicVolume", musicVolumePauseSlider.value);
            PlayerPrefs.SetFloat("soundEffectsVolume", soundEffectsVolumePauseSlider.value);
            PlayerPrefs.Save();
        }
    }
    
    public void GuideMenu()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        guideMenu.SetActive(!guideMenu.activeSelf);
        if (mainMenu.activeSelf){playButton.Select();}
        else{backButton.Select();}
    }

    public void AdjustMusicVolumeFromMenuSlider()
    {
        music.volume = musicVolumeMenuSlider.value;
    }

    public void AdjustSoundEffectsVolumeFromMenuSlider()
    {
        float soundEffectsVolumeDifference = soundEffectsVolume - soundEffectsVolumeMenuSlider.value;
        soundEffectsVolume = soundEffectsVolumeMenuSlider.value;
        if (soundEffectsVolumeDifference < -0.05 || soundEffectsVolumeDifference > 0.05)
        {
            PlaySound(hurtSound);
        }
    }

    public void AdjustMusicVolumeFromPauseSlider()
    {
        music.volume = musicVolumePauseSlider.value;
    }

    public void AdjustSoundEffectsVolumeFromPauseSlider()
    {
        float soundEffectsVolumeDifference = soundEffectsVolume - soundEffectsVolumePauseSlider.value;
        soundEffectsVolume = soundEffectsVolumePauseSlider.value;
        if (soundEffectsVolumeDifference < -0.05 || soundEffectsVolumeDifference > 0.05)
        {
            PlaySound(hurtSound);
        }
    }

    public void PlaySound(AudioClip clipToPlay)
    {
        SoundOrEffect soundOrEffect = soundOrEffectPool.Get();
        soundOrEffect.transform.position = cameraPosition;
        soundOrEffect.SetAsSound(clipToPlay, soundEffectsVolume);
    }

    public void PlayParticleEffect(Vector3 positionToPlay, SoundOrEffect.Purpose purpose)
    {
        SoundOrEffect soundOrEffect = soundOrEffectPool.Get();
        soundOrEffect.transform.position = positionToPlay;
        soundOrEffect.SetAsParticleEffect(purpose);
    }

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
        Enemy drone = dronePool.Get();
        Vector3 generatedSpawn = GenerateSpawn(enemySpawnDirection, drone.transform.position.y);
        drone.transform.position = generatedSpawn;
        switch (enemySpawnDirection)
        {
            case Direction.LEFT:
                drone.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case Direction.TOP:
                drone.transform.rotation = Quaternion.Euler(0, -180, 0);
                break;
            case Direction.RIGHT:
                drone.transform.rotation = Quaternion.Euler(0, -90, 0);
                break;
        }
        if (generatedSpawn.z > -2)
        {
            int randNum = Random.Range(1, 10);
            if (randNum == 3)
            {
                Vector3 shootingPosition;
                switch (enemySpawnDirection)
                {
                    case Direction.LEFT or Direction.RIGHT:
                        float randX = Random.Range(-xBound + 4, xBound - 4);
                        shootingPosition = new(randX, generatedSpawn.y, generatedSpawn.z);
                        drone.SetAsShootingEnemy(shootingPosition);
                        break;
                    case Direction.TOP:
                        float randZ = Random.Range(-3f, zBound - 4);
                        shootingPosition = new(generatedSpawn.x, generatedSpawn.y, randZ);
                        drone.SetAsShootingEnemy(shootingPosition);
                        break;
                }
            }
        }
        drone.SetSpeed(enemySpeed);
        Invoke(nameof(SpawnEnemy), enemySpawnRate);
    }

    void SpawnTree(){
        Tree tree = treePool.Get();
        tree.transform.SetPositionAndRotation(GenerateSpawn(Direction.TOP, tree.transform.position.y), tree.transform.rotation);
        tree.SetSpeed(treeSpeed);
        Invoke(nameof(SpawnTree), treeSpawnRate);
    }

    Vector3 GenerateSpawn(Direction selectedDirection,float spawnedObjectYPos){
        float randZ;
        switch (selectedDirection)
        {
            case Direction.LEFT:
                randZ = Random.Range(-zBound+4, zBound-4);
                return new Vector3(-xBound, spawnedObjectYPos, randZ);
            case Direction.TOP:
                float randX = Random.Range(-xBound+4, xBound-4);
                return new Vector3(randX, spawnedObjectYPos, zBound);
            case Direction.RIGHT:
                randZ = Random.Range(-zBound+4, zBound-4);
                return new Vector3(xBound, spawnedObjectYPos, randZ);
            default:
                return new Vector3(xBound, spawnedObjectYPos, 3);
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
        int randNum = Random.Range(0, 6);
        if (randNum == 4 && treeSpeed < 1.5f)
        {
            randNum = 0;
        }
        GameObject powerUp = Instantiate(powerUps[randNum], GenerateSpawn(spawnDirection, powerUps[randNum].transform.position.y), powerUps[randNum].transform.rotation);
        powerUp.GetComponent<PowerUps>().SetDirection(spawnDirection);
        playerController.IncreaseSpeed(0.5f);
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

    public void DecreaseSpeed(int speedToDecrease)
    {
        switch (speedToDecrease)
        {
            case 1:
                enemySpeed -= 1;
                GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in activeEnemies) { enemy.GetComponent<Enemy>().SetSpeed(enemySpeed); }
                break;
            case 2:
                treeSpeed -= 1;
                GameObject[] activeTrees = GameObject.FindGameObjectsWithTag("Tree");
                foreach (GameObject tree in activeTrees) { tree.GetComponent<Tree>().SetSpeed(treeSpeed); }
                backgroundScript.SetSpeed(treeSpeed);
                break;
        }
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
        if (numToAdd < 0){PlaySound(hurtSound);}
        livesText.text=$"Lives:{lives}";
        if (lives==0){GameOver(causeOfFailure);}
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
                gameOverMenu.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Game over, you were hit by a drone!";
                break;
            case CauseOfFailure.LASER:
                gameOverMenu.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Game over, you were hit by a drone's laser!";
                break;
            case CauseOfFailure.MISSED_TREE:
                gameOverMenu.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Game over, you missed a tree!";
                break;
        }
        gameOverMenu.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = $"Your final score was {score}";
        if (score > PlayerPrefs.GetInt("highScore"))
        {
            gameOverMenu.transform.GetChild(2).gameObject.SetActive(true);
            PlayerPrefs.SetInt("highScore", score);
        }
        gameOverRestartButton.Select();
        gameOverMenu.SetActive(true);
    }

    public void RestartGame()
    {
        if (isPaused)
        {
            PlayerPrefs.SetFloat("musicVolume", musicVolumePauseSlider.value);
            PlayerPrefs.SetFloat("soundEffectsVolume", soundEffectsVolumePauseSlider.value);
            PlayerPrefs.Save();
            Time.timeScale = 1;
        }
        PlayerPrefs.SetFloat("musicTime", music.time);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        PlayerPrefs.SetFloat("musicVolume", musicVolumeMenuSlider.value);
        PlayerPrefs.SetFloat("musicTime", 0);
        PlayerPrefs.SetFloat("soundEffectsVolume", soundEffectsVolumeMenuSlider.value);
        PlayerPrefs.Save();
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
