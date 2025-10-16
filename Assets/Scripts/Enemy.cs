using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    private float speed;
    private readonly float zBound = GameManager.zBound;
    private readonly float xBound = GameManager.xBound;
    private IObjectPool<Enemy> dronePool;
    [SerializeField] private AudioClip crash;
    [SerializeField] private ParticleSystem explosion;

    void Start()
    {
        speed = GameManager.Instance.GetEnemySpeed();
    }

    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            transform.Translate(speed * Time.deltaTime * Vector3.forward);
            if (transform.position.x>xBound ||
            transform.position.x<-xBound ||
            transform.position.z<-zBound)
            {
                transform.rotation = Quaternion.identity;
                dronePool.Release(this);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.CompareTag("Player") || (otherObject.CompareTag("Stick") && otherObject.GetComponent<Stick>().GetIsSuperStick()))
        {
            GameManager.Instance.PlaySound(crash);
            GameManager.Instance.PlayParticleEffect(transform.position);
            if ((otherObject.CompareTag("Player") && otherObject.GetComponent<PlayerController>().GetHasArmour()) ||
            (otherObject.CompareTag("Stick") && otherObject.GetComponent<Stick>().GetIsSuperStick()))
            {
                GameManager.Instance.UpdateScore(1);
            }
            dronePool.Release(this);
        }
        else if (otherObject.CompareTag("Stick"))
        {
            otherObject.GetComponent<Stick>().ReturnStickToPool();
        }
    }

    public void SetSpeed(float speedToSet) { speed = speedToSet; }

    public void SetDronePool(IObjectPool<Enemy> poolToSet)
    {
        dronePool = poolToSet;
    }
}
