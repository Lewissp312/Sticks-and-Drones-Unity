using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

/// <summary>
/// Controls the flow of the game (e.g spawning enemies, trees, lives tracking)
/// </summary>
public class GameManager : MonoBehaviour
{
    public const float zBound = 10;
    public const float xBound = 17;
    public static GameManager Instance;
    public enum Direction { LEFT, RIGHT, TOP };
    public enum CauseOfFailure { DRONE, LASER, MISSED_TREE }
    private bool _isGameActive;
    private bool _isPaused;
    private int _score;
    private int _highScore;
    private int _lives;
    //Separate drone spawning bounds so that the drones do not appear offscreen
    private readonly float _droneSpawningXBound = xBound - 4;
    private readonly float _droneSpawningZBound = zBound - 4;
    private float _enemySpeed;
    private float _treeSpeed;
    private float _enemySpawnRate;
    private float _treeSpawnRate;
    private float _soundEffectsVolume;
    private Vector3 _cameraPosition;
    private IObjectPool<Enemy> _dronePool;
    private IObjectPool<Tree> _treePool;
    private IObjectPool<SoundOrEffect> _soundOrEffectPool;
    private InputAction _pauseAction;
    [SerializeField] private GameObject[] _powerUps;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _guideMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private MoveBackground _background;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _gameOverRestartButton;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private TextMeshProUGUI _gameOverCauseOfFailureText;
    [SerializeField] private TextMeshProUGUI _gameOverScoreText;
    [SerializeField] private TextMeshProUGUI _gameOverHighScoreText;
    [SerializeField] private AudioClip _hurtSound;
    [SerializeField] private AudioSource _music;
    [SerializeField] private Slider _musicVolumeMenuSlider;
    [SerializeField] private Slider _musicVolumePauseSlider;
    [SerializeField] private Slider _soundEffectsVolumeMenuSlider;
    [SerializeField] private Slider _soundEffectsVolumePauseSlider;
    [SerializeField] private PlayerController _playerController;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Unity methods

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        _pauseAction = InputSystem.actions.FindAction("Pause");
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", _musicVolumeMenuSlider.value);
            PlayerPrefs.Save();
        }
        _music.volume = PlayerPrefs.GetFloat("musicVolume");
        _musicVolumeMenuSlider.value = _music.volume;
        if (!PlayerPrefs.HasKey("musicTime"))
        {
            PlayerPrefs.SetFloat("musicTime", 0);
            PlayerPrefs.Save();
        }
        _music.time = PlayerPrefs.GetFloat("musicTime");
        if (!PlayerPrefs.HasKey("soundEffectsVolume"))
        {
            PlayerPrefs.SetFloat("soundEffectsVolume", _soundEffectsVolumeMenuSlider.value);
            PlayerPrefs.Save();
        }
        _soundEffectsVolume = PlayerPrefs.GetFloat("soundEffectsVolume");
        _soundEffectsVolumeMenuSlider.value = _soundEffectsVolume;
        _dronePool = ObjectPooler.Instance.GetDronePool();
        _treePool = ObjectPooler.Instance.GetTreePool();
        _soundOrEffectPool = ObjectPooler.Instance.GetSoundOrEffectPool();
        _cameraPosition = Camera.main.transform.position;
    }

    void Update()
    {
        if (_pauseAction.WasPressedThisFrame() && _isGameActive)
        {
            PauseMenu();
        }
    }
    
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Public class methods

    public void StartGame()
    {
        _mainMenu.SetActive(false);
        PlayerPrefs.SetFloat("musicVolume", _musicVolumeMenuSlider.value);
        PlayerPrefs.SetFloat("soundEffectsVolume", _soundEffectsVolumeMenuSlider.value);
        PlayerPrefs.Save();
        _musicVolumePauseSlider.value = _musicVolumeMenuSlider.value;
        _soundEffectsVolumePauseSlider.value = _soundEffectsVolumeMenuSlider.value;
        _enemySpawnRate = 2;
        _treeSpawnRate = 5;
        _enemySpeed = 4f;
        _treeSpeed = 0.5f;
        _lives = 3;
        _livesText.text = $"Lives:{_lives}";
        _scoreText.text = $"Score:{_score}";
        if (!PlayerPrefs.HasKey("highScore"))
        {
            PlayerPrefs.SetInt("highScore", 0);
            PlayerPrefs.Save();
        }
        _highScore = PlayerPrefs.GetInt("highScore");
        _highScoreText.text = $"High Score: {_highScore}";
        _background.SetSpeed(_treeSpeed);
        _isGameActive = true;
        Invoke(nameof(SpawnEnemy), _enemySpawnRate);
        Invoke(nameof(SpawnTree), _treeSpawnRate);
        InvokeRepeating(nameof(IncreaseSpeed), 10, 25);
        InvokeRepeating(nameof(IncreaseSpawnRate), 25, 25);
    }

    public void PauseMenu()
    {
        _isPaused = !_isPaused;
        _scoreText.gameObject.SetActive(!_scoreText.gameObject.activeSelf);
        _highScoreText.gameObject.SetActive(!_highScoreText.gameObject.activeSelf);
        _livesText.gameObject.SetActive(!_livesText.gameObject.activeSelf);
        _pauseMenu.SetActive(!_pauseMenu.activeSelf);
        Time.timeScale = _isPaused ? 0 : 1;
        if (_isPaused) { _resumeButton.Select(); }
        else
        {
            PlayerPrefs.SetFloat("musicVolume", _musicVolumePauseSlider.value);
            PlayerPrefs.SetFloat("soundEffectsVolume", _soundEffectsVolumePauseSlider.value);
            PlayerPrefs.Save();
        }
    }

    public void GuideMenu()
    {
        _mainMenu.SetActive(!_mainMenu.activeSelf);
        _guideMenu.SetActive(!_guideMenu.activeSelf);
        if (_mainMenu.activeSelf) { _playButton.Select(); }
        else { _backButton.Select(); }
    }

    public void AdjustMusicVolumeFromMenuSlider()
    {
        _music.volume = _musicVolumeMenuSlider.value;
    }

    public void AdjustSoundEffectsVolumeFromMenuSlider()
    {
        // This is done so that the testing sound is only played when the player makes a significant movement
        // of the slider, rather than everytime it changes value when being dragged
        float soundEffectsVolumeDifference = _soundEffectsVolume - _soundEffectsVolumeMenuSlider.value;
        _soundEffectsVolume = _soundEffectsVolumeMenuSlider.value;
        if (soundEffectsVolumeDifference < -0.05 || soundEffectsVolumeDifference > 0.05)
        {
            PlaySound(_hurtSound);
        }
    }

    public void AdjustMusicVolumeFromPauseSlider()
    {
        _music.volume = _musicVolumePauseSlider.value;
    }

    public void AdjustSoundEffectsVolumeFromPauseSlider()
    {
        float soundEffectsVolumeDifference = _soundEffectsVolume - _soundEffectsVolumePauseSlider.value;
        _soundEffectsVolume = _soundEffectsVolumePauseSlider.value;
        if (soundEffectsVolumeDifference < -0.05 || soundEffectsVolumeDifference > 0.05)
        {
            PlaySound(_hurtSound);
        }
    }

    public void PlaySound(AudioClip clipToPlay)
    {
        SoundOrEffect soundOrEffect = _soundOrEffectPool.Get();
        // All sound is played from the camera position, otherwise it is too far away
        // for the player to hear properly
        soundOrEffect.transform.position = _cameraPosition;
        soundOrEffect.SetAsSound(clipToPlay, _soundEffectsVolume);
    }

    public void PlayParticleEffect(Vector3 positionToPlay, SoundOrEffect.Purpose purpose)
    {
        SoundOrEffect soundOrEffect = _soundOrEffectPool.Get();
        soundOrEffect.transform.position = positionToPlay;
        soundOrEffect.SetAsParticleEffect(purpose);
    }

    public void ChangeEnemySpeed(float numToAdd)
    {
        _enemySpeed += numToAdd;
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in activeEnemies) { enemy.GetComponent<Enemy>().SetSpeed(_enemySpeed); }
    }
    
    public void ChangeTreeSpeed(float numToAdd)
    {
        _treeSpeed += numToAdd;
        GameObject[] activeTrees = GameObject.FindGameObjectsWithTag("Tree");
        foreach (GameObject tree in activeTrees) { tree.GetComponent<Tree>().SetSpeed(_treeSpeed); }  
        _background.SetSpeed(_treeSpeed);      
    }

    public void UpdateScore(int numToAdd)
    {
        _score += numToAdd;
        _scoreText.text = $"Score:{_score}";
        if (_score > _highScore)
        {
            _highScoreText.text = $"High Score:{_score}";
        }
    }

    public void UpdateLives(int numToAdd, CauseOfFailure causeOfFailure = CauseOfFailure.DRONE)
    {
        // Default value is never actually used, this is for when the player's lives are increased by a power up
        _lives += numToAdd;
        // If the player was hurt
        if (numToAdd < 0) { PlaySound(_hurtSound); }
        _livesText.text = $"Lives:{_lives}";
        if (_lives == 0) { GameOver(causeOfFailure); }
    }

    public void GameOver(CauseOfFailure causeOfFailure)
    {
        _isGameActive = false;
        CancelInvoke();
        _scoreText.gameObject.SetActive(false);
        _highScoreText.gameObject.SetActive(false);
        _livesText.gameObject.SetActive(false);
        switch (causeOfFailure)
        {
            case CauseOfFailure.DRONE:
                _gameOverCauseOfFailureText.text = "Game over, you were hit by a drone!";
                break;
            case CauseOfFailure.LASER:
                _gameOverCauseOfFailureText.text = "Game over, you were hit by a drone's laser!";
                break;
            case CauseOfFailure.MISSED_TREE:
                _gameOverCauseOfFailureText.text = "Game over, you missed a tree!";
                break;
        }
        _gameOverScoreText.text = $"Your final score was {_score}";
        if (_score > _highScore)
        {
            _gameOverHighScoreText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("highScore", _score);
            PlayerPrefs.Save();
        }
        _gameOverRestartButton.Select();
        _gameOverMenu.SetActive(true);
    }

    public void RestartGame()
    {
        if (_isPaused)
        {
            PlayerPrefs.SetFloat("musicVolume", _musicVolumePauseSlider.value);
            PlayerPrefs.SetFloat("soundEffectsVolume", _soundEffectsVolumePauseSlider.value);
            if (_score > _highScore){PlayerPrefs.SetInt("highScore", _score);}
            Time.timeScale = 1;
        }
        PlayerPrefs.SetFloat("musicTime", _music.time);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        PlayerPrefs.SetFloat("musicVolume", _musicVolumeMenuSlider.value);
        PlayerPrefs.SetFloat("musicTime", 0);
        PlayerPrefs.SetFloat("soundEffectsVolume", _soundEffectsVolumeMenuSlider.value);
        PlayerPrefs.Save();
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public bool GetIsGameActive() { return _isGameActive; }
    public bool GetIsGamePaused() { return _isPaused; }
    public int GetLives() { return _lives; }
    public float GetEnemySpeed() { return _enemySpeed; }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Private class methods

    void SpawnEnemy()
    {
        Direction enemySpawnDirection = ChooseDirection();
        Enemy drone = _dronePool.Get();
        Vector3 generatedSpawn = GenerateSpawn(enemySpawnDirection, drone.transform.position.y);
        drone.transform.position = generatedSpawn;
        switch (enemySpawnDirection)
        {
            case Direction.LEFT:
                drone.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case Direction.RIGHT:
                drone.transform.rotation = Quaternion.Euler(0, -90, 0);
                break;
            case Direction.TOP:
                drone.transform.rotation = Quaternion.Euler(0, -180, 0);
                break;
        }
        // If a laser drone had a z value of less than -2, it would be difficult for the
        // player to get under them and shoot them
        if (generatedSpawn.z > -2)
        {
            int randNum = Random.Range(1, 11);
            if (randNum == 3)
            {
                Vector3 shootingPosition;
                switch (enemySpawnDirection)
                {
                    case Direction.LEFT or Direction.RIGHT:
                        float randX = Random.Range(-_droneSpawningXBound, _droneSpawningXBound);
                        shootingPosition = new(randX, generatedSpawn.y, generatedSpawn.z);
                        drone.SetAsShootingEnemy(shootingPosition);
                        break;
                    case Direction.TOP:
                        float randZ = Random.Range(-3f, _droneSpawningZBound);
                        shootingPosition = new(generatedSpawn.x, generatedSpawn.y, randZ);
                        drone.SetAsShootingEnemy(shootingPosition);
                        break;
                }
            }
        }
        drone.SetSpeed(_enemySpeed);
        Invoke(nameof(SpawnEnemy), _enemySpawnRate);
    }

    void SpawnTree()
    {
        Tree tree = _treePool.Get();
        tree.transform.SetPositionAndRotation(GenerateSpawn(Direction.TOP, tree.transform.position.y),
                                              tree.transform.rotation);
        tree.SetSpeed(_treeSpeed);
        Invoke(nameof(SpawnTree), _treeSpawnRate);
    }

    void IncreaseSpeed()
    {
        ChangeEnemySpeed(0.5f);
        ChangeTreeSpeed(0.5f);
        _playerController.IncreaseSpeed(0.5f);
        Direction spawnDirection = ChooseDirection();
        int randNum = Random.Range(0, 6);
        // Getting the tree slowing power up too early can cause the trees to freeze or move backwards
        if (randNum == 4 && _treeSpeed < 1.5f) { randNum = 0; }
        //Power ups are not object pooled as they do not appear very frequently
        GameObject powerUp = Instantiate(_powerUps[randNum],
                            GenerateSpawn(spawnDirection, _powerUps[randNum].transform.position.y),
                            _powerUps[randNum].transform.rotation);
        powerUp.GetComponent<PowerUps>().SetDirection(spawnDirection);
    }

    void IncreaseSpawnRate()
    {
        if (!(_enemySpawnRate - 0.3f < 1)){_enemySpawnRate -= 0.3f;}
        if (!(_treeSpawnRate - 0.3f < 1)){_treeSpawnRate -= 0.3f;}
        Direction spawnDirection = ChooseDirection();
        // The tree and drone slowing power ups are not available to get here, as you would only see their effects for 
        // a few seconds before the drones and trees were sped up again
        int randNum = Random.Range(0, 4);
        GameObject powerUp = Instantiate(_powerUps[randNum],
                                         GenerateSpawn(spawnDirection, _powerUps[randNum].transform.position.y),
                                         _powerUps[randNum].transform.rotation);
        powerUp.GetComponent<PowerUps>().SetDirection(spawnDirection);
    }

    Direction ChooseDirection()
    {
        int randNum = Random.Range(1, 4);
        return randNum switch
        {
            1 => Direction.LEFT,
            2 => Direction.RIGHT,
            3 => Direction.TOP,
            _ => Direction.TOP,
        };
    }

    Vector3 GenerateSpawn(Direction selectedDirection, float spawnedObjectYPos)
    {
        float randZ;
        switch (selectedDirection)
        {
            case Direction.LEFT:
                randZ = Random.Range(-_droneSpawningZBound, _droneSpawningZBound);
                return new Vector3(-xBound, spawnedObjectYPos, randZ);
            case Direction.RIGHT:
                randZ = Random.Range(-_droneSpawningZBound, _droneSpawningZBound);
                return new Vector3(xBound, spawnedObjectYPos, randZ);
            case Direction.TOP:
                float randX = Random.Range(-_droneSpawningXBound, _droneSpawningXBound);
                return new Vector3(randX, spawnedObjectYPos, zBound);
            default:
                return new Vector3(xBound, spawnedObjectYPos, 3);
        }
    }
}
