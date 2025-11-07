using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(AudioSource))]

/// <summary>
/// Controls the behaviour of SoundOrEffect objects for playing sounds or spawning
/// particle effects at specific places
/// </summary>
public class SoundOrEffect : MonoBehaviour
{
    public enum Purpose { SOUND, DRONE_EXPLOSION, DRONE_DAMAGE,LASER_EXPLOSION }
    private Purpose _purpose;
    private AudioSource _audioSource;
    private IObjectPool<SoundOrEffect> _soundOrEffectPool;
    [SerializeField] private ParticleSystem _droneExplosion;
    [SerializeField] private ParticleSystem _droneDamage;
    [SerializeField] private ParticleSystem _laserExplosion;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Unity methods

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _purpose = Purpose.SOUND;
    }
    
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Public class methods

    public void SetAsSound(AudioClip clipToPlay,float volumeToPlayAt)
    {
        _purpose = Purpose.SOUND;
        _audioSource.clip = clipToPlay;
        _audioSource.volume = volumeToPlayAt;
        _audioSource.Play();
        StartCoroutine(WaitForSoundOrEffectToFinish(clipToPlay.length));
    }

    public void SetAsParticleEffect(Purpose purpose)
    {
        _purpose = purpose;
        switch (_purpose)
        {
            case Purpose.DRONE_EXPLOSION:
                _droneExplosion.Play();
                StartCoroutine(WaitForSoundOrEffectToFinish(_droneExplosion.main.duration));
                break;
            case Purpose.DRONE_DAMAGE:
                _droneDamage.Play();
                StartCoroutine(WaitForSoundOrEffectToFinish(_droneDamage.main.duration));
                break;
            case Purpose.LASER_EXPLOSION:
                _laserExplosion.Play();
                StartCoroutine(WaitForSoundOrEffectToFinish(_laserExplosion.main.duration));
                break;
        }
    }

    public void SetSoundOrEffectPool(IObjectPool<SoundOrEffect> soundOrEffectPool)
    {
        _soundOrEffectPool = soundOrEffectPool;
    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Coroutines

    IEnumerator WaitForSoundOrEffectToFinish(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        switch (_purpose)
        {
            case Purpose.DRONE_EXPLOSION:
                _droneExplosion.Clear();
                _droneExplosion.Pause();
                break;
            case Purpose.DRONE_DAMAGE:
                _droneDamage.Clear();
                _droneDamage.Pause();
                break;
            case Purpose.LASER_EXPLOSION:
                _laserExplosion.Clear();
                _laserExplosion.Pause();
                break;
        }
        try{_soundOrEffectPool.Release(this);} catch (Exception){}
    }
}
