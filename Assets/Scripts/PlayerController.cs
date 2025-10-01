using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool canShoot=true;
    private bool hasArmour;
    private bool hasPowerUp;
    private bool hasSuperSticks;
    private bool hasFourWaySticks;
    private float horizontalInput;
    private float verticalInput;
    private const float speed = 10.0f;
    private const float zBound = 7f;
    private const float xBound=14;
    private AudioSource audioSource;
    private static readonly WaitForSeconds _waitForSeconds0_1 = new(0.1f);
    private static readonly WaitForSeconds _waitForSeconds10 = new(10);
    [SerializeField] private AudioClip crash;
    [SerializeField] private GameObject stick;
    [SerializeField] private GameObject armourRing;
    [SerializeField] private GameObject fourWaySticksRing;
    [SerializeField] private ParticleSystem explosion;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = GameManager.Instance.GetSoundEffectsVolume();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            transform.Translate(speed * Time.deltaTime * verticalInput * Vector3.forward);
            transform.Translate(horizontalInput * speed * Time.deltaTime * Vector3.right);
            PlayerMovementConstraints();
            if (Input.GetKeyDown(KeyCode.Space) && canShoot && !GameManager.Instance.GetIsGamePaused())
            {
                GameObject stickCopy = Instantiate(stick, transform.position + new Vector3(0, 0.3f, 1), stick.transform.rotation);
                if (hasSuperSticks)
                {
                    stickCopy.GetComponent<Stick>().SetIsSuperStick();
                }
                else if (hasFourWaySticks)
                {
                    //downwards
                    Instantiate(stick, transform.position + new Vector3(0, 0.3f, -1),Quaternion.identity);
                    //right
                    Instantiate(stick, transform.position + new Vector3(1, 0.3f, 0), Quaternion.Euler(0, 90, 0));
                    //left
                    Instantiate(stick, transform.position + new Vector3(-1, 0.3f, 0), Quaternion.Euler(0, -90, 0));
                }
                canShoot = false;
                StartCoroutine(WaitToShoot());
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
        GameObject otherObject = other.gameObject;
        if (otherObject.CompareTag("Enemy") && !hasArmour)
        {
            GameManager.Instance.UpdateLives(-1, GameManager.CauseOfFailure.DRONE);
        }
        else if (otherObject.CompareTag("Heal"))
        {
            GameManager.Instance.UpdateLives(3 - GameManager.Instance.GetLives());
            Destroy(otherObject);
        }
        else if (otherObject.CompareTag("SuperSticks") || otherObject.CompareTag("Armor") || otherObject.CompareTag("FourWaySticks"))
        {
            if (hasPowerUp)
            {
                StopAllCoroutines();
                canShoot = true;
                hasSuperSticks = false;
                hasArmour = false;
                hasFourWaySticks = false;
                armourRing.SetActive(false);
                fourWaySticksRing.SetActive(false);
            }
            else { hasPowerUp = true; }
            if (otherObject.CompareTag("SuperSticks"))
            {
                hasSuperSticks = true;
            }
            else if (otherObject.CompareTag("Armor"))
            {
                fourWaySticksRing.SetActive(false);
                armourRing.SetActive(true);
                hasArmour = true;
            }
            else
            {
                armourRing.SetActive(false);
                fourWaySticksRing.SetActive(true);
                hasFourWaySticks = true;
            }
            Destroy(otherObject);
            StartCoroutine(PowerUpCountdown());
        }
    }

    public bool GetHasArmour() {return hasArmour;}



    IEnumerator WaitToShoot(){
        yield return _waitForSeconds0_1;
        canShoot=true;
    }

    IEnumerator PowerUpCountdown()
    {
        yield return _waitForSeconds10;
        hasPowerUp = false;
        hasSuperSticks = false;
        hasArmour = false;
        hasFourWaySticks = false;
        armourRing.SetActive(false);
        fourWaySticksRing.SetActive(false);
    }
}
