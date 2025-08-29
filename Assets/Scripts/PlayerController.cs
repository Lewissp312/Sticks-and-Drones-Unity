using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float speed = 10.0f;
    private float horizontalInput;
    private float verticalInput;
    private const float zBound=7f;
    private const float xBound=13;
    private bool canShoot=true;
    private bool armor;
    private bool hasPowerUp;
    public bool superSticks;
    private static readonly WaitForSeconds _waitForSeconds0_1 = new(0.1f);
    private static readonly WaitForSeconds _waitForSeconds1_5 = new(1.5f);
    private static readonly WaitForSeconds _waitForSeconds10 = new(10);
    public AudioSource audioSource;
    public AudioClip crash;
    public GameObject stick;
    public GameObject powerUpRing;
    public ParticleSystem explosion;

    void Start()
    {
        audioSource=GetComponent<AudioSource>();
        audioSource.volume = GameManager.Instance.GetSoundEffectsVolume();
        print(GameManager.Instance.GetSoundEffectsVolume());
        powerUpRing = Instantiate(powerUpRing,powerUpRing.transform.position,powerUpRing.transform.rotation);
        powerUpRing.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isGameActive){
            horizontalInput=Input.GetAxis("Horizontal");
            verticalInput=Input.GetAxis("Vertical");
            transform.Translate(speed * Time.deltaTime * verticalInput * Vector3.forward);
            transform.Translate(horizontalInput * speed * Time.deltaTime * Vector3.right);
            PlayerMovementConstraints();
            if (Input.GetKeyDown(KeyCode.Space) && canShoot){
                Instantiate(stick,transform.position + new Vector3(0,0.3f,1),stick.transform.rotation);
                canShoot=false;
                StartCoroutine(WaitToShoot());
            }
            if (powerUpRing.activeSelf){
                powerUpRing.transform.position=transform.position;
            }
        }
    }

    void PlayerMovementConstraints(){
        if (transform.position.z > zBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zBound);
        }
        if (transform.position.z<-zBound){
            transform.position=new Vector3(transform.position.x,transform.position.y,-zBound);
        }
        if (transform.position.x>xBound){
            transform.position=new Vector3(xBound,transform.position.y,transform.position.z);
        }
        if (transform.position.x<-xBound){
            transform.position=new Vector3(-xBound,transform.position.y,transform.position.z);
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("Enemy")){
            // explosion.transform.position=transform.position;
            // explosion.Play();
            ParticleSystem explosionMod=Instantiate(explosion,transform.position,explosion.transform.rotation);
            explosionMod.Play();
            StartCoroutine(WaitForExplosion(explosionMod));
            if (!armor){
                GameManager.Instance.UpdateLives();
            }
            Debug.Log("Player collision");
        }
        if (other.gameObject.CompareTag("Heal")){
            GameManager.Instance.lives=3;
            GameManager.Instance.livesText.text=$"Lives:{GameManager.Instance.lives}";
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("SuperSticks") && !hasPowerUp){
            Debug.Log("Super");
            hasPowerUp=true;
            superSticks=true;
            powerUpRing.SetActive(true);
            Destroy(other.gameObject);
            StartCoroutine(PowerUpCountdown());
        }
        if (other.gameObject.CompareTag("Armor") && !hasPowerUp){
            Debug.Log("Armor");
            hasPowerUp=true;
            powerUpRing.SetActive(true);
            armor=true;
            Destroy(other.gameObject);
            StartCoroutine(PowerUpCountdown());
        }
    }



    IEnumerator WaitToShoot(){
        yield return _waitForSeconds0_1;
        canShoot=true;
    }

    public IEnumerator WaitForExplosion(ParticleSystem explosionMod){
        yield return _waitForSeconds1_5;
        Destroy(explosionMod.gameObject); 
    }

    IEnumerator PowerUpCountdown(){
        yield return _waitForSeconds10;
        hasPowerUp=false;
        superSticks=false;
        armor=false;
        powerUpRing.SetActive(false);
    }
}
