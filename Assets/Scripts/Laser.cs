using System;
using UnityEngine;
using UnityEngine.Pool;

public class Laser : MonoBehaviour
{
    private float speed;
    private readonly float zBound = GameManager.zBound;
    private readonly float xBound = GameManager.xBound;
    private IObjectPool<Laser> laserPool;
    [SerializeField] private AudioClip explosionSound;
    void Start()
    {
        speed = GameManager.Instance.GetEnemySpeed();
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.up);
        if (transform.position.x>xBound ||
        transform.position.x<-xBound ||
        transform.position.z<-zBound)
        {
            transform.rotation = Quaternion.identity;
            try{laserPool.Release(this);} catch (Exception){}
        }            
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.PlaySound(explosionSound);
            GameManager.Instance.PlayParticleEffect(transform.position, SoundOrEffect.Purpose.LASER_EXPLOSION);
            try { laserPool.Release(this); } catch (Exception) { }
        }
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
    
    public void SetLaserPool(IObjectPool<Laser> laserPool)
    {
        this.laserPool = laserPool;
    }
}
