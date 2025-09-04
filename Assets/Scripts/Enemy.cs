using UnityEngine;

public class Enemy : MonoBehaviour
{

    // public float xBound=21.0f; //18
    // public float zBound=50.0f; //11
    private GameManager.Direction direction;
    private PlayerController playerController;
    [SerializeField] private AudioClip crash;
    [SerializeField] private ParticleSystem explosion;
    // private Animator animator;
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        direction = GameManager.Instance.enemySelectedDirection;
        speed = GameManager.Instance.enemySpeed;
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.Instance.DirectionalMovement(gameObject, speed, direction);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || (other.gameObject.CompareTag("Stick") && playerController.superSticks))
        {
            AudioSource.PlayClipAtPoint(crash, Camera.main.transform.position, GameManager.Instance.GetSoundEffectsVolume());
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Stick"))
        {
            Destroy(other.gameObject);
        }
    }
}
