using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float speed = 10.0f;
    private float horizontalInput;
    private float verticalInput;
    private const float zBound=7f;
    private float xBound=13;
    private bool canShoot=true;
    private bool armor=false;
    private bool hasPowerUp=false;
    private GameManager gameManager;
    public AudioSource audioSource;
    public bool superSticks=false;
    public AudioClip crash;
    public GameObject stick;
    public GameObject powerUpRing;
    public ParticleSystem explosion;
    // Start is called before the first frame update
    void Start()
    {
        gameManager=GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioSource=GetComponent<AudioSource>();
        powerUpRing=Instantiate(powerUpRing,powerUpRing.transform.position,powerUpRing.transform.rotation);
        powerUpRing.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameActive){
            horizontalInput=Input.GetAxis("Horizontal");
            verticalInput=Input.GetAxis("Vertical");
            transform.Translate(Vector3.forward*verticalInput*speed*Time.deltaTime);
            transform.Translate(Vector3.right*horizontalInput*speed*Time.deltaTime);
            PlayerMovementConstraints();
            if (Input.GetKeyDown(KeyCode.Space) && canShoot){
                Instantiate(stick,transform.position + new Vector3(0,0,1),stick.transform.rotation);
                canShoot=false;
                StartCoroutine(waitToShoot());
            }
            if (powerUpRing.activeSelf){
                powerUpRing.transform.position=transform.position;
            }
        }
    }

    void PlayerMovementConstraints(){
        if(transform.position.z>zBound){
            transform.position=new Vector3(transform.position.x,transform.position.y,zBound);
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
            audioSource.PlayOneShot(crash);
            if (!armor){
                gameManager.UpdateLives();
            }
            Debug.Log("Player collision");
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Heal")){
            gameManager.lives=3;
            gameManager.livesText.text=$"Lives:{gameManager.lives}";
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("SuperSticks") && !hasPowerUp){
            Debug.Log("Super");
            hasPowerUp=true;
            superSticks=true;
            powerUpRing.SetActive(true);
            Destroy(other.gameObject);
            StartCoroutine(powerUpCountdown());
        }
        if (other.gameObject.CompareTag("Armor") && !hasPowerUp){
            Debug.Log("Armor");
            hasPowerUp=true;
            powerUpRing.SetActive(true);
            armor=true;
            Destroy(other.gameObject);
            StartCoroutine(powerUpCountdown());
        }
    }



    IEnumerator waitToShoot(){
        yield return new WaitForSeconds(0.1f);
        canShoot=true;
    }

    public IEnumerator WaitForExplosion(ParticleSystem explosionMod){
        yield return new WaitForSeconds(1.5f);
        Destroy(explosionMod.gameObject); 
    }

    IEnumerator powerUpCountdown(){
        yield return new WaitForSeconds(10);
        hasPowerUp=false;
        superSticks=false;
        armor=false;
        powerUpRing.SetActive(false);
    }
}
