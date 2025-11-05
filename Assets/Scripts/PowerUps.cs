using UnityEngine;

public class PowerUps : MonoBehaviour
{
    private float speed;
    private const float zBound = GameManager.zBound;
    private const float xBound = GameManager.xBound;
    private Vector3 directionVector;
    private GameManager.Direction direction;

    void Awake()
    {
        directionVector = Vector3.back;
    } 

    void Start()
    {
        speed = GameManager.Instance.GetEnemySpeed();
    }

    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            transform.Translate(speed * Time.deltaTime * directionVector, Space.World);
            if (transform.position.x > xBound ||
            transform.position.x < -xBound ||
            transform.position.z < -zBound)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(GameManager.Direction directionToSet)
    {
        direction = directionToSet;
        switch (direction)
            {
                case GameManager.Direction.TOP:
                    directionVector = Vector3.back;
                    break;
                case GameManager.Direction.LEFT:
                    directionVector = Vector3.right;
                    break;
                case GameManager.Direction.RIGHT:
                    directionVector = Vector3.left;
                    break;
            }
    }
}
