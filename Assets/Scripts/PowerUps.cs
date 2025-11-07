using UnityEngine;

/// <summary>
/// Controls the behaviour of the power ups
/// </summary>
public class PowerUps : MonoBehaviour
{
    private float _speed;
    private readonly float _zBound = GameManager.zBound;
    private readonly float _xBound = GameManager.xBound;
    private Vector3 _directionVector;
    private GameManager.Direction _direction;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Unity methods

    void Awake()
    {
        _directionVector = Vector3.back;
    }

    void Start()
    {
        _speed = GameManager.Instance.GetEnemySpeed();
    }

    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            // World space is needed here as the power ups each have different rotations
            // and are constantly facing the same way
            transform.Translate(_speed * Time.deltaTime * _directionVector, Space.World);
            if (transform.position.x > _xBound ||
                transform.position.x < -_xBound ||
                transform.position.z < -_zBound)
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

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Public class methods

    public void SetDirection(GameManager.Direction direction)
    {
        _direction = direction;
        switch (_direction)
        {
            case GameManager.Direction.LEFT:
                _directionVector = Vector3.right;
                break;
            case GameManager.Direction.RIGHT:
                _directionVector = Vector3.left;
                break;
            case GameManager.Direction.TOP:
                _directionVector = Vector3.back;
                break;
        }
    }
}
