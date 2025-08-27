using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
// using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGameActive;
    public int score;
    public int lives;
    public float enemySpeed;
    public float treeSpeed;
    public enum Direction {Left,Top,Right};
    public Direction enemySelectedDirection;
    public Direction powerUpSelectedDirection;
    public GameObject[] enemies;
    public GameObject[] powerUps;
    public GameObject tree;
    public GameObject player;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI pauseText;
    public GameObject titleText;
    public GameObject gameOverText;
    [SerializeField] private AudioClip crash;

    private int randNum;
    private float enemySpawnRate;
    private float treeSpawnRate;
    private float randX;
    private float randZ;
    private const float zBound = 5;
    private const float yValue = 3;
    private const float xBound = 12;
    private AudioSource soundEffects;
    [SerializeField] private AudioSource music;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundEffectsVolumeSlider;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        soundEffects = GetComponent<AudioSource>();
        // soundEffectsVolumeSlider.RegisterCallback<onexi>(PlayTestEffect);
        // soundEffectsVolumeSlider.OnPointerExit.AddListener(delegate {PlayTestEffect();});
        // musicVolumeSlider=GameObject.Find("Volume Slider").GetComponent<Slider>();
        // music=GameObject.Find("Main Camera").GetComponent<AudioSource>();
        // player=Instantiate(player,player.transform.position,player.transform.rotation);
    }

    public void StartGame(){
        titleText.SetActive(false);
        isGameActive=true;
        enemySpawnRate=2;
        treeSpawnRate=5;
        enemySpeed=4.0f;
        treeSpeed=0.5f;
        lives=3;
        livesText.text=$"Lives:{lives}";
        scoreText.text=$"Score:{score}";
        Invoke(nameof(SpawnEnemy), enemySpawnRate);
        Invoke(nameof(SpawnTree), treeSpawnRate);
        InvokeRepeating(nameof(IncreaseSpeed), 20,30);
        InvokeRepeating(nameof(IncreaseSpawnRate), 30,30);
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

    public void AdjustMusicVolume(){
        music.volume=musicVolumeSlider.value;
    }

    public void AdjustSoundEffectsVolume()
    {
        soundEffects.volume = soundEffectsVolumeSlider.value;
    }

    public void PlayTestEffect()
    { 
        AudioSource.PlayClipAtPoint(crash, transform.position,soundEffects.volume);
    }

    public float GetSoundEffectsVolume()
    {
        return soundEffectsVolumeSlider.value;
    }

    Direction ChooseDirection(){
        randNum=Random.Range(1,4);
        if (randNum==1){
            return Direction.Left;
        } else if(randNum==2){
            return Direction.Top;
        } else if(randNum==3){
            return Direction.Right;
        } else{
            return Direction.Top;
        }
    }

    void SpawnEnemy(){
        enemySelectedDirection=ChooseDirection();
        randNum=Random.Range(0,3);
        if (enemySelectedDirection==Direction.Top){
            Quaternion rotation=Quaternion.Euler(0,-180,0);
            GameObject drone = Instantiate(enemies[randNum],GenerateSpawn(enemySelectedDirection,enemies[randNum]),rotation);
        } else if (enemySelectedDirection==Direction.Left){
            Quaternion rotation=Quaternion.Euler(0,90,0);
            GameObject drone = Instantiate(enemies[randNum],GenerateSpawn(enemySelectedDirection,enemies[randNum]),rotation);
        } else{
            Quaternion rotation=Quaternion.Euler(0,-90,0);
            GameObject drone = Instantiate(enemies[randNum],GenerateSpawn(enemySelectedDirection,enemies[randNum]),rotation);
        }
        Invoke(nameof(SpawnEnemy), enemySpawnRate);
    }

    void SpawnTree(){
        Instantiate(tree,GenerateSpawn(Direction.Top,tree),tree.transform.rotation);
        Invoke(nameof(SpawnTree), treeSpawnRate);
    }

    Vector3 GenerateSpawn(Direction selectedDirection,GameObject spawnedObject){
        if (selectedDirection==Direction.Left){
            randZ=Random.Range(-zBound,zBound);
            return new Vector3(-17.1f,yValue,randZ);
            //x value will always be -17.1
            //top z value will be 5
            //y value will be 3

        } else if(selectedDirection==Direction.Top){
            randX = Random.Range(-xBound,xBound);
            if(spawnedObject.CompareTag("Tree")){
                return new Vector3(randX,0.56f,10);
            } else{
                return new Vector3(randX,yValue,10);
            }
            //left x value will be -12
            //z value is 8
            //y value 3

        } else if(selectedDirection==Direction.Right){
            randZ=Random.Range(-zBound,zBound);
            return new Vector3(17.1f,yValue,randZ);
            //x value 17.1
            //top z value is 5
            //y value 3
        } else{
            return new Vector3(17.1f,yValue,5);
        }
    }

    public void DirectionalMovement(GameObject movingObject,float speed,Direction direction){
        if(movingObject.CompareTag("Tree") || movingObject.CompareTag("Enemy")){
            movingObject.transform.Translate(speed * Time.deltaTime * Vector3.forward);
        } else{
            if (direction==Direction.Top){
                movingObject.transform.Translate(speed * Time.deltaTime * Vector3.down);
            } else if(direction==Direction.Left){
                movingObject.transform.Translate(speed * Time.deltaTime * Vector3.right);
            } else if(direction==Direction.Right){
                movingObject.transform.Translate(speed * Time.deltaTime * Vector3.left);
            }
        }
        if (movingObject.transform.position.x> xBound+7 || movingObject.transform.position.x<-(xBound+7) || movingObject.transform.position.z<-(zBound+7)){
            if(movingObject.CompareTag("Tree")){
                if(!movingObject.GetComponent<Tree>().rebuilt){
                    UpdateLives();
                }
                Destroy(movingObject.GetComponent<Tree>().treeText);
                Destroy(movingObject);
            } else{
                Destroy(movingObject);
            }
        }
    }

    void IncreaseSpeed(){
        enemySpeed+=0.5f;
        treeSpeed+=0.4f;
        powerUpSelectedDirection=ChooseDirection();
        randNum=Random.Range(0,3);
        Instantiate(powerUps[randNum],GenerateSpawn(powerUpSelectedDirection,powerUps[randNum]),powerUps[randNum].transform.rotation);
    }

    void IncreaseSpawnRate(){
        if (!(enemySpawnRate-0.3f<1)){
            enemySpawnRate-=0.3f;
        }
        if (!(treeSpawnRate-0.3f<1)){
            treeSpawnRate-=0.3f;
        }
        powerUpSelectedDirection=ChooseDirection();
        randNum=Random.Range(0,3);
        Instantiate(powerUps[randNum],GenerateSpawn(powerUpSelectedDirection,powerUps[randNum]),powerUps[randNum].transform.rotation);
    }

    public void UpdateScore(int numToAdd){
        score+=numToAdd;
        scoreText.text=$"Score:{score}";
    }

    public void UpdateLives(){
        lives--;
        livesText.text=$"Lives:{lives}";
        if (lives==0){
            GameOver();
        }
    }

    public void GameOver(){
        isGameActive=false;
        player.GetComponent<PlayerController>().powerUpRing.SetActive(false);
        Destroy(player);
        gameOverText.SetActive(true);
        CancelInvoke();
    }

    public void RestartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
