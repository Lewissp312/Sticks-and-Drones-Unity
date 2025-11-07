using System;
using UnityEngine;
using UnityEngine.Pool;
/// <summary>
/// Controls the behaviour of enemy lasers
/// </summary>
public class Laser : MonoBehaviour
{
    private float _speed;
    private readonly float _zBound = GameManager.zBound;
    private readonly float _xBound = GameManager.xBound;
    private IObjectPool<Laser> _laserPool;
    [SerializeField] private AudioClip _explosionSound;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Unity methods

    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.up);
        if (transform.position.x>_xBound ||
            transform.position.x<-_xBound ||
            transform.position.z<-_zBound)
        {
            try{_laserPool.Release(this);} catch (Exception){}
        }            
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.PlaySound(_explosionSound);
            GameManager.Instance.PlayParticleEffect(transform.position, SoundOrEffect.Purpose.LASER_EXPLOSION);
            try { _laserPool.Release(this); } catch (Exception) { }
        }
    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Public class methods

    public void SetSpeed(float speed){_speed = speed;}
    
    public void SetLaserPool(IObjectPool<Laser> laserPool){_laserPool = laserPool;}
}
