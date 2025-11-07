using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Controls behaviour for enemies, drones that fly across the screen which
/// can be destroyed by the player and can hurt them if they collide with one.
/// </summary>
public class Enemy : MonoBehaviour
{
    private int _health;
    private float _speed;
    private readonly float _fireRate = 1;
    private bool _isAShootingEnemy;
    private bool _isAtShootingPosition;
    private bool _canShoot;
    private readonly float _zBound = GameManager.zBound;
    private readonly float _xBound = GameManager.xBound;
    private GameObject _player;
    private Vector3 _shootingPosition;
    private IObjectPool<Enemy> _dronePool;
    private IObjectPool<Laser> _laserPool;
    [SerializeField] private AudioClip _explosionSound;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Unity methods

    void Awake()
    {
        _health = 5;
    }

    void Start()
    {
        _player = GameObject.Find("Player");
    }

    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            if (_isAtShootingPosition)
            {
                transform.LookAt(_player.transform.position);
                if (_canShoot)
                {
                    Laser laser = _laserPool.Get();
                    Vector3 laserPosition = transform.forward + transform.position;
                    Quaternion laserRotation = transform.rotation * Quaternion.Euler(90, 0, 0);
                    laser.transform.SetPositionAndRotation(laserPosition, laserRotation);
                    laser.SetSpeed(_speed + 1);
                    _canShoot = false;
                    StartCoroutine(WaitToShoot());
                }
            }
            else
            {
                if (_isAShootingEnemy)
                {
                    if (Vector3.Distance(transform.position, _shootingPosition) <= 0.5f)
                    {
                        _isAtShootingPosition = true;
                    }
                }
                transform.Translate(_speed * Time.deltaTime * Vector3.forward);
                if (transform.position.x > _xBound ||
                    transform.position.x < -_xBound ||
                    transform.position.z < -_zBound)
                {
                    try { _dronePool.Release(this); } catch (Exception) { }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.CompareTag("Player"))
        {
            if (otherObject.GetComponent<PlayerController>().GetHasArmour())
            {
                if (_isAShootingEnemy) { GameManager.Instance.UpdateScore(6); }
                else { GameManager.Instance.UpdateScore(3); }
            }
            GameManager.Instance.PlaySound(_explosionSound);
            GameManager.Instance.PlayParticleEffect(transform.position, SoundOrEffect.Purpose.DRONE_EXPLOSION);
            try { _dronePool.Release(this); } catch (Exception) { }
        }
        else if (otherObject.CompareTag("Stick"))
        {
            _health -= 1;
            if (_health <= 0 || otherObject.GetComponent<Stick>().GetIsSuperStick())
            {
                if (_isAShootingEnemy) { GameManager.Instance.UpdateScore(6); }
                else { GameManager.Instance.UpdateScore(3); }
                GameManager.Instance.PlaySound(_explosionSound);
                GameManager.Instance.PlayParticleEffect(transform.position, SoundOrEffect.Purpose.DRONE_EXPLOSION);
                try { _dronePool.Release(this); } catch (Exception) { }
            }
            else
            {
                GameManager.Instance.PlayParticleEffect(transform.position, SoundOrEffect.Purpose.DRONE_DAMAGE);
            }

        }
    }

    void OnDisable()
    {
        _health = 5;
        if (_isAShootingEnemy)
        {
            _isAShootingEnemy = false;
            _isAtShootingPosition = false;
            _canShoot = false;
        }
    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Public class methods

    public void SetAsShootingEnemy(Vector3 shootingPosition)
    {
        _shootingPosition = shootingPosition;
        _isAShootingEnemy = true;
        _canShoot = true;
        _health = 10;
    }

    public void SetSpeed(float speed) { _speed = speed; }

    public void SetDronePool(IObjectPool<Enemy> dronePool){_dronePool = dronePool;}

    public void SetLaserPool(IObjectPool<Laser> laserPool){_laserPool = laserPool;}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Coroutines

    IEnumerator WaitToShoot()
    {
        yield return new WaitForSeconds(_fireRate);
        _canShoot = true;
    }
}
