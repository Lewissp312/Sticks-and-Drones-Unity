using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool isDirectionSet;
    private float speed;
    private GameManager.Direction direction;
    private PlayerController playerController;
    [SerializeField] private AudioClip crash;
    [SerializeField] private ParticleSystem explosion;
    // private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        speed = GameManager.Instance.GetEnemySpeed();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDirectionSet)
        {
            GameManager.Instance.DirectionalMovement(gameObject, speed, direction);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.CompareTag("Player") || (otherObject.CompareTag("Stick") && otherObject.GetComponent<Stick>().GetIsSuperStick()))
        {
            AudioSource.PlayClipAtPoint(crash, Camera.main.transform.position, GameManager.Instance.GetSoundEffectsVolume());
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (otherObject.CompareTag("Stick"))
        {
            Destroy(otherObject);
        }
    }

    public void SetDirection(GameManager.Direction directionToSet)
    {
        direction = directionToSet;
        isDirectionSet = true;
    }

    public void SetSpeed(float speedToSet) { speed = speedToSet; }
}
