using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum Direction { Left, Top, Right };
    private bool isGameActive;
    private int score;
    private int lives;
    private float enemySpeed;
    private float treeSpeed;
    private float enemySpawnRate;
    private float treeSpawnRate;
    private const float zBound = 5;
    private const float xBound = 12;
    private AudioSource soundEffects;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] powerUps;
    [SerializeField] private GameObject tree;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject titleText;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private TextMeshProUGUI scoreText;
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
    }

    public void StartGame()
    {
        titleText.SetActive(false);
        enemySpawnRate = 2;
        treeSpawnRate = 5;
        enemySpeed = 4.0f;
        treeSpeed = 0.5f;
        lives = 3;
        livesText.text = $"Lives:{lives}";
        scoreText.text = $"Score:{score}";
        backgroundScript.SetSpeed(treeSpeed);
        isGameActive =true;
        Invoke(nameof(SpawnEnemy), enemySpawnRate);
        Invoke(nameof(SpawnTree), treeSpawnRate);
        InvokeRepeating(nameof(IncreaseSpeed), 20, 30);
        InvokeRepeating(nameof(IncreaseSpawnRate), 30, 30);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)){
            if(!pauseText.IsActive()){
                pauseText.gameObject.SetActive(true);
                Time.timeScale=0;
            } else{
                Time.timeScale=1;
                pauseText.gameObject.SetActive(false);
            }
        }

    }

    public void AdjustMusicVolume(){music.volume=musicVolumeSlider.value;}

    public void AdjustSoundEffectsVolume(){soundEffects.volume = soundEffectsVolumeSlider.value;}

    public void PlayTestEffect(){ AudioSource.PlayClipAtPoint(crash, Camera.main.transform.position,soundEffectsVolumeSlider.value);}

    public void PlayExplosion(Vector3 positionToPlayEffect)
    {
        ParticleSystem explosionTemp = Instantiate(explosion, positionToPlayEffect, Quaternion.identity);
        explosionTemp.Play();
        Destroy(explosionTemp, 4);
    }

    public float GetSoundEffectsVolume(){return soundEffectsVolumeSlider.value;}

    public bool GetIsGameActive(){return isGameActive;}

    public int GetLives(){return lives;}

    public float GetEnemySpeed(){return enemySpeed;}

    public float GetTreeSpeed() { return treeSpeed; }

    Direction ChooseDirection(){
        int randNum = Random.Range(1,4);
        return randNum switch
        {
            1 => Direction.Left,
            2 => Direction.Top,
            3 => Direction.Right,
            _ => Direction.Top,
        };
    }

    void SpawnEnemy()
    {
        Direction enemySpawnDirection = ChooseDirection();
        int randNum = Random.Range(0, 3);
        GameObject enemyToSpawn = enemySpawnDirection switch
        {
            Direction.Left => Instantiate(enemies[randNum], GenerateSpawn(enemySpawnDirection, enemies[randNum]), Quaternion.Euler(0, 90, 0)),
            Direction.Top => Instantiate(enemies[randNum], GenerateSpawn(enemySpawnDirection, enemies[randNum]), Quaternion.Euler(0, -180, 0)),
            _ => Instantiate(enemies[randNum], GenerateSpawn(enemySpawnDirection, enemies[randNum]), Quaternion.Euler(0, -90, 0)),
        };
        enemyToSpawn.GetComponent<Enemy>().SetDirection(enemySpawnDirection);
        Invoke(nameof(SpawnEnemy), enemySpawnRate);
    }

    void SpawnTree(){
        Instantiate(tree,GenerateSpawn(Direction.Top,tree),tree.transform.rotation);
        Invoke(nameof(SpawnTree), treeSpawnRate);
    }

    Vector3 GenerateSpawn(Direction selectedDirection,GameObject spawnedObject){
        float randZ;
        switch (selectedDirection)
        {
            case Direction.Left:
                randZ = Random.Range(-zBound, zBound);
                return new Vector3(-17.1f, spawnedObject.transform.position.y, randZ);
            case Direction.Top:
                float randX = Random.Range(-xBound, xBound);
                return new Vector3(randX, spawnedObject.transform.position.y, 10);
            case Direction.Right:
                randZ = Random.Range(-zBound, zBound);
                return new Vector3(17.1f, spawnedObject.transform.position.y, randZ);
            default:
                return new Vector3(17.1f, spawnedObject.transform.position.y, 3);
        }
    }

    public void DirectionalMovement(GameObject movingObject,float speed,Direction direction){
        if(movingObject.CompareTag("Tree") || movingObject.CompareTag("Enemy"))
        {
            movingObject.transform.Translate(speed * Time.deltaTime * Vector3.forward);
        }
        else
        {
            switch (direction)
            {
                case Direction.Top:
                    movingObject.transform.Translate(speed * Time.deltaTime * Vector3.down);
                    break;
                case Direction.Left:
                    movingObject.transform.Translate(speed * Time.deltaTime * Vector3.right);
                    break;
                case Direction.Right:
                    movingObject.transform.Translate(speed * Time.deltaTime * Vector3.left);
                    break;
            }
        }
        if (movingObject.transform.position.x> xBound+7 ||
        movingObject.transform.position.x<-(xBound+7) ||
        movingObject.transform.position.z<-(zBound+7))
        {
            if(movingObject.CompareTag("Tree"))
            {
                if(!movingObject.GetComponent<Tree>().GetIsRebuilt())
                {
                    UpdateLives(-1);
                }
                Destroy(movingObject.GetComponent<Tree>().GetTreeText());
                Destroy(movingObject);
            } else
            {
                Destroy(movingObject);
            }
        }
    }

    void IncreaseSpeed()
    {
        enemySpeed += 0.5f;
        treeSpeed += 0.4f;
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] activeTrees = GameObject.FindGameObjectsWithTag("Tree");
        foreach (GameObject enemy in activeEnemies) { enemy.GetComponent<Enemy>().SetSpeed(enemySpeed); }
        foreach (GameObject tree in activeTrees) { tree.GetComponent<Tree>().SetSpeed(treeSpeed); }
        backgroundScript.SetSpeed(treeSpeed);
        Direction spawnDirection = ChooseDirection();
        int randNum = Random.Range(0, 3);
        GameObject powerUp = Instantiate(powerUps[randNum], GenerateSpawn(spawnDirection, powerUps[randNum]), powerUps[randNum].transform.rotation);
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
        int randNum = Random.Range(0, 3);
        GameObject powerUp = Instantiate(powerUps[randNum], GenerateSpawn(powerUpSpawnDirection, powerUps[randNum]), powerUps[randNum].transform.rotation);
        powerUp.GetComponent<PowerUps>().SetDirection(powerUpSpawnDirection);
        
    }

    public void UpdateScore(int numToAdd){
        score+=numToAdd;
        scoreText.text=$"Score:{score}";
    }

    public void UpdateLives(int numToAdd){
        lives+=numToAdd;
        livesText.text=$"Lives:{lives}";
        if (lives==0){
            GameOver();
        }
    }

    public void GameOver(){
        isGameActive=false;
        player.GetComponent<PlayerController>().GetPowerUpRing().SetActive(false);
        Destroy(player);
        gameOverText.SetActive(true);
        CancelInvoke();
    }

    public void RestartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
