using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class SoundOrEffect : MonoBehaviour
{
    public enum Purpose { SOUND, DRONE_EXPLOSION, DRONE_DAMAGE,LASER_EXPLOSION }
    private Purpose purpose;
    private AudioSource audioSource;
    private IObjectPool<SoundOrEffect> soundOrEffectPool;
    [SerializeField] private ParticleSystem droneExplosion;
    [SerializeField] private ParticleSystem droneDamage;
    [SerializeField] private ParticleSystem laserExplosion;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        purpose = Purpose.SOUND;
    }

    void OnDisable()
    {
    }

    public void SetAsSound(AudioClip clipToPlay,float volumeToPlayAt)
    {
        purpose = Purpose.SOUND;
        audioSource.clip = clipToPlay;
        audioSource.volume = volumeToPlayAt;
        audioSource.Play();
        StartCoroutine(WaitForSoundOrEffectToFinish(clipToPlay.length));
    }

    public void SetAsParticleEffect(Purpose purpose)
    {
        this.purpose = purpose;
        switch (purpose)
        {
            case Purpose.DRONE_EXPLOSION:
                droneExplosion.Play();
                StartCoroutine(WaitForSoundOrEffectToFinish(droneExplosion.main.duration));
                break;
            case Purpose.DRONE_DAMAGE:
                droneDamage.Play();
                StartCoroutine(WaitForSoundOrEffectToFinish(droneDamage.main.duration));
                break;
            case Purpose.LASER_EXPLOSION:
                laserExplosion.Play();
                StartCoroutine(WaitForSoundOrEffectToFinish(laserExplosion.main.duration));
                break;
        }
    }

    public void SetSoundOrEffectPool(IObjectPool<SoundOrEffect> soundOrEffectPool)
    {
        this.soundOrEffectPool = soundOrEffectPool;
    }

    IEnumerator WaitForSoundOrEffectToFinish(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        switch (purpose)
        {
            case Purpose.DRONE_EXPLOSION:
                droneExplosion.Clear();
                droneExplosion.Pause();
                break;
            case Purpose.DRONE_DAMAGE:
                droneDamage.Clear();
                droneDamage.Pause();
                break;
            case Purpose.LASER_EXPLOSION:
                laserExplosion.Clear();
                laserExplosion.Pause();
                break;
        }
        try{soundOrEffectPool.Release(this);} catch (Exception){}
    }
}
