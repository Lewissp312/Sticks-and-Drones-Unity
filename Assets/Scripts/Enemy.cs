using UnityEngine;

public class Enemy : MonoBehaviour
{

    // public float xBound=21.0f; //18
    // public float zBound=50.0f; //11
    private GameManager gameManager;
    private GameManager.Direction direction;
    private AudioSource audioSource;
    [SerializeField] AudioClip crash;
    // private Animator animator;
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        direction = GameManager.Instance.enemySelectedDirection;
        speed = GameManager.Instance.enemySpeed;
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.Instance.DirectionalMovement(gameObject, speed, direction);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Stick"))
        {
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().superSticks)
            {
                AudioSource.PlayClipAtPoint(crash,Camera.main.transform.position,GameManager.Instance.GetSoundEffectsVolume());
                Destroy(gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            print("Collided with player");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Vector3 posToPlaySound = new Vector3(transform.position.x, 30, transform.position.z); 
            AudioSource.PlayClipAtPoint(crash, Camera.main.transform.position,GameManager.Instance.GetSoundEffectsVolume());
            Destroy(gameObject);
            // print("Collided with player");
        }
    }
}
