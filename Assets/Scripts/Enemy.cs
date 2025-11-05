using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    private int health;
    private float speed;
    private readonly float fireRate = 1;
    private bool isAShootingEnemy;
    private bool isAtShootingPosition;
    private bool canShoot;
    private readonly float zBound = GameManager.zBound;
    private readonly float xBound = GameManager.xBound;
    private GameObject player;
    private Vector3 shootingPosition;
    private IObjectPool<Enemy> dronePool;
    private IObjectPool<Laser> laserPool;
    [SerializeField] private AudioClip explosionSound;
    void Awake()
    {
        health = 5;
    } 

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (GameManager.Instance.GetIsGameActive())
        {
            if (isAtShootingPosition)
            {
                transform.LookAt(player.transform.position); 
                if (canShoot)
                {
                    Laser laser = laserPool.Get();
                    Vector3 laserPosition = transform.forward + transform.position;
                    Quaternion laserRotation = transform.rotation * Quaternion.Euler(90, 0, 0);
                    laser.transform.SetPositionAndRotation(laserPosition, laserRotation);
                    laser.SetSpeed(speed + 1);
                    canShoot = false;
                    StartCoroutine(WaitToShoot());
                }  
            }
            else
            {
                if (isAShootingEnemy)
                {
                    if (Vector3.Distance(transform.position, shootingPosition) <= 0.5f)
                    {
                        isAtShootingPosition = true;
                    }
                }
                transform.Translate(speed * Time.deltaTime * Vector3.forward);
                if (transform.position.x>xBound ||
                transform.position.x<-xBound ||
                transform.position.z<-zBound)
                {
                    transform.rotation = Quaternion.identity;
                    try{dronePool.Release(this);} catch (Exception){}
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
                if (isAShootingEnemy) { GameManager.Instance.UpdateScore(6); }
                else { GameManager.Instance.UpdateScore(3); }
            }
            GameManager.Instance.PlaySound(explosionSound);
            GameManager.Instance.PlayParticleEffect(transform.position, SoundOrEffect.Purpose.DRONE_EXPLOSION);
            try { dronePool.Release(this); } catch (Exception) { }
        }
        else if (otherObject.CompareTag("Stick"))
        {
            health -= 1;
            if (health <= 0 || otherObject.GetComponent<Stick>().GetIsSuperStick())
            {
                if (isAShootingEnemy) { GameManager.Instance.UpdateScore(6); }
                else { GameManager.Instance.UpdateScore(3); }
                GameManager.Instance.PlaySound(explosionSound);
                GameManager.Instance.PlayParticleEffect(transform.position, SoundOrEffect.Purpose.DRONE_EXPLOSION);
                try { dronePool.Release(this); } catch (Exception) { }
            }
            else
            {
                GameManager.Instance.PlayParticleEffect(transform.position, SoundOrEffect.Purpose.DRONE_DAMAGE);
            }

        }
    }

    void OnDisable()
    {
        health = 5;
        if (isAShootingEnemy)
        {
            isAShootingEnemy = false;
            isAtShootingPosition = false;
            canShoot = false;
        }
    }

    public void SetAsShootingEnemy(Vector3 shootingPosition)
    {
        this.shootingPosition = shootingPosition;
        isAShootingEnemy = true;
        canShoot = true;
        health = 10;
    }

    public void SetSpeed(float speed) { this.speed = speed; }

    public void SetDronePool(IObjectPool<Enemy> dronePool)
    {
        this.dronePool = dronePool;
    }

    public void SetLaserPool(IObjectPool<Laser> laserPool)
    {
        this.laserPool = laserPool;
    }

    IEnumerator WaitToShoot()
    {
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }
}
