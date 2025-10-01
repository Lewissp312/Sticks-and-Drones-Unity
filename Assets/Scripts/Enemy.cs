using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool isDirectionSet;
    private float speed;
    [SerializeField] private AudioClip crash;
    [SerializeField] private ParticleSystem explosion;

    // Start is called before the first frame update
    void Start()
    {
        speed = GameManager.Instance.GetEnemySpeed();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
        GameManager.Instance.MovementRestrictions(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.CompareTag("Player") || (otherObject.CompareTag("Stick") && otherObject.GetComponent<Stick>().GetIsSuperStick()))
        {
            AudioSource.PlayClipAtPoint(crash, Camera.main.transform.position, GameManager.Instance.GetSoundEffectsVolume());
            Instantiate(explosion, transform.position, Quaternion.identity);
            if ((otherObject.CompareTag("Player") && otherObject.GetComponent<PlayerController>().GetHasArmour()) ||
            (otherObject.CompareTag("Stick") && otherObject.GetComponent<Stick>().GetIsSuperStick()))
            {
                GameManager.Instance.UpdateScore(1);
            }
            Destroy(gameObject);
        }
        else if (otherObject.CompareTag("Stick"))
        {
            Destroy(otherObject);
        }
    }

    public void SetSpeed(float speedToSet) { speed = speedToSet; }
}
